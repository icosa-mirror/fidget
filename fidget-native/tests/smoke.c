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

int main(void) {
    FidgetMesh *mesh = NULL;
    FidgetMeshCounts counts = {0};
    FidgetBounds bounds = {0};
    CHECK(fidget_native_abi_version() == 2);
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
