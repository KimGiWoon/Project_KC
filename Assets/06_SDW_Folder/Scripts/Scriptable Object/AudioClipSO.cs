using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    [CreateAssetMenu(fileName = "AudioClipSO", menuName = "Media/AudioClipSO")]
    public class AudioClipSO : ScriptableObject
    {
        public List<AudioEntry> AudioEntries;
    }
}