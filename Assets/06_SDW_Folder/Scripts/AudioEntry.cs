using System;
using UnityEngine;

namespace SDW
{
    [Serializable]
    public class AudioEntry
    {
        public AudioClipName Name;
        public AudioType Type;
        public AudioClip Clip;
    }
}