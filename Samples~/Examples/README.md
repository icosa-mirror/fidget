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
3. Open Scenes/SharedShapeInstances.unity to see one compiled expression drive
   several objects with independent parameters and mesh streams.
4. Open Scenes/ChangingSphere.unity for the minimal sphere stream and a
   positions-only flat-shaded material.

AnimatedBoolean and InteractiveParameters derive from `FidgetMesh<T>`. Each
component defines its expression in `Define` and exposes ordinary C# properties
such as `CutterOffset` and `MajorRadius`. No initialization or compilation calls
are required in these examples. SharedShapeInstances demonstrates the advanced
API for binding four model instances to one compiled definition.

The lit examples request expression-derived surface normals, with hard transitions
selected from Fidget's branch traces. See the package README for face-classification
limitations. The included URP Flat Preview shader
is an alternative that needs only positions and indices.

The repository's examples/unity project already has this URP pipeline configured.
Its Assets/FidgetExamples folder is the source for these packaged samples.
