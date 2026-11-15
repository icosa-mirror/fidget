#ifndef FIDGET_NATIVE_H
#define FIDGET_NATIVE_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

/* All functions use the platform C calling convention (cdecl on Windows). */
typedef struct FidgetMesh FidgetMesh;

/* ABI v2 replaces borrowed packed buffers with caller-owned destinations. */
typedef struct FidgetMeshCounts {
    uint32_t vertex_count;
    uint32_t index_count;
} FidgetMeshCounts;

typedef struct FidgetBounds {
    float min[3];
    float max[3];
} FidgetBounds;

enum FidgetStatus {
    FIDGET_OK = 0,
    FIDGET_INVALID_ARGUMENT = 1,
    FIDGET_MESH_FAILED = 2,
    FIDGET_PANIC = 3
};

uint32_t fidget_native_abi_version(void);

/* Synchronous single-threaded probe. Fixed domain [-1, 1]^3.
 * Requires finite 0 < radius < 1 and 1 <= depth <= 8.
 * out_mesh must point to writable pointer storage; it is set to NULL on error.
 * A successful handle must be released exactly once with fidget_mesh_free.
 * Rust panics return FIDGET_PANIC; process aborts/OOM cannot be recovered.
 */
int32_t fidget_mesh_sphere(float radius, uint32_t depth, FidgetMesh **out_mesh);

/* Reads counts without allocating or copying; clears counts on NULL mesh.
 * counts must point to writable storage. The handle must remain live.
 */
int32_t fidget_mesh_get_counts(const FidgetMesh *mesh, FidgetMeshCounts *counts);

/* Writes directly into caller-owned buffers (e.g. Unity writable MeshData).
 * vertices: vertex_capacity * 3 writable floats; indices: index_capacity uint32s.
 * bounds: writable output, computed in the position-copy pass.
 * No intermediate packing allocation. Buffers must be aligned and non-overlapping.
 * Invalid arguments leave all outputs untouched. Data pointers may be NULL only
 * for empty outputs. Empty meshes have zero bounds. Never free the mesh while
 * copying. Caller buffers remain valid after fidget_mesh_free.
 */
int32_t fidget_mesh_copy_to(const FidgetMesh *mesh,
    float *vertices, uint32_t vertex_capacity,
    uint32_t *indices, uint32_t index_capacity, FidgetBounds *bounds);

/* NULL is permitted. Double-free and stale handles are invalid. */
void fidget_mesh_free(FidgetMesh *mesh);

#ifdef __cplusplus
}
#endif

#endif
