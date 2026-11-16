Run the graph checks with .NET 8 and a Unity 6 Editor installation:

```sh
DYLD_LIBRARY_PATH="$PWD/target/native-artifacts/aarch64-apple-darwin" \
  dotnet run --project unity/tests/ShapeLibraryChecks.csproj \
  -p:UnityManagedPath=/Applications/Unity/Hub/Editor/6000.6.0f1/Unity.app/Contents/Resources/Scripting/Managed
```

Build the matching ABI 5 native library first with `fidget-native/build.py`.
On Linux set `LD_LIBRARY_PATH`; on Windows put the native DLL on `PATH`.
`UnityManagedPath` is the directory containing the `UnityEngine` assembly folder.
The checks compile the production builder sources against real Unity types,
evaluate their graphs at known points, and compile the result through the C ABI.
They do not establish Unity worker/rendering or player/device compatibility.
