
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using Lib.Rm;

public static class ActorPostProcessor
{
    [MenuItem("Assets/Orca/Actor/Update Root Motion Data", true)]
    private static bool UpdateRootMotionDataValidate()
    {
        return null != Selection.activeGameObject.GetComponent<Animator>() /*&& (PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.ModelPrefab)*/;
    }

    [MenuItem("Assets/Orca/Actor/Update Root Motion Data", false, 13)]
    private static void UpdateRootMotionData()
    {
        var animator = Selection.activeGameObject.GetComponent<Animator>();

        if (null != animator && animator.hasRootMotion && animator.applyRootMotion)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string filename = System.IO.Path.GetFileNameWithoutExtension(path);
            string controllerpath = System.IO.Path.GetDirectoryName(path);

            string copycontrollerpath = string.Format("{0}/{1}.controller", controllerpath, filename);
            filename = filename.Replace("Mesh", "RM");

            CreateRootMotionData(copycontrollerpath, filename);
        }
    }

    private static void CreateRootMotionData(string copycontrollerpath, string rootMotionFileName)
    {
        // TODO(Mepi : 2017.07.06 루트모션
        // 애니메이터가 루트모션을 가지고 있다면 루트모션의 Position/Rotation 정보를 추출하여 Json파일로 저장.
        // 애니메이션은 30프레임 기준으로 만들어졌으나 게임내부 프레임에 따라 해당 프레임(targetFrame)에 맞춰 정보를 저장하도록 했음.
        // 게임 내부 프레임을 얼마로 할지 결정되면 아래 targetFrame의 수치가 결정됩니다. 되도록 외부에서 해당 수치를 기록 할수 있도록 해야할듯..
        var runtimeAnimatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(copycontrollerpath, typeof(AnimatorController));
        if (null == runtimeAnimatorController)
            return;

        RMUnityJsonData RootMotionData = new RMUnityJsonData();

        GetAllAnimationClipRootMotion(runtimeAnimatorController, ref RootMotionData);

        SaveToJsonRootMotion(RootMotionData, rootMotionFileName);
    }

    private static void GetAllAnimationClipRootMotion(AnimatorController runtimeAnimatorController, ref RMUnityJsonData rootMotionData)
    {
        // !! 변경 가능한 데이터 외부로 빼고 싶다.
        int targetFrame = 50;

        Dictionary<string, string> DicAnimationStateName = new Dictionary<string, string>();
        var layer = runtimeAnimatorController.layers[0];
        GetAnamationStateName(layer.stateMachine, ref DicAnimationStateName);

#if DEBUG_FILE_LOG
        var sb = new System.Text.StringBuilder();
        foreach (var aa in DicAnimationStateName)
        {
            var str = string.Format("{0} : {1}", aa.Key, aa.Value);
            sb.AppendLine(str);
        }

        Lib.Fd.FileLog.Log("AnimationStateNameList.txt", sb.ToString());
#endif

        foreach (AnimationClip ac in runtimeAnimatorController.animationClips)
        {
            float framePerTime = 1.0f / targetFrame;
            EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetCurveBindings(ac);
            List<AnimationCurve> curveDatas = new List<AnimationCurve>();

            for (int itr = 0; itr < editorCurveBindings.Length; ++itr)
            {
                curveDatas.Add(AnimationUtility.GetEditorCurve(ac, editorCurveBindings[itr]));
            }

            if (curveDatas.Count == 0)
                continue;

            string stateName = "";
            string clipName = "";
            var next = DicAnimationStateName.GetEnumerator();
            while (next.MoveNext())
            {
                if (next.Current.Value.Equals(ac.name))
                {
                    stateName = next.Current.Key;
                    clipName = next.Current.Value;
                    break;
                }
            }
            DicAnimationStateName.Remove(stateName);

            RMUnityJsonClipData clipData = new RMUnityJsonClipData(stateName, clipName);
            float i = 0.0f;
            for (int itr = 0; i <= ac.length; i = framePerTime * ++itr)
            {
                Vector3 pos = new Vector3(curveDatas[0].Evaluate(i),
                                          curveDatas[1].Evaluate(i),
                                          curveDatas[2].Evaluate(i));

                Quaternion rot = new Quaternion(curveDatas[3].Evaluate(i),
                                                curveDatas[4].Evaluate(i),
                                                curveDatas[5].Evaluate(i),
                                                curveDatas[6].Evaluate(i));

                clipData.AddFrameData(pos, rot);
            }

            if (rootMotionData.AddClips(stateName, clipData))
            {
            }
            else
            {
                Debug.LogError("Contains key : " + stateName);
            }
        }
    }

    private static void GetAnamationStateName(AnimatorStateMachine animatorStateMachine, ref Dictionary<string, string> dicAnimationStateName)
    {
        // [Main] State
        AddChildStateMachine(animatorStateMachine.states, ref dicAnimationStateName);

        // [Main] Sub-State
        foreach (var machine in animatorStateMachine.stateMachines)
        {
            AddChildStateMachine(machine.stateMachine.states, ref dicAnimationStateName);
        }
    }

    private static void AddChildStateMachine(ChildAnimatorState[] childAnimatorState, ref Dictionary<string, string> dicAnimationStateName)
    {
        foreach (var childState in childAnimatorState)
        {
            if (null != childState.state.motion)
            {
                var stateName = childState.state.name;
                var motionName = childState.state.motion.name;
                dicAnimationStateName.Add(stateName, motionName);
            }
        }
    }

    private static void SaveToJsonRootMotion(RMUnityJsonData rootMotionData, string rootMotionFileName)
    {
        string ResourcesPath = "Assets/Resources/";
        string RMPathFormat = "RootMotion/{0}/";
        string RMJsonFileFormat = "{0}.json";

        var sb = new System.Text.StringBuilder();
        sb.Append(ResourcesPath);
        sb.AppendFormat(RMPathFormat, rootMotionFileName);

        string directory = sb.ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        sb.AppendFormat(RMJsonFileFormat, rootMotionFileName);
        var assetPath = sb.ToString();
        if (File.Exists(assetPath))
        {
            File.Delete(assetPath);
        }

        var jsonData = JsonUtility.ToJson(rootMotionData, false);
        using (TextWriter writer = new StreamWriter(assetPath))
        {
            writer.WriteLine(jsonData);

            writer.Close();
        }
        AssetDatabase.ImportAsset(assetPath);
        AssetDatabase.Refresh();
    }
}
