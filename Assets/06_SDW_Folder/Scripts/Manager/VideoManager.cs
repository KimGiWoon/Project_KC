using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

namespace SDW
{
    public class VideoManager : MonoBehaviour
    {
        [SerializeField] private VideoClipSO _videoClipSO;

        private Dictionary<VideoClipName, VideoEntry> _videoDictionary = new Dictionary<VideoClipName, VideoEntry>();
        public IReadOnlyDictionary<VideoClipName, VideoEntry> VideoDictionary => _videoDictionary;

        private Dictionary<VideoClipName, VideoClip> _loadedVideoClips = new Dictionary<VideoClipName, VideoClip>();
        private Dictionary<VideoClipName, AsyncOperationHandle<VideoClip>> _loadedHandles =
            new Dictionary<VideoClipName, AsyncOperationHandle<VideoClip>>();

        private GameManager _gameManager;
        private bool _isLoaded;
        public bool IsLoaded => _isLoaded;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            StartCoroutine(LoadCoroutine());
        }

        private void OnDestroy()
        {
            UnloadAllVideos();
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
            if (_videoClipSO.VideoEntries == null) return;

            foreach (var videoEntry in _videoClipSO.VideoEntries)
            {
                if (!_videoDictionary.ContainsKey(videoEntry.Name))
                {
                    _videoDictionary.Add(videoEntry.Name, videoEntry);
                }
            }

            _isLoaded = true;
            Debug.Log("[VideoManager] Video dictionary initialized. Videos will be loaded on demand.");
        }

        // -------------------------------------------------------------

        #region Video Loading/Unloading

        /// <summary>
        /// 지정된 이름의 비디오 클립을 비동기적으로 로드합니다.
        /// </summary>
        public async Task<VideoClip> GetVideoClipAsync(VideoClipName videoName)
        {
            if (!_videoDictionary.TryGetValue(videoName, out var entry))
            {
                Debug.LogError($"[VideoManager] VideoClipName '{videoName}' not found in dictionary.");
                return null;
            }

            //# 이미 로드된 경우 바로 반환
            if (_loadedVideoClips.TryGetValue(videoName, out var loadedClip))
            {
                return loadedClip;
            }

            //# 다른 비디오들을 언로드
            UnloadAllExcept(videoName);

            var handle = Addressables.LoadAssetAsync<VideoClip>(entry.AddressKey);
            _loadedHandles.Add(videoName, handle);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedVideoClips[videoName] = handle.Result;
                Debug.Log($"[VideoManager] Successfully loaded video: {videoName}");
                return handle.Result;
            }
            else
            {
                Debug.LogError($"[VideoManager] Failed to load video: {videoName} from Addressable Key: {entry.AddressKey}");
                _loadedHandles.Remove(videoName);
                return null;
            }
        }

        /// <summary>
        /// 현재 로드된 모든 비디오 클립을 Addressables에서 해제합니다.
        /// </summary>
        public void UnloadAllVideos()
        {
            foreach (var kvp in _loadedHandles)
            {
                if (kvp.Value.IsValid())
                {
                    Addressables.Release(kvp.Value);
                    Debug.Log($"[VideoManager] Released video handle for: {kvp.Key}");
                }
            }
            _loadedHandles.Clear();
            _loadedVideoClips.Clear();
        }

        /// <summary>
        /// 특정 비디오를 제외하고 현재 로드된 모든 비디오 클립을 해제합니다.
        /// </summary>
        private void UnloadAllExcept(VideoClipName keepVideo)
        {
            // 해제할 핸들 목록 임시 저장
            var handlesToRelease = new List<AsyncOperationHandle<VideoClip>>();
            var namesToClear = new List<VideoClipName>();

            foreach (var kvp in _loadedHandles)
            {
                if (kvp.Key != keepVideo)
                {
                    handlesToRelease.Add(kvp.Value);
                    namesToClear.Add(kvp.Key);
                }
            }

            // 해제 및 딕셔너리 정리
            foreach (var name in namesToClear)
            {
                if (_loadedHandles.TryGetValue(name, out var handle) && handle.IsValid())
                {
                    Addressables.Release(handle);
                    Debug.Log($"[VideoManager] Released video handle for: {name} (Unloaded to make space)");
                }
                _loadedHandles.Remove(name);
                _loadedVideoClips.Remove(name);
            }
        }

        #endregion
    }
}