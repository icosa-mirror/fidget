using System;
using System.Collections.Generic;
using System.Reflection;
using Icosa.Fidget;
using UnityEngine;

namespace FidgetExamples.Editor
{
    internal static class ExampleControls
    {
        // Sample the same serialized fields Unity draws as Inspector sliders.
        internal static void Sweep(FidgetMesh mesh, Action<string> sample)
        {
            var controls = new List<(FieldInfo field, RangeAttribute range, object original)>();
            for (var t = mesh.GetType(); t != null && t != typeof(FidgetMesh); t = t.BaseType)
            {
                foreach (var field in t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    var range = field.GetCustomAttribute<RangeAttribute>();
                    if (range != null && (field.IsPublic || field.IsDefined(typeof(SerializeField))))
                        controls.Add((field, range, field.GetValue(mesh)));
                }
            }

            void Check(string label)
            {
                try { sample(label); }
                catch (Exception error)
                {
                    var values = controls.ConvertAll(c => c.field.Name.TrimStart('<').Split('>')[0] + "=" + c.field.GetValue(mesh));
                    throw new InvalidOperationException($"{mesh.name} ({string.Join(", ", values)}): {error.Message}", error);
                }
            }

            void Set(FieldInfo field, float value) => field.SetValue(mesh, Convert.ChangeType(value, field.FieldType));
            try
            {
                foreach (var (field, range, original) in controls)
                {
                    string label = field.Name.TrimStart('<').Split('>')[0];
                    foreach (float value in new[] { range.min, range.max })
                    {
                        Set(field, value);
                        Check(label);
                    }
                    field.SetValue(mesh, original);
                }
                if (controls.Count > 1)
                {
                    foreach (float fraction in new[] { 0f, 0.5f, 1f })
                    {
                        foreach (var (field, range, _) in controls)
                            Set(field, Mathf.Lerp(range.min, range.max, fraction));
                        Check("combined sliders");
                    }
                }
            }
            finally
            {
                foreach (var (field, _, original) in controls) field.SetValue(mesh, original);
            }
        }
    }
}
