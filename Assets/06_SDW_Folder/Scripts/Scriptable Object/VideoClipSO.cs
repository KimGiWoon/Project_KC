using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    [CreateAssetMenu(fileName = "VideoClipSO", menuName = "Media/VideoClipSO")]
    public class VideoClipSO : ScriptableObject
    {
        public List<VideoEntry> VideoEntries;
    }
}