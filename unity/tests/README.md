Run the graph checks with .NET 8 and a Unity 6 Editor installation:

```sh
DYLD_LIBRARY_PATH="$PWD/target/native-artifacts/aarch64-apple-darwin" \
  dotnet run --project unity/tests/ShapeLibraryChecks.csproj \
  -p:UnityManagedPath=/Applications/Unity/Hub/Editor/6000.6.0f1/Unity.app/Contents/Resources/Scripting/Managed
```

Build the matching ABI 8 native library first with `fidget-native/build.py`.
On Linux set `LD_LIBRARY_PATH`; on Windows put the native DLL on `PATH`.
`UnityManagedPath` is the directory containing the `UnityEngine` assembly folder.
The checks compile the production builder sources against real Unity types,
evaluate their graphs at known points, and compile the result through the C ABI.
They do not establish Unity worker/rendering or player/device compatibility.

Mercury checks cover every exported overload, including mutated domain coordinates
and cell IDs, against the original numeric GLSL formulas. The pinned reference (with whitespace normalized)
source is `hg-sdf/hg_sdf.glsl`; regenerate the numeric C# oracle with
`python3 unity/tests/hg-sdf/generate_reference.py`. Its comments retain Mercury's
copyright and MIT notice. The generator changes syntax/types, not the formulas.

IQ checks cover all 102 computational overloads available to the standalone
harness, native graph compilation, independent primitive distances, numerical
ellipse/Bezier closest-point checks, and six native meshes with surface normals.
The fixed seed keeps results reproducible. Unity's matrix-based `opTx` is checked
in the Editor probe. These checks validate the port, not universal conditioning
of every source formula or device/player compatibility.
