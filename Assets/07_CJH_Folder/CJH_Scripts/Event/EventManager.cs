using UnityEngine;
using SDW;
using KSH;

namespace CJH
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private DataManager _dataManager;
        [SerializeField] private RelicDropManager _relicDropManager;
        [SerializeField] private RelicInventoryUI _relicInventoryUI;
        private GameObject currentEventInstance;

        // MapView가 사건 ID를 직접 전달하도록 변경
        public void StartEncounter(int encounterID)
        {
            if (currentEventInstance != null) Destroy(currentEventInstance);

            // DataManager에서 ID로 정확한 사건 데이터를 가져옵니다.
            var encounterData = _dataManager.GetEncounterByID(encounterID);

            if (encounterData.EncounterID != 0) // 유효한 데이터인지 확인
            {
                var prefab = Resources.Load<GameObject>("Event");
                if (prefab == null)
                {
                    Debug.LogError("'Event.prefab'을 'Assets/Resources' 폴더에서 찾을 수 없습니다!");
                    return;
                }

                var mainCanvas = FindObjectOfType<Canvas>();
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
        }

        // 유물 획득
        public void GetRelic(RelicDatas relic)
        {
            _relicDropManager.GetRelic(relic);
            _relicInventoryUI.gameObject.SetActive(true); // 인벤토리 UI 활성화
        }

        // 유물 잃기
        public void LoseRelic(RelicDatas relic)
        {
            GameManager.Instance.InGameItem.RemoveItem(relic);
            Debug.Log($"{relic.relicName} 잃음");
            _relicInventoryUI.gameObject.SetActive(true); // 인벤토리 UI 활성화
        }
    }
}