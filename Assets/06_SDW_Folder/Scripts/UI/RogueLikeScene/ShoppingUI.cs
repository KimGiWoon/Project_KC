using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JJY;

namespace SDW
{
    public class ShoppingUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private List<Button> _ingredientButton;
        [SerializeField] private List<Image> _ingredientImage;
        [SerializeField] private List<TextMeshProUGUI> _ingredientNameText;
        [SerializeField] private List<TextMeshProUGUI> _ingredientPriceText;

        [Header("Button Components")]
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _buyButton;
        [SerializeField] private RectTransform _mainPanelRect;
        [SerializeField] private List<RectTransform> _buttonsRect;

        [Header("ETC")]
        [SerializeField] private GameObject _failedPopup;
        [SerializeField] private TextMeshProUGUI _failedText;
        [SerializeField] private GameObject _backgroundObject;

        private TweenAnimation _tweenAnimation;
        private WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
        private bool _canInteract;
        private IngredientStoreUIManager _ingredientStoreManager;

        public Action<UIName, bool> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
            _ingredientStoreManager = IngredientStoreUIManager.Instance;
        }

        private void OnEnable()
        {
            _resetButton.onClick.AddListener(ResetButtonClicked);
            _buyButton.onClick.AddListener(BuyButtonClicked);

            int index = 0;
            foreach (var ingredientButton in _ingredientButton)
            {
                var buttonId = ingredientButton.GetComponent<ButtonId>();
                buttonId.Id = index++;
                ingredientButton.onClick.AddListener(() => OnIngredientButtonClicked(buttonId.Id));
            }
        }

        private void OnDisable()
        {
            _resetButton.onClick.RemoveListener(ResetButtonClicked);
            _buyButton.onClick.RemoveListener(BuyButtonClicked);


            foreach (var ingredientButton in _ingredientButton)
            {
                int id = ingredientButton.GetComponent<ButtonId>().Id;
                ingredientButton.onClick.RemoveListener(() => OnIngredientButtonClicked(id));
            }
        }

        public override void Open()
        {
            _backgroundObject.SetActive(true);
            StartCoroutine(InteractDelay());
            base.Open();
            _tweenAnimation.moveAway();
        }

        public override void Close()
        {
            _tweenAnimation.moveBack();
            StartCoroutine(DelayedClose());
            _backgroundObject.SetActive(false);
        }

        private IEnumerator InteractDelay()
        {
            yield return _waitForSeconds;
            _canInteract = true;
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void Update()
        {
            if (!_panelContainer.activeSelf || !_canInteract) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (RectTransformUtility.RectangleContainsScreenPoint(
                        _mainPanelRect,
                        touchPos,
                        Camera.main)) return;

                //# 버튼을 클릭했는지 확인
                foreach (var buttonRect in _buttonsRect)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, touchPos, Camera.main))
                    {
                        if (buttonRect.CompareTag("ShopButton")) return;
                        StartCoroutine(InteractDelay());
                        return;
                    }
                }

                StartCoroutine(DelayedCloseCall());
            }
        }

        private IEnumerator DelayedCloseCall()
        {
            _canInteract = false;
            StartCoroutine(InteractDelay());
            yield return new WaitForSeconds(0.1f);

            OnUICloseRequested?.Invoke(UIName.ShoppingUI, false);
        }

        //todo 상점 관련 설정(재료, 가격, Reset, Buy Button 연동 필요)

        #region Button Methods

        private void ResetButtonClicked()
        {
            _ingredientStoreManager.RefreshStore();

            ResetIngredientColor();
        }

        public void ResetIngredientColor()
        {
            for (int i = 0; i < _ingredientImage.Count; i++)
            {
                _ingredientImage[i].color = Color.white;
            }
        }

        private void BuyButtonClicked()
        {
            _ingredientStoreManager.TryBuyIngredient();
        }

        private void OnIngredientButtonClicked(int index)
        {
            _ingredientStoreManager.OnSlotClicked(index);

            for (int i = 0; i < _ingredientImage.Count; i++)
            {
                if (!_ingredientButton[i].interactable) continue;
                _ingredientImage[i].color = index == i ? Color.gray : Color.white;
            }
        }

        #endregion

        public void UpdateSlotUI(List<StoreSlotUIData> slotDatas)
        {
            for (int i = 0; i < slotDatas.Count; i++)
            {
                _ingredientImage[i].gameObject.SetActive(true);
                _ingredientImage[i].sprite = slotDatas[i].iconSprite;
                _ingredientNameText[i].text = slotDatas[i].ingNameText;
                _ingredientPriceText[i].text = slotDatas[i].priceText;

                _ingredientImage[i].enabled = slotDatas[i].isEnabled;
                _ingredientButton[i].interactable = slotDatas[i].canInteractable;
                if (!slotDatas[i].canInteractable)
                    _ingredientImage[i].color = new Color(1, 1, 1, 0.5f);
            }
        }

        public void ActiveFailedPanel(string text)
        {
            _failedText.text = text;
            _failedPopup.SetActive(true);
        }
    }
}