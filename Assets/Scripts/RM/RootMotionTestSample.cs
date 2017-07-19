using Lib.Rm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionTestSample : MonoBehaviour
{
    public GameObject RM;

    public Transform cube;

    public Player player;

    public Transform PlayerRootTr;

    private Animator anim;

    private int stepCnt = 0;

    private RootMotionMain m_rootMotionMain;

    private RMUnityJsonFrameData keyFrame;

    private int keyItr = 0;

    private void Awake()
    {
        if (null == m_rootMotionMain)
            m_rootMotionMain = new RootMotionMain("Hero_R_Paladin_Mesh 1");
    }

    void Start()
    {
        Time.fixedDeltaTime = 1f / 50f;
    }

    private void OnEnable()
    {
        string clipName;
        keyFrame = m_rootMotionMain.GetRootMotionKeyFrame(m_rootMotionMain.RMKeyList[keyItr], out clipName);
        Debug.LogError("Play RootMotion : " + m_rootMotionMain.RMKeyList[keyItr] + ", " + clipName);

        anim = player.GetComponent<Animator>();
    }

    void DrawCube(int stepCnt)
    {
        //Vector3 pos;
        //Quaternion rot;
        //m_rootMotionMain.GetRootMotionByFrame(m_rootMotionMain.RMKeyList[keyItr], stepCnt, out pos, out rot);
        //cube.transform.position = pos;
        //cube.transform.rotation = rot;

        for (int itr = 0; itr < stepCnt; ++itr)
        {
            if (itr > 0)
                Debug.DrawLine(keyFrame.Positions[itr - 1], keyFrame.Positions[itr], Color.red);

            if (cube)
            {
                cube.transform.position = keyFrame.Positions[itr];
                cube.transform.rotation = keyFrame.Rotations[itr];
            }
        }
    }

    void FixedUpdate()
    {
        if (null != keyFrame)
        {
            if (stepCnt >= keyFrame.Count)
            {
                stepCnt = 0;
                ++keyItr;
                if (m_rootMotionMain.RMKeyList.Count - 1 == keyItr)
                    keyItr = 0;

                string clipName;
                keyFrame = m_rootMotionMain.GetRootMotionKeyFrame(m_rootMotionMain.RMKeyList[keyItr], out clipName);
                Debug.LogError("Play RootMotion : " + m_rootMotionMain.RMKeyList[keyItr] + ", " + clipName);

                

                //RM.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                //cube.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                //player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                //PlayerRootTr.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                RM.transform.position = Vector3.zero;
                cube.gameObject.transform.position = keyFrame.Positions[0];

                var vec3 = new Vector3(keyFrame.Positions[0].x, 0, keyFrame.Positions[0].z);
                player.gameObject.transform.position = vec3;
                //player.gameObject.transform.position = keyFrame.Positions[0];
                PlayerRootTr.position = keyFrame.Positions[0];

                anim.speed = 0;
                anim.Play(m_rootMotionMain.RMKeyList[keyItr]);
                anim.speed = 1;

                anim.enabled = true;

                return;
            }

            DrawCube(stepCnt++);
        }
    }

    void Update()
    {
    }
}
