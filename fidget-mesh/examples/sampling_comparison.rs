//! Rounded-block fixture used by the Unity Sampling/RoundedBlockComparison scene.
//! Run with cargo run --release -p fidget-mesh --example sampling_comparison
use fidget_core::{context::Tree, shape::ShapeVars, var::Var, vm::VmShape};
use fidget_mesh::{Mesh, Octree, Settings};
use nalgebra::Vector3;
use std::{
    alloc::{GlobalAlloc, Layout, System},
    collections::HashMap,
    sync::atomic::{AtomicUsize, Ordering::Relaxed},
    time::Instant,
};

// The standalone process builds on one thread. Measure incremental live Rust
// heap during build + dual traversal; exclude fixture setup and geometry checks.
struct CountingAllocator;
static LIVE: AtomicUsize = AtomicUsize::new(0);
static PEAK: AtomicUsize = AtomicUsize::new(0);
#[global_allocator]
static ALLOCATOR: CountingAllocator = CountingAllocator;

unsafe impl GlobalAlloc for CountingAllocator {
    unsafe fn alloc(&self, layout: Layout) -> *mut u8 {
        let p = unsafe { System.alloc(layout) };
        if !p.is_null() {
            PEAK.fetch_max(
                LIVE.fetch_add(layout.size(), Relaxed) + layout.size(),
                Relaxed,
            );
        }
        p
    }
    unsafe fn dealloc(&self, p: *mut u8, layout: Layout) {
        LIVE.fetch_sub(layout.size(), Relaxed);
        unsafe { System.dealloc(p, layout) }
    }
    unsafe fn realloc(
        &self,
        p: *mut u8,
        layout: Layout,
        size: usize,
    ) -> *mut u8 {
        let next = unsafe { System.realloc(p, layout, size) };
        if !next.is_null() {
            if size > layout.size() {
                let delta = size - layout.size();
                PEAK.fetch_max(LIVE.fetch_add(delta, Relaxed) + delta, Relaxed);
            } else {
                LIVE.fetch_sub(layout.size() - size, Relaxed);
            }
        }
        next
    }
}

fn fixture() -> (VmShape, ShapeVars<f32>) {
    let bore = Var::new();
    let bridge = Var::new();
    let mut values = ShapeVars::new();
    values.insert(bore.index().unwrap(), 0.12);
    values.insert(bridge.index().unwrap(), 0.085);
    let (x, y, z) = Tree::axes();
    let pad = |cx: f32| {
        let a = (x.clone() - cx).abs() - (0.30 - 0.065);
        let b = y.abs() - (0.38 - 0.065);
        let c = z.abs() - (0.15 - 0.065);
        (a.max(0.0).square() + b.max(0.0).square() + c.max(0.0).square()).sqrt()
            + a.max(b.max(c)).min(0.0)
            - 0.065
    };
    let link = (x.abs() - 0.49)
        .max(y.abs() - Tree::from(bridge) * 0.5)
        .max(z.abs() - 0.09);
    let hole = ((x.clone() + 0.48).square() + y.square()).sqrt()
        - Tree::from(bore) * 0.5;
    (
        VmShape::from(pad(-0.48).min(pad(0.48)).min(link).max(-hole)),
        values,
    )
}

type Point = Vector3<f32>;

fn field(p: Point) -> f32 {
    let pad = |cx: f32| {
        let q = (p - Point::new(cx, 0.0, 0.0)).abs()
            - Point::new(0.235, 0.315, 0.085);
        q.sup(&Point::zeros()).norm() + q.max().min(0.0) - 0.065
    };
    let link = (p.abs() - Point::new(0.49, 0.0425, 0.09)).max();
    pad(-0.48)
        .min(pad(0.48))
        .min(link)
        .max(0.06 - (p.x + 0.48).hypot(p.y))
}

fn triangle_distance(p: Point, a: Point, b: Point, c: Point) -> f32 {
    let n = (b - a).cross(&(c - a));
    let edges = [(a, b), (b, c), (c, a)];
    if n.norm_squared() > 0.0
        && edges
            .iter()
            .all(|(u, v)| (v - u).cross(&(p - u)).dot(&n) >= 0.0)
    {
        return (p - a).dot(&n).abs() / n.norm();
    }
    edges
        .iter()
        .map(|(u, v)| {
            let e = v - u;
            let t = if e.norm_squared() == 0.0 {
                0.0
            } else {
                ((p - u).dot(&e) / e.norm_squared()).clamp(0.0, 1.0)
            };
            (p - u - e * t).norm()
        })
        .fold(f32::INFINITY, f32::min)
}

fn check(mesh: &Mesh) -> (f32, f32) {
    let mut edges = HashMap::new();
    let mut neighbors = vec![Vec::new(); mesh.vertices.len()];
    let mut field_error = 0f32;
    for f in &mesh.triangles {
        let [a, b, c] = [f.x, f.y, f.z].map(|i| mesh.vertices[i]);
        for p in [
            a,
            (a + b) * 0.5,
            (b + c) * 0.5,
            (c + a) * 0.5,
            (a + b + c) / 3.0,
        ] {
            field_error = field_error.max(field(p).abs());
        }
        for (i, j) in [(f.x, f.y), (f.y, f.z), (f.z, f.x)] {
            *edges.entry((i.min(j), i.max(j))).or_insert(0) += 1;
            neighbors[i].push(j);
            neighbors[j].push(i);
        }
    }
    assert!(edges.values().all(|n| *n == 2), "open or nonmanifold edge");
    assert_eq!(
        mesh.vertices.len() + mesh.triangles.len(),
        edges.len(),
        "expected a through-hole"
    );
    let mut seen = vec![false; neighbors.len()];
    let mut stack = vec![0];
    while let Some(i) = stack.pop() {
        if !seen[i] {
            seen[i] = true;
            stack.extend(&neighbors[i]);
        }
    }
    assert!(seen.into_iter().all(|v| v), "bridge disconnected");
    let mut probes = vec![
        Point::new(0.0, 0.0425, 0.0),
        Point::new(0.0, -0.0425, 0.0),
        Point::new(0.0, 0.0, 0.09),
    ];
    for i in 0..16 {
        let (s, c) = (std::f32::consts::TAU * i as f32 / 16.0).sin_cos();
        for z in [-0.12, 0.0, 0.12] {
            probes.push(Point::new(-0.48 + 0.06 * c, 0.06 * s, z));
        }
        let (s, c) = (std::f32::consts::FRAC_PI_2 * i as f32 / 15.0).sin_cos();
        probes.push(Point::new(
            0.48 + 0.235 + 0.065 * c,
            0.0,
            0.085 + 0.065 * s,
        ));
    }
    let surface_error = probes
        .into_iter()
        .map(|p| {
            mesh.triangles
                .iter()
                .map(|f| {
                    triangle_distance(
                        p,
                        mesh.vertices[f.x],
                        mesh.vertices[f.y],
                        mesh.vertices[f.z],
                    )
                })
                .fold(f32::INFINITY, f32::min)
        })
        .fold(0.0, f32::max);
    (field_error, surface_error)
}

// Positions avoid tangencies. The rays cross the bore wall, both pads, the
// fillet and the bridge. A missing hole changes the intersection count.
fn ray_crossings(mesh: &Mesh) -> Vec<Vec<f64>> {
    let mut rays = vec![
        (2, -0.477, 0.007),
        (2, -0.389, 0.016),
        (2, 0.013, 0.004),
        (2, 0.013, 0.079),
    ];
    for i in 0..12 {
        rays.push((2, 0.723 + i as f64 * 0.0047, 0.014));
        rays.push((0, -0.048 + i as f64 * 0.0083, 0.033));
    }
    rays.into_iter()
        .map(|(axis, u, v)| {
            let j = (axis + 1) % 3;
            let k = (axis + 2) % 3;
            let mut hits = Vec::new();
            for f in &mesh.triangles {
                let a = mesh.vertices[f.x].cast::<f64>();
                let ab = mesh.vertices[f.y].cast::<f64>() - a;
                let ac = mesh.vertices[f.z].cast::<f64>() - a;
                let det = ab[j] * ac[k] - ab[k] * ac[j];
                if det.abs() < 1e-14 {
                    continue;
                }
                let b = ((u - a[j]) * ac[k] - (v - a[k]) * ac[j]) / det;
                let c = (ab[j] * (v - a[k]) - ab[k] * (u - a[j])) / det;
                if b >= 0.0 && c >= 0.0 && b + c <= 1.0 {
                    hits.push(a[axis] + b * ab[axis] + c * ac[axis]);
                }
            }
            hits.sort_by(f64::total_cmp);
            hits.dedup_by(|a, b| (*a - *b).abs() < 1e-9);
            hits
        })
        .collect()
}

fn main() {
    let mut depths = Vec::new();
    for arg in std::env::args().skip(1) {
        let depth = arg.parse::<u8>().expect("expected a depth from 6 to 9");
        assert!((6..=9).contains(&depth), "expected a depth from 6 to 9");
        depths.push(depth);
    }
    if depths.is_empty() {
        depths.extend([6, 7, 8]);
    }
    let (shape, values) = fixture();
    let shape = shape.bind(&values).unwrap();
    println!(
        "depth,phase,mode,visited,planar_leaves,curved_leaves,triangles,median_ms,peak_rust_heap_bytes,max_field_residual,max_probe_distance,max_ray_shift_from_reference,rays_with_different_crossing_count,reference_depth"
    );
    for depth in depths {
        for phase in [0.0, 0.37, 0.81] {
            let edge = 2.0 / (1 << depth) as f32;
            let mut cfg = Settings {
                depth,
                threads: None,
                ..Default::default()
            };
            for (axis, scale) in [1.0, 0.5, 0.25].into_iter().enumerate() {
                cfg.world_to_model[(axis, 3)] = edge * phase * scale;
            }
            let fine = Settings {
                depth: (depth + 1).max(9),
                threads: None,
                world_to_model: cfg.world_to_model,
                ..Default::default()
            };
            let reference = ray_crossings(
                &Octree::build(&shape, &fine).unwrap().walk_dual(),
            );
            let names = ["fixed", "planes", "error_0.008", "error_0.002"];
            let mut times = names.map(|_| Vec::new());
            // Warm up, time five builds per mode, then run geometry checks
            // separately so their allocations do not influence timing.
            for run in 0..7 {
                for j in 0..names.len() {
                    let mode = (run + j) % names.len();
                    let start_bytes = LIVE.load(Relaxed);
                    PEAK.store(start_bytes, Relaxed);
                    let start = Instant::now();
                    let (tree, stats) = match mode {
                        0 => Octree::build_with_budget(&shape, &cfg, 2_000_000),
                        1 => Octree::build_adaptive_planes(
                            &shape, &cfg, 2_000_000,
                        ),
                        _ => Octree::build_with_surface_error(
                            &shape,
                            &cfg,
                            2_000_000,
                            if mode == 2 { 0.008 } else { 0.002 },
                        ),
                    }
                    .unwrap();
                    let mesh = tree.walk_dual();
                    let ms = start.elapsed().as_secs_f64() * 1000.0;
                    let peak = PEAK.load(Relaxed) - start_bytes;
                    if (1..=5).contains(&run) {
                        times[mode].push(ms);
                    }
                    if run == 6 {
                        times[mode].sort_by(f64::total_cmp);
                        let (residual, distance) = check(&mesh);
                        let mut ray_shift = 0.0f64;
                        let mut crossing_count_changes = 0;
                        for (hits, expected) in
                            ray_crossings(&mesh).iter().zip(&reference)
                        {
                            if hits.len() != expected.len() {
                                crossing_count_changes += 1;
                            }
                            for (from, to) in
                                [(hits, expected), (expected, hits)]
                            {
                                for a in from {
                                    let nearest = to
                                        .iter()
                                        .map(|b| (a - b).abs())
                                        .fold(f64::INFINITY, f64::min);
                                    ray_shift = ray_shift.max(nearest);
                                }
                            }
                        }
                        let name = names[mode];
                        println!(
                            "{depth},{phase:.2},{name},{},{},{},{},{:.2},{peak},{residual:.6},{distance:.6},{ray_shift:.6},{crossing_count_changes},{}",
                            stats.visited,
                            stats.planar_leaves,
                            stats.curved_leaves,
                            mesh.triangles.len(),
                            times[mode][2],
                            fine.depth
                        );
                    }
                }
            }
        }
    }
}
