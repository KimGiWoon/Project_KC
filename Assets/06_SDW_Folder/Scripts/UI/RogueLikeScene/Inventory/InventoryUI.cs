using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JJY;

namespace SDW
{
    public class InventoryUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private GameObject _contents;
        [SerializeField] private Button _foodButton;
        [SerializeField] private Button _relicButton;
        [SerializeField] private RectTransform _inventoryPanelRect;
        [SerializeField] private RectTransform[] _buttonsRect;
        [SerializeField] private GameObject _backgroundObject;

        [Header("Description Components")]
        [SerializeField] private GameObject _descriptionPanel;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemDescriptionText;
        [SerializeField] private TextMeshProUGUI _itemEffectText;
        [SerializeField] private Button _confirmButton;

        private TweenAnimation _tweenAnimation;
        private WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
        private bool _canInteract;
        private bool _isProgress;
        private InventoryUIManager _inventoryUIManager;

        public Action<UIName, bool> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
            _descriptionPanel.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            _inventoryUIManager = InventoryUIManager.Instance;
        }

        private void OnEnable()
        {
            _foodButton.onClick.AddListener(FoodButtonClicked);
            _relicButton.onClick.AddListener(RelicButtonClicked);
            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        }

        private void OnDisable()
        {
            _foodButton.onClick.RemoveListener(FoodButtonClicked);
            _relicButton.onClick.RemoveListener(RelicButtonClicked);
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
        }

        public override void Open()
        {
            _isProgress = true;
            _backgroundObject.SetActive(true);
            base.Open();
            _tweenAnimation.moveAway();
            StartCoroutine(DelayedOpen());
            _inventoryUIManager.InitFoodInventory();
            _canInteract = true;
        }

        private IEnumerator DelayedOpen()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            _isProgress = false;
        }

        public override void Close()
        {
            _isProgress = true;
            _tweenAnimation.moveBack();
            _descriptionPanel.SetActive(false);
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
            _inventoryUIManager.SeenNewItems();
            base.Close();
            _isProgress = false;
        }

        private void Update()
        {
            if (!_panelContainer.activeSelf || !_canInteract || _isProgress) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (RectTransformUtility.RectangleContainsScreenPoint(
                        _inventoryPanelRect,
                        touchPos,
                        Camera.main)) return;

                //# 버튼을 클릭했는지 확인
                foreach (var buttonRect in _buttonsRect)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, touchPos, Camera.main))
                    {
                        if (buttonRect.CompareTag("InventoryButton")) return;
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

            _descriptionPanel.SetActive(false);
            OnUICloseRequested?.Invoke(UIName.InventoryUI, false);
        }

        public void PopupDescription(PopupDescription description)
        {
            _itemIcon.sprite = description.Sprite;
            _itemNameText.text = description.Name;
            _itemDescriptionText.text = description.Description;
            _itemEffectText.text = description.Effect;
            _descriptionPanel.SetActive(true);
        }

        public void SetContent(List<Button> itemList)
        {
            foreach (var item in itemList)
            {
                item.transform.SetParent(_contents.transform, false);
                item.gameObject.SetActive(true);
            }
        }

        private void FoodButtonClicked()
        {
            _descriptionPanel.SetActive(false);
            _inventoryUIManager.InitFoodInventory();
        }
        private void RelicButtonClicked()
        {
            _descriptionPanel.SetActive(false);
            _inventoryUIManager.InitRelicInventory();
        }

        private void ConfirmButtonClicked()
        {
            _descriptionPanel.SetActive(false);
        }
    }
}