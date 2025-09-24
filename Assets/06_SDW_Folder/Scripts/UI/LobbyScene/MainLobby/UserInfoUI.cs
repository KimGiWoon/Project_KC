using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        [SerializeField] private TweenAlpha_Image _backgroundPanel;
        [SerializeField] private GameObject _userInfoPanel;
        [SerializeField] private GameObject _medalPanel;
        [SerializeField] private RectTransform _changeIconPanelRect;
        [SerializeField] private RectTransform _editUserNamePanelRect;

        [Header("Other Container")]
        [SerializeField] private Button _editUserNameButton;
        [SerializeField] private Button _changeIconButton;

        [Header("Animations")]
        [SerializeField] private TweenAnimation _containerTweenAnimation;
        [SerializeField] private TweenAnimation _panelTweenAnimation;

        private GameManager _gameManager;
        private bool _isLoaded;

        public Action<UIName> OnUICloseRequested;
        public Action<UIName> OnUIOpenButtonRequested;
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
            _backgroundPanel.gameObject.SetActive(false);
            _medalPanel.SetActive(false);
            _gameManager = GameManager.Instance;
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            //# Change Nickname
            _editUserNameButton.onClick.AddListener(EditUserNameButtonClicked);
            _changeIconButton.onClick.AddListener(ChangeIconButtonClicked);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            //# Change Nickname
            _editUserNameButton.onClick.RemoveListener(EditUserNameButtonClicked);
            _changeIconButton.onClick.RemoveListener(ChangeIconButtonClicked);
        }

        public override void Open()
        {
            GameManager.Instance.Firebase.RequestUserInfo();
            _containerTweenAnimation.moveAway();
            base.Open();
            _backgroundPanel.gameObject.SetActive(true);
            _medalPanel.SetActive(true);
            _uiStack.Push(UIName.UserInfoUI);
        }

        public override void Close()
        {
            _backgroundPanel.FadeOut();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            _containerTweenAnimation.moveBack();
            yield return new WaitForSeconds(_containerTweenAnimation.tweenTime);
            base.Close();
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

                //# 패널 안에 터치가 있는지 확인
                if (RectTransformUtility.RectangleContainsScreenPoint(_userInfoPanelRect, touchPos)) return;
                if (RectTransformUtility.RectangleContainsScreenPoint(_changeIconPanelRect, touchPos)) return;
                if (RectTransformUtility.RectangleContainsScreenPoint(_editUserNamePanelRect, touchPos)) return;

                if (_uiStack.Count == 0) return;

                var uiName = _uiStack.Pop();

                if (uiName == UIName.ChangeIconUI) SetIconCanceled();

                _panelTweenAnimation.moveBack();
                OnUICloseRequested?.Invoke(uiName);
            }
        }

        #region Update User Info

        /// <summary>
        /// 사용자 정보를 UI 컴포넌트에 업데이트
        /// </summary>
        /// <param name="user">사용자 정보</param>
        public void UpdateUserInfo(UserInfo user)
        {
            _uidInfoText.text = $"uid : {user.UserId}";
            _nicknameInfoText.text = $"{user.Nickname}";
        }

        #endregion

        #region Buttons Methods

        private void EditUserNameButtonClicked()
        {
            if (_uiStack.Peek() == UIName.ChangeIconUI)
            {
                var uiName = _uiStack.Pop();
                OnUICloseRequested?.Invoke(UIName.ChangeIconUI);
            }
            _panelTweenAnimation.moveAway();
            _uiStack.Push(UIName.EditUsernameUI);
            OnUIOpenButtonRequested?.Invoke(UIName.EditUsernameUI);
        }

        /// <summary>
        /// Change Icon 버튼 클릭 이벤트 핸들러
        /// </summary>
        private void ChangeIconButtonClicked()
        {
            if (_uiStack.Peek() == UIName.EditUsernameUI)
            {
                var uiName = _uiStack.Pop();
                OnUICloseRequested?.Invoke(UIName.EditUsernameUI);
            }
            _panelTweenAnimation.moveAway();
            _uiStack.Push(UIName.ChangeIconUI);
            OnUIOpenButtonRequested?.Invoke(UIName.ChangeIconUI);
        }

        /// <summary>
        /// 사용자 아이콘 이미지를 지정된 스프라이트로 변경
        /// </summary>
        /// <param name="sprite">변경할 새로운 아이콘 스프라이트</param>
        public void SetIcon(Sprite sprite)
        {
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
            _panelTweenAnimation.moveBack();
        }

        #endregion
    }
}