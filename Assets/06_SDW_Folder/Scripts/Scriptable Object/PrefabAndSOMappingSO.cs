using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    // PrefabAndSOMappingSO.cs
    [CreateAssetMenu(fileName = "PrefabAndSOMapping", menuName = "SDW/PrefabAndSOMapping")]
    public class PrefabAndSOMappingSO : ScriptableObject
    {
        [Header("Scene Information")]
        public string sceneName;
        public string sceneLabel;

        [Header("Mapping Data")]
        public List<Entry> prefabEntries = new List<Entry>();
        public List<Entry> soEntries = new List<Entry>();

        [Serializable]
        public class Entry
        {
            public ulong PathHash;
            public string AddressKey;
            public string fieldName;
            public int index = -1;

#if UNITY_EDITOR
            [Header("Editor Preview Only")]
            public string AssetPath;
            public string PathPreview;
#endif
        }
    }
}