using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        //# Test
        public ImageSpriteMappingSO _mappingSo;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _userInfoPanelRect = _panelContainer.GetComponent<RectTransform>();
            _panelContainer.SetActive(false);
            _backgroundPanel.SetActive(false);
            _medalPanel.SetActive(false);
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            //# Sign Out & Delete Buttons
            _deleteAccountButton.onClick.AddListener(DeleteAccountButtonClicked);
            _signOutButton.onClick.AddListener(SignOutButtonClicked);

            //# Change Nickname
            _editUserNameButton.onClick.AddListener(EditUserNameButtonClicked);
            _changeIconButton.onClick.AddListener(ChangeIconButtonClicked);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            //# Sign Out & Delete Buttons
            _deleteAccountButton.onClick.RemoveListener(DeleteAccountButtonClicked);
            _signOutButton.onClick.RemoveListener(SignOutButtonClicked);

            //# Change Nickname
            _editUserNameButton.onClick.RemoveListener(EditUserNameButtonClicked);
            _changeIconButton.onClick.RemoveListener(ChangeIconButtonClicked);
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

        /// <summary>
        /// UI 외부 터치 시 UI를 Close
        /// </summary>
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

        /// <summary>
        /// 사용자 정보를 UI 컴포넌트에 업데이트
        /// </summary>
        /// <param name="nickname">사용자 닉네임</param>
        /// <param name="email">사용자 이메일</param>
        /// <param name="uid">사용자 고유 ID</param>
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

        /// <summary>
        /// 사용자 아이콘 이미지를 지정된 스프라이트로 변경
        /// </summary>
        /// <param name="sprite">변경할 새로운 아이콘 스프라이트</param>
        public void SetIcon(Sprite sprite)
        {
            Debug.Log($"Old sprite atlas: {_userIcon.sprite?.texture?.name}");
            Debug.Log($"New sprite atlas: {sprite?.texture?.name}");

            // 해당 Image가 ImageSpriteLoader 대상인지 확인
            string path = ImageSpriteLoader.GetPath(_userIcon.gameObject);
            ulong hash = PathHasher.Hash(path);
            Debug.Log($"Image Path: {path}");
            // Debug.Log($"Is in mapping: {_mappingSo.entries.Any(e => e.PathHash == hash)}");

            _userIconBackUp = _userIcon.sprite;
            _userIcon.sprite = sprite;
        }

        /// <summary>
        /// 사용자 아이콘 설정이 확정되었을 때의 처리를 수행
        /// 이전 아이콘 상태를 저장하고, 아이콘 변경 이벤트를 트리거
        /// </summary>
        public void SetIconConfirmed()
        {
            _userIconBackUp = _userIcon.sprite;
            OnIconChanged?.Invoke(_userIcon.sprite);
        }

        /// <summary>
        /// 사용자 아이콘 변경 작업을 취소하고 원래 아이콘으로 복원
        /// </summary>
        private void SetIconCanceled()
        {
            if (_userIconBackUp == null) return;
            _userIcon.sprite = _userIconBackUp;
        }

        /// <summary>
        /// UI 스택에서 지정된 UI를 제거
        /// </summary>
        /// <param name="uiName">제거할 UI의 이름</param>
        public void PopUI(UIName uiName)
        {
            var ui = _uiStack.Peek();
            if (ui == uiName)
                _uiStack.Pop();
        }

        #endregion
    }
}