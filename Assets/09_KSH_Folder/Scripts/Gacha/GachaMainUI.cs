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
        [Header("CharacterGacha")]
        [SerializeField] private CharacterGacha gacha;

        [Header("Top Components")]
        [SerializeField] private TextMeshProUGUI _shiningStartValueText;
        [SerializeField] private Button _shiningStartButton;
        [SerializeField] private TextMeshProUGUI _sugarStartValueText;
        [SerializeField] private Button _sugarStartButton;

        [Header("Bottom Components")]
        [SerializeField] private Button _possibilityButton;
        [SerializeField] private Button singleButton; //1회 뽑기 버튼
        [SerializeField] private Button multipleButton; //10회 뽑기 버튼
        [SerializeField] private GameObject _possibilityGameObject;
        [SerializeField] private Button _possibilityBackButton;

        [Header("Animation")]
        [SerializeField] private GameObject _backgroundPanelObject;
        private TweenAlpha_Image _backgroundPanel;
        [SerializeField] private TweenAnimation _tweenAnimation;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        [SerializeField] private CharacterLevelUpUIManager characterLevelUpUIManager;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
            _backgroundPanel = _backgroundPanelObject.GetComponent<TweenAlpha_Image>();
            _backgroundPanelObject.SetActive(false);
            _possibilityGameObject.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            gacha = GameManager.Instance.Gacha;
        }

        private void OnEnable()
        {
            _possibilityButton.onClick.AddListener(PossibilityButtonClicked);
            _possibilityBackButton.onClick.AddListener(PossibilityBackButtonClicked);
            singleButton.onClick.AddListener(SingleButtonClicked);
            multipleButton.onClick.AddListener(MultipleButtonClicked);
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Reward.OnStarCandyChange -= CandyUpdate;
                GameManager.Instance.Reward.OnShiningStarCandyChange -= ShiningCandyUpdate;
            }

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
                if (!RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, touchPos))
                {
                    if (_possibilityGameObject.activeSelf) return;

                    OnUICloseRequested?.Invoke(UIName.GachaMainUI);
                }
            }
        }

        public override void Open()
        {
            _tweenAnimation.moveAway();
            base.Open();
            _backgroundPanelObject.SetActive(true);
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
            GameManager.Instance.Reward.OnStarCandyChange += CandyUpdate;
            GameManager.Instance.Reward.OnShiningStarCandyChange += ShiningCandyUpdate;
            CandyUpdate(GameManager.Instance.Coin.starCandy);
        }

        private void CandyUpdate(int value)
        {
            _sugarStartValueText.text = value.ToString();
        }

        private void ShiningCandyUpdate(int value)
        {
            _shiningStartValueText.text = value.ToString();
        }

        private void PossibilityButtonClicked()
        {
            _possibilityGameObject.SetActive(true);
        }

        private void PossibilityBackButtonClicked()
        {
            _possibilityGameObject.SetActive(false);
        }

        //todo 별사탕에 따라서 GachaConfirmUI 또는 GachaNotEnoughUI를 띄워야 함
        private void SingleButtonClicked()
        {
            CandyUpdate(GameManager.Instance.Reward.StarCandy);
            if (GameManager.Instance.Reward.StarCandy >= 150) //별사탕이 150개 이상 가지고 있으면 1회 뽑기
            {
                GameManager.Instance.Reward.AddStarCandy(-150);
                gacha.SetGachaType(true);
                OnUIOpenRequested?.Invoke(UIName.GachaResultUI);
                OnUICloseRequested?.Invoke(UIName.GachaMainUI);
            }
            else
            {
                Debug.Log("별사탕이 부족합니다.");
            }
        }

        private void MultipleButtonClicked()
        {
            CandyUpdate(GameManager.Instance.Reward.StarCandy);
            if (GameManager.Instance.Reward.StarCandy >= 1500) //별사탕을 1500개 이상 가지고 있으면 10회 뽑기
            {
                GameManager.Instance.Reward.AddStarCandy(-1500);
                gacha.SetGachaType(false);
                OnUIOpenRequested?.Invoke(UIName.GachaResultUI);
                OnUICloseRequested?.Invoke(UIName.GachaMainUI);
            }
            else
            {
                Debug.Log("별사탕이 부족합니다.");
            }
        }
    }
}