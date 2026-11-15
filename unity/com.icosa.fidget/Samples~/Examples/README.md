# Examples

Import this sample from Package Manager. Each script builds its expression once
and supplies its MeshRenderer through FidgetMesh or MeshStream.

1. AnimatedBoolean: add to a GameObject, assign a URP Lit material, and press Play.
2. InteractiveParameters: add to a GameObject with a URP Lit material; adjust its
   Inspector sliders in Play mode or connect UI Slider events to the setters.
3. SharedShapeInstances: add to an empty GameObject and assign its Material field.

All three request smooth vertex normals. Configure URP in the consuming project.
The repository's examples/unity project includes ready-to-run scenes and materials.
