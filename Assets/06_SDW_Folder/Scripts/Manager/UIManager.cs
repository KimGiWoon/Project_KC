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
                case UIName.SignInUI:
                    var signUI = _uiDic[uiName] as SignInUI;

                    if (_firebase != null)
                    {
                        signUI.OnSignInButtonClicked += _firebase.SignInWithGoogle;
                        _firebase.OnSignInSetButtonType += signUI.SetButtonImage;
                    }

                    break;
                case UIName.SetNicknameUI:
                    var setNicknameUI = _uiDic[uiName] as SetNicknameUI;

                    if (_firebase != null)
                    {
                        setNicknameUI.OnNicknameChange += _firebase.SetNickname;
                    }
                    break;
                case UIName.MainLobbyUI:
                    var mainLobbyUI = _uiDic[uiName] as MainLobbyUI;
                    mainLobbyUI.OnButtonClicked += OpenPanel;
                    break;
                case UIName.UserInfoUI:
                    var userInfoUI = _uiDic[uiName] as UserInfoUI;

                    if (_firebase != null)
                    {
                        userInfoUI.OnSignOutButtonClicked += _firebase.SignOut;
                        userInfoUI.OnDeleteButtonClicked += _firebase.DeleteAccount;
                    }
                    break;
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
                case UIName.SignInUI:
                    var signUI = _uiDic[uiName] as SignInUI;

                    if (_firebase != null)
                    {
                        signUI.OnSignInButtonClicked -= _firebase.SignInWithGoogle;
                        _firebase.OnSignInSetButtonType -= signUI.SetButtonImage;
                    }
                    break;
                case UIName.SetNicknameUI:
                    var setNicknameUI = _uiDic[uiName] as SetNicknameUI;

                    if (_firebase != null)
                    {
                        setNicknameUI.OnNicknameChange -= _firebase.SetNickname;
                    }
                    break;
                case UIName.MainLobbyUI:
                    var mainLobbyUI = _uiDic[uiName] as MainLobbyUI;
                    mainLobbyUI.OnButtonClicked -= OpenPanel;
                    break;
                case UIName.UserInfoUI:
                    var userInfoUI = _uiDic[uiName] as UserInfoUI;

                    if (_firebase != null)
                    {
                        userInfoUI.OnSignOutButtonClicked -= _firebase.SignOut;
                        userInfoUI.OnDeleteButtonClicked -= _firebase.DeleteAccount;
                    }
                    break;
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

        #region Loading Methods

        private void ConnectLoading()
        {
            var loadingObject = Resources.Load<GameObject>("UI/LoadingCanvas");
            _loadingCanvas = Instantiate(loadingObject, transform);
            _loadingCanvas.SetActive(false);

            var children = loadingObject.GetComponentsInChildren<RectTransform>(true);

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

        public void InitSceneLoadingUI()
        {
            _loadingCanvas.SetActive(true);

            //# 로딩 UI 초기화
            if (_loadingProgressBar != null) _loadingProgressBar.value = 0f;
            if (_loadingProgressText != null) _loadingProgressText.text = "0%";
            if (_loadingText != null) _loadingText.text = "Loading...";
        }
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