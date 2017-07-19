using System.Collections.Generic;
using UnityEngine;

namespace Lib.Rm
{
    public interface IRootMotion
    {
        void GetRootMotionByFrame(string key, int frameIdx, out Vector3 pos, out Quaternion rot);

        RMUnityJsonFrameData GetRootMotionKeyFrame(string keyName, out string clipName);
    }

    public class RootMotionMain : IRootMotion
    {
        private Dictionary<string, RMUnityJsonClipData> DicRMClipData = new Dictionary<string, RMUnityJsonClipData>();

        private List<string> m_rmKeyList = new List<string>();
        public List<string> RMKeyList
        {
            get { return m_rmKeyList; }
        }

        public RootMotionMain(string actorName)
        {
            LoadRootMotionData(actorName);
        }

        private void LoadRootMotionData(string actorName)
        {
            string RMPathFormat = "RootMotion/{0}/";
            string rmName = actorName.Replace("Mesh", "RM");

            var sb = new System.Text.StringBuilder();
            sb.AppendFormat(RMPathFormat, rmName);
            sb.Append(rmName);

            var assetPath = sb.ToString();
            RMUnityJsonData rootMotionData = null;
            TextAsset textAsset = Resources.Load(assetPath, typeof(TextAsset)) as TextAsset;
            if (null != textAsset)
            {
                rootMotionData = JsonUtility.FromJson<RMUnityJsonData>(textAsset.text);
            }

            if (null != rootMotionData)
            {
                for (int itr = 0; itr < rootMotionData.Clips.Count; ++itr)
                {
                    var key = rootMotionData.Clips[itr].StateName;
                    var data = rootMotionData.Clips[itr];
                    DicRMClipData.Add(key, data);

                    m_rmKeyList.Add(key);
                }
            }
        }

        //public void GetFrame(this RMUnityJsonFrameData data, int idx, out Vector3 pos, out Quaternion rot)
        //{
        //    pos = data.Positions[idx];
        //    rot = data.Rotations[idx];
        //}

        public void GetRootMotionByFrame(string key, int frameIdx, out Vector3 pos, out Quaternion rot)
        {
            Debug.Assert(false == string.IsNullOrEmpty(key));
            Debug.Assert(0 <= frameIdx);

            pos = Vector3.zero;
            rot = Quaternion.identity;
            if (null != DicRMClipData)
            {
                if (DicRMClipData.ContainsKey(key))
                {
                    var clipData = DicRMClipData[key];

                    Debug.Assert(frameIdx < clipData.Frame.Positions.Count);
                    pos = clipData.Frame.Positions[frameIdx];
                    rot = clipData.Frame.Rotations[frameIdx];
                }
            }
        }

        public RMUnityJsonFrameData GetRootMotionKeyFrame(string stateName, out string clipName)
        {
            if (DicRMClipData.ContainsKey(stateName))
            {
                clipName = DicRMClipData[stateName].ClipName;
                return DicRMClipData[stateName].Frame;
            }

            clipName = "NONE";
            return null;
        }
    }
}
