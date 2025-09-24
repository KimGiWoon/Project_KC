using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class GachaNotEnoughUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _paidStoreButton;
        [SerializeField] private Button _exchangeButton;
        private RectTransform _rectTransform;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            _paidStoreButton.onClick.AddListener(PaidStoreButtonClicked);
            _exchangeButton.onClick.AddListener(ExchangeButtonClicked);
        }

        private void OnDisable()
        {
            _paidStoreButton.onClick.RemoveListener(PaidStoreButtonClicked);
            _exchangeButton.onClick.RemoveListener(ExchangeButtonClicked);
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
                    OnUICloseRequested?.Invoke(UIName.GachaNotEnoughUI);
                }
            }
        }

        private void PaidStoreButtonClicked()
        {
            //todo PaidStore를 열어야 함
            Debug.Log("Paid Store");
        }

        private void ExchangeButtonClicked()
        {
            //todo Exchange를 열어야 함
            Debug.Log("Exchange");
        }
    }
}