using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KSH;

namespace SDW
{
    public class PermanentGrowthUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _pointText;
        [SerializeField] private GameObject _contents;
        [SerializeField] private GameObject _descriptionRectTrasnfrom;
        [SerializeField] private GameObject _backgroundPanel;
        private RectTransform _rectTransform;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _tweenAnimation;

        private List<GrowthNodeUI> _growthNodes = new List<GrowthNodeUI>();
        private List<Button> _growthNodeButtons = new List<Button>();

        public Action<UIName, GrowthNodeUI> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private bool _isProgress;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _backgroundPanel.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            GameManager.Instance.Coin.OnPointChanged += SetTotalPoint;
        }

        private void OnDisable()
        {
            GameManager.Instance.Coin.OnPointChanged -= SetTotalPoint;

            for (int i = 0; i < _growthNodes.Count; i++)
            {
                _growthNodeButtons[i].onClick.RemoveAllListeners();
            }
            _growthNodeButtons.Clear();
            _growthNodes.Clear();
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
                    if (_descriptionRectTrasnfrom.activeSelf) return;

                    OnUIOpenRequested?.Invoke(UIName.StageSelectUI, null);
                    OnUICloseRequested?.Invoke(UIName.PermanentGrowthUI);
                }
            }
        }

        public override void Open()
        {
            _isProgress = true;
            _backgroundPanel.SetActive(true);
            _tweenAnimation.moveAway();
            StartCoroutine(DelayedOpen());
            SetTotalPoint();
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
            _backgroundPanel.SetActive(false);
            _tweenAnimation.moveBack();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
            _isProgress = false;
        }

        private void SetTotalPoint()
        {
            _pointText.text = "x" + GameManager.Instance.Coin.point;
        }

        public void AddNode(GrowthNodeUI growthNode)
        {
            _growthNodes.Add(growthNode);
            var button = growthNode.GetComponentInChildren<Button>();
            _growthNodeButtons.Add(button);

            button.onClick.AddListener(() =>
                OpenNodeDescription(growthNode)
            );
        }

        private void OpenNodeDescription(GrowthNodeUI growthNode)
        {
            OnUIOpenRequested?.Invoke(UIName.NodeDescriptionUI, growthNode);
        }
    }
}