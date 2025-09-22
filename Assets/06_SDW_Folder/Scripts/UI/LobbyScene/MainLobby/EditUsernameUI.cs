using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class EditUsernameUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private GameObject _errorMessagePopup;
        [SerializeField] private TextMeshProUGUI _erroMessageText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TMP_FontAsset _font;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _tweenAnimation;
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
            _cancelButton.onClick.AddListener(CancelButtonClicked);
            _nicknameInputField.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
            _cancelButton.onClick.RemoveListener(CancelButtonClicked);
            _nicknameInputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        public override void Open()
        {
            _tweenAnimation.moveAway();
            GameManager.Instance.Firebase.RequestUserInfo();
            base.Open();
        }

        public override void Close()
        {
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            _tweenAnimation.moveBack();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
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
                _erroMessageText.text = "닉네임을 입력해주세요";
                _nicknameInputField.text = "";
                _errorMessagePopup.SetActive(true);
                return;
            }

            if (_currentNickname.Equals(nickname))
            {
                _erroMessageText.text = "기존 닉네임과 동일합니다.";
                _nicknameInputField.text = "";
                _errorMessagePopup.SetActive(true);
                return;
            }

            OnConfirmButtonClicked?.Invoke(nickname);
            OnCloseRequested?.Invoke(UIName.EditUsernameUI);
            _erroMessageText.text = "";
            _nicknameInputField.text = "";
        }

        private void CancelButtonClicked()
        {
            _nicknameInputField.text = "";
            OnCloseRequested?.Invoke(UIName.EditUsernameUI);
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