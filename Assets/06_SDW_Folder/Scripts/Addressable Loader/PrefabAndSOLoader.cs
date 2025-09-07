using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // async/await 사용을 위해 추가
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SDW
{
    public class PrefabAndSOLoader : MonoBehaviour
    {
        [Tooltip("씬별로 생성된 PrefabAndSOMapping SO 할당")]
        public PrefabAndSOMappingSO _mappingSo;

        private bool _bound;

        private void OnEnable()
        {
            GameManagerEvents.OnDownloadCompleted += TryBind;
            GameManagerEvents.OnSceneIndexed += TryBind;
            // TryBind();
        }

        private void OnDisable()
        {
            GameManagerEvents.OnDownloadCompleted -= TryBind;
            GameManagerEvents.OnSceneIndexed -= TryBind;
        }

        private void TryBind()
        {
            if (_bound) return;
            if (_mappingSo == null) return;
            if (!ScenePathIndex.IsBuilt) return;

            LoadFromAddressables();
            _bound = true;
        }

        // private bool _isDownloaded;

//         private async void Update()
//         {
//             if (!GameManager.Instance.CompleteDownload || _isDownloaded) return;
//
// #if UNITY_EDITOR
//             if (!Application.isPlaying)
//             {
//                 ApplyEditorPreview();
//                 return;
//             }
// #endif
//             // async void 메서드 호출
//             await LoadFromAddressables();
//             _isDownloaded = true;
//         }

/*
#if UNITY_EDITOR
        private void ApplyEditorPreview()
        {
            if (_mappingSo == null) return;

            // PrefabEntries와 SOEntries를 모두 포함하는 리스트 생성
            var allEntries = _mappingSo.prefabEntries.Cast<PrefabAndSOMappingSO.Entry>()
                .Concat(_mappingSo.soEntries.Cast<PrefabAndSOMappingSO.Entry>());

            // 로드할 에셋의 AddressKey를 GroupBy로 묶어 중복 제거
            var groupedEntries = allEntries.GroupBy(e => e.AddressKey);

            foreach (var group in groupedEntries)
            {
                var entry = group.First();

                var loadedObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(entry.AssetPath);

                if (loadedObj == null) continue;

                foreach (var singleEntry in group)
                {
                    if (loadedObj is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
                    {
                        AssignToMonoBehaviourField(singleEntry.Path, singleEntry.fieldName, singleEntry.index, go);
                    }
                    else if (loadedObj is ScriptableObject so)
                    {
                        AssignToMonoBehaviourField(singleEntry.Path, singleEntry.fieldName, singleEntry.index, so);
                    }
                }
            }
        }
#endif

        private async Task LoadFromAddressables()
        {
            if (_mappingSo == null) return;

            // 로드할 에셋의 AddressKey를 GroupBy로 묶어 중복 제거
            var allEntries = _mappingSo.prefabEntries.Cast<PrefabAndSOMappingSO.Entry>()
                .Concat(_mappingSo.soEntries.Cast<PrefabAndSOMappingSO.Entry>());

            var groupedEntries = allEntries.GroupBy(e => e.AddressKey);

            foreach (var group in groupedEntries)
            {
                var entry = group.First();

                var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(entry.AddressKey);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var loadedObj = handle.Result;

                    foreach (var singleEntry in group)
                    {
                        AssignToMonoBehaviourField(singleEntry.Path, singleEntry.fieldName, singleEntry.index, loadedObj);
                    }
                }
                else
                {
                    Debug.LogError($"[PrefabAndSOLoader] Asset load failed: {entry.AddressKey}");
                }
            }
        }

        /// <summary>
        /// 필드 이름과 인덱스를 사용하여 MonoBehaviour 필드에 객체를 할당합니다.
        /// </summary>
        private void AssignToMonoBehaviourField(string path, string fieldName, int index, UnityEngine.Object loadedObj)
        {
            var monos = FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in monos)
            {
                if (mb == null) continue;

                string mbPath = GetPath(mb.gameObject);
                if (mbPath != path) continue;

                var fields = mb.GetType().GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (field.Name == fieldName)
                    {
                        if (index == -1) // 단일 객체 필드
                        {
                            if (field.FieldType.IsAssignableFrom(loadedObj.GetType()))
                            {
                                field.SetValue(mb, loadedObj);
                            }
                        }
                        else // 리스트/배열 필드
                        {
                            if (typeof(IList).IsAssignableFrom(field.FieldType) && field.FieldType.IsGenericType)
                            {
                                var genericType = field.FieldType.GetGenericArguments()[0];
                                if (genericType.IsAssignableFrom(loadedObj.GetType()))
                                {
                                    var list = field.GetValue(mb) as IList;

                                    if (list == null)
                                    {
                                        list = (IList)Activator.CreateInstance(field.FieldType);
                                        field.SetValue(mb, list);
                                    }

                                    // 필요한 경우 리스트 크기를 확장하여 인덱스에 할당
                                    while (list.Count <= index)
                                    {
                                        list.Add(null);
                                    }
                                    list[index] = loadedObj;
                                }
                            }
                        }
                        break;
                    }
                }
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
    */

        private async void LoadFromAddressables()
        {
            // if (_mappingSo == null) return;
            //
            // var loadedKeys = new HashSet<string>();
            //
            // foreach (var entry in _mappingSo.prefabEntries)
            // {
            //     await LoadAndAssign(entry, loadedKeys, typeof(GameObject));
            // }
            //
            // foreach (var entry in _mappingSo.soEntries)
            // {
            //     await LoadAndAssign(entry, loadedKeys, typeof(ScriptableObject));
            // }

            if (_mappingSo == null) return;

            var loadedKeys = new HashSet<string>();

            var tasks = new List<Task>();

            foreach (var entry in _mappingSo.prefabEntries)
            {
                tasks.Add(LoadAndAssign(entry, loadedKeys, typeof(GameObject)));
            }

            foreach (var entry in _mappingSo.soEntries)
            {
                tasks.Add(LoadAndAssign(entry, loadedKeys, typeof(ScriptableObject)));
            }

            await Task.WhenAll(tasks);


            GameManager.Instance.SetPrefabAndSoConnected(true);
        }

        private async Task LoadAndAssign(PrefabAndSOMappingSO.Entry e, HashSet<string> loaded, Type type)
        {
            if (string.IsNullOrEmpty(e.AddressKey)) return;
            if (!ScenePathIndex.TryGet(e.PathHash, out var go)) return;

            if (type == typeof(GameObject))
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(e.AddressKey);
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    AssignToMonoBehaviourField(go, e.fieldName, e.index, handle.Result);
                    loaded.Add(e.AddressKey);
                }
                else
                {
                    Debug.LogError($"[PrefabAndSOLoader] Prefab load failed: {e.AddressKey}");
                }
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<ScriptableObject>(e.AddressKey);
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    AssignToMonoBehaviourField(go, e.fieldName, e.index, handle.Result);
                }
                else
                {
                    Debug.LogError($"[PrefabAndSOLoader] SO load failed: {e.AddressKey}");
                }
            }
        }

        private void AssignToMonoBehaviourField(GameObject go, string fieldName, int index, UnityEngine.Object loadedObj)
        {
            if (go == null) return;

            var monos = go.GetComponents<MonoBehaviour>(); // 전역이 아닌 "해당 GO"에 붙은 컴포넌트만
            foreach (var mb in monos)
            {
                if (!mb) continue;

                var fields = mb.GetType().GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (field.Name != fieldName) continue;

                    if (index == -1) // 단일
                    {
                        field.SetValue(mb, loadedObj);
                    }
                    else // 리스트/배열
                    {
                        object val = field.GetValue(mb);
                        if (val is IList list)
                        {
                            while (list.Count <= index)
                            {
                                list.Add(null);
                            }
                            list[index] = loadedObj;
                        }
                        else
                        {
                            // 리스트가 null이면 생성 시도
                            if (field.FieldType.IsGenericType)
                            {
                                var inst = Activator.CreateInstance(field.FieldType) as IList;
                                while (inst.Count <= index)
                                {
                                    inst.Add(null);
                                }
                                inst[index] = loadedObj;
                                field.SetValue(mb, inst);
                            }
                            else
                            {
                                Debug.LogWarning($"[PrefabAndSOLoader] Field '{field.Name}' is not a list/array on {mb.name}");
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
}