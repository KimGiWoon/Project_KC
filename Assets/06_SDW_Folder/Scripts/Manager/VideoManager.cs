using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class VideoManager : MonoBehaviour
    {
        [SerializeField] private VideoClipSO _videoClip;

        private Dictionary<VideoClipName, VideoEntry> _videoDictionary = new Dictionary<VideoClipName, VideoEntry>();
        public IReadOnlyDictionary<VideoClipName, VideoEntry> VideoDictionary => _videoDictionary;

        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;

            StartCoroutine(LoadCoroutine());
        }

        private IEnumerator LoadCoroutine()
        {
            while (true)
            {
                yield return null;
                if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected)
                    continue;

                break;
            }

            Initialize();
        }

        private void Initialize()
        {
            if (_videoClip.VideoEntries == null) return;

            foreach (var videoEntry in _videoClip.VideoEntries)
            {
                _videoDictionary.Add(videoEntry.Name, videoEntry);
            }
        }
    }
}