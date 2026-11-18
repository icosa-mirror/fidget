# Fidget Unity examples

Open this directory with **Unity 6000.6.0f1** (Unity 6.6), using **URP 17.6.0**.
Let Package Manager resolve the pinned `com.icosa.fidget` distribution, then open
an example under **Assets/FidgetExamples/Scenes** and press Play. Matching native
libraries ship in the package; no Rust build is required for package consumers.

## Finding an example

Scenes and recipe scripts are organized by capability:

| Folder | Examples |
| --- | --- |
| Basics | Primitives and an animated sphere |
| Parameters | On-screen controls that update bound properties |
| Sharing | Independent instances sharing a compiled definition |
| Solids | Capsules, cones, rings, prisms, polyhedra, rounded solids and surfaces |
| Profiles | Rectangles, triangles, polygons, stars, lenses, curves and decorative profiles |
| Construction | Extrusion, revolution, lofting and blended extrusion |
| Combinations | Animated subtraction, smooth/rounded/chamfered joins, columns, stairs and surface features |
| Transforms | Twist, bend, displacement, elongation, rounding and shells |
| Repetition | Axes, grids, finite intervals, mirrors, folds and polar copies |

Start with `Basics/Primitives` and `Combinations/AnimatedSubtraction`.
`Parameters/InteractiveParameters` has pointer/touch sliders for a torus.
`Sharing/SharedDefinitions` uses `CreateSharedInstance` and animates independent
radius properties.

In `Repetition/Polar`, compare the single vent slot with the two patterned rotors.
In Play mode, select either repeated rotor and change **Count** to set how many
slots are cut around its local axis, from 1 to 16. The bore, thickness, slot
length, width and radial position are editable on all three rotors.

Feature scenes show a small set of related recipes. **Each displayed recipe has
its own mesh and fitted local sampling bounds.** GameObject transforms arrange
the display; unrelated solids are never unioned merely to fit them into one
component. Repetition and CSG examples combine geometry only when that is the
operation being demonstrated. Thin extrusions make the XY profiles visible.

Each recipe derives from `FidgetMesh<T>` and overrides `Define`. Parameterized
recipes bind their Inspector properties with `shape.Parameter`. Changing a property
updates its mesh without rebuilding the expression. Detail defaults to 6 (7 for columns and stairs) and is
adjustable. Bounds cover a sampled sweep of the demonstrated parameter range,
including individual endpoints and combined settings. For example, the box exposes
three half extents, the rounded box adds corner radius, and the regular star
exposes point count, indentation, radius and extrusion depth. When extending a
recipe beyond its tested range, adjust its Bounds and rebuild.

The public modelling API is `ShapeBuilder`, documented in the package's
`SdfModelling.md`. Mercury and IQ are implementation sources and attributions,
not separate public APIs or example categories. Their notices ship in
`ThirdPartyNotices.md`.

## Source and packaging

`Assets/FidgetExamples` is the single source for sample scripts, scenes, rendering
assets and scene tools. `unity/package_unity.py` copies it into the published
package's `Samples~/Examples`. Import **Examples** from Package Manager in any
other Unity project. Runtime code lives in `unity/com.icosa.fidget`.

The manifest pins a published UPM commit. The default distribution includes iOS
device libraries; use `upm-simulator` for ARM64 simulator builds. Do not link both
ARM64 archives together.

For local development, run from the repository root:

```sh
python3 fidget-native/build.py --target aarch64-apple-darwin --smoke
python3 unity/package_unity.py --artifacts target/native-artifacts --targets aarch64-apple-darwin --output target/unity-package/com.icosa.fidget
```

Install that package directory through Package Manager. Restore the published
manifest pin before committing; local absolute paths are not distributable.

## Validation and scene tools

`FidgetExamples.Editor.ExampleScenes.Create("Solids/Cones")` recreates a feature
scene using Unity's scene API and fits each recipe's bounds across its parameter
range. **Fidget > Create Feature Examples** recreates all feature scenes; close
any feature scene you intend to recreate first. The four introductory scenes are
maintained separately so their UI and animation remain focused.

`FidgetExamples.Editor.ExampleValidation.Run("Solids/Cones")` checks nonempty
meshes, finite vertices and surface normals, parameter updates without graph
rebuilds, and margins inside the saved bounds. It also saves a rendered preview
under `target/example-checks` when run in this repository. The checks sample the
exposed parameter range at five points; they are not a proof for arbitrary inputs.

`Icosa.Fidget.Editor.IntegrationCheck.RunModelling()` additionally checks Unity
matrix transforms and worker output. The .NET checks cover the unified builder
API and comparisons with the original formulas. Native CI builds all eight
architectures. Local Unity checks use macOS ARM64; they do not establish player,
IL2CPP or device performance for every target.
