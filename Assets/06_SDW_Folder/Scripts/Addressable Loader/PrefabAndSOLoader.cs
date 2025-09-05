using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // async/await 사용을 위해 추가
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SDW
{
    public class PrefabAndSOLoader : MonoBehaviour
    {
        [Tooltip("씬별로 생성된 PrefabAndSOMapping SO 할당")]
        public PrefabAndSOMappingSO _mappingSo;
        private bool _isDownloaded;
        public bool IsDownloaded => _isDownloaded;

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
            // async void 메서드 호출
            LoadFromAddressables();
            _isDownloaded = true;
        }

#if UNITY_EDITOR
        private void ApplyEditorPreview()
        {
            if (_mappingSo == null) return;

            foreach (var entry in _mappingSo.prefabEntries)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath);
                if (go == null) continue;
            }

            foreach (var entry in _mappingSo.soEntries)
            {
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(entry.AssetPath);
                if (so == null) continue;
            }
        }
#endif

        // 'async void'로 메서드 시그니처 변경
        private async void LoadFromAddressables()
        {
            if (_mappingSo == null) return;

            // 이미 처리된 키를 저장하기 위한 HashSet
            var loadedKeys = new HashSet<string>();

            // 1) Prefab 처리
            foreach (var entry in _mappingSo.prefabEntries)
            {
                if (loadedKeys.Contains(entry.AddressKey)) continue; // 이미 로드된 키는 건너뜁니다.

                var handle = Addressables.LoadAssetAsync<GameObject>(entry.AddressKey);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var prefab = handle.Result;
                    AssignToMonoBehaviourField(entry.Path, entry.fieldName, entry.index, prefab);
                    loadedKeys.Add(entry.AddressKey); // 로드 성공 시 키를 추가합니다.
                }
                else
                {
                    Debug.LogError($"[PrefabAndSOLoader] Prefab load failed: {entry.AddressKey}");
                }
            }

            // 2) ScriptableObject 처리
            foreach (var entry in _mappingSo.soEntries)
            {
                var handle = Addressables.LoadAssetAsync<ScriptableObject>(entry.AddressKey);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var so = handle.Result;
                    // 수정된 부분: fieldName과 index를 함께 전달
                    AssignToMonoBehaviourField(entry.Path, entry.fieldName, entry.index, so);
                    GameManager.Instance.SetPrefabAndSoConnected(true);
                }
                else
                {
                    Debug.LogError($"[PrefabAndSOLoader] SO load failed: {entry.AddressKey}");
                }
            }
            GameManager.Instance.SetPrefabAndSoConnected(true);
        }

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
                            field.SetValue(mb, loadedObj);
                        }
                        else // 리스트/배열 필드
                        {
                            var list = field.GetValue(mb) as IList;
                            if (list == null)
                            {
                                list = (IList)Activator.CreateInstance(field.FieldType);
                                field.SetValue(mb, list);
                            }

                            // ✨핵심: 빌드 시 null로 바뀐 요소를 인덱스를 사용해 덮어씁니다.✨
                            if (list.Count <= index)
                            {
                                while (list.Count <= index)
                                {
                                    list.Add(null);
                                }
                            }
                            list[index] = loadedObj;
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
    }
}