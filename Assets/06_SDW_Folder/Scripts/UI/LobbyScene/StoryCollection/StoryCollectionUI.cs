using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class StoryCollectionUI : BaseUI
    {
        [Header("Panel")]
        [SerializeField] private TweenAlpha_Image _backgroundPanel;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _tweenAnimation;
        private RectTransform _rectTransform;

        public Action<UIName> OnUICloseRequested;
        private bool _isProgress;

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
                    OnUICloseRequested?.Invoke(UIName.StoryCollectionUI);
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
    }
}