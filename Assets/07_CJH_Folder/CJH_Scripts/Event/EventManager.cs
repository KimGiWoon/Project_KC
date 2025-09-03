using UnityEngine;
using UnityEngine.UI;

namespace CJH
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;

        [Header("관리자 연결")]
        public MapView mapView;
        public EventStart evenStart;

        [Header("UI 패널 그룹")]
        public GameObject mapGroup;   // 맵 관련 UI들을 담고 있는 부모 오브젝트
        public GameObject eventGroup; // 이벤트 씬 UI들을 담고 있는 부모 오브젝트

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지되도록 설정
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // 게임 시작 시, 맵 화면은 보이고 이벤트 화면은 숨김
            mapGroup.SetActive(true);
            eventGroup.SetActive(false);
        }

        /// <summary>
        /// 지정된 ID의 사건을 시작합니다.
        /// </summary>
        public void StartEncounter(int encounterID)
        {
            Debug.Log($"GameManager: 사건 시작 (ID: {encounterID})");
            mapGroup.SetActive(false); // 맵 UI 숨기기
            eventGroup.SetActive(true);  // 이벤트 UI 보이기
            evenStart.StartEncounter(encounterID);
        }

        /// <summary>
        /// 사건이 종료되고 맵으로 돌아갑니다.
        /// </summary>
        public void EndEncounter()
        {
            Debug.Log("GameManager: 사건 종료, 맵으로 복귀");
            eventGroup.SetActive(false); // 이벤트 UI 숨기기
            mapGroup.SetActive(true);   // 맵 UI 보이기

            // 맵 상태를 업데이트하여 다음 진행을 준비합니다.
            mapView.UpdateMapState();
        }

    }
}