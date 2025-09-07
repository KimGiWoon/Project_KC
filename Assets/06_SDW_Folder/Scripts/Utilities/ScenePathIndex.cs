using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SDW
{
    /// <summary>
    /// 씬 로드 시 한 번 계층을 순회해 pathHash→GameObject 인덱스를 만든다.
    /// 씬 전환마다 자동 갱신. DontDestroyOnLoad 오브젝트에 붙여 사용.
    /// </summary>
    public class ScenePathIndex : MonoBehaviour
    {
        private static readonly Dictionary<ulong, GameObject> _index = new Dictionary<ulong, GameObject>();
        public static bool IsBuilt { get; private set; }
        private static bool isDontDestroyOnLoadLoaded;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Rebuild(scene);

            if (scene.buildIndex != 0)
                GameManagerEvents.RaiseSceneIndexed();
        }

        public static void Rebuild(Scene scene)
        {
            _index.Clear();
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                BuildRecursive(root.transform, root.name);
            }
            // DontDestroyOnLoad 씬의 오브젝트들도 추가

            if (!isDontDestroyOnLoadLoaded)
                AddDontDestroyOnLoadObjects();

            IsBuilt = true;
        }

        private static void AddDontDestroyOnLoadObjects()
        {
            // DontDestroyOnLoad 오브젝트들을 찾는 방법
            var dontDestroyObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => go.scene.name == "DontDestroyOnLoad")
                .Where(go => go.transform.parent == null); // 루트 오브젝트만

            foreach (var obj in dontDestroyObjects)
            {
                BuildRecursive(obj.transform, obj.name);
            }

            isDontDestroyOnLoadLoaded = true;
        }

        private static void BuildRecursive(Transform t, string path)
        {
            // DownloadContainer 등 제외하고 싶다면 여기서 필터
            if (t.name == "DownloadContainer")
            {
                /* skip branch */
            }
            var go = t.gameObject;
            ulong h = PathHasher.Hash(path);
            _index[h] = go;

            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                BuildRecursive(c, path + "/" + c.name);
            }
        }

        public static bool TryGet(ulong pathHash, out GameObject go) => _index.TryGetValue(pathHash, out go);
    }
}