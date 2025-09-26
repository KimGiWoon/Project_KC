using System;
using System.Collections;
using JJY;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

namespace KSH
{
    public class GachaMainUI : BaseUI
    {
        [Header("Top Components")]
        [SerializeField] private TextMeshProUGUI _shiningStarValueText;
        [SerializeField] private Button _shiningStarButton;
        [SerializeField] private TextMeshProUGUI _sugarStarValueText;
        [SerializeField] private Button _sugarStarButton;

        [Header("Bottom Components")]
        [SerializeField] private Button _possibilityButton;
        [SerializeField] private Button singleButton; //1회 뽑기 버튼
        [SerializeField] private Button multipleButton; //10회 뽑기 버튼
        [SerializeField] private Button _possibilityBackButton;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _tweenAnimation;

        [Header("Panel")]
        [SerializeField] private RectTransform _topGamePanelRectTransform;
        [SerializeField] private GameObject _possibilityPanel;
        [SerializeField] private GameObject _gachaConfirmPanel;
        [SerializeField] private GameObject _gachaNotEnoughPanel;
        [SerializeField] private TweenAlpha_Image _backgroundPanel;
        [SerializeField] private GameObject _singlePanel;
        [SerializeField] private GameObject _tenPanel;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        public Action<int, int> OnGachaButtonClicked;
        [SerializeField] private CharacterLevelUpUIManager characterLevelUpUIManager;

        private RectTransform _rectTransform;
        private GameManager _gameManager;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
            _backgroundPanel.gameObject.SetActive(false);
            _possibilityPanel.SetActive(false);
            _gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            _shiningStarButton.onClick.AddListener(ShiningStarButtonClicked);
            _sugarStarButton.onClick.AddListener(SugarStarButtonClicked);
            _possibilityButton.onClick.AddListener(PossibilityButtonClicked);
            _possibilityBackButton.onClick.AddListener(PossibilityBackButtonClicked);
            singleButton.onClick.AddListener(SingleButtonClicked);
            multipleButton.onClick.AddListener(MultipleButtonClicked);
        }

        private void OnDisable()
        {
            if (_gameManager != null)
            {
                _gameManager.Reward.OnStarCandyChange -= CandyUpdate;
                _gameManager.Reward.OnShiningStarCandyChange -= ShiningCandyUpdate;
                _gameManager.Coin.OnStarCandyChanged -= CandyUpdate;
                _gameManager.Coin.OnShiningStarCandyChanged -= ShiningCandyUpdate;
            }

            _shiningStarButton.onClick.RemoveListener(ShiningStarButtonClicked);
            _sugarStarButton.onClick.RemoveListener(SugarStarButtonClicked);
            _possibilityButton.onClick.RemoveListener(PossibilityButtonClicked);
            _possibilityBackButton.onClick.RemoveListener(PossibilityBackButtonClicked);
            singleButton.onClick.RemoveListener(SingleButtonClicked);
            multipleButton.onClick.RemoveListener(MultipleButtonClicked);
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
                if (!RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, touchPos) &&
                    !RectTransformUtility.RectangleContainsScreenPoint(_topGamePanelRectTransform, touchPos))
                {
                    if (_possibilityPanel.activeSelf) return;
                    if (_gachaConfirmPanel.activeSelf) return;
                    if (_gachaNotEnoughPanel.activeSelf) return;
                    if (_singlePanel.activeSelf) return;
                    if (_tenPanel.activeSelf) return;

                    OnUICloseRequested?.Invoke(UIName.GachaMainUI);
                }
            }
        }

        public override void Open()
        {
            _tweenAnimation.moveAway();
            base.Open();
            _backgroundPanel.gameObject.SetActive(true);
            Initialize();
        }

        public override void Close()
        {
            characterLevelUpUIManager.InitCharacterList();
            _backgroundPanel.FadeOut();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            _tweenAnimation.moveBack();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void Initialize()
        {
            _gameManager.Reward.OnStarCandyChange += CandyUpdate;
            _gameManager.Reward.OnShiningStarCandyChange += ShiningCandyUpdate;
            _gameManager.Coin.OnStarCandyChanged += CandyUpdate;
            _gameManager.Coin.OnShiningStarCandyChanged += ShiningCandyUpdate;
            CandyUpdate(_gameManager.Coin.starCandy);
            ShiningCandyUpdate(_gameManager.Coin.shiningStarCandy);
        }

        private void CandyUpdate(int value)
        {
            _sugarStarValueText.text = value.ToString();
        }

        private void ShiningCandyUpdate(int value)
        {
            _shiningStarValueText.text = value.ToString();
        }

        private void ShiningStarButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.GachaNotEnoughUI);
            OnUICloseRequested?.Invoke(UIName.GachaMainUI);
            OnUIOpenRequested?.Invoke(UIName.PaidStoreUI);
        }

        private void SugarStarButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.GachaNotEnoughUI);
            OnUICloseRequested?.Invoke(UIName.GachaMainUI);
            OnUIOpenRequested?.Invoke(UIName.SugarStarExchangeUI);
        }

        private void PossibilityButtonClicked()
        {
            _possibilityPanel.SetActive(true);
        }

        private void PossibilityBackButtonClicked()
        {
            _possibilityPanel.SetActive(false);
        }

        //todo 별사탕에 따라서 GachaConfirmUI 또는 GachaNotEnoughUI를 띄워야 함
        private void SingleButtonClicked()
        {
            CandyUpdate(GameManager.Instance.Reward.StarCandy);
            if (GameManager.Instance.Reward.StarCandy >= 150) //별사탕이 150개 이상 가지고 있으면 1회 뽑기
            {
                OnUIOpenRequested?.Invoke(UIName.GachaConfirmUI);
                OnGachaButtonClicked?.Invoke(150, 1);
                GameManager.Instance.DailyQuest.CompleteQuest(QuestType.Gacha, 1);
            }
            else
            {
                OnUIOpenRequested?.Invoke(UIName.GachaNotEnoughUI);
            }
        }

        private void MultipleButtonClicked()
        {
            CandyUpdate(GameManager.Instance.Reward.StarCandy);
            if (GameManager.Instance.Reward.StarCandy >= 1500) //별사탕을 1500개 이상 가지고 있으면 10회 뽑기
            {
                OnUIOpenRequested?.Invoke(UIName.GachaConfirmUI);
                OnGachaButtonClicked?.Invoke(1500, 10);
                GameManager.Instance.DailyQuest.CompleteQuest(QuestType.Gacha, 1);
            }
            else
            {
                OnUIOpenRequested?.Invoke(UIName.GachaNotEnoughUI);
            }
        }
    }
}