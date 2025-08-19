using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class UserInfoUI : BaseUI
    {
        [Header("Close Setting")]
        [SerializeField] private Button _closeButton;
        public Action<UIName> OnCloseButtonClicked;

        [Header("User Info Settings")]
        [SerializeField] private TextMeshProUGUI _emialInfoText;
        [SerializeField] private TextMeshProUGUI _nicknameInfoText;

        [Header("Change Nickname Settings")]
        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private Button _editButton;
        [SerializeField] private TextMeshProUGUI _errorText;
        public Action<string> OnEditButtonClicked;

        [Header("Sign Out & Delete Buttons")]
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _signOutButton;

        public Action OnDeleteButtonClicked;
        public Action OnSignOutButtonClicked;

        private string _currentNickname;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);

            //# Close Button
            _closeButton.onClick.AddListener(CloseButtonClicked);

            //# Change Nickname
            _editButton.onClick.AddListener(EditButtonClicked);

            //# Sign Out & Delete Buttons
            _signOutButton.onClick.AddListener(SignOutButtonClicked);
            _deleteButton.onClick.AddListener(DeleteButtonClicked);
        }

        private void CloseButtonClicked() => OnCloseButtonClicked?.Invoke(UIName.UserInfoUI);

        #region Update User Info

        public void UpdateUserInfo(string email, string nickname)
        {
            _emialInfoText.text = $"Email : {email}";
            _nicknameInfoText.text = $"Nickname : {nickname}";
            _currentNickname = nickname;
        }

        #endregion

        #region Changed Nickname Info

        private void EditButtonClicked()
        {
            string nickname = _nicknameInputField.text;

            if (string.IsNullOrEmpty(nickname))
            {
                _errorText.text = "닉네임을 입력해주세요";
                _nicknameInputField.text = "";
                return;
            }

            if (_currentNickname.Equals(nickname))
            {
                _errorText.text = "기존 닉네임과 동일합니다.";
                _nicknameInputField.text = "";
                return;
            }

            _errorText.text = "";
            OnEditButtonClicked?.Invoke(nickname);
        }

        #endregion

        #region Sign Out & Delete Buttons

        /// <summary>
        /// 호출된 경우 사용자 정보 UI에서로그아웃 기능을 실행하는 이벤트 핸들러 메소드
        /// </summary>
        private void SignOutButtonClicked() => OnSignOutButtonClicked?.Invoke();

        /// <summary>
        /// Delete 버튼 클릭 이벤트 핸들러
        /// </summary>
        private void DeleteButtonClicked() => OnDeleteButtonClicked?.Invoke();

        #endregion
    }
}