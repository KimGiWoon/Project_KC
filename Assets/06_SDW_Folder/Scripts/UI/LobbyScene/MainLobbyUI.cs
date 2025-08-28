using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class MainLobbyUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private Button _userInfoButton;
        [SerializeField] private Button _dailyQuestButton;
        [SerializeField] private Button _gachaButton;
        [SerializeField] private TextMeshProUGUI _nicknameText;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        private void OnEnable()
        {
            _gameStartButton.onClick.AddListener(GameStartButtonClicked);
            _userInfoButton.onClick.AddListener(UserInfoButtonClicked);
            _dailyQuestButton.onClick.AddListener(DailyQuestButtonClicked);
            _gachaButton.onClick.AddListener(GachaButtonClicked);
        }

        private void OnDisable()
        {
            _gameStartButton.onClick.RemoveListener(GameStartButtonClicked);
            _userInfoButton.onClick.RemoveListener(UserInfoButtonClicked);
            _dailyQuestButton.onClick.RemoveListener(DailyQuestButtonClicked);
            _gachaButton.onClick.RemoveListener(GachaButtonClicked);
        }

        #region Button Methods

        private void GameStartButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.KGW_StageSelectUI);
            OnUICloseRequested?.Invoke(UIName.MainLobbyUI);
        }

        /// <summary>
        /// 사용자 정보 버튼 클릭 이벤트 핸들러 메서드 호출
        /// </summary>
        private void UserInfoButtonClicked() => OnUIOpenRequested?.Invoke(UIName.UserInfoUI);

        private void DailyQuestButtonClicked() => OnUIOpenRequested?.Invoke(UIName.DailyQuestUI);

        private void GachaButtonClicked() => OnUIOpenRequested?.Invoke(UIName.GachaMainUI);

        #endregion

        #region Update User Info

        /// <summary>
        /// 메인 로비 UI의 닉네임을 업데이트
        /// </summary>
        /// <param name="email">사용자의 이메일 주소</param>
        /// <param name="nickname">업데이트할 사용자의 닉네임</param>
        public void UpdateUserInfo(string nickname, string email = null, string uid = null) => _nicknameText.text = nickname;

        #endregion
    }
}