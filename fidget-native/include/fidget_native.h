#ifndef FIDGET_NATIVE_H
#define FIDGET_NATIVE_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

/* All functions use the platform C calling convention (cdecl on Windows). */
typedef struct FidgetMesh FidgetMesh;

typedef struct FidgetMeshView {
    const float *vertices; /* Packed XYZ, vertex_count * 3 floats. */
    uint32_t vertex_count;
    const uint32_t *indices; /* index_count entries, three per triangle. */
    uint32_t index_count;
} FidgetMeshView;

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

/* Borrowed immutable buffers, valid only while mesh remains live.
 * Do not free these buffers separately or modify them.
 * out_view must point to writable storage; a NULL mesh clears the view.
 * Calls on independent meshes may run concurrently. Do not free a mesh while
 * another thread uses its handle or buffers. Counts and indices are uint32_t.
 */
int32_t fidget_mesh_get_view(const FidgetMesh *mesh, FidgetMeshView *out_view);

/* NULL is permitted. Double-free and stale handles are invalid. */
void fidget_mesh_free(FidgetMesh *mesh);

#ifdef __cplusplus
}
#endif

#endif
