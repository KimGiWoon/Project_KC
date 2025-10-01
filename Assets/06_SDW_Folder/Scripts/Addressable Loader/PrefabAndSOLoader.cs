using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using UnityEngine.SceneManagement;

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
        private static List<AsyncOperationHandle> _loadedHandles = new List<AsyncOperationHandle>();
        private static string _currentSceneLabel = "";

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
                Debug.Log($"PrefabAndSO Mapping SO 연결 안됨 {GameManager.Instance.Scene.GetActiveScene()}");
                return;
            }
            if (!ScenePathIndex.IsBuilt) return;

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
            Debug.Log($"[PrefabAndSOLoader] Unloading assets with label: {label}");

            // 모든 핸들 릴리즈
            foreach (var handle in _loadedHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _loadedHandles.Clear();

            Debug.Log($"[PrefabAndSOLoader] Released {_loadedHandles.Count} handles for label: {label}");
        }

        private async void LoadFromAddressables()
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
                Debug.LogWarning($"[PrefabAndSOLoader] Scene '{_mappingSo.sceneName}' has no label assigned!");
            }

            var loadedKeys = new HashSet<string>();
            var tasks = new List<Task>();

            // Label을 기준으로 한번에 로드
            if (!string.IsNullOrEmpty(_currentSceneLabel))
            {
                Debug.Log($"[PrefabAndSOLoader] Loading assets with label: {_currentSceneLabel}");

                // Label로 에셋 로드
                var labelHandle = Addressables.LoadAssetsAsync<UnityEngine.Object>(
                    _currentSceneLabel,
                    (loadedAsset) =>
                    {
                        // 각 에셋이 로드될 때마다 호출됨
                    });

                await labelHandle.Task;

                if (labelHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _loadedHandles.Add(labelHandle);
                    Debug.Log(
                        $"[PrefabAndSOLoader] Successfully loaded {labelHandle.Result.Count} assets with label: {_currentSceneLabel}");
                }
                else
                {
                    Debug.LogError($"[PrefabAndSOLoader] Failed to load assets with label: {_currentSceneLabel}");
                }
            }

            // 각 Entry를 개별적으로 로드하고 할당
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
            Debug.Log(
                $"[PrefabAndSOLoader] Scene '{_mappingSo.sceneName}' (Label: {_currentSceneLabel}) - All assets loaded and assigned");
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
                    if (!loaded.Contains(e.AddressKey))
                    {
                        _loadedHandles.Add(handle);
                        loaded.Add(e.AddressKey);
                    }
                    AssignToMonoBehaviourField(go, e.fieldName, e.index, handle.Result);
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
                    if (!loaded.Contains(e.AddressKey))
                    {
                        _loadedHandles.Add(handle);
                        loaded.Add(e.AddressKey);
                    }
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

            var monos = go.GetComponents<MonoBehaviour>();
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

                    if (index == -1)
                    {
                        field.SetValue(mb, loadedObj);
                    }
                    else
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