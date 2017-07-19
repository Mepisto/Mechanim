using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class RootMotionPostProcessor
{
    public static Dictionary<string, List<RootMotionData>> DicRootMotionData = new Dictionary<string, List<RootMotionData>>();

    //[MenuItem("Assets/Orca/Actor/Create Root Motion Data", false, 0)]
    private static void GetAnimationClipData()
    {
        DicRootMotionData.Clear();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string controllerpath = System.IO.Path.GetDirectoryName(path);
        string copycontrollerpath = controllerpath;
        string filename = System.IO.Path.GetFileNameWithoutExtension(path);
        controllerpath += "/" + filename + ".controller";
        copycontrollerpath += "/" + filename + " - Copy.controller";

        var runtimeAnimatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(controllerpath, typeof(AnimatorController));

        if (null == runtimeAnimatorController)
            return;

        foreach (AnimationClip ac in runtimeAnimatorController.animationClips)
        {
            string dicKey = ac.name;
            EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetCurveBindings(ac);
            List<AnimationCurve> curveDatas = new List<AnimationCurve>();

            var rootMotionDatas = new List<RootMotionData>();
            for (int i = 0; i < editorCurveBindings.Length; ++i)
            {
                curveDatas.Add(AnimationUtility.GetEditorCurve(ac, editorCurveBindings[i]));
            }

            var index = -1;
            var frame = 1.0f / 50f;// _baseFrame.intValue;
            for (float i = 0.0f; i <= ac.length; i += frame)
            {
                ++index;
                Vector3 pos = new Vector3(curveDatas[0].Evaluate(i),
                                            curveDatas[1].Evaluate(i),
                                            curveDatas[2].Evaluate(i));

                Quaternion rot = new Quaternion(curveDatas[3].Evaluate(i),
                                                curveDatas[4].Evaluate(i),
                                                curveDatas[5].Evaluate(i),
                                                curveDatas[6].Evaluate(i));

                var rm = new RootMotionData()
                {
                    RootPosition = pos,
                    RootRotation = rot
                };
                rootMotionDatas.Add(rm);
            }

            if (false == DicRootMotionData.ContainsKey(dicKey))
            {
                DicRootMotionData.Add(dicKey, rootMotionDatas);
            }
        }
    }
}
