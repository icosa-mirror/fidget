# Examples

Import **Examples** from Package Manager. The Rendering folder contains a URP
pipeline asset, renderer, and materials. In a project without a configured URP
pipeline, assign Rendering/FidgetURP.asset in Project Settings > Graphics and
ensure Quality settings do not override it with a different pipeline.

1. Open Scenes/AnimatedBoolean.unity and press Play. Change the cutter radius,
   travel, speed, and FidgetMesh detail in the Inspector.
2. Open Scenes/InteractiveParameters.unity and use the on-screen major-radius and
   tube-radius sliders. Their persistent events update the shape in Play mode.
   The controls use Unity UI and the Input System UI module; install those packages
   alongside URP when importing the samples into another project.
3. Open Scenes/SharedShapeInstances.unity to see its nested `SharedSphere` components created
   with `CreateSharedInstance`, sharing one definition with independent radii.
4. Open Scenes/ShapeLibrary.unity for four standard-library demonstrations: extrusion
   and smooth blend, loft and arbitrary-axis rotation, revolution and reflection,
   and scaled repetition. Press Play and edit the
   properties on ExtrudedBlend, LoftedProfile, RevolvedProfile, and RepeatedShape
   in the Inspector. Each follows the same `FidgetMesh<T>` / `Define` pattern as
   AnimatedBoolean. The scene requires
   the extended wrapper with ABI 5 native libraries; older published packages
   must be updated before importing these scripts.
5. Open Scenes/ChangingSphere.unity for a minimal `FidgetMesh<ChangingSphere>`
   component that animates its radius, using a positions-only flat-shaded material.

AnimatedBoolean and InteractiveParameters derive from `FidgetMesh<T>`. Each
component defines its expression in `Define` and exposes ordinary C# properties
such as `CutterOffset` and `MajorRadius`. No initialization or compilation calls
are required in these examples. SharedShapeInstances creates shared components,
positions them, and animates their `Radius` properties. Sharing and stream
lifetimes are handled by the runtime.

The lit examples request expression-derived surface normals, with hard transitions
selected from Fidget's branch traces. See the package README for face-classification
limitations. The included URP Flat Preview shader
is an alternative that needs only positions and indices.

The repository's examples/unity project already has this URP pipeline configured.
Its Assets/FidgetExamples folder is the source for these packaged samples.

The Shape Library scene can be recreated with **Fidget > Create Shape Library Sample**.
Its profiles compile once; property changes update bound parameters. The repeated
field is clipped to a box before meshing so its output stays inside explicit bounds.

The ShapeLibrary sample assemblies require package version 0.4.0 or later. With
the older pinned package, Unity skips these assemblies so the existing scenes
continue to compile. Publish/install the matching ABI 5 package before opening
ShapeLibrary.unity.
