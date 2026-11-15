#include "fidget_native.h"
#include <math.h>
#include <stddef.h>
#include <stdio.h>
#include <stdlib.h>

/* Keep checks active in Release builds. */
#define CHECK(condition) do { \
    if (!(condition)) { \
        fprintf(stderr, "Failed at line %d: %s\n", __LINE__, #condition); \
        return 1; \
    } \
} while (0)

static int parameterized_shape(void) {
    FidgetNode nodes[] = {
        {FIDGET_X,0,0,0,0,0}, {FIDGET_Y,0,0,0,0,0}, {FIDGET_Z,0,0,0,0,0},
        {FIDGET_CONSTANT,0,0,0,0,2}, {FIDGET_SUBTRACT,0,3,0,0,0},
        {FIDGET_SQUARE,4,0,0,0,0}, {FIDGET_SQUARE,1,0,0,0,0},
        {FIDGET_SQUARE,2,0,0,0,0}, {FIDGET_ADD,5,6,0,0,0},
        {FIDGET_ADD,8,7,0,0,0}, {FIDGET_SQRT,9,0,0,0,0},
        {FIDGET_PARAMETER,0,0,0,0,0}, {FIDGET_SUBTRACT,10,11,0,0,0}
    };
    FidgetShape *shape = NULL;
    FidgetMesher *a = NULL, *b = NULL;
    FidgetBounds domain = {{0,-2,-2}, {4,2,2}};
    CHECK(fidget_shape_compile(nodes, 13, 1, &shape) == FIDGET_OK);
    CHECK(fidget_mesher_create(shape, &a) == FIDGET_OK);
    CHECK(fidget_mesher_create(shape, &b) == FIDGET_OK);
    fidget_shape_free(shape); /* Each mesher must retain the compiled shape. */
    for (int i = 0; i < 3; ++i) {
        float radius = i == 1 ? 0.35f : 0.7f;
        FidgetMesh *mesh = NULL;
        FidgetMeshCounts counts;
        FidgetBounds bounds;
        CHECK(fidget_mesher_generate(i == 1 ? b : a, &radius, 1, &domain, 5, &mesh) == FIDGET_OK);
        if (i == 2) CHECK(fidget_mesh_prepare_surface_normals(a, mesh) == FIDGET_OK);
        CHECK(fidget_mesh_get_counts(mesh, &counts) == FIDGET_OK);
        CHECK(counts.vertex_count > 0 && counts.index_count > 0);
        float *vertices = malloc(counts.vertex_count * 3 * sizeof(float));
        float *normals = malloc(counts.vertex_count * 3 * sizeof(float));
        uint32_t *indices = malloc(counts.index_count * sizeof(uint32_t));
        CHECK(vertices && normals && indices);
        CHECK(fidget_mesh_copy_to(mesh, vertices, counts.vertex_count, indices, counts.index_count, &bounds) == FIDGET_OK);
        CHECK(fidget_mesh_write_normals(mesh, normals, counts.vertex_count) == FIDGET_OK);
        CHECK(fabsf((bounds.min[0] + bounds.max[0]) * 0.5f - 2) < 0.03f);
        CHECK(fabsf(bounds.max[0] - bounds.min[0] - 2 * radius) < 0.08f);
        for (uint32_t v = 0; v < counts.vertex_count; ++v) {
            float *n = normals + 3 * v;
            CHECK(fabsf(n[0]*n[0] + n[1]*n[1] + n[2]*n[2] - 1) < 0.001f);
            CHECK(n[0]*(vertices[v*3]-2) + n[1]*vertices[v*3+1] + n[2]*vertices[v*3+2] > 0);
        }
        free(vertices); free(normals); free(indices); fidget_mesh_free(mesh);
    }
    FidgetMesh *invalid = NULL;
    float value = NAN;
    CHECK(fidget_mesher_generate(a, &value, 1, &domain, 5, &invalid) == FIDGET_INVALID_ARGUMENT);
    CHECK(invalid == NULL);
    CHECK(fidget_mesher_generate(a, &value, 0, &domain, 5, &invalid) == FIDGET_INVALID_ARGUMENT);
    fidget_mesher_free(a); fidget_mesher_free(b);
    nodes[0].opcode = FIDGET_ADD; /* Forward references must be rejected. */
    nodes[0].a = 12;
    CHECK(fidget_shape_compile(nodes, 13, 1, &shape) == FIDGET_INVALID_ARGUMENT);
    CHECK(shape == NULL);
    return 0;
}

int main(void) {
    CHECK(parameterized_shape() == 0);
    FidgetMesh *mesh = NULL;
    FidgetMeshCounts counts = {0};
    FidgetBounds bounds = {0};
    CHECK(fidget_native_abi_version() == 4);
    CHECK(fidget_mesh_sphere(0.6f, 4, NULL) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_sphere(NAN, 4, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(mesh == NULL);
    CHECK(fidget_mesh_sphere(1.0f, 4, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_sphere(0.6f, 0, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_sphere(0.6f, 9, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_get_counts(NULL, &counts) == FIDGET_INVALID_ARGUMENT);
    CHECK(counts.vertex_count == 0 && counts.index_count == 0);
    fidget_mesh_free(NULL);

    /* Exercise allocation, ABI layout, geometry and cleanup more than once. */
    for (int iteration = 0; iteration < 3; ++iteration) {
        CHECK(fidget_mesh_sphere(0.6f, 4, &mesh) == FIDGET_OK);
        CHECK(mesh != NULL);
        CHECK(fidget_mesh_get_counts(mesh, NULL) == FIDGET_INVALID_ARGUMENT);
        CHECK(fidget_mesh_get_counts(mesh, &counts) == FIDGET_OK);
        CHECK(counts.vertex_count > 0 && counts.index_count > 0);
        CHECK(counts.index_count % 3 == 0);
        float *vertices = malloc((counts.vertex_count * 3 + 1) * sizeof(float));
        uint32_t *indices = malloc((counts.index_count + 1) * sizeof(uint32_t));
        CHECK(vertices != NULL && indices != NULL);
        vertices[0] = -999.0f;
        vertices[counts.vertex_count * 3] = 123.0f;
        indices[counts.index_count] = 456;
        CHECK(fidget_mesh_copy_to(mesh, vertices, counts.vertex_count - 1,
              indices, counts.index_count, &bounds) == FIDGET_INVALID_ARGUMENT);
        CHECK(vertices[0] == -999.0f);
        CHECK(fidget_mesh_copy_to(mesh, vertices, counts.vertex_count,
              indices, counts.index_count, &bounds) == FIDGET_OK);
        CHECK(vertices[counts.vertex_count * 3] == 123.0f);
        CHECK(indices[counts.index_count] == 456);
        fidget_mesh_free(mesh);
        mesh = NULL; /* Copies must survive native handle cleanup. */
        for (uint32_t i = 0; i < counts.vertex_count; ++i) {
            const float *v = &vertices[i * 3];
            float radius = sqrtf(v[0]*v[0] + v[1]*v[1] + v[2]*v[2]);
            for (int axis = 0; axis < 3; ++axis) {
                CHECK(v[axis] >= bounds.min[axis] && v[axis] <= bounds.max[axis]);
            }
            CHECK(isfinite(radius) && fabsf(radius - 0.6f) < 0.05f);
        }
        for (uint32_t i = 0; i < counts.index_count; ++i) {
            CHECK(indices[i] < counts.vertex_count);
        }
        if (iteration == 0) {
            printf("Sphere: %u vertices, %u triangles\n",
                   (unsigned)counts.vertex_count, (unsigned)(counts.index_count / 3));
        }
        free(vertices);
        free(indices);
    }
    return 0;
}
