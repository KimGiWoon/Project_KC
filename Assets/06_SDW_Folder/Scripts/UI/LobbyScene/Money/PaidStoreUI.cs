using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class PaidStoreUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private List<Button> _itemButtonList;
        [SerializeField] private List<int> _getStarList;
        [SerializeField] private List<int> _paidPriceList;

        [Header("Panel")]
        [SerializeField] private TweenAlpha_Image _backgroundPanel;
        [SerializeField] private GameObject _noticePaidConfirmPanel;
        [SerializeField] private GameObject _noticePaidCompletePanel;
        [SerializeField] private GameObject _noticeNotPaid;

        private RectTransform _rectTransform;
        private bool _isProgress;

        [Header("Animations")]
        [SerializeField] private TweenAnimation _tweenAnimation;

        public Action<int, int> OnItemSelected;
        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
            _backgroundPanel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            for (int i = 0; i < _itemButtonList.Count; i++)
            {
                var buttonId = _itemButtonList[i].GetComponent<ButtonId>();

                _itemButtonList[i].onClick.AddListener(() => { ItemButtonClicked(buttonId.Id); });
            }
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
                    if (_noticePaidConfirmPanel.activeSelf) return;
                    if (_noticePaidCompletePanel.activeSelf) return;
                    if (_noticeNotPaid.activeSelf) return;

                    OnUICloseRequested?.Invoke(UIName.PaidStoreUI);
                }
            }
        }

        public override void Open()
        {
            _tweenAnimation.moveAway();
            _backgroundPanel.gameObject.SetActive(true);
            base.Open();
        }

        public override void Close()
        {
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            _tweenAnimation.moveBack();
            _backgroundPanel.FadeOut();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void OnDisable()
        {
            for (int i = 0; i < _itemButtonList.Count; i++)
            {
                var buttonId = _itemButtonList[i].GetComponent<ButtonId>();

                _itemButtonList[i].onClick.RemoveListener(() => { ItemButtonClicked(buttonId.Id); });
            }
        }

        private void ItemButtonClicked(int buttonIdId)
        {
            OnItemSelected?.Invoke(_getStarList[buttonIdId], _paidPriceList[buttonIdId]);
            OnUIOpenRequested?.Invoke(UIName.NoticePaidConfirmUI);
        }
    }
}