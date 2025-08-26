using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class EditUsernameUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private Button _confirmButton;

        public Action<string> OnConfirmButtonClicked;
        public Action<UIName> OnCloseRequested;

        private string _currentNickname;

        private void Awake()
        {
            _panelContainer.SetActive(false);

            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        }

        private void ConfirmButtonClicked()
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

            OnConfirmButtonClicked?.Invoke(nickname);
            OnCloseRequested?.Invoke(UIName.EditUsernameUI);
            _errorText.text = "";
            _nicknameInputField.text = "";
        }

        /// <summary>
        /// 메인 로비 UI의 닉네임을 업데이트
        /// </summary>
        /// <param name="email">사용자의 이메일 주소</param>
        /// <param name="nickname">업데이트할 사용자의 닉네임</param>
        public void UpdateUserInfo(string nickname, string email = null, string uid = null) => _currentNickname = nickname;
    }
}