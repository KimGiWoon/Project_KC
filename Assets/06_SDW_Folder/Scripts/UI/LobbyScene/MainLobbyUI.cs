using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class MainLobbyUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _userInfoButton;
        [SerializeField] private TextMeshProUGUI _nicknameText;

        public Action<UIName> OnButtonClicked;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _userInfoButton.onClick.AddListener(UserInfoButtonClicked);
        }

        /// <summary>
        /// 사용자 정보 버튼 클릭 이벤트 핸들러 메서드 호출
        /// </summary>
        private void UserInfoButtonClicked() => OnButtonClicked?.Invoke(UIName.UserInfoUI);

        #region Update User Info

        /// <summary>
        /// 메인 로비 UI의 닉네임을 업데이트
        /// </summary>
        /// <param name="email">사용자의 이메일 주소</param>
        /// <param name="nickname">업데이트할 사용자의 닉네임</param>
        public void UpdateUserInfo(string email, string nickname) => _nicknameText.text = nickname;

        #endregion
    }
}