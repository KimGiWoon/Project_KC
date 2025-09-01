using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDW
{
    public class UserInfoUI : BaseUI
    {
        [Header("User Info Settings")]
        [SerializeField] private TextMeshProUGUI _uidInfoText;
        [SerializeField] private TextMeshProUGUI _nicknameInfoText;
        [SerializeField] private Image _userIcon;
        private Sprite _userIconBackUp;

        [Header("Panels")]
        [SerializeField] private GameObject _backgroundPanel;
        [SerializeField] private GameObject _userInfoPanel;
        [SerializeField] private GameObject _medalPanel;
        // [SerializeField] private GameObject _deleteAccountPanel;
        // [SerializeField] private GameObject _editUserNamePanel;
        // [SerializeField] private GameObject _changeIconPanel;

        [Header("Buttons")]
        [SerializeField] private Button _deleteAccountButton;
        [SerializeField] private Button _signOutButton;
        [SerializeField] private Button _editUserNameButton;
        [SerializeField] private Button _changeIconButton;

        public Action<UIName> OnUICloseRequested;
        public Action<UIName> OnUIOpenButtonClicked;
        public Action OnSignOutButtonClicked;
        public Action<Sprite> OnIconChanged;

        private RectTransform _userInfoPanelRect;
        private Stack<UIName> _uiStack = new Stack<UIName>();

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _userInfoPanelRect = _panelContainer.GetComponent<RectTransform>();
            _panelContainer.SetActive(false);
            _backgroundPanel.SetActive(false);
            _medalPanel.SetActive(false);

            //# Sign Out & Delete Buttons
            _deleteAccountButton.onClick.AddListener(DeleteAccountButtonClicked);
            _signOutButton.onClick.AddListener(SignOutButtonClicked);

            //# Change Nickname
            _editUserNameButton.onClick.AddListener(EditUserNameButtonClicked);
            _changeIconButton.onClick.AddListener(ChangeIconButtonClicked);
        }

        public override void Open()
        {
            base.Open();
            _backgroundPanel.SetActive(true);
            _medalPanel.SetActive(true);
            _uiStack.Push(UIName.UserInfoUI);
        }

        public override void Close()
        {
            base.Close();
            _backgroundPanel.SetActive(false);
            _medalPanel.SetActive(false);
            _uiStack.Clear();
        }

        public void Update()
        {
            if (!_panelContainer.activeSelf) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //todo 영역이 각 panel의 영억으로 변경해야 함

                //# 패널 안에 터치가 있는지 확인
                if (!RectTransformUtility.RectangleContainsScreenPoint(_userInfoPanelRect, touchPos))
                {
                    if (_uiStack.Count == 0) return;

                    var uiName = _uiStack.Pop();

                    if (uiName == UIName.ChangeIconUI) SetIconCanceled();

                    OnUICloseRequested?.Invoke(uiName);
                }
            }
        }

        #region Update User Info

        public void UpdateUserInfo(string nickname, string email, string uid)
        {
            _uidInfoText.text = $"uid : {uid}";
            _nicknameInfoText.text = $"{nickname}";
        }

        #endregion

        #region Buttons Methods

        /// <summary>
        /// Delete Account 버튼 클릭 이벤트 핸들러
        /// </summary>
        private void DeleteAccountButtonClicked()
        {
            _uiStack.Push(UIName.DeleteAccountUI);
            OnUIOpenButtonClicked?.Invoke(UIName.DeleteAccountUI);
        }

        /// <summary>
        /// 호출된 경우 사용자 정보 UI에서로그아웃 기능을 실행하는 이벤트 핸들러 메소드
        /// </summary>
        private void SignOutButtonClicked() => OnSignOutButtonClicked?.Invoke();

        private void EditUserNameButtonClicked()
        {
            _uiStack.Push(UIName.EditUsernameUI);
            OnUIOpenButtonClicked?.Invoke(UIName.EditUsernameUI);
        }

        /// <summary>
        /// Change Icon 버튼 클릭 이벤트 핸들러
        /// </summary>
        private void ChangeIconButtonClicked()
        {
            _uiStack.Push(UIName.ChangeIconUI);
            OnUIOpenButtonClicked?.Invoke(UIName.ChangeIconUI);
        }

        public void SetIcon(Sprite sprite)
        {
            _userIconBackUp = _userIcon.sprite;
            _userIcon.sprite = sprite;
        }

        public void SetIconConfirmed()
        {
            _userIconBackUp = _userIcon.sprite;
            OnIconChanged?.Invoke(_userIcon.sprite);
        }

        public void SetIconCanceled()
        {
            if (_userIconBackUp == null) return;
            _userIcon.sprite = _userIconBackUp;
        }

        #endregion
    }
}