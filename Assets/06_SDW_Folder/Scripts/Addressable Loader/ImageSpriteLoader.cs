using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SDW
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ImageSpriteLoader : MonoBehaviour
    {
        [Tooltip("씬별로 생성된 ImageSpriteMapping SO 할당")]
        public ImageSpriteMappingSO _mappingSo;

        private static List<AsyncOperationHandle> _loadedHandles = new List<AsyncOperationHandle>();
        private static string _currentSceneLabel = "";
        private bool _bound;

        private void OnEnable()
        {
            GameManagerEvents.OnDownloadCompleted += TryBind;
            GameManagerEvents.OnSceneIndexed += TryBind;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            GameManagerEvents.OnDownloadCompleted -= TryBind;
            GameManagerEvents.OnSceneIndexed -= TryBind;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void TryBind()
        {
            if (_bound) return;
            if (_mappingSo == null)
            {
                Debug.Log($"Image Mapping SO 연결 안됨 {GameManager.Instance.Scene.GetActiveScene()}");
                return;
            }
            if (!ScenePathIndex.IsBuilt) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ApplyEditorPreview();
                _bound = true;
                return;
            }
#endif

            LoadFromAddressables();
            _bound = true;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            // 씬이 언로드될 때 해당 씬의 라벨로 로드된 에셋들을 언로드
            if (!string.IsNullOrEmpty(_currentSceneLabel))
            {
                UnloadAddressablesByLabel(_currentSceneLabel);
            }
        }

        private static void UnloadAddressablesByLabel(string label)
        {
            Debug.Log($"[ImageSpriteLoader] Unloading sprites with label: {label}");

            // 모든 핸들 릴리즈
            foreach (var handle in _loadedHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _loadedHandles.Clear();

            Debug.Log($"[ImageSpriteLoader] Released {_loadedHandles.Count} handles for label: {label}");
        }

#if UNITY_EDITOR
        private void ApplyEditorPreview()
        {
            foreach (var e in _mappingSo.entries)
            {
                if (!ScenePathIndex.TryGet(e.PathHash, out var go)) continue;
                var img = go.GetComponent<Image>();
                if (!img) continue;
                if (string.IsNullOrEmpty(e.AssetPath)) continue;

                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(e.AssetPath);
                if (sprite) img.sprite = sprite;

                var monos = go.GetComponents<MonoBehaviour>();
                foreach (var mb in monos)
                {
                    if (!mb) continue;
                    var so = new SerializedObject(mb);
                    var prop = so.GetIterator();
                    bool modified = false;
                    while (prop.NextVisible(true))
                    {
                        if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                            prop.objectReferenceValue is Sprite)
                        {
                            prop.objectReferenceValue = sprite;
                            modified = true;
                        }
                    }
                    if (modified) so.ApplyModifiedProperties();
                }
            }
        }
#endif

        private void LoadFromAddressables()
        {
            if (_mappingSo == null) return;

            // 이전 씬의 라벨이 있으면 언로드
            if (!string.IsNullOrEmpty(_currentSceneLabel))
            {
                UnloadAddressablesByLabel(_currentSceneLabel);
            }

            // 현재 씬의 라벨로 설정
            _currentSceneLabel = _mappingSo.sceneLabel;

            if (string.IsNullOrEmpty(_currentSceneLabel))
            {
                Debug.LogWarning($"[ImageSpriteLoader] Scene '{_mappingSo.sceneName}' has no label assigned!");
            }
            else
            {
                Debug.Log($"[ImageSpriteLoader] Loading sprites with label: {_currentSceneLabel}");
            }

            var loadedKeys = new HashSet<string>();

            foreach (var e in _mappingSo.entries)
            {
                if (!ScenePathIndex.TryGet(e.PathHash, out var go)) continue;
                if (go == null) continue;

                var img = go.GetComponent<Image>();
                if (img) img.sprite = null;

                if (string.IsNullOrEmpty(e.AddressKey)) continue;
                if (loadedKeys.Contains(e.AddressKey)) continue; // 중복 방지

                Addressables.LoadAssetAsync<Sprite>(e.AddressKey).Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        if (!loadedKeys.Contains(e.AddressKey))
                        {
                            _loadedHandles.Add(handle);
                            loadedKeys.Add(e.AddressKey);
                        }

                        if (img) img.sprite = handle.Result;
                    }
                    else
                    {
                        Debug.LogError($"[ImageSpriteLoader] Load failed: {e.AddressKey}");
                    }
                };
            }

            GameManager.Instance.SetImageSpriteConnected(true);
            Debug.Log($"[ImageSpriteLoader] Scene '{_mappingSo.sceneName}' (Label: {_currentSceneLabel}) - All sprites loaded");
        }

        public static string GetPath(GameObject go)
        {
            string p = go.name;
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                p = go.name + "/" + p;
            }
            return p;
        }
    }
}