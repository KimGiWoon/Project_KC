using UnityEngine;

namespace CJH
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;
        private GameObject currentEventInstance;

        public void StartEncounterForStage(int stage)
        {
            if (currentEventInstance != null)
            {
                Destroy(currentEventInstance);
            }

            EncounterDataSO randomEncounter = EncounterStageManager.Instance.GetRandomEncounter(stage);

            if (randomEncounter != null)
            {
                Debug.Log($"EventManager: {stage} 스테이지의 사건 시작 (ID: {randomEncounter.Row.EncounterID})");

                // Event을 Resources 폴더에서 불러옵니다.
                GameObject prefab = Resources.Load<GameObject>("Event");
                if (prefab == null)
                {
                    Debug.LogError("'Event'을 Resources 폴더에서 찾을 수 없습니다!");
                    return;
                }

                // 씬의 메인 캔버스 아래에 프리팹을 생성합니다.
                Canvas mainCanvas = FindObjectOfType<Canvas>();
                if (mainCanvas != null)
                {
                    currentEventInstance = Instantiate(prefab, mainCanvas.transform);
                    EventStart eventStart = currentEventInstance.GetComponentInChildren<EventStart>();
                    if (eventStart != null)
                    {
                        // EventStart에게 데이터를 넘겨 UI를 초기화시킵니다.
                        eventStart.Initialize(randomEncounter.Row);
                    }
                }
                else
                {
                    Debug.LogError("씬에 Canvas가 없습니다!");
                }
            }
            else
            {
                Debug.LogError($"{stage} 스테이지에 대한 인카운터를 시작할 수 없습니다.");
            }
        }

        public void EndEncounter()
        {
            Debug.Log("EventManager: 사건 종료");
            if (currentEventInstance != null)
            {
                Destroy(currentEventInstance);
            }
        }
    }
}