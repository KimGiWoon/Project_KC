using System;
using UnityEngine.Video; // VideoClipName을 정의한 곳

namespace SDW
{
    [Serializable]
    public class VideoEntry
    {
        public VideoClipName Name;
        // public VideoClip Video; // 이 필드를 제거
        public string AddressKey; // Addressables 키를 저장하는 필드 추가
    }
}