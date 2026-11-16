#ifndef FIDGET_NATIVE_H
#define FIDGET_NATIVE_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

/* All functions use the platform C calling convention (cdecl on Windows). */
typedef struct FidgetMesh FidgetMesh;
typedef struct FidgetShape FidgetShape;
typedef struct FidgetMesher FidgetMesher;

/* ABI v6 adds expression math and conditionals for hg_sdf. */
typedef struct FidgetNode {
    uint32_t opcode, a, b, c, d;
    float value;
} FidgetNode;

enum FidgetOpcode {
    FIDGET_CONSTANT = 0, FIDGET_X = 1, FIDGET_Y = 2, FIDGET_Z = 3,
    FIDGET_PARAMETER = 4, FIDGET_NEGATE = 5, FIDGET_ABS = 6,
    FIDGET_SQRT = 7, FIDGET_SQUARE = 8, FIDGET_SIN = 9, FIDGET_COS = 10,
    FIDGET_ADD = 11, FIDGET_SUBTRACT = 12, FIDGET_MULTIPLY = 13,
    FIDGET_DIVIDE = 14, FIDGET_MIN = 15, FIDGET_MAX = 16, FIDGET_REMAP = 17,
    FIDGET_MODULO = 18, FIDGET_FLOOR = 19, FIDGET_ATAN2 = 20,
    FIDGET_COMPARE = 21, FIDGET_AND = 22, FIDGET_OR = 23,
    FIDGET_NOT = 24, FIDGET_EXP = 25, FIDGET_LN = 26
};

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

/* Instructions are topologically ordered; the final node is the root.
 * CONSTANT uses value; PARAMETER uses a as its zero-based parameter index.
 * Unary/binary operators use a / a,b. REMAP uses a=shape,b=x,c=y,d=z.
 * Every node reference must refer to an earlier instruction. Limits: 1 million
 * nodes and 4096 parameters. Inputs are borrowed only for this call.
 * out_shape is cleared on failure. Free the returned handle exactly once.
 */
int32_t fidget_shape_compile(const FidgetNode *nodes, uint32_t count,
    uint32_t parameter_count, FidgetShape **out_shape);
void fidget_shape_free(FidgetShape *shape);

/* Meshers retain the compiled shape, so shape may be freed after creation.
 * Different meshers may execute concurrently. Each mesher is single-caller:
 * never generate/free it concurrently. No managed callbacks or JIT are used.
 */
int32_t fidget_mesher_create(const FidgetShape *shape, FidgetMesher **out_mesher);
int32_t fidget_mesher_generate(FidgetMesher *mesher, const float *values,
    uint32_t count, const FidgetBounds *bounds, uint32_t depth, FidgetMesh **out_mesh);
void fidget_mesher_free(FidgetMesher *mesher);

/* Optional area-weighted smooth normals: capacity counts vertices, not floats.
 * Output contains three floats per vertex. No additional allocation occurs.
 */
int32_t fidget_mesh_write_normals(const FidgetMesh *mesh, float *normals, uint32_t capacity);

/* Optional expression-derived normals. Uses the trace selected at each face's
 * centroid, then evaluates that branch's gradient at its corners. Vertices with
 * different normals are split, without changing geometry. No crease threshold.
 * This is recovered face classification, not mesh-exported surface provenance.
 * Call before querying counts; then write_normals copies the prepared normals.
 * The mesh must come from this mesher's most recent generate call. On failure,
 * free the mesh; it has been consumed. No concurrent use of either handle.
 */
int32_t fidget_mesh_prepare_surface_normals(FidgetMesher *mesher, FidgetMesh *mesh);

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
