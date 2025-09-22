using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public partial class UIManager : MonoBehaviour, ISceneLoadable
    {
        private Dictionary<UIName, BaseUI> _uiDic = new Dictionary<UIName, BaseUI>();
        public IReadOnlyDictionary<UIName, BaseUI> UiDic = new Dictionary<UIName, BaseUI>();

        private FirebaseManager _firebase;

        //# Loading Settings 
        private GameObject _loadingCanvas;
        private TMP_Text _loadingText;
        private TMP_Text _loadingProgressText;
        private Slider _loadingProgressBar;

        private UIName _prevOpenedUI = UIName.None;
        private UIName _prevClosedUI = UIName.None;

        private GameManager _gameManager;
        private bool _isLoaded;
        private bool _uiOnly;

        /// <summary>
        /// Firebase 연결 및 초기화
        /// </summary>
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _firebase = _gameManager.Firebase;
            ConnectLoading();
            StartCoroutine(DelayedOpenDownloadUI());
        }

        private IEnumerator DelayedOpenDownloadUI()
        {
            yield return new WaitForSeconds(0.5f);
            OpenPanel(UIName.DownloadUI);
            _firebase?.ConnectToFirebase();
        }

        /// <summary>
        /// 해당 Scene의 모든 Sprite와 Prefab, SO를 연결한 후 loading canvas off
        /// </summary>
        private void Update()
        {
            if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                _isLoaded) return;

            StartCoroutine(DelayedLoading());
            _isLoaded = true;
        }

        /// <summary>
        /// Loading 후 해당 Scene의 Sprite와 Prefab, SO가 모두 로딩될 때까지 딜레이 후 loading canvas off
        /// </summary>
        private IEnumerator DelayedLoading()
        {
            yield return new WaitForSeconds(1.5f);
            _loadingCanvas.SetActive(false);
        }

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
            _isLoaded = false;

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
        public void CompleteSceneLoading(UIName targetUI)
        {
            if (_loadingText != null) _loadingText.text = "Complete!";

            var activeScene = (SceneName)Enum.Parse(typeof(SceneName), GameManager.Instance.Scene.GetActiveScene());

            switch (activeScene)
            {
                case SceneName.SDW_SignInScene:
                    // _firebase.ConnectToFirebase();
                    OpenPanel(UIName.DownloadUI);
                    break;
                case SceneName.SDW_LobbyScene:
                    if (targetUI == UIName.None)
                        OpenPanel(UIName.MainLobbyUI);
                    else
                        OpenPanel(targetUI);
                    break;
                case SceneName.SDW_RoguelikeScene:
                    if (targetUI == UIName.None)
                    {
                        if (_gameManager.Score == 0) OpenPanel(UIName.StageGlobalUI);
                        // else OpenPanel(UIName.RoguelikeClosingUI);
                        else StartCoroutine(DelayedOpen(UIName.RoguelikeClosingUI));
                    }
                    else
                        OpenPanel(targetUI);
                    break;
            }
        }

        private IEnumerator DelayedOpen(UIName uiName)
        {
            yield return new WaitForSeconds(1.5f);
            OpenPanel(uiName);
        }

        #endregion
    }
}