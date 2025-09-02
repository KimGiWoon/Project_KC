using UnityEngine;
using UnityEngine.UI;

namespace CJH
{
    [RequireComponent(typeof(Button))]
    public class Organization : MonoBehaviour
    {
        [Header("UI 요소")]
        public GameObject teamPanel;
        public Button openButton;
        public Button closeButton; // X 버튼

        [Header("관리자 연결")]
        public TeamManager teamManager;
        public UIPanelSwitcher uiPanelSwitcher;

        void Start()
        {
            // 초기 UI 상태 설정
            teamPanel.SetActive(false);
            openButton.gameObject.SetActive(true);

            // 버튼 이벤트 연결
            openButton.onClick.AddListener(OpenTeamPanel);
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        // 편성 창 열기
        public void OpenTeamPanel()
        {
            teamPanel.SetActive(true);
            openButton.gameObject.SetActive(false);

            // TeamManager에 알려서 현재 팀 상태를 백업
            if (teamManager != null)
            {
                teamManager.OnPanelOpen();
            }

            // 하단 UI를 편성으로 고정하고 스위치 버튼 비활성화
            if (uiPanelSwitcher != null)
            {
                uiPanelSwitcher.ShowCharacterPanel();
                uiPanelSwitcher.SetSwitchButtonActive(false);
            }
        }

        // X 버튼 클릭 시
        private void OnCloseButtonClick()
        {
            // 팀이 3명으로 꽉 찼는지 확인
            if (teamManager != null && teamManager.IsTeamFull())
            {
                // 변경사항을 확정하고 창을 닫음
                teamManager.OnConfirmChanges();
                CloseTeamPanel();
            }
            else
            {
                // 팀원이 부족할 경우 메시지 출력
                Debug.Log("팀을 3명으로 모두 구성해야 합니다.");
                // 여기서 사용자에게 알림 UI를 띄워주는 로직을 추가
            }
        }

        // 편성 창 닫기 (내부 로직)
        private void CloseTeamPanel()
        {
            teamPanel.SetActive(false);
            openButton.gameObject.SetActive(true);

            // 하단 UI를 다시 노드 선택으로 바꾸고 스위치 버튼 활성화
            if (uiPanelSwitcher != null)
            {
                uiPanelSwitcher.ShowNodePanel();
                uiPanelSwitcher.SetSwitchButtonActive(true);
            }
        }
    }
}