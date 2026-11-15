# Fidget Unity example

Open this directory with **Unity 6000.6.0f1** (Unity 6.6). It uses **URP 17.6.0**,
with the supplied pipeline assigned in Graphics and every Quality level. The
project includes the usual Unity packages and Unity Pipeline for Editor automation.

1. Let Package Manager resolve `com.icosa.fidget`. The pinned UPM distribution
   includes matching C# and native libraries for Windows, macOS, Linux, Android
   and iOS. No Rust build or separate native-library installation is required.
2. Open a scene in **Assets/FidgetExamples/Scenes** and press Play.
3. Start with **AnimatedBoolean**: a cylinder cuts a rounded box. Adjust its
   cutter radius, travel, speed, and detail on its single shape component.
4. **InteractiveParameters** has on-screen sliders for the torus's major and tube
   radii. They accept pointer/touch input and regenerate the mesh as values change.
5. **SharedShapeInstances** creates four `SharedSphere` components using
   `CreateSharedInstance`, then animates each component's `Radius` property.
6. **ChangingSphere** defines a sphere in `Define` and animates its `Radius`
   property. It uses a positions-only
   flat-shaded material.

All scenes default to detail 6; detail remains adjustable in the Inspector.
AnimatedBoolean and InteractiveParameters define shapes by deriving from
`FidgetMesh<T>` and overriding `Define`. Update their C# properties directly; the
component handles preparation, scheduling, and cleanup. SharedShapeInstances
uses the same component API and shares a definition explicitly through
`CreateSharedInstance`.

The three general API examples use ordinary URP Lit and `MeshNormals.Surface`.
Fidget's expression branches select surface gradients, keeping curved surfaces
smooth and splitting render vertices at differing normals. No crease-angle
threshold is used. Classification samples each triangle's centroid: triangles
that cross a branch boundary remain a limitation. See the package README for
API usage, ownership, and meshing settings.

Meshes are generated on worker jobs and applied through Unity MeshData. Each
stream keeps one request in flight; changing parameters does not build a queue
of obsolete frames.

## Source and packaging

`Assets/FidgetExamples` is the single source for scenes, scripts, rendering assets,
and sample documentation. `unity/package_unity.py` copies it into the published
UPM package's `Samples~/Examples` directory. There is no second maintained copy.
Other Unity projects can import **Examples** from Package Manager.

`Assets`, `Packages` and `ProjectSettings` are versioned. Caches, logs, generated
IDE files and player builds are ignored. Package runtime source lives under
`unity/com.icosa.fidget`; the `upm` distribution branch includes native binaries.
The manifest pins a published UPM commit to keep managed and native APIs aligned.

The default package includes **iOS device** libraries. For ARM64 iOS simulator
builds, use the `upm-simulator` distribution instead. Do not link the device and
simulator ARM64 archives together.

## Validation

On Windows with Unity 6000.6.0f1 / URP 17.6.0:

1. AnimatedBoolean's centered cut was visually accepted with expression-derived
   normals and ordinary URP Lit at depth 5.
2. InteractiveParameters generated a lit mesh and applied radius changes on
   demand; SharedShapeInstances generated all four independently sized meshes.
3. Live API checks covered parameter snapshots, independent streams, disposing
   the compiled shape while streams remain alive, depth changes with a stable
   Mesh, and disposal with a request in flight.

Native CI builds all eight desktop/mobile architectures. These checks do not
establish Unity player, IL2CPP, or device performance on every target platform.
