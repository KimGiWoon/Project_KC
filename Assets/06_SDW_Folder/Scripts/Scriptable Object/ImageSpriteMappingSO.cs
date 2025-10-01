using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
// ImageSpriteMappingSO.cs
    [CreateAssetMenu(fileName = "ImageSpriteMapping", menuName = "SDW/ImageSpriteMapping")]
    public class ImageSpriteMappingSO : ScriptableObject
    {
        [Header("Scene Information")]
        public string sceneName;
        public string sceneLabel;

        [Header("Mapping Data")]
        public List<Entry> entries = new List<Entry>();

        [Serializable]
        public class Entry
        {
            public ulong PathHash;
            public string AddressKey;

#if UNITY_EDITOR
            [Header("Editor Preview Only")]
            public string AssetPath;
            public string PathPreview;
#endif
        }
    }
}