using UnityEngine;
using SDW;
using KSH;
using System.Collections.Generic;
using System.Linq;

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

        public List<RelicDatas> allRelicsDatabase;

        public EncounterTable CurrentEncounterData { get; private set; }

        void Awake()
        {
            allRelicsDatabase = Resources.LoadAll<RelicDatas>("Relics").ToList();
        }

        // MapView가 사건 ID를 직접 전달하도록 변경
        public void StartEncounter(int eventGroupID)
        {
            if (currentEventInstance != null)
            {
                Debug.LogWarning($"[EventManager] 이전 이벤트 인스턴스가 남아있어 파괴합니다. ID: {currentEventInstance.GetInstanceID()}");
                Destroy(currentEventInstance);
                currentEventInstance = null;
            }


            // GroupID로 해당 그룹의 모든 이벤트 리스트를 가져옵니다.
            var encountersInGroup = _dataManager.GetEncountersByGroupID(eventGroupID);

            // 그룹에 이벤트가 없으면 오류를 출력하고 종료합니다.
            if (encountersInGroup == null || encountersInGroup.Count == 0)
            {
                Debug.LogError($"[EventManager] EventGroupID '{eventGroupID}'에 해당하는 이벤트를 찾을 수 없습니다.");
                return;
            }

            // 리스트에서 랜덤하게 하나의 이벤트를 선택합니다.
            int randomIndex = Random.Range(0, encountersInGroup.Count);
            var encounterData = encountersInGroup[randomIndex];

            this.CurrentEncounterData = encounterData;

            if (encounterData.EncounterID != 0) // 유효한 데이터인지 확인
            {
                if (_stageGlobalCanvas != null)
                {

                    currentEventInstance = Instantiate(_eventPrefab, _stageGlobalCanvas.transform);
                    Debug.Log($"[EventManager] 새 이벤트 인스턴스를 생성했습니다. ID: {currentEventInstance.GetInstanceID()}");
                    var eventStart = currentEventInstance.GetComponentInChildren<EventStart>();
                    if (eventStart != null)
                    {
                        // 이제 선택된 랜덤 이벤트의 ID가 로그에 찍힙니다.
                        Debug.Log($"[EventManager] 이벤트 시작 - Group: {eventGroupID}, Selected ID: {encounterData.EncounterID}");
                        eventStart.Initialize(encounterData, this);
                    }
                }
            }
        }

        /// <summary>
        /// 특정 EncounterID를 직접 지정하여 이벤트를 시작하는 함수입니다.
        /// </summary>
        public void StartEncounterByID(int encounterID)
        {
            if (currentEventInstance != null)
            {
                Destroy(currentEventInstance);
                currentEventInstance = null;
            }

            var encounterData = _dataManager.GetEncounterByID(encounterID);

            if (encounterData.EncounterID != 0)
            {
                this.CurrentEncounterData = encounterData; // 현재 데이터 저장

                if (_stageGlobalCanvas != null)
                {
                    currentEventInstance = Instantiate(_eventPrefab, _stageGlobalCanvas.transform);
                    var eventStart = currentEventInstance.GetComponentInChildren<EventStart>();
                    if (eventStart != null)
                    {
                        eventStart.Initialize(encounterData, this);
                    }
                }
            }
            else
            {
                Debug.LogError($"[EventManager] StartEncounterByID Error: ID '{encounterID}'에 해당하는 이벤트를 찾을 수 없습니다.");
            }
        }

        public void EndEncounter()
        {
            Debug.LogWarning($"[EventManager] 현재 이벤트 인스턴스를 파괴하고 참조를 null로 설정합니다. ID: {currentEventInstance.GetInstanceID()}");
            Destroy(currentEventInstance);
            currentEventInstance = null;
        }

    }
}