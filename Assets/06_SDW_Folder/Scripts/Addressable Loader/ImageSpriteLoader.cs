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

        private bool _isDownloaded;

        private void Update()
        {
            if (!GameManager.Instance.CompleteDownload || _isDownloaded) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ApplyEditorPreview();
                return;
            }
#endif
            LoadFromAddressables();

            _isDownloaded = true;
        }

#if UNITY_EDITOR
        private void ApplyEditorPreview()
        {
            if (_mappingSo == null) return;

            // 1) Image 처리
            var images = FindObjectsOfType<Image>(true);
            foreach (var img in images)
            {
                ApplyEditorSprite(img.gameObject, sprite => img.sprite = sprite);
            }

            // 2) MonoBehaviour Sprite 필드 처리
            var monos = FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in monos)
            {
                if (mb == null) continue;
                var so = new SerializedObject(mb);
                var prop = so.GetIterator();
                bool modified = false;

                while (prop.NextVisible(true))
                {
                    if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                        prop.objectReferenceValue is Sprite)
                    {
                        var p = prop.Copy(); // 클로저 안전 복사
                        ApplyEditorSprite(mb.gameObject, sprite =>
                        {
                            p.objectReferenceValue = sprite;
                            modified = true;
                        });
                    }
                }

                if (modified)
                    so.ApplyModifiedProperties();
            }
        }

        private void ApplyEditorSprite(GameObject owner, System.Action<Sprite> assign)
        {
            string path = GetPath(owner);
            var entry = _mappingSo.entries.Find(e => e.Path == path);
            if (entry == null || string.IsNullOrEmpty(entry.AssetPath)) return;

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(entry.AssetPath);
            if (sprite != null)
                assign(sprite);
        }
#endif

        private void LoadFromAddressables()
        {
            if (_mappingSo == null) return;

            // 1) Image 처리
            // var images = GetComponentsInChildren<Image>(true);
            var images = FindObjectsOfType<Image>(true);
            foreach (var img in images)
            {
                LoadSpriteFromAddressables(img.gameObject, sprite => img.sprite = sprite);
            }

            // 2) MonoBehaviour Sprite 필드 처리 (런타임: Reflection)
            // var monos = GetComponentsInChildren<MonoBehaviour>(true);
            var monos = FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in monos)
            {
                if (mb == null) continue;
                var fields = mb.GetType().GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(Sprite))
                    {
                        LoadSpriteFromAddressables(mb.gameObject, sprite => { field.SetValue(mb, sprite); });
                    }
                }
            }
            GameManager.Instance.SetImageSpriteConnected(true);
        }

        private void LoadSpriteFromAddressables(GameObject owner, System.Action<Sprite> assign)
        {
            string path = GetPath(owner);
            var entry = _mappingSo.entries.Find(e => e.Path == path);
            if (entry == null || string.IsNullOrEmpty(entry.AddressKey)) return;

            assign(null); // 런타임에서는 우선 제거

            Addressables.LoadAssetAsync<Sprite>(entry.AddressKey).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    assign(handle.Result);
                else
                    Debug.LogError($"[ImageSpriteLoader] Load failed: {entry.AddressKey} (path: {path})");
            };
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