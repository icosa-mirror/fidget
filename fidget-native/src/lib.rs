//! Native meshing with caller-owned output buffers for Unity MeshData.
//!
//! The library uses Fidget's VM evaluator; it does not depend on `fidget-jit`.

use fidget_core::{context::Tree, vm::VmShape};
use fidget_mesh::{Octree, Settings};
use std::{panic::catch_unwind, ptr};

mod shape;

const OK: i32 = 0;
const INVALID_ARGUMENT: i32 = 1;
const MESH_FAILED: i32 = 2;
const PANIC: i32 = 3;

/// Opaque allocation owned by this library. Release with `fidget_mesh_free`.
pub struct FidgetMesh {
    mesh: fidget_mesh::Mesh,
}

/// Fixed-width output sizes, without allocating packed staging buffers.
#[repr(C)]
#[derive(Default)]
pub struct FidgetMeshCounts {
    pub vertex_count: u32,
    pub index_count: u32,
}

/// Axis-aligned bounds computed while writing positions.
#[repr(C)]
#[derive(Default)]
pub struct FidgetBounds {
    pub min: [f32; 3],
    pub max: [f32; 3],
}

/// Version of the C ABI defined by `include/fidget_native.h`.
#[unsafe(no_mangle)]
pub extern "C" fn fidget_native_abi_version() -> u32 {
    3
}

/// Meshes a sphere at the origin inside the fixed [-1, 1]^3 domain.
///
/// Radius must be finite and strictly between 0 and 1; depth must be 1..=8.
/// The depth cap bounds this smoke-test API, not Fidget's general capabilities.
/// Meshing is synchronous and single-threaded. On failure, `*out_mesh` is null.
/// Rust panics are caught; process aborts (including allocation failure) are not.
///
/// # Safety
/// `out_mesh` must be null or point to writable, aligned pointer storage.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_sphere(
    radius: f32,
    depth: u32,
    out_mesh: *mut *mut FidgetMesh,
) -> i32 {
    if out_mesh.is_null() {
        return INVALID_ARGUMENT;
    }
    // SAFETY: the caller provides writable pointer storage.
    unsafe { out_mesh.write(ptr::null_mut()) };
    if !radius.is_finite()
        || radius <= 0.0
        || radius >= 1.0
        || !(1..=8).contains(&depth)
    {
        return INVALID_ARGUMENT;
    }

    match catch_unwind(|| mesh_sphere(radius, depth as u8)) {
        Ok(Some(mesh)) => {
            // SAFETY: checked and initialized above.
            unsafe { out_mesh.write(Box::into_raw(Box::new(mesh))) };
            OK
        }
        Ok(None) => MESH_FAILED,
        Err(_) => PANIC,
    }
}

fn mesh_sphere(radius: f32, depth: u8) -> Option<FidgetMesh> {
    let tree = (Tree::x().square() + Tree::y().square() + Tree::z().square())
        .sqrt()
        - radius;
    let shape = VmShape::from(tree).try_into().ok()?;
    let settings = Settings {
        depth,
        threads: None,
        ..Default::default()
    };
    let mesh = Octree::build(&shape, &settings)?.walk_dual();
    // The C ABI and Unity index buffers use fixed-width counts and indices.
    u32::try_from(mesh.vertices.len()).ok()?;
    u32::try_from(mesh.triangles.len().checked_mul(3)?).ok()?;
    Some(FidgetMesh { mesh })
}

/// Reads output sizes without converting or copying mesh data.
///
/// # Safety
/// `mesh` must be null or a live handle; `counts` must be null or writable,
/// aligned storage. Neither may overlap, and the mesh must remain live.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_get_counts(
    mesh: *const FidgetMesh,
    counts: *mut FidgetMeshCounts,
) -> i32 {
    if counts.is_null() {
        return INVALID_ARGUMENT;
    }
    // SAFETY: writable output storage is required by the caller contract.
    unsafe { counts.write(FidgetMeshCounts::default()) };
    // SAFETY: non-null mesh handles must remain live during this call.
    let Some(mesh) = (unsafe { mesh.as_ref() }) else {
        return INVALID_ARGUMENT;
    };
    // SAFETY: counts were checked during generation; output storage is valid.
    unsafe {
        counts.write(FidgetMeshCounts {
            vertex_count: mesh.mesh.vertices.len() as u32,
            index_count: (mesh.mesh.triangles.len() * 3) as u32,
        });
    }
    OK
}

/// Writes packed XYZ positions and uint32 indices directly into caller buffers.
/// No allocation, intermediate packed arrays, or mesh ownership transfer occurs.
/// Capacities count vertices and indices, respectively (not bytes).
/// Returns INVALID_ARGUMENT without writing any outputs if validation fails.
/// Empty meshes produce zero bounds and permit null data buffers.
///
/// # Safety
/// Non-null `mesh` must be live. Output buffers must be aligned and writable for
/// their stated capacities; `vertices` has three floats per vertex. `bounds`
/// must point to writable FidgetBounds storage. Outputs must not overlap each
/// other or the mesh. The mesh must not be freed while this call executes.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_copy_to(
    mesh: *const FidgetMesh,
    vertices: *mut f32,
    vertex_capacity: u32,
    indices: *mut u32,
    index_capacity: u32,
    bounds: *mut FidgetBounds,
) -> i32 {
    // SAFETY: non-null mesh handles must remain live during this call.
    let Some(mesh) = (unsafe { mesh.as_ref() }) else {
        return INVALID_ARGUMENT;
    };
    let mesh = &mesh.mesh;
    if bounds.is_null()
        || (vertex_capacity as usize) < mesh.vertices.len()
        || (index_capacity as usize) < mesh.triangles.len() * 3
        || (!mesh.vertices.is_empty() && vertices.is_null())
        || (!mesh.triangles.is_empty() && indices.is_null())
    {
        return INVALID_ARGUMENT;
    }
    let mut result = FidgetBounds::default();
    if let Some(first) = mesh.vertices.first() {
        result.min = [first.x, first.y, first.z];
        result.max = result.min;
    }
    for (i, vertex) in mesh.vertices.iter().enumerate() {
        for axis in 0..3 {
            let value = vertex[axis];
            // SAFETY: destination capacity and nullness were checked above.
            unsafe { vertices.add(i * 3 + axis).write(value) };
            result.min[axis] = result.min[axis].min(value);
            result.max[axis] = result.max[axis].max(value);
        }
    }
    for (i, triangle) in mesh.triangles.iter().enumerate() {
        for axis in 0..3 {
            // Fidget indices address vertices, whose count fits uint32.
            // SAFETY: destination capacity and nullness were checked above.
            unsafe { indices.add(i * 3 + axis).write(triangle[axis] as u32) };
        }
    }
    // SAFETY: bounds is non-null and points to writable output storage.
    unsafe { bounds.write(result) };
    OK
}

/// Frees the native mesh. Caller-owned copies remain valid. Null is a no-op.
///
/// # Safety
/// A non-null handle must come from this library and be freed exactly once.
/// No other thread may access the native mesh during or after this call.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_free(mesh: *mut FidgetMesh) {
    if !mesh.is_null() {
        // SAFETY: ownership is transferred back exactly once by the caller.
        unsafe { drop(Box::from_raw(mesh)) };
    }
}
