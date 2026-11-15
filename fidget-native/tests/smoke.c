#include "fidget_native.h"
#include <math.h>
#include <stddef.h>
#include <stdio.h>

/* Keep checks active in Release builds. */
#define CHECK(condition) do { \
    if (!(condition)) { \
        fprintf(stderr, "Failed at line %d: %s\n", __LINE__, #condition); \
        return 1; \
    } \
} while (0)

int main(void) {
    FidgetMesh *mesh = NULL;
    FidgetMeshView view = {0};
    CHECK(fidget_native_abi_version() == 1);
    CHECK(fidget_mesh_sphere(0.6f, 4, NULL) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_sphere(NAN, 4, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(mesh == NULL);
    CHECK(fidget_mesh_sphere(1.0f, 4, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_sphere(0.6f, 0, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_sphere(0.6f, 9, &mesh) == FIDGET_INVALID_ARGUMENT);
    CHECK(fidget_mesh_get_view(NULL, &view) == FIDGET_INVALID_ARGUMENT);
    CHECK(view.vertices == NULL && view.indices == NULL);
    CHECK(view.vertex_count == 0 && view.index_count == 0);
    fidget_mesh_free(NULL);

    /* Exercise allocation, ABI layout, geometry and cleanup more than once. */
    for (int iteration = 0; iteration < 3; ++iteration) {
        CHECK(fidget_mesh_sphere(0.6f, 4, &mesh) == FIDGET_OK);
        CHECK(mesh != NULL);
        CHECK(fidget_mesh_get_view(mesh, NULL) == FIDGET_INVALID_ARGUMENT);
        CHECK(fidget_mesh_get_view(mesh, &view) == FIDGET_OK);
        CHECK(view.vertices != NULL && view.indices != NULL);
        CHECK(view.vertex_count > 0 && view.index_count > 0);
        CHECK(view.index_count % 3 == 0);
        for (uint32_t i = 0; i < view.vertex_count; ++i) {
            const float *v = &view.vertices[i * 3];
            float radius = sqrtf(v[0]*v[0] + v[1]*v[1] + v[2]*v[2]);
            CHECK(isfinite(radius) && fabsf(radius - 0.6f) < 0.05f);
        }
        for (uint32_t i = 0; i < view.index_count; ++i) {
            CHECK(view.indices[i] < view.vertex_count);
        }
        if (iteration == 0) {
            printf("Sphere: %u vertices, %u triangles\n",
                   (unsigned)view.vertex_count, (unsigned)(view.index_count / 3));
        }
        fidget_mesh_free(mesh);
        mesh = NULL;
    }
    return 0;
}
