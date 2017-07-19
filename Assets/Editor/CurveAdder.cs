using UnityEngine;
using UnityEditor;
using System.Collections;

using System.Linq;

public static class CurveAdder {

    /// <summary>
    /// Dumps curve data to log for selected clips
    /// </summary>
    [MenuItem("My Commands/Dump Curves")]
    public static void DumpCurves() 
    { 
        var clips = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.Assets).Cast<AnimationClip>();
        foreach (var clip in clips) 
        {
            Debug.Log(
            AnimationUtility.GetAllCurves(clip)
                .Select(binding => binding.type + " | " + binding.path + " : " + binding.propertyName)
                .Aggregate("Dumping clip: " + AssetDatabase.GetAssetPath(clip), 
                (acc, s) => acc + "\n" + s));
        }
    }

    /// <summary>
    /// Creates root motion curve from root node's position curve for selected clips
    /// </summary>
    [MenuItem("My Commands/Add root motion curve")]
    public static void AddMotionCurve()
    {
        var clips = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.Assets)
            .Cast<AnimationClip>();

        foreach (AnimationClip clip in clips)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);

            foreach (EditorCurveBinding sourceBinding in bindings)
            {
                if (sourceBinding.path != "")
                {
                    // We are only looking at the root component
                    continue;
                }

                var property = sourceBinding.propertyName;

                if (property.StartsWith("m_LocalPosition."))
                {
                    property = property.Replace("m_LocalPosition.", "MotionT.");
                }
                else
                {
                    // Not interested in this property
                    continue;
                }

                var binding = new EditorCurveBinding();
                binding.path = "";
                binding.type = typeof(Animator);
                binding.propertyName = property;

                var curve = AnimationUtility.GetEditorCurve(clip, sourceBinding);

                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }
        }
    }
}
