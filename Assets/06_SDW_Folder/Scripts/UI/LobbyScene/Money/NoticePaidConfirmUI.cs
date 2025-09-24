using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class NoticePaidConfirmUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _payButton;
        private RectTransform _rectTransform;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private int _price;
        private int _sugarStar;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            _cancelButton.onClick.AddListener(CancelButtonClicked);
            _payButton.onClick.AddListener(PayButtonClicked);
        }

        private void OnDisable()
        {
            _cancelButton.onClick.RemoveListener(CancelButtonClicked);
            _payButton.onClick.RemoveListener(PayButtonClicked);
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
                    OnUICloseRequested?.Invoke(UIName.NoticePaidCompleteUI);
                }
            }
        }

        private void CancelButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.NoticePaidCompleteUI);
        }

        private void PayButtonClicked()
        {
            //todo 구글 인앱 결제 이후에 진행되어야 함
            Debug.Log("IAP 관련 구현 필요");
            //# 구매 성공시
            GameManager.Instance.Coin.AddShiningStarCandy(_sugarStar);
            OnUIOpenRequested?.Invoke(UIName.NoticePaidCompleteUI);
            //# 구매 실패 시
            // OnUIOpenRequested?.Invoke(UIName.NoticeNotPaidUI);
            OnUICloseRequested?.Invoke(UIName.NoticePaidConfirmUI);
        }

        public void SetItemInfo(int sugarStar, int price)
        {
            _price = price;
            _sugarStar = sugarStar;

            _currencyValueText.text = $"{_price}냥";
        }
    }
}