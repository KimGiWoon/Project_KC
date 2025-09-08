namespace SDW
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Addressables/PrefabAndSOMappingSO")]
    public class PrefabAndSOMappingSO : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            // public string Path; // Hierarchy 경로
            //# 문자열 대신 Hash를 사용
            public ulong PathHash;
            public string AddressKey; // Addressables GUID
            public string fieldName; // 필드 이름
            public int index = -1; // ✨추가: 리스트/배열 인덱스✨
#if UNITY_EDITOR
            public string AssetPath; // Editor 프리뷰용
            public string PathPreview;
#endif
        }

        public List<Entry> prefabEntries = new List<Entry>();
        public List<Entry> soEntries = new List<Entry>();
    }
}