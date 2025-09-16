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
        [SerializeField] private TMP_FontAsset _font;
        private string _currentNickname;

        public Action<string> OnConfirmButtonClicked;
        public Action<UIName> OnCloseRequested;

        /// <summary>
        /// UI 요소가 활성화 준비를 마치고 초기화 작업을 수행하는 메서드
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
            _nicknameInputField.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
            _nicknameInputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string nickname)
        {
            string filtered = "";

            foreach (char c in nickname)
            {
                if (_font.HasCharacter(c)) filtered += c;
            }

            if (filtered != nickname) _nicknameInputField.text = filtered;
        }

        /// <summary>
        /// ConfirmButtonClicked 핸들러 메서드 호출로 사용자가 확인 버튼을 클릭했을 때 Nickname을 설정
        /// </summary>
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
        /// <param name="user">사용자 정보</param>
        public void UpdateUserInfo(UserInfo user)
        {
            _currentNickname = user.Nickname;
        }
    }
}