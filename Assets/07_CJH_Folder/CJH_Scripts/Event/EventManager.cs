using UnityEngine;
using SDW;

namespace CJH
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;
        private GameObject currentEventInstance;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // MapView가 사건 ID를 직접 전달하도록 변경
        public void StartEncounter(int encounterID)
        {
            if (currentEventInstance != null) Destroy(currentEventInstance);

            // DataManager에서 ID로 정확한 사건 데이터를 가져옵니다.
            EncounterTable encounterData = DataManager.Instance.GetEncounterByID(encounterID);

            if (encounterData.EncounterID != 0) // 유효한 데이터인지 확인
            {
                GameObject prefab = Resources.Load<GameObject>("Event");
                if (prefab == null)
                {
                    Debug.LogError("'Event.prefab'을 'Assets/Resources' 폴더에서 찾을 수 없습니다!");
                    return;
                }

                Canvas mainCanvas = FindObjectOfType<Canvas>();
                if (mainCanvas != null)
                {
                    currentEventInstance = Instantiate(prefab, mainCanvas.transform);
                    var eventStart = currentEventInstance.GetComponentInChildren<EventStart>();
                    if (eventStart != null)
                    {
                        eventStart.Initialize(encounterData);
                    }
                }
            }
        }

        public void EndEncounter()
        {
            if (currentEventInstance != null) Destroy(currentEventInstance);
            if (MapView.Instance != null) MapView.Instance.UpdateMapState();
        }
    }
}