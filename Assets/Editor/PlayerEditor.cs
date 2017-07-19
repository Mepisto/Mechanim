using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.Animations;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    private Player m_player;

    private Animator anim;

    private SerializedObject _serObj;

    private SerializedProperty _baseFrame;

    private SerializedProperty _replay;

    //private SerializedProperty _rootPosList;

    //private SerializedProperty _rootRotList;       

    private SerializedProperty _index;

    private SerializedProperty _dicRootMotionData;

    private bool m_foldPosition = false;

    private bool m_foldRotation = false;

    private int index;

    private void OnEnable()
    {
        m_player = target as Player;

        if (null != m_player)
            anim = m_player.GetComponent<Animator>();
        
        _serObj = new SerializedObject(target);

        _baseFrame = _serObj.FindProperty("BaseFrame");

        _replay = _serObj.FindProperty("Replay");

        //_rootPosList = _serObj.FindProperty("RootPosList");

        //_rootRotList = _serObj.FindProperty("RootRotList");

        _index = _serObj.FindProperty("Index");
    }

    public override void OnInspectorGUI()
    {
        _serObj.Update();

        EditorGUILayout.BeginVertical("box");
        {
            //EditorGUI.indentLevel++;
            //m_foldPosition = EditorGUILayout.Foldout(m_foldPosition, new GUIContent("Root Motion Position"));
            //if (m_foldPosition)
            //{
            //    for (int i = 0; i < _rootPosList.arraySize; i++)
            //    {
            //        SerializedProperty posProp = _rootPosList.GetArrayElementAtIndex(i);

            //        EditorGUILayout.PropertyField(posProp);
            //    }
            //}

            //m_foldRotation = EditorGUILayout.Foldout(m_foldRotation, new GUIContent("Root Motion Rotation"));
            //if (m_foldRotation)
            //{
            //    for (int i = 0; i < _rootPosList.arraySize; i++)
            //    {
            //        SerializedProperty rotProp = _rootPosList.GetArrayElementAtIndex(i);

            //        EditorGUILayout.PropertyField(rotProp);
            //    }
            //}
            //EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_baseFrame, new GUIContent("base Frame"));

            EditorGUILayout.LabelField(string.Format("index Count : {0}", index));            

            if (GUILayout.Button("GetAnimationClipData"))
            {
                //GetAnimationClipData();
            }

            if (GUILayout.Button("Replay position"))
            {
                anim.enabled = false;
                _replay.boolValue = true;
            }

            if (GUILayout.Button("stop"))
            {
                anim.enabled = true;
                _replay.boolValue = false;
            }
        }

        _serObj.ApplyModifiedProperties();
    }
    
    private void GetAnimationClipData()
    {
        if (null != anim)
        {
            m_player.DicRootMotionData.Clear();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var runtimeAnimatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath("", typeof(AnimatorController));

            foreach (AnimationClip ac in anim.runtimeAnimatorController.animationClips)
            {
                string dicKey = ac.name;
                EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetCurveBindings(ac);
                List<AnimationCurve> curveDatas = new List<AnimationCurve>();

                var rootMotionDatas = new List<RootMotionData>();
                for (int i = 0; i < editorCurveBindings.Length; ++i)
                {
                    curveDatas.Add(AnimationUtility.GetEditorCurve(ac, editorCurveBindings[i]));
                }

                index = -1;
                var frame = 1.0f / _baseFrame.intValue;
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

                    //_rootPosList.InsertArrayElementAtIndex(index);
                    //_rootPosList.GetArrayElementAtIndex(index).vector3Value = pos;

                    //_rootRotList.InsertArrayElementAtIndex(index);
                    //_rootRotList.GetArrayElementAtIndex(index).quaternionValue = rot;
                }

                if (false == m_player.DicRootMotionData.ContainsKey(dicKey))
                {
                    m_player.DicRootMotionData.Add(dicKey, rootMotionDatas);
                }
            }            
        }
    }
}

/*
            //var aa = anim.GetNextAnimatorClipInfo(0);
            //var info = anim.GetCurrentAnimatorClipInfo(0);
            //EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetCurveBindings(info[0].clip);

            //List<AnimationCurve> curveDatas = new List<AnimationCurve>();

            //for (int i = 0; i < editorCurveBindings.Length; ++i)
            //{
            //    curveDatas.Add(AnimationUtility.GetEditorCurve(info[0].clip, editorCurveBindings[i]));
            //}

            //index = -1;
            //var frame = 1.0f / _baseFrame.intValue;
            //for (float i = 0.0f; i <= info[0].clip.length; i += frame)
            //{
            //    ++index;
            //    Vector3 pos = new Vector3(
            //                                        curveDatas[0].Evaluate(i),
            //                                        curveDatas[1].Evaluate(i),
            //                                        curveDatas[2].Evaluate(i));

            //    Quaternion rot = new Quaternion(
            //                                        curveDatas[3].Evaluate(i),
            //                                        curveDatas[4].Evaluate(i),
            //                                        curveDatas[5].Evaluate(i),
            //                                        curveDatas[6].Evaluate(i));

            //    _rootPosList.InsertArrayElementAtIndex(index);
            //    _rootPosList.GetArrayElementAtIndex(index).vector3Value = pos;

            //    _rootRotList.InsertArrayElementAtIndex(index);
            //    _rootRotList.GetArrayElementAtIndex(index).quaternionValue = rot;
            //}
            ////for (int i = 0; i < m_player.quaternions.Count; ++i)
            ////{
            ////    Debug.LogError(m_player.vector3s[i] + " : " + m_player.quaternions[i]);
            ////}
*/
