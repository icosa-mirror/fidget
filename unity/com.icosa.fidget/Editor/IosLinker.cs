#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Icosa.Fidget.Editor
{
    internal static class IosLinker
    {
        [PostProcessBuild(10)]
        private static void AddRustSystemLibraries(BuildTarget target, string buildPath)
        {
            if (target != BuildTarget.iOS) return;
            string path = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(path);
            // Rust 1.95 reports -lc -lm -liconv -lSystem. Xcode supplies the
            // standard C/System libraries; iconv must be linked explicitly.
            project.AddBuildProperty(project.GetUnityFrameworkTargetGuid(), "OTHER_LDFLAGS", "-liconv");
            project.WriteToFile(path);
        }
    }
}
#endif
