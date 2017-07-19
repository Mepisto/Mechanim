using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RootMotionData
{
    public Vector3 RootPosition;
    public Quaternion RootRotation;
}

public class Player : MonoBehaviour
{    
    public Dictionary<string, List<RootMotionData>> DicRootMotionData = new Dictionary<string, List<RootMotionData>>();

    [SerializeField]
    private List<Vector3> RootPosList = new List<Vector3>();

    [SerializeField]
    private List<Quaternion> RootRotList = new List<Quaternion>();

    [SerializeField]
    private bool Replay = false;        

    [SerializeField]
    private int BaseFrame = 50;

    [SerializeField]
    private int Index = 0;

    private void Start()
    {
        var ani = this.GetComponent<Animator>();
        ani.speed = 0;
    }

    private void FixedUpdate()
    {        
        //var animator = this.GetComponent<Animator>();
        //var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //if (!clipInfo[0].clip.name.Contains("Entrance"))
        //{
        //    animator.Play("Stage_Start");
        //}
        //if (Replay)
        //{
        //    var child = this.gameObject.transform.Find("Root");
        //    child.gameObject.transform.SetPositionAndRotation(RootPosList[Index], RootRotList[Index]);
        //    ++Index;

        //    if (Index == RootPosList.Count - 1)
        //    {
        //        Replay = false;
        //        Index = 0;

        //        var animator = this.GetComponent<Animator>();
        //        if (null != animator)
        //            animator.enabled = true;
        //    }
        //}
    }
}
