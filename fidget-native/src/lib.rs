//! Build-validation API, not the final per-frame Unity integration.
//!
//! The library uses Fidget's VM evaluator; it does not depend on `fidget-jit`.

use fidget_core::{context::Tree, vm::VmShape};
use fidget_mesh::{Octree, Settings};
use std::{panic::catch_unwind, ptr};

const OK: i32 = 0;
const INVALID_ARGUMENT: i32 = 1;
const MESH_FAILED: i32 = 2;
const PANIC: i32 = 3;

/// Opaque allocation owned by this library. Release with `fidget_mesh_free`.
pub struct FidgetMesh {
    vertices: Vec<[f32; 3]>,
    indices: Vec<u32>,
}

/// Borrowed buffers, valid until the owning mesh is freed.
#[repr(C)]
pub struct FidgetMeshView {
    /// Packed XYZ positions (three floats per vertex).
    pub vertices: *const f32,
    pub vertex_count: u32,
    /// Triangle indices (three indices per triangle).
    pub indices: *const u32,
    pub index_count: u32,
}

/// Version of the C ABI defined by `include/fidget_native.h`.
#[unsafe(no_mangle)]
pub extern "C" fn fidget_native_abi_version() -> u32 {
    1
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
    let vertices = mesh.vertices.iter().map(|v| [v.x, v.y, v.z]).collect();
    let indices = mesh
        .triangles
        .iter()
        .flat_map(|t| [t.x, t.y, t.z])
        .map(|i| u32::try_from(i).ok())
        .collect::<Option<Vec<_>>>()?;
    Some(FidgetMesh { vertices, indices })
}

/// Returns immutable buffers owned by `mesh`; no allocation or copying occurs.
/// On an invalid (null) mesh, clears `out_view` and returns INVALID_ARGUMENT.
///
/// # Safety
/// `mesh` must be null or a live handle from `fidget_mesh_sphere`.
/// `out_view` must be null or point to writable, aligned `FidgetMeshView` storage
/// that does not overlap the mesh. The mesh must not be freed during this call
/// or while any returned buffer is being read.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_get_view(
    mesh: *const FidgetMesh,
    out_view: *mut FidgetMeshView,
) -> i32 {
    if out_view.is_null() {
        return INVALID_ARGUMENT;
    }
    // SAFETY: the caller provides writable view storage.
    unsafe {
        out_view.write(FidgetMeshView {
            vertices: ptr::null(),
            vertex_count: 0,
            indices: ptr::null(),
            index_count: 0,
        });
    }
    // SAFETY: a non-null handle must be live by the caller's contract.
    let Some(mesh) = (unsafe { mesh.as_ref() }) else {
        return INVALID_ARGUMENT;
    };
    // SAFETY: the counts were checked during mesh construction.
    unsafe {
        out_view.write(FidgetMeshView {
            vertices: mesh.vertices.as_ptr().cast(),
            vertex_count: mesh.vertices.len() as u32,
            indices: mesh.indices.as_ptr(),
            index_count: mesh.indices.len() as u32,
        });
    }
    OK
}

/// Frees a mesh and invalidates its borrowed buffers. Null is a no-op.
///
/// # Safety
/// A non-null handle must come from this library and be freed exactly once.
/// No other thread may access the mesh or its buffers during or after this call.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn fidget_mesh_free(mesh: *mut FidgetMesh) {
    if !mesh.is_null() {
        // SAFETY: ownership is transferred back exactly once by the caller.
        unsafe { drop(Box::from_raw(mesh)) };
    }
}
