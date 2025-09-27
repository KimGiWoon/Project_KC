using System;
using System.Collections;
using System.Collections.Generic;
using JJY;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class StageGlobalUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _inventoryButton;
        [SerializeField] private Button _cookButton;
        [SerializeField] private TextMeshProUGUI _stageText;
        [SerializeField] private TextMeshProUGUI _yeopjeonText;
        [SerializeField] public TweenAnimation[] _buttonTwwenAnimations;
        [SerializeField] public TweenAnimation _buttonContainerTweenAnimation;
        [SerializeField] private GameObject _buttuonContainer;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName, bool> OnUICloseRequested;

        private Stack<UIName> _uiStack = new Stack<UIName>();
        private UIName _prevUIName;
        private GameManager _gameManager;
        private CoinManager _coin;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _buttuonContainer.SetActive(false);
            _prevUIName = UIName.RouteSelectUI;
        }

        protected override void Start()
        {
            base.Start();
            _settingButton.onClick.AddListener(SettingButtonClicked);
            _shopButton.onClick.AddListener(ShopButtonClicked);
            _inventoryButton.onClick.AddListener(InventoryButtonClicked);
            _cookButton.onClick.AddListener(CookButtonClicked);
            RoguelikeManager.Instance.OnBattleStart += () => { OnUICloseRequested?.Invoke(UIName.StageGlobalUI, true); };

            _gameManager = GameManager.Instance;
            _coin = _gameManager.Coin;
        }

        private void OnDisable()
        {
            _settingButton.onClick.RemoveListener(SettingButtonClicked);
            _shopButton.onClick.RemoveListener(ShopButtonClicked);
            _inventoryButton.onClick.RemoveListener(InventoryButtonClicked);
            _cookButton.onClick.RemoveListener(CookButtonClicked);
            RoguelikeManager.Instance.OnBattleStart -= () => { OnUICloseRequested?.Invoke(UIName.StageGlobalUI, true); };
        }

        public override void Open()
        {
            base.Open();
            _buttuonContainer.SetActive(true);
            StartCoroutine(DelayedOpenAndClose(true));

            SetChapterStageText(_gameManager.Chapter, _gameManager.Stage);
            SetYeopjeonText(_coin.yeopjeon);

            _coin.OnYeopjeonChanged += SetYeopjeonText;
        }

        public override void Close()
        {
            StartCoroutine(DelayedOpenAndClose(false));
            _buttuonContainer.SetActive(false);
            base.Close();

            _coin.OnYeopjeonChanged -= SetYeopjeonText;
        }

        private IEnumerator DelayedOpenAndClose(bool isOpen)
        {
            yield return new WaitForSeconds(0.01f);

            if (isOpen)
            {
                OnUIOpenRequested?.Invoke(_prevUIName);
            }
            else
            {
                OnUICloseRequested?.Invoke(_prevUIName, false);
                _uiStack.Clear();
            }
        }

        private IEnumerator DelayedDeactive(GameObject target, float delay)
        {
            yield return new WaitForSeconds(delay);

            target.SetActive(false);
        }

        public void PushPrevUI()
        {
            OnUIOpenRequested?.Invoke(_prevUIName);
            _uiStack.Push(_prevUIName);
        }

        public void SetPrevUI(UIName uiName)
        {
            while (_uiStack.Count > 0)
            {
                _uiStack.Pop();
            }
            _uiStack.Push(uiName);
            _prevUIName = uiName;
        }

        public void SetChapterStageText(int chapter, int stage) => _stageText.text = $"{chapter}-{stage}";

        public void SetYeopjeonText(int yeopjeon) => _yeopjeonText.text = yeopjeon.ToString();

        #region Button Methods

        private void SettingButtonClicked()
        {
            // if (_uiStack.Count > 0)
            // {
            //     var tempUI = _uiStack.Peek();
            //     tempUI = _uiStack.Pop();
            //     if (tempUI != UIName.RoguelikeSettingUI)
            //         OnUICloseRequested?.Invoke(tempUI, false);
            // }
            OnUIOpenRequested?.Invoke(UIName.RoguelikeSettingUI);
        }

        private void ShopButtonClicked()
        {
            var tempUI = _uiStack.Peek();
            if (_uiStack.Count > 0)
            {
                tempUI = _uiStack.Pop();
                if (_prevUIName != UIName.ShoppingUI)
                    OnUICloseRequested?.Invoke(tempUI, true);
            }

            _uiStack.Push(UIName.ShoppingUI);
            if (_prevUIName != UIName.ShoppingUI)
                OnUIOpenRequested?.Invoke(UIName.ShoppingUI);
        }

        private void InventoryButtonClicked()
        {
            var tempUI = _uiStack.Peek();
            if (_uiStack.Count > 0)
            {
                tempUI = _uiStack.Pop();
                if (tempUI != UIName.InventoryUI)
                    OnUICloseRequested?.Invoke(tempUI, true);
            }

            _uiStack.Push(UIName.InventoryUI);
            if (tempUI != UIName.InventoryUI)
                OnUIOpenRequested?.Invoke(UIName.InventoryUI);
        }

        private void CookButtonClicked()
        {
            var tempUI = _uiStack.Peek();
            if (_uiStack.Count > 0)
            {
                tempUI = _uiStack.Pop();
                if (tempUI != UIName.CookingUI)
                    OnUICloseRequested?.Invoke(tempUI, true);
            }

            _uiStack.Push(UIName.CookingUI);
            if (tempUI != UIName.CookingUI)
                OnUIOpenRequested?.Invoke(UIName.CookingUI);
        }

        #endregion

        #region DoTWeen Methods

        public void ButtonContainerMoveAway()
        {
            _buttonContainerTweenAnimation.moveAway();
            StartCoroutine(DelayedDeactive(_buttonContainerTweenAnimation.gameObject, _buttonContainerTweenAnimation.tweenTime));
        }

        public void ButtonToBottomMoveAway()
        {
            foreach (var buttonTween in _buttonTwwenAnimations)
            {
                buttonTween.moveAway();
            }
        }

        public void ButtonContainerMoveBack()
        {
            _buttonContainerTweenAnimation.gameObject.SetActive(true);
            _buttonContainerTweenAnimation.moveBack();
        }

        public void ButtonToMoveBack()
        {
            foreach (var buttonTween in _buttonTwwenAnimations)
            {
                buttonTween.moveBack();
            }
        }

        #endregion
    }
}