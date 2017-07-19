using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RootMotionKeyFrame
{
    public int count;
    public List<Vector3> pos;
    public List<Quaternion> rot;
}

public class RootMotionDataTest : MonoBehaviour
{
    public Transform cube;

    public Player player;

    public Transform PlayerRootTr;

    RootMotionKeyFrame keyFrame = new RootMotionKeyFrame();

    int stepCnt = 0;

    void Start()
    {
        Time.fixedDeltaTime = 1f / 50f;        
    }

    private void OnEnable()
    {
        LoadRootMotionData("Entrance");
    }

    void LoadRootMotionData(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;
        if (null != textAsset)
        {
            keyFrame = JsonUtility.FromJson<RootMotionKeyFrame>(textAsset.text);
        }
    }

    void DrawCube(int stepCnt)
    {
        for (int itr = 0; itr < stepCnt; ++itr)
        {
            if (itr > 0)
                Debug.DrawLine(keyFrame.pos[itr - 1], keyFrame.pos[itr], Color.red);

            if (cube)
            {
                cube.transform.position = keyFrame.pos[itr];
                cube.transform.rotation = keyFrame.rot[itr];

                //Debug.Assert(keyFrame.pos[stepCnt].ToString() == PlayerRootTr.localPosition.ToString(), string.Format("{0}, {1}", keyFrame.pos[stepCnt], PlayerRootTr.localPosition));
            }
        }
    }

    void FixedUpdate()
    {
        if (null != keyFrame)
        {
            if (stepCnt >= keyFrame.count)
            {
                stepCnt = 0;
                var Animator = player.GetComponent<Animator>();
                Animator.speed = 0;
                Animator.Play("Stage_Start");
                Animator.speed = 1;
                
                return;
            }

            DrawCube(stepCnt++);
        }
    }

    void Update()
    {
        //Debug.Assert(keyFrame.pos[stepCnt] == PlayerRootTr.localPosition, string.Format("{0}, {1}", keyFrame.pos[stepCnt], PlayerRootTr.localPosition));
    }
}
