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
        [SerializeField] public GameObject _eventPrefab;
        [SerializeField] public Canvas _stageGlobalCanvas;
        private GameObject currentEventInstance;

        // MapView가 사건 ID를 직접 전달하도록 변경
        public void StartEncounter(int encounterID)
        {
            if (currentEventInstance != null)
            {
                Debug.LogWarning($"[EventManager] 이전 이벤트 인스턴스가 남아있어 파괴합니다. ID: {currentEventInstance.GetInstanceID()}");
                Destroy(currentEventInstance);
                currentEventInstance = null;
            }

            // DataManager에서 ID로 정확한 사건 데이터를 가져옵니다.
            var encounterData = _dataManager.GetEncounterByID(encounterID);


            if (encounterData.EncounterID != 0) // 유효한 데이터인지 확인
            {


                if (_stageGlobalCanvas != null)
                {
                    currentEventInstance = Instantiate(_eventPrefab, _stageGlobalCanvas.transform);
                    Debug.Log($"[EventManager] 새 이벤트 인스턴스를 생성했습니다. ID: {currentEventInstance.GetInstanceID()}");
                    var eventStart = currentEventInstance.GetComponentInChildren<EventStart>();
                    if (eventStart != null)
                    {
                        Debug.Log("[EventManager] 이벤트 시작 - ID: " + encounterID);
                        eventStart.Initialize(encounterData);
                    }
                }
            }
        }

        //todo 1번만 뜨는 애들 -> 창이 뜰 때 돈/유물
        //todo 2

        public void EndEncounter()
        {
            //todo End인지, 선택지인지
            Debug.LogWarning($"[EventManager] 현재 이벤트 인스턴스를 파괴하고 참조를 null로 설정합니다. ID: {currentEventInstance.GetInstanceID()}");
            Destroy(currentEventInstance);
            currentEventInstance = null;
        }

    }
}