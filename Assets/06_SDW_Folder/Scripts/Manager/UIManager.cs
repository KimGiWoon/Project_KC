using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class UIManager : MonoBehaviour, ISceneLoadable
    {
        //todo Addressable에서 사용 - 다운로드 관련 UI 추가 필요
        private Dictionary<UIName, BaseUI> _uiDic = new Dictionary<UIName, BaseUI>();

        private FirebaseManager _firebase;

        //# Loading Settings 
        private GameObject _loadingCanvas;
        private TMP_Text _loadingText;
        private TMP_Text _loadingProgressText;
        private Slider _loadingProgressBar;

        /// <summary>
        /// Firebase 컴포넌트 연결 
        /// </summary>
        private void Awake()
        {
            _firebase = GetComponent<FirebaseManager>();
        }

        /// <summary>
        /// Firebase 연결 초기화
        /// </summary>
        private void Start()
        {
            _firebase?.ConnectToFirebase();
            ConnectLoading();
        }

        #region Panel Methods

        /// <summary>
        /// 지정된 UI 패널을 활성화하고 관련 이벤트 핸들러를 연결
        /// </summary>
        /// <param name="uiName">활성화할 패널의 이름</param>
        public void OpenPanel(UIName uiName)
        {
            Debug.Log($"Open UI name : {uiName}");
            _uiDic[uiName].Open();

            switch (uiName)
            {
                //# signin scene
                case UIName.SignInUI: ConnectSignInUI(uiName); break;
                case UIName.SetNicknameUI: ConnectNicknameUI(uiName); break;
                case UIName.DownloadUI: ConnectDownloadUI(uiName); break;
                //# Main Lobby Scene
                case UIName.MainLobbyUI: ConnectMainLobbyUI(uiName); break;
                case UIName.UserInfoUI: ConnectUserInfoUI(uiName); break;
                case UIName.DeleteAccountUI: ConnectDeleteAccountUI(uiName); break;
                case UIName.EditUsernameUI: ConnectEditUsernameUI(uiName); break;
                case UIName.ChangeIconUI: ConnectChangeIconUI(uiName); break;
            }
        }

        /// <summary>
        /// 현재 활성화된 패널을 비활성화하고 관련 이벤트 핸들러에서 해당 메서드를 제거
        /// </summary>
        /// <param name="uiName">비활성화할 패널의 이름</param>
        public void ClosePanel(UIName uiName)
        {
            Debug.Log($"Close UI name : {uiName}");
            _uiDic[uiName].Close();

            switch (uiName)
            {
                //# signin scene
                case UIName.SignInUI: DisconnectSignInUI(uiName); break;
                case UIName.SetNicknameUI: DisconnectNicknameUI(uiName); break;
                case UIName.DownloadUI: DisconnectDownloadUI(uiName); break;
                //# Main Lobby Scene
                case UIName.MainLobbyUI: DisconnectMainLobbyUI(uiName); break;
                case UIName.UserInfoUI: DisconnectUserInfoUI(uiName); break;
                case UIName.DeleteAccountUI: DisconnectDeleteAccountUI(uiName); break;
                case UIName.EditUsernameUI: DisconnectEdiUsernameUI(uiName); break;
                case UIName.ChangeIconUI: DisconnectChangeIconUI(uiName); break;
            }
        }

        /// <summary>
        /// 지정된 UI 패널을 관리자에 추가
        /// </summary>
        /// <param name="ui">추가할 BaseUI 파생 클래스 인스턴스</param>
        public void AddPanel(BaseUI ui) => _uiDic[ui.Name] = ui;

        /// <summary>
        /// 지정된 UI 패널을 UIManager에서 제거
        /// </summary>
        /// <param name="ui">제거할 BaseUI 파생 클래스 인스턴스</param>
        public void RemovePanel(BaseUI ui) => _uiDic.Remove(ui.Name);

        #endregion

        #region UI Connect Methods

        private void ConnectSignInUI(UIName uiName)
        {
            var signUI = _uiDic[uiName] as SignInUI;

            if (_firebase != null)
            {
                signUI.OnSignInButtonClicked += _firebase.SignInWithGoogle;
                _firebase.OnSignInSetButtonType += signUI.SetButtonImage;
            }
        }

        private void ConnectNicknameUI(UIName uiName)
        {
            var setNicknameUI = _uiDic[uiName] as SetNicknameUI;

            if (_firebase != null)
            {
                setNicknameUI.OnNicknameChange += _firebase.SetNickname;
            }
        }

        private void ConnectDownloadUI(UIName uiName)
        {
            var downloadUI = _uiDic[uiName] as DownloadUI;
            downloadUI.OnUICloseRequested += ClosePanel;

            if (_firebase != null)
            {
                _firebase.OnCheckUpdate += downloadUI.OnCheckUpdate;
            }
        }

        private void ConnectMainLobbyUI(UIName uiName)
        {
            var mainLobbyUI = _uiDic[uiName] as MainLobbyUI;
            mainLobbyUI.OnButtonClicked += OpenPanel;

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo += mainLobbyUI.UpdateUserInfo;
                _firebase.RequestUserInfo();
            }
        }

        private void ConnectUserInfoUI(UIName uiName)
        {
            var userInfoUI = _uiDic[uiName] as UserInfoUI;
            userInfoUI.OnUICloseRequsted += ClosePanel;
            userInfoUI.OnUIOpenButtonClicked += OpenPanel;

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo += userInfoUI.UpdateUserInfo;
                userInfoUI.OnSignOutButtonClicked += _firebase.SignOut;
                _firebase.RequestUserInfo();
            }
        }

        private void ConnectDeleteAccountUI(UIName uiName)
        {
            var deleteAccountUI = _uiDic[uiName] as DeleteAccountUI;
            deleteAccountUI.OnDeleteAcceptButtonClicked += _firebase.DeleteAccount;
            deleteAccountUI.OnCloseButtonClicked += ClosePanel;
        }

        private void ConnectEditUsernameUI(UIName uiName)
        {
            var editUsernameUI = _uiDic[uiName] as EditUsernameUI;
            editUsernameUI.OnConfirmButtonClicked += _firebase.SetNickname;
            editUsernameUI.OnCloseRequested += ClosePanel;

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo += editUsernameUI.UpdateUserInfo;
                _firebase.RequestUserInfo();
            }
        }

        private void ConnectChangeIconUI(UIName uiName)
        {
            var changeIconUI = _uiDic[uiName] as ChangeIconUI;
            changeIconUI.OnCloseRequested += ClosePanel;
        }

        #endregion

        #region UI Disconnect Methods

        private void DisconnectSignInUI(UIName uiName)
        {
            var signUI = _uiDic[uiName] as SignInUI;

            if (_firebase != null)
            {
                signUI.OnSignInButtonClicked -= _firebase.SignInWithGoogle;
                _firebase.OnSignInSetButtonType -= signUI.SetButtonImage;
            }
        }

        private void DisconnectNicknameUI(UIName uiName)
        {
            var setNicknameUI = _uiDic[uiName] as SetNicknameUI;

            if (_firebase != null)
                setNicknameUI.OnNicknameChange -= _firebase.SetNickname;
        }

        private void DisconnectDownloadUI(UIName uiName)
        {
            var downloadUI = _uiDic[uiName] as DownloadUI;
            downloadUI.OnUICloseRequested -= ClosePanel;

            if (_firebase != null)
            {
                _firebase.OnCheckUpdate -= downloadUI.OnCheckUpdate;
            }
        }

        private void DisconnectMainLobbyUI(UIName uiName)
        {
            var mainLobbyUI = _uiDic[uiName] as MainLobbyUI;
            mainLobbyUI.OnButtonClicked -= OpenPanel;

            if (_firebase != null)
                _firebase.OnSendUserInfo -= mainLobbyUI.UpdateUserInfo;
        }

        private void DisconnectUserInfoUI(UIName uiName)
        {
            var userInfoUI = _uiDic[uiName] as UserInfoUI;
            userInfoUI.OnUICloseRequsted -= ClosePanel;

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo += userInfoUI.UpdateUserInfo;
                userInfoUI.OnSignOutButtonClicked -= _firebase.SignOut;
            }
        }

        private void DisconnectDeleteAccountUI(UIName uiName)
        {
            var deleteAccountUI = _uiDic[uiName] as DeleteAccountUI;
            deleteAccountUI.OnDeleteAcceptButtonClicked -= _firebase.DeleteAccount;
            deleteAccountUI.OnCloseButtonClicked -= ClosePanel;
        }

        private void DisconnectEdiUsernameUI(UIName uiName)
        {
            var editUsernameUI = _uiDic[uiName] as EditUsernameUI;
            editUsernameUI.OnConfirmButtonClicked -= _firebase.SetNickname;
            editUsernameUI.OnCloseRequested -= ClosePanel;

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo -= editUsernameUI.UpdateUserInfo;
                _firebase.RequestUserInfo();
            }
        }

        private void DisconnectChangeIconUI(UIName uiName)
        {
            var changeIconUI = _uiDic[uiName] as ChangeIconUI;
            changeIconUI.OnCloseRequested += ClosePanel;
        }

        #endregion

        #region Loading Methods

        /// <summary>
        /// 초기 로딩 화면을 설정하고 로딩 관련 UI 컴포넌트를 초기화
        /// </summary>
        private void ConnectLoading()
        {
            var loadingObject = Resources.Load<GameObject>("UI/LoadingCanvas");
            _loadingCanvas = Instantiate(loadingObject, transform);
            _loadingCanvas.SetActive(false);

            var children = _loadingCanvas.GetComponentsInChildren<RectTransform>(true);

            foreach (var child in children)
            {
                if (child.gameObject.name.Equals("Loading Text"))
                    _loadingText = child.GetComponent<TMP_Text>();
                else if (child.gameObject.name.Equals("Loading Progress Bar"))
                    _loadingProgressBar = child.GetComponent<Slider>();
                else if (child.gameObject.name.Equals("Loading Progress Text"))
                    _loadingProgressText = child.GetComponent<TMP_Text>();
            }
        }

        /// <summary>
        /// Scene Loading과 관련된 UI 요소를 초기화하고 로딩 화면, Progress bar 등의 시각적인 요소를 초기화
        /// </summary>
        public void InitSceneLoadingUI()
        {
            _loadingCanvas.SetActive(true);

            //# 로딩 UI 초기화
            if (_loadingProgressBar != null) _loadingProgressBar.value = 0f;
            if (_loadingProgressText != null) _loadingProgressText.text = "0%";
            if (_loadingText != null) _loadingText.text = "Loading...";
        }

        /// <summary>
        /// Scene 로딩 중인 UI 요소의 진행 상황을 업데이트
        /// </summary>
        /// <param name="progress">로딩 진행률 (0.0f부터 1.0f 사이의 값)</param>
        public void UpdateLoadingUI(float progress)
        {
            if (_loadingProgressBar != null) _loadingProgressBar.value = progress;

            if (_loadingProgressText != null) _loadingProgressText.text = $"{Mathf.FloorToInt(progress * 100)}%";

            if (_loadingText != null)
            {
                int dotCount = Mathf.FloorToInt(Time.unscaledTime * 2) % 4;
                _loadingText.text = "Loading" + new string('.', dotCount);
            }
        }

        /// <summary>
        /// 지정된 장면 로딩을 완료하고 관련 로딩 UI 요소를 해제
        /// 로딩 완료 메시지 표시와 함께 로딩 화면을 비활성화
        /// </summary>
        public void CompleteSceneLoading()
        {
            if (_loadingText != null) _loadingText.text = "Complete!";

            _loadingCanvas.SetActive(false);

            var activeScene = (SceneName)Enum.Parse(typeof(SceneName), GameManager.Instance.Scene.GetActiveScene());

            switch (activeScene)
            {
                case SceneName.SDW_SignInScene:
                    _firebase.ConnectToFirebase();
                    break;
                case SceneName.SDW_LobbyScene:
                    OpenPanel(UIName.MainLobbyUI);
                    break;
            }
        }

        #endregion
    }
}