//! Surface normals from Fidget's expression, rather than triangle averaging.
//!
//! A face's centroid selects an expression trace. We evaluate that trace's
//! differentiable branch at each corner, retaining separate render vertices
//! when normals differ. This is face-interior classification, not provenance
//! exported by the mesher: a triangle crossing a branch boundary is a limitation.
use fidget_core::{
    eval::Function,
    shape::{EzShape, ShapeBulkEval, ShapeTracingEval, ShapeVars},
    types::Grad,
    vm::{VmFunction, VmShape},
};
use fidget_mesh::Mesh;
use nalgebra::Vector3;
use std::collections::HashMap;

type NormalKey = (usize, [u32; 3]);

/// Evaluator allocations are retained by a mesher between requests.
#[derive(Default)]
pub(crate) struct SurfaceWorkspace {
    point: ShapeTracingEval<<VmFunction as Function>::PointEval>,
    gradient: ShapeBulkEval<<VmFunction as Function>::GradSliceEval>,
}

impl SurfaceWorkspace {
    pub(crate) fn prepare(
        &mut self,
        shape: &VmShape,
        values: &ShapeVars<f32>,
        mesh: Mesh,
    ) -> Option<(Mesh, Vec<Vector3<f32>>)> {
        let tape = shape.ez_point_tape();
        // Bound this cache to the current mesh; arbitrary expressions can have
        // many traces, so retaining all traces across animation could grow forever.
        let mut branches = HashMap::new();
        let mut key = Vec::new();
        let mut output = Mesh::new();
        output.vertices.reserve(mesh.vertices.len());
        output.triangles.reserve(mesh.triangles.len());
        let mut normals = Vec::with_capacity(mesh.vertices.len());
        let mut vertices: HashMap<NormalKey, usize> = HashMap::new();
        for face in mesh.triangles {
            let centroid = (mesh.vertices[face[0]]
                + mesh.vertices[face[1]]
                + mesh.vertices[face[2]])
                / 3.0;
            let (_, trace) = self
                .point
                .eval_with_vars(
                    &tape, centroid.x, centroid.y, centroid.z, values,
                )
                .ok()?;
            key.clear();
            if let Some(trace) = trace {
                key.extend(trace.as_slice().iter().map(|choice| *choice as u8));
            }
            if !branches.contains_key(&key) {
                let branch = match trace {
                    Some(trace) => shape.ez_simplify(trace).ok()?,
                    None => shape.clone(),
                };
                branches.insert(
                    key.clone(),
                    (branch.ez_grad_slice_tape(), branch.ez_point_tape()),
                );
            }
            let (branch, point_branch) = &branches[&key];
            let points = face.map(|i| mesh.vertices[i]);
            let xs = points.map(|p| Grad::new(p.x, 1.0, 0.0, 0.0));
            let ys = points.map(|p| Grad::new(p.y, 0.0, 1.0, 0.0));
            let zs = points.map(|p| Grad::new(p.z, 0.0, 0.0, 1.0));
            let gradients = self
                .gradient
                .eval_with_vars(
                    branch,
                    xs.as_slice(),
                    ys.as_slice(),
                    zs.as_slice(),
                    values,
                )
                .ok()?;
            // Release the evaluator borrow before a possible face-interior query.
            let gradients: [Grad; 3] = gradients.try_into().ok()?;
            let xs = [Grad::new(centroid.x, 1.0, 0.0, 0.0)];
            let ys = [Grad::new(centroid.y, 0.0, 1.0, 0.0)];
            let zs = [Grad::new(centroid.z, 0.0, 0.0, 1.0)];
            let g = self
                .gradient
                .eval_with_vars(branch, &xs, &ys, &zs, values)
                .ok()?[0];
            let mut n = Vector3::new(g.dx, g.dy, g.dz);
            let mut len = n.norm();
            if !len.is_finite() || len == 0.0 {
                // Dual contouring may collapse a face to a pole. Its
                // centroid is singular too; sample the selected
                // branch symmetrically to recover a finite normal.
                let step = centroid.abs().max().max(1.0) * 1e-4;
                // A signed-distance formula may select a zero-valued
                // sign branch exactly on a planar face. That trace is
                // locally constant even though the full field has a
                // valid derivative across the face. Try the full field
                // only after the selected branch cannot recover one.
                for sample_tape in [point_branch, &tape] {
                    for axis in 0..3 {
                        let mut hi = centroid;
                        let mut lo = centroid;
                        hi[axis] += step;
                        lo[axis] -= step;
                        let (a, _) = self
                            .point
                            .eval_with_vars(
                                sample_tape,
                                hi.x,
                                hi.y,
                                hi.z,
                                values,
                            )
                            .ok()?;
                        let (b, _) = self
                            .point
                            .eval_with_vars(
                                sample_tape,
                                lo.x,
                                lo.y,
                                lo.z,
                                values,
                            )
                            .ok()?;
                        n[axis] = (a - b) / (2.0 * step);
                    }
                    len = n.norm();
                    if len.is_finite() && len > 0.0 {
                        break;
                    }
                }
            }
            let interior_normal = if len.is_finite() && len > 0.0 {
                Some(n / len)
            } else {
                None
            };
            let mut triangle = Vector3::zeros();
            for corner in 0..3 {
                let g = gradients[corner];
                let mut normal = Vector3::new(g.dx, g.dy, g.dz);
                let length = normal.norm();
                if !length.is_finite() || length == 0.0 {
                    // Cone tips and capsule poles have no unique derivative (or
                    // contain 0 * NaN from an inactive distance term). Use the
                    // selected face's interior derivative as the corner limit.
                    normal = interior_normal?;
                } else {
                    normal /= length;
                }
                // Frozen absolute-value branches can oppose a dynamic sign
                // comparison at corners a few ulps across the surface. Keep
                // the analytic direction in the face-interior hemisphere.
                if let Some(reference) = interior_normal
                    && normal.dot(&reference) < 0.0
                {
                    normal = -normal;
                }
                // Treat signed zeros as equal; no angular/position tolerance is used.
                for n in normal.iter_mut() {
                    if *n == 0.0 {
                        *n = 0.0;
                    }
                }
                let normal_key = (
                    face[corner],
                    [
                        normal.x.to_bits(),
                        normal.y.to_bits(),
                        normal.z.to_bits(),
                    ],
                );
                let index = *vertices.entry(normal_key).or_insert_with(|| {
                    let index = output.vertices.len();
                    output.vertices.push(points[corner]);
                    normals.push(normal);
                    index
                });
                triangle[corner] = index;
            }
            output.triangles.push(triangle);
        }
        Some((output, normals))
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use fidget_core::{context::Tree, var::Var};
    use fidget_mesh::{Octree, Settings};

    #[test]
    fn boolean_surface_normals_preserve_geometry_and_surface_derivatives() {
        let parameter = Var::new();
        let sphere =
            (Tree::x().square() + Tree::y().square() + Tree::z().square())
                .sqrt()
                - 0.85;
        let block =
            Tree::x().abs().max(Tree::y().abs()).max(Tree::z().abs()) - 0.65;
        let cut = ((Tree::x() - Tree::from(parameter)).square()
            + Tree::z().square())
        .sqrt()
            - 0.32;
        let shape = VmShape::from(sphere.max(block).max(-cut));
        let mut values = ShapeVars::new();
        let mut workspace = SurfaceWorkspace::default();
        for offset in [0.0f32, 0.35, -0.35] {
            values.insert(parameter.index().unwrap(), offset);
            let start = std::time::Instant::now();
            let input = Octree::build(
                &shape.bind(&values).unwrap(),
                &Settings {
                    depth: 5,
                    threads: None,
                    ..Default::default()
                },
            )
            .unwrap()
            .walk_dual();
            let meshing_time = start.elapsed();
            let original_positions = input.vertices.clone();
            let original_faces = input.triangles.clone();
            let start = std::time::Instant::now();
            let (output, normals) =
                workspace.prepare(&shape, &values, input).unwrap();
            let normals_time = start.elapsed();
            assert_eq!(original_faces.len(), output.triangles.len());
            assert!(
                output.vertices.len() > original_positions.len(),
                "sharp boundaries need distinct render normals"
            );
            for (before, after) in original_faces.iter().zip(&output.triangles)
            {
                let center = (original_positions[before[0]]
                    + original_positions[before[1]]
                    + original_positions[before[2]])
                    / 3.0;
                let sphere = center.norm() - 0.85;
                let plane = center.abs().max() - 0.65;
                let cut = 0.32
                    - ((center.x - offset).powi(2) + center.z.powi(2)).sqrt();
                for corner in 0..3 {
                    let p = original_positions[before[corner]];
                    assert_eq!(
                        p, output.vertices[after[corner]],
                        "normal preparation moved geometry"
                    );
                    let expected = if cut > sphere.max(plane) {
                        -Vector3::new(p.x - offset, 0.0, p.z).normalize()
                    } else if sphere > plane {
                        p.normalize()
                    } else {
                        let axis = center.abs().iamax();
                        let mut n = Vector3::zeros();
                        n[axis] = center[axis].signum();
                        n
                    };
                    assert!(
                        normals[after[corner]].dot(&expected) > 0.99999,
                        "incorrect surface normal at {p:?}"
                    );
                }
            }
            eprintln!(
                "offset={offset}: {} geometric / {} render vertices; meshing={meshing_time:?}; surface_normals={normals_time:?}",
                original_positions.len(),
                output.vertices.len()
            );
        }
    }

    #[test]
    fn transformed_sphere_has_smooth_gradients_without_vertex_splitting() {
        let tree = ((Tree::x() - 0.2).square()
            + (Tree::y() + 0.1).square()
            + Tree::z().square())
        .sqrt()
            - 0.5;
        let shape = VmShape::from(tree);
        let values = ShapeVars::new();
        let input = Octree::build(
            &shape.bind(&values).unwrap(),
            &Settings {
                depth: 4,
                threads: None,
                ..Default::default()
            },
        )
        .unwrap()
        .walk_dual();
        let vertex_count = input.vertices.len();
        let (output, normals) = SurfaceWorkspace::default()
            .prepare(&shape, &values, input)
            .unwrap();
        assert_eq!(vertex_count, output.vertices.len());
        for (p, n) in output.vertices.iter().zip(normals) {
            assert!(
                n.dot(&(*p - Vector3::new(0.2, -0.1, 0.0)).normalize())
                    > 0.99999
            );
        }
    }
    #[test]
    fn zero_sign_branch_on_a_planar_face_uses_the_full_field_limit() {
        // sign(y) * abs(y) is exactly y. Trace simplification at y=0
        // nevertheless selects a constant zero sign and loses that derivative.
        let (x, y, z) = Tree::axes();
        let signed_plane = y.clone().compare(0.0) * y.clone().abs();
        let shape = VmShape::from(
            signed_plane
                .max(x.abs() - 0.6)
                .max(z.abs() - 0.6)
                .max(-y - 0.5),
        );
        let values = ShapeVars::new();
        let input = Octree::build(
            &shape.bind(&values).unwrap(),
            &Settings {
                depth: 5,
                threads: None,
                ..Default::default()
            },
        )
        .unwrap()
        .walk_dual();
        let triangle_count = input.triangles.len();
        let (output, normals) = SurfaceWorkspace::default()
            .prepare(&shape, &values, input)
            .expect("valid signed-plane normal limit");
        assert_eq!(triangle_count, output.triangles.len());
        let mut top_vertices = 0;
        for (p, n) in output.vertices.iter().zip(normals) {
            assert!(n.iter().all(|v| v.is_finite()));
            assert!((n.norm() - 1.0).abs() < 1e-5);
            if p.y.abs() < 1e-6 && p.x.abs() < 0.5 && p.z.abs() < 0.5 {
                assert!(n.y > 0.9999, "wrong planar normal at {p:?}: {n:?}");
                top_vertices += 1;
            }
        }
        assert!(top_vertices > 0);
    }

    #[test]
    fn planar_normals_do_not_flip_when_corners_straddle_a_frozen_sign_branch() {
        let (x, y, z) = Tree::axes();
        let signed_plane = y.clone().compare(0.0) * y.clone().abs();
        let shape = VmShape::from(
            signed_plane
                .max(x.abs() - 0.6)
                .max(z.abs() - 0.6)
                .max(-y - 0.5),
        );
        let values = ShapeVars::new();
        let mut input = Octree::build(
            &shape.bind(&values).unwrap(),
            &Settings {
                depth: 5,
                threads: None,
                ..Default::default()
            },
        )
        .unwrap()
        .walk_dual();
        for (index, p) in input.vertices.iter_mut().enumerate() {
            if p.y.abs() < 1e-6 {
                p.y = if index % 2 == 0 { 5e-7 } else { -5e-7 };
            }
        }
        let positions = input.vertices.clone();
        let triangles = input.triangles.clone();
        let (output, normals) = SurfaceWorkspace::default()
            .prepare(&shape, &values, input)
            .unwrap();
        let mut checked = 0;
        for (before, after) in triangles.iter().zip(&output.triangles) {
            for corner in 0..3 {
                let p = positions[before[corner]];
                assert_eq!(
                    p, output.vertices[after[corner]],
                    "normal preparation moved geometry"
                );
                if p.y.abs() < 1e-6 && p.x.abs() < 0.5 && p.z.abs() < 0.5 {
                    let n = normals[after[corner]];
                    assert!(
                        n.y > 0.9999,
                        "flipped planar normal at {p:?}: {n:?}"
                    );
                    checked += 1;
                }
            }
        }
        assert!(checked > 0);
    }

    #[test]
    fn capsule_poles_use_the_face_interior_gradient() {
        // Mercury's exact mix formula has a 0 * undefined radial derivative
        // at the poles. The field and its face-interior gradients are valid.
        let (x, y, z) = Tree::axes();
        let radial = (x.square() + z.square()).sqrt() - 0.22;
        let cap =
            (x.square() + (y.abs() - 0.35).square() + z.square()).sqrt() - 0.22;
        let t = Tree::from(0.35).compare(y.abs()).max(0.0).not();
        let shape = VmShape::from(radial * (1.0 - t.clone()) + cap * t);
        let values = ShapeVars::new();
        let input = Octree::build(
            &shape.bind(&values).unwrap(),
            &Settings {
                depth: 6,
                threads: None,
                world_to_model: nalgebra::Matrix4::new_scaling(1.6),
                ..Default::default()
            },
        )
        .unwrap()
        .walk_dual();
        let triangles = input.triangles.len();
        let (output, normals) = SurfaceWorkspace::default()
            .prepare(&shape, &values, input)
            .unwrap();
        assert_eq!(triangles, output.triangles.len());
        assert!(!normals.is_empty());
        for n in normals {
            assert!(n.iter().all(|v| v.is_finite()));
            assert!((n.norm() - 1.0).abs() < 1e-5);
        }
    }
}
