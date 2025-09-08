namespace SDW
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Addressables/ImageSpriteMappingSO")]
    public class ImageSpriteMappingSO : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            //# Hierarchy 경로 (Canvas/Panel/Icon)
            // public string Path;
            //# 문자열 대신 Hash를 사용
            public ulong PathHash;
            //# Addressables Address (여기서는 Sprite GUID 문자열 사용)
            public string AddressKey;
#if UNITY_EDITOR
            //# Editor 전용 - Sprite 프리뷰용
            public string AssetPath;
            public string PathPreview; // 디버그용(사람이 읽는 경로)

#endif
        }

        public List<Entry> entries = new List<Entry>();
    }
}