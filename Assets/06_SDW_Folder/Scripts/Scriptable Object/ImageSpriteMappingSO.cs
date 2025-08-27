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
            public string Path;
            //# Addressables Address (여기서는 Sprite GUID 문자열 사용)
            public string AddressKey;
#if UNITY_EDITOR
            //# Editor 전용 - Sprite 프리뷰용
            public string AssetPath;
#endif
        }

        public List<Entry> entries = new List<Entry>();
    }
}