using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lib.Rm
{
    [System.Serializable]
    public class RMUnityJsonData
    {
        public List<RMUnityJsonClipData> Clips;

#if UNITY_EDITOR
        private Dictionary<string, RMUnityJsonClipData> DicClipData = new Dictionary<string, RMUnityJsonClipData>();

        public bool AddClips(string stateName, RMUnityJsonClipData clipData)
        {
            if (DicClipData.ContainsKey(stateName))
                return false;

            DicClipData.Add(stateName, clipData);

            if (null == Clips)
                Clips = new List<RMUnityJsonClipData>();

            Clips.Add(clipData);

            return true;
        }
#endif
    }

    [System.Serializable]
    public class RMUnityJsonClipData
    {
        // Is Key
        public string StateName;
        public string ClipName;
        public RMUnityJsonFrameData Frame;
#if UNITY_EDITOR
        public RMUnityJsonClipData(string stateName, string clipName)
        {
            this.StateName = stateName;
            this.ClipName = clipName;
            if (null == Frame)
                Frame = new RMUnityJsonFrameData();
        }

        public void AddFrameData(Vector3 pos, Quaternion rot)
        {
            Frame.AddFrameData(pos, rot);
        }
#endif
    }

    [System.Serializable]
    public class RMUnityJsonFrameData
    {
        public int Count;
        public List<Vector3> Positions;
        public List<Quaternion> Rotations;
#if UNITY_EDITOR
        public RMUnityJsonFrameData()
        {
            if (null == Positions)
                Positions = new List<Vector3>();

            if (null == Rotations)
                Rotations = new List<Quaternion>();
        }

        public void AddFrameData(Vector3 pos, Quaternion rot)
        {
            ++Count;
            Positions.Add(pos);
            Rotations.Add(rot);
        }
#endif
    }
}
