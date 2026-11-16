//! Compiled expression graphs and independent parameterized meshers.
use crate::{
    FidgetBounds, FidgetMesh, INVALID_ARGUMENT, MESH_FAILED, OK, PANIC,
};
use fidget_core::{context::Tree, shape::ShapeVars, var::Var, vm::VmShape};
use fidget_mesh::{Octree, Settings};
use std::{
    panic::{AssertUnwindSafe, catch_unwind},
    ptr,
    sync::Arc,
};

/// A topologically ordered expression instruction (see the C header).
#[repr(C)]
pub struct FidgetNode {
    opcode: u32,
    a: u32,
    b: u32,
    c: u32,
    d: u32,
    value: f32,
}

struct Compiled {
    shape: VmShape,
    parameters: Vec<Var>,
}

/// Immutable compiled expression; meshers retain their own reference.
pub struct FidgetShape(Arc<Compiled>);

/// Single-caller mesher with reusable parameter storage.
pub struct FidgetMesher {
    compiled: Arc<Compiled>,
    values: ShapeVars<f32>,
    surface: crate::surface::SurfaceWorkspace,
}

fn compile(nodes: &[FidgetNode], parameter_count: u32) -> Option<FidgetShape> {
    let parameters: Vec<_> = (0..parameter_count).map(|_| Var::new()).collect();
    let mut trees: Vec<Tree> = Vec::with_capacity(nodes.len());
    for n in nodes {
        let at = |i: u32| trees.get(i as usize).cloned();
        let t = match n.opcode {
            0 if n.value.is_finite() => Tree::from(n.value),
            1 => Tree::x(),
            2 => Tree::y(),
            3 => Tree::z(),
            4 => Tree::from(*parameters.get(n.a as usize)?),
            5 => at(n.a)?.neg(),
            6 => at(n.a)?.abs(),
            7 => at(n.a)?.sqrt(),
            8 => at(n.a)?.square(),
            9 => at(n.a)?.sin(),
            10 => at(n.a)?.cos(),
            11 => at(n.a)? + at(n.b)?,
            12 => at(n.a)? - at(n.b)?,
            13 => at(n.a)? * at(n.b)?,
            14 => at(n.a)? / at(n.b)?,
            15 => at(n.a)?.min(at(n.b)?),
            16 => at(n.a)?.max(at(n.b)?),
            17 => at(n.a)?.remap_xyz(at(n.b)?, at(n.c)?, at(n.d)?),
            18 => at(n.a)?.modulo(at(n.b)?),
            19 => at(n.a)?.floor(),
            20 => at(n.a)?.atan2(at(n.b)?),
            21 => at(n.a)?.compare(at(n.b)?),
            22 => at(n.a)?.and(at(n.b)?),
            23 => at(n.a)?.or(at(n.b)?),
            24 => at(n.a)?.not(),
            25 => at(n.a)?.exp(),
            26 => at(n.a)?.ln(),
            _ => return None,
        };
        trees.push(t);
    }
    Some(FidgetShape(Arc::new(Compiled {
        shape: VmShape::from(trees.pop()?),
        parameters,
    })))
}

/// Compiles the final node as the root. References must point backwards.
/// # Safety
/// `nodes` must address `count` initialized instructions, and `out` must be
/// writable pointer storage. Neither pointer is retained.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_shape_compile(
    nodes: *const FidgetNode,
    count: u32,
    parameter_count: u32,
    out: *mut *mut FidgetShape,
) -> i32 {
    if out.is_null() {
        return INVALID_ARGUMENT;
    }
    unsafe { out.write(ptr::null_mut()) };
    if nodes.is_null()
        || count == 0
        || count > 1_000_000
        || parameter_count > 4096
    {
        return INVALID_ARGUMENT;
    }
    match catch_unwind(|| {
        compile(
            unsafe { std::slice::from_raw_parts(nodes, count as usize) },
            parameter_count,
        )
    }) {
        Ok(Some(shape)) => {
            unsafe { out.write(Box::into_raw(Box::new(shape))) };
            OK
        }
        Ok(None) => INVALID_ARGUMENT,
        Err(_) => PANIC,
    }
}

/// Releases a compiled-shape reference; existing meshers remain usable.
/// # Safety
/// Non-null `shape` must be a live handle owned by the caller, freed once.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_shape_free(shape: *mut FidgetShape) {
    if !shape.is_null() {
        unsafe { drop(Box::from_raw(shape)) };
    }
}

/// Creates independent meshing state sharing the compiled expression.
/// # Safety
/// `shape` must be live and `out` writable. No concurrent freeing of `shape`.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesher_create(
    shape: *const FidgetShape,
    out: *mut *mut FidgetMesher,
) -> i32 {
    if out.is_null() {
        return INVALID_ARGUMENT;
    }
    unsafe { out.write(ptr::null_mut()) };
    let Some(shape) = (unsafe { shape.as_ref() }) else {
        return INVALID_ARGUMENT;
    };
    match catch_unwind(AssertUnwindSafe(|| {
        let mut values = ShapeVars::new();
        for var in &shape.0.parameters {
            values.insert(var.index().unwrap(), 0.0);
        }
        Box::new(FidgetMesher {
            compiled: shape.0.clone(),
            values,
            surface: Default::default(),
        })
    })) {
        Ok(mesher) => {
            unsafe { out.write(Box::into_raw(mesher)) };
            OK
        }
        Err(_) => PANIC,
    }
}

/// Meshes inside explicit model-space bounds. Parameter values are copied into
/// reusable native storage, without rebuilding the compiled expression.
/// # Safety
/// `mesher` must be live and exclusively accessed for this call. `values` must
/// address `count` floats (null permitted for zero). Bounds/out must be valid.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesher_generate(
    mesher: *mut FidgetMesher,
    values: *const f32,
    count: u32,
    bounds: *const FidgetBounds,
    depth: u32,
    out: *mut *mut FidgetMesh,
) -> i32 {
    if out.is_null() {
        return INVALID_ARGUMENT;
    }
    unsafe { out.write(ptr::null_mut()) };
    let Some(mesher) = (unsafe { mesher.as_mut() }) else {
        return INVALID_ARGUMENT;
    };
    let Some(bounds) = (unsafe { bounds.as_ref() }) else {
        return INVALID_ARGUMENT;
    };
    if count as usize != mesher.compiled.parameters.len()
        || (count != 0 && values.is_null())
        || !(1..=8).contains(&depth)
    {
        return INVALID_ARGUMENT;
    }
    let values = if count == 0 {
        &[]
    } else {
        unsafe { std::slice::from_raw_parts(values, count as usize) }
    };
    if values.iter().any(|v| !v.is_finite()) {
        return INVALID_ARGUMENT;
    }
    let mut transform = nalgebra::Matrix4::identity();
    for axis in 0..3 {
        let lo = bounds.min[axis];
        let hi = bounds.max[axis];
        let extent = (hi - lo) * 0.5;
        let center = lo * 0.5 + hi * 0.5;
        if !extent.is_finite() || extent <= 0.0 || !center.is_finite() {
            return INVALID_ARGUMENT;
        }
        transform[(axis, axis)] = extent;
        transform[(axis, 3)] = center;
    }
    match catch_unwind(AssertUnwindSafe(|| {
        for (var, value) in mesher.compiled.parameters.iter().zip(values) {
            mesher.values.insert(var.index().unwrap(), *value);
        }
        let bound = mesher.compiled.shape.bind(&mesher.values).ok()?;
        let settings = Settings {
            depth: depth as u8,
            world_to_model: transform,
            threads: None,
            ..Default::default()
        };
        let mesh = Octree::build(&bound, &settings)?.walk_dual();
        u32::try_from(mesh.vertices.len()).ok()?;
        u32::try_from(mesh.triangles.len().checked_mul(3)?).ok()?;
        Some(Box::new(FidgetMesh {
            mesh,
            normals: None,
        }))
    })) {
        Ok(Some(mesh)) => {
            unsafe { out.write(Box::into_raw(mesh)) };
            OK
        }
        Ok(None) => MESH_FAILED,
        Err(_) => PANIC,
    }
}

/// # Safety
/// Non-null `mesher` must be live, idle, and freed exactly once.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesher_free(mesher: *mut FidgetMesher) {
    if !mesher.is_null() {
        unsafe { drop(Box::from_raw(mesher)) };
    }
}

/// Prepares expression-derived normals using each face's interior branch trace.
/// Can duplicate render vertices, but does not change positions or triangles.
/// Call before querying mesh counts. Returns MESH_FAILED for undefined gradients.
/// # Safety
/// Both handles must be live and exclusively accessed. `mesh` must come from the
/// most recent generation on `mesher`, whose parameters must not have changed.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_prepare_surface_normals(
    mesher: *mut FidgetMesher,
    mesh: *mut FidgetMesh,
) -> i32 {
    let Some(mesher) = (unsafe { mesher.as_mut() }) else {
        return INVALID_ARGUMENT;
    };
    let Some(mesh) = (unsafe { mesh.as_mut() }) else {
        return INVALID_ARGUMENT;
    };
    match catch_unwind(AssertUnwindSafe(|| {
        let input = std::mem::take(&mut mesh.mesh);
        mesher
            .surface
            .prepare(&mesher.compiled.shape, &mesher.values, input)
    })) {
        Ok(Some((prepared, normals))) => {
            mesh.mesh = prepared;
            mesh.normals = Some(normals);
            OK
        }
        Ok(None) => MESH_FAILED,
        Err(_) => PANIC,
    }
}

/// Writes area-weighted smooth vertex normals to an optional separate stream.
/// # Safety
/// `mesh` must be live; `normals` must address `capacity * 3` writable floats.
/// The destination must not overlap the mesh. Null is permitted for empty output.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_write_normals(
    mesh: *const FidgetMesh,
    normals: *mut f32,
    capacity: u32,
) -> i32 {
    let Some(mesh) = (unsafe { mesh.as_ref() }) else {
        return INVALID_ARGUMENT;
    };
    if (capacity as usize) < mesh.mesh.vertices.len()
        || (!mesh.mesh.vertices.is_empty() && normals.is_null())
    {
        return INVALID_ARGUMENT;
    }
    if mesh.mesh.vertices.is_empty() {
        return OK;
    }
    let out = unsafe {
        std::slice::from_raw_parts_mut(normals, mesh.mesh.vertices.len() * 3)
    };
    if let Some(prepared) = &mesh.normals {
        for (out, normal) in out.as_chunks_mut::<3>().0.iter_mut().zip(prepared)
        {
            out.copy_from_slice(normal.as_slice());
        }
        return OK;
    }
    let mesh = &mesh.mesh;
    out.fill(0.0);
    for tri in &mesh.triangles {
        let n = (mesh.vertices[tri[1]] - mesh.vertices[tri[0]])
            .cross(&(mesh.vertices[tri[2]] - mesh.vertices[tri[0]]));
        for &i in tri.iter() {
            for axis in 0..3 {
                out[i * 3 + axis] += n[axis];
            }
        }
    }
    for n in out.as_chunks_mut::<3>().0 {
        let v = nalgebra::Vector3::new(n[0], n[1], n[2]);
        let normal = v.try_normalize(1.0e-20).unwrap_or(nalgebra::Vector3::y());
        n.copy_from_slice(normal.as_slice());
    }
    OK
}

#[cfg(test)]
mod mercury_tests {
    use super::*;
    use fidget_core::shape::EzShape;

    fn node(opcode: u32, a: u32, b: u32) -> FidgetNode {
        FidgetNode {
            opcode,
            a,
            b,
            c: 0,
            d: 0,
            value: 0.0,
        }
    }

    #[test]
    fn mercury_opcodes_evaluate_in_the_vm() {
        for (op, x, y, expected) in [
            (19, -1.25_f32, 0.0_f32, -2.0_f32),
            (20, 1.0, 0.0, std::f32::consts::FRAC_PI_2),
            (21, -1.0, 0.0, -1.0),
            (22, 0.0, 2.0, 0.0),
            (22, -1.0, 2.0, 2.0),
            (23, 0.0, 2.0, 2.0),
            (23, -1.0, 2.0, -1.0),
            (24, 0.0, 0.0, 1.0),
            (24, -1.0, 0.0, 0.0),
            (25, 0.0, 0.0, 1.0),
            (26, 1.0, 0.0, 0.0),
        ] {
            let shape =
                compile(&[node(1, 0, 0), node(2, 0, 0), node(op, 0, 1)], 0)
                    .unwrap();
            let tape = shape.0.shape.ez_point_tape();
            let (value, _) = VmShape::new_point_eval()
                .eval_with_vars(&tape, x, y, 0.0, &ShapeVars::<f32>::new())
                .unwrap();
            assert!(
                (value - expected).abs() < 1e-6,
                "opcode {op}: {value} != {expected}"
            );
        }
    }

    #[test]
    fn conditional_ignores_nan_in_an_unselected_branch() {
        // x AND ln(y): at x=0, y=-1 the result must be zero, not NaN.
        let shape = compile(
            &[node(1, 0, 0), node(2, 0, 0), node(26, 1, 0), node(22, 0, 2)],
            0,
        )
        .unwrap();
        let tape = shape.0.shape.ez_point_tape();
        let (value, _) = VmShape::new_point_eval()
            .eval_with_vars(
                &tape,
                0.0_f32,
                -1.0_f32,
                0.0_f32,
                &ShapeVars::<f32>::new(),
            )
            .unwrap();
        assert_eq!(value, 0.0);
    }
}
