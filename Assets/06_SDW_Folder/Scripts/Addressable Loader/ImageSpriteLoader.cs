using System.Collections;
using System.Collections.Generic;

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
        private readonly Dictionary<string, AsyncOperationHandle<Sprite>> _spriteHandles =
            new Dictionary<string, AsyncOperationHandle<Sprite>>();

        private bool _bound;

        private void OnEnable()
        {
            GameManagerEvents.OnDownloadCompleted += TryBind;
            GameManagerEvents.OnSceneIndexed += TryBind;
            // TryBind(); // 이미 조건 충족 상태일 수 있음
        }

        private void OnDisable()
        {
            GameManagerEvents.OnDownloadCompleted -= TryBind;
            GameManagerEvents.OnSceneIndexed -= TryBind;
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
            // StartCoroutine(DelayedLoad());
        }

        // private IEnumerator DelayedLoad()
        // {
        //     yield return new WaitForSeconds(0.5f);
        //     LoadFromAddressables();
        //     _bound = true;
        // }

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

                //# 같은 GO에 붙은 MB들의 Sprite 필드만 로컬로 할당(전역 순회 X)
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
            foreach (var e in _mappingSo.entries)
            {
                if (!ScenePathIndex.TryGet(e.PathHash, out var go)) continue;

                if (go == null) continue;
                var img = go.GetComponent<Image>();
                if (img) img.sprite = null;

                if (string.IsNullOrEmpty(e.AddressKey)) continue;

                Addressables.LoadAssetAsync<Sprite>(e.AddressKey).Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        _spriteHandles[e.AddressKey] = handle;

                        if (img) img.sprite = handle.Result;

                        //# 같은 GO에 붙은 MB들의 Sprite 필드만 로컬로 할당(전역 순회 X)
                        // var monos = go.GetComponents<MonoBehaviour>();
                        // foreach (var mb in monos)
                        // {
                        //     if (!mb) continue;
                        //     var fields = mb.GetType().GetFields(
                        //         System.Reflection.BindingFlags.Public |
                        //         System.Reflection.BindingFlags.NonPublic |
                        //         System.Reflection.BindingFlags.Instance);
                        //
                        //     foreach (var f in fields)
                        //     {
                        //         if (f.FieldType == typeof(Sprite))
                        //         {
                        //             f.SetValue(mb, handle.Result);
                        //             Debug.Log(
                        //                 $"Found Sprite field: {mb.GetType().Name}.{f.Name} = {((Sprite)f.GetValue(mb))?.name}");
                        //         }
                        //     }
                        // }
                    }
                    else
                    {
                        Debug.LogError($"[ImageSpriteLoader] Load failed: {e.AddressKey}");
                    }
                };
            }

            // StartCoroutine(DelayedConnect());
            GameManager.Instance.SetImageSpriteConnected(true);
        }

        private IEnumerator DelayedConnect()
        {
            yield return null;
        }

        // private void OnDestroy()
        // {
        //     foreach (var kvp in _spriteHandles)
        //     {
        //         Addressables.Release(kvp.Value);
        //     }
        //     _spriteHandles.Clear();
        // }

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