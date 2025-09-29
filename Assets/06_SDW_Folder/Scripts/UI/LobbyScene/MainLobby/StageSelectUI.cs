using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class StageSelectUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _stageNameText;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _permanentButton;
        [SerializeField] private Image _permanentRemarkImage;
        [SerializeField] private List<Image> _stageImageList;
        [SerializeField] private List<string> _stageNameList;
        [SerializeField] private GameObject _permanentPanel;
        [SerializeField] private TweenAlpha_Image _backgroundPanel;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _tweenAnimation;
        [SerializeField] private List<float> _stageXLocationList;
        [SerializeField] private RectTransform _stageRectTransform;
        [SerializeField] private float _tweenTime = 0.6f;

        [SerializeField] private GrowthManager _growthManager;

        private int _index = 0;
        private Vector2 _targetPos = new Vector2();
        private RectTransform _rectTransform;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private GameManager _gameManager;
        private bool _isProgress;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
            _backgroundPanel.gameObject.SetActive(false);
            _permanentRemarkImage.gameObject.SetActive(false);
            _gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            _leftButton.onClick.AddListener(LeftButtonClicked);
            _rightButton.onClick.AddListener(RightButtonClicked);
            _startButton.onClick.AddListener(StartButtonClicked);
            _permanentButton.onClick.AddListener(PermanentButtonClicked);
        }

        private void OnDisable()
        {
            _leftButton.onClick.RemoveListener(LeftButtonClicked);
            _rightButton.onClick.RemoveListener(RightButtonClicked);
            _startButton.onClick.RemoveListener(StartButtonClicked);
            _permanentButton.onClick.RemoveListener(PermanentButtonClicked);
        }

        /// <summary>
        /// UI 외부 터치 시 UI를 Close
        /// </summary>
        public void Update()
        {
            if (!_panelContainer.activeSelf || _isProgress) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (!RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, touchPos))
                {
                    if (_permanentPanel.activeSelf) return;

                    OnUICloseRequested?.Invoke(UIName.StageSelectUI);
                }
            }
        }

        public override void Open()
        {
            _isProgress = true;
            CheckPermanentRemark();
            _index = 0;
            _tweenAnimation.moveAway();
            StartCoroutine(DelayedOpen());
            base.Open();
            _backgroundPanel.gameObject.SetActive(true);
        }

        private IEnumerator DelayedOpen()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            _isProgress = false;
        }

        public override void Close()
        {
            _isProgress = true;
            _permanentRemarkImage.gameObject.SetActive(false);
            _backgroundPanel.FadeOut();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            _tweenAnimation.moveBack();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
            _isProgress = false;
        }

        public void CheckPermanentRemark()
        {
            int currentUnlockedNode = 0;
            for (int i = _gameManager.GrowthUnlockNodes.Count - 1; i >= 0; i--)
            {
                if (_gameManager.GrowthUnlockNodes[i] < 80100)
                    continue;

                currentUnlockedNode = _gameManager.GrowthUnlockNodes[i];
                break;
            }

            int currentCompletedNode = 0;
            for (int i = _gameManager.GrowthCompleteNodes.Count - 1; i >= 0; i--)
            {
                if (_gameManager.GrowthCompleteNodes[i] < 80100)
                    continue;

                currentCompletedNode = _gameManager.GrowthCompleteNodes[i];
                break;
            }

            if (_gameManager.GrowthCompleteNodes.Count != 0 && currentUnlockedNode == currentCompletedNode)
            {
                _permanentRemarkImage.gameObject.SetActive(false);
                return;
            }

            if (_growthManager.GrowthDataDic[currentUnlockedNode].nodeCurrency > _gameManager.Coin.point)
            {
                _permanentRemarkImage.gameObject.SetActive(false);
                return;
            }

            _permanentRemarkImage.gameObject.SetActive(true);
        }

        #region Button Methods

        private void LeftButtonClicked()
        {
            _index--;
            if (_index < 0) _index = 2;
            MoveBackgroundPanel();
        }

        private void RightButtonClicked()
        {
            _index++;
            if (_index > 2) _index = 0;
            MoveBackgroundPanel();
        }

        private void StartButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.MainLobbyUI);
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_RoguelikeScene);
        }

        private void PermanentButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.PermanentGrowthUI);
            OnUICloseRequested?.Invoke(UIName.StageSelectUI);
        }

        #endregion

        private void MoveBackgroundPanel()
        {
            if (_index == 0) _startButton.interactable = true;
            else _startButton.interactable = false;

            _targetPos.Set(_stageXLocationList[_index], _stageRectTransform.anchoredPosition.y);
            _stageRectTransform.DOAnchorPos(_targetPos, _tweenTime).SetEase(Ease.OutCubic);
            _stageNameText.text = _stageNameList[_index];
        }

        public void ClearButtonInteractable()
        {
            _startButton.interactable = false;
        }
    }
}