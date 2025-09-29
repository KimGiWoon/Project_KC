using System;
using System.Collections;
using JJY;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class SugarStarExchangeUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _shiningStarText;
        [SerializeField] private TextMeshProUGUI _sugarStarText;
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _selectedValueText;
        [SerializeField] private Button _exchangeButton;

        [Header("Panel")]
        [SerializeField] private TweenAlpha_Image _backgroundPanel;
        private RectTransform _rectTransform;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _tweenAnimation;

        public Action<UIName> OnUICloseRequested;
        private int _selectedValue;
        private CoinManager _coin;
        private bool _isProgress;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
            _coin = GameManager.Instance.Coin;
        }

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(SliderValueChanged);
            _exchangeButton.onClick.AddListener(ExchangeButtonClicked);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(SliderValueChanged);
            _exchangeButton.onClick.RemoveListener(ExchangeButtonClicked);
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
                    OnUICloseRequested?.Invoke(UIName.SugarStarExchangeUI);
                }
            }
        }

        public override void Open()
        {
            _isProgress = true;
            Initialize();
            _backgroundPanel.gameObject.SetActive(true);
            _tweenAnimation.moveAway();
            StartCoroutine(DelayedOpen());
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

        private void SliderValueChanged(float value)
        {
            _slider.maxValue = _coin.shiningStarCandy;
            _selectedValue = (int)value;
            _selectedValueText.text = $"{_selectedValue}/{_coin.shiningStarCandy}";
        }

        private void ExchangeButtonClicked()
        {
            _coin.SubtractShiningStarCandy(_selectedValue);
            _coin.AddStarCandy(_selectedValue);

            Initialize();
        }

        private void Initialize()
        {
            if (_coin.shiningStarCandy == 0)
            {
                _slider.value = 0;
                _slider.minValue = 0;
                _selectedValue = 0;
            }
            else
            {
                _slider.value = 1;
                _slider.minValue = 1;
                _selectedValue = 1;
            }
            _slider.maxValue = _coin.shiningStarCandy;

            _shiningStarText.text = _coin.shiningStarCandy.ToString();
            _sugarStarText.text = _coin.starCandy.ToString();
            _selectedValueText.text = $"{_slider.minValue:F0}/{_coin.shiningStarCandy}";
        }
    }
}