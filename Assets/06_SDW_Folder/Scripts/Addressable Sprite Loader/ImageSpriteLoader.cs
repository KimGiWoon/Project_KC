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

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ApplyEditorPreview();
                return;
            }
#endif
            LoadFromAddressables();
        }

#if UNITY_EDITOR
        private void ApplyEditorPreview()
        {
            if (_mappingSo == null) return;

            var images = FindObjectsOfType<Image>(true);
            foreach (var img in images)
            {
                string path = GetPath(img.gameObject);
                var entry = _mappingSo.entries.Find(e => e.Path == path);
                if (entry == null || string.IsNullOrEmpty(entry.AssetPath)) continue;

                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(entry.AssetPath);
                if (sprite != null)
                    img.sprite = sprite;
            }
        }
#endif

        private void LoadFromAddressables()
        {
            if (_mappingSo == null) return;

            var images = GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                string path = GetPath(img.gameObject);
                var entry = _mappingSo.entries.Find(e => e.Path == path);
                if (entry == null || string.IsNullOrEmpty(entry.AddressKey)) continue;

                //# 런타임에서는 Addressables로만 로드 (주소 키=GUID)
                img.sprite = null;

                Addressables.LoadAssetAsync<Sprite>(entry.AddressKey).Completed += (AsyncOperationHandle<Sprite> handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        img.sprite = handle.Result;
                    }
                    else
                    {
                        Debug.LogError($"[ImageSpriteLoader] Load failed: {entry.AddressKey} (path: {path})");
                    }
                };
            }
        }

        private static string GetPath(GameObject go)
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