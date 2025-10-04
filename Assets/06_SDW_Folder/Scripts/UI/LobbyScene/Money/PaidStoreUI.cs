using System;
using System.Collections;
using UnityEngine;

namespace SDW
{
    public class PaidStoreUI : BaseUI
    {
        [SerializeField] private IAPController _iapController;

        [Header("Panel")]
        [SerializeField] private TweenAlpha_Image _backgroundPanel;
        [SerializeField] private GameObject _noticePaidConfirmPanel;
        [SerializeField] private GameObject _noticePaidCompletePanel;
        [SerializeField] private GameObject _noticeNotPaid;

        private RectTransform _rectTransform;
        private bool _isProgress;

        [Header("Animations")]
        [SerializeField] private TweenAnimation _tweenAnimation;

        public Action<int, double, string> OnItemSelected;
        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
            _backgroundPanel.gameObject.SetActive(false);
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
                    if (_noticePaidConfirmPanel.activeSelf) return;
                    if (_noticePaidCompletePanel.activeSelf) return;
                    if (_noticeNotPaid.activeSelf) return;

                    OnUICloseRequested?.Invoke(UIName.PaidStoreUI);
                }
            }
        }

        public override void Open()
        {
            _isProgress = true;
            _tweenAnimation.moveAway();
            StartCoroutine(DelayedOpen());
            _backgroundPanel.gameObject.SetActive(true);
            base.Open();
        }

        private IEnumerator DelayedOpen()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            _isProgress = false;
        }

        public override void Close()
        {
            _isProgress = true;
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            _tweenAnimation.moveBack();
            _backgroundPanel.FadeOut();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
            _isProgress = false;
        }

        public void ItemSelected(int getStart, double paidPrice, string productId)
        {
            OnItemSelected?.Invoke(getStart, paidPrice, productId);
            OnUIOpenRequested?.Invoke(UIName.NoticePaidConfirmUI);
        }
    }
}