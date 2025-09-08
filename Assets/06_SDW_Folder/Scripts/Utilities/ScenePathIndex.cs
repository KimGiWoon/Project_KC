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

        /// <summary>
        /// Scene 로드가 완료될 때 호출할 메서드 연결
        /// </summary>
        private void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;

        /// <summary>
        /// Scene 로드가 완료될 때 호출할 메서드 연결 해제
        /// </summary>
        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        /// <summary>
        /// Scene 로드가 완료될 때 호출될 메서드를 이벤트 핸들러로 등록
        /// </summary>
        /// <param name="scene">로드 완료된 Scene 객체</param>
        /// <param name="mode">로드 모드 (LoadSceneMode)</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Rebuild(scene);

            if (scene.buildIndex != 0)
                GameManagerEvents.RaiseSceneIndexed();
        }

        /// <summary>
        /// 지정된 Scene에 대해 경로 해시와 GameObject 간의 인덱스를 재구성
        /// </summary>
        /// <param name="scene">재구성할 Scene 객체</param>
        private static void Rebuild(Scene scene)
        {
            _index.Clear();
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                BuildRecursive(root.transform, root.name);
            }

            //# DontDestroyOnLoad 씬의 오브젝트들도 추가
            if (!isDontDestroyOnLoadLoaded)
                AddDontDestroyOnLoadObjects();

            IsBuilt = true;
        }

        /// <summary>
        /// 씬 내에서 DontDestroyOnLoad 설정된 오브젝트들을 _index에 추가하여 경로 해시와 GameObject 간의 매핑을 완성
        /// </summary>
        private static void AddDontDestroyOnLoadObjects()
        {
            //# DontDestroyOnLoad 오브젝트들을 찾는 방법
            var dontDestroyObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => go.scene.name == "DontDestroyOnLoad")
                .Where(go => go.transform.parent == null);

            foreach (var obj in dontDestroyObjects)
            {
                BuildRecursive(obj.transform, obj.name);
            }

            isDontDestroyOnLoadLoaded = true;
        }

        /// <summary>
        /// 지정된 Scene 내에서 경로 해시와 GameObject 간의 인덱스를 재구성
        /// </summary>
        /// <param name="t">재구성할 GameObject의 Transform 컴포넌트</param>
        /// <param name="path">GameObject의 경로를 나타내는 문자열</param>
        private static void BuildRecursive(Transform t, string path)
        {
            //# DownloadContainer 등 제외하고 싶다면 여기서 필터
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

        /// <summary>
        /// 지정된 경로 해시에 해당하는 게임 오브젝트를 검색
        /// </summary>
        /// <param name="pathHash">검색할 경로 해시 값</param>
        /// <param name="go">검색 결과로 반환되는 게임 오브젝트 참조</param>
        /// <returns>해당 경로 해시의 게임 오브젝트가 존재하면 true, 그렇지 않으면 false</returns>
        public static bool TryGet(ulong pathHash, out GameObject go) => _index.TryGetValue(pathHash, out go);
    }
}