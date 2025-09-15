using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SDW
{
    public class FoodDescriptionUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Image _foodImage;
        [SerializeField] private TextMeshProUGUI _foodNameText;
        [SerializeField] private TextMeshProUGUI _foodEffectText;
        [SerializeField] private TextMeshProUGUI _foodDescriptionText;
        [SerializeField] private RectTransform _mainPanelRect;

        public Action<UIName> OnUICloseRequrested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        private void Update()
        {
            if (!_panelContainer.activeSelf) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (!RectTransformUtility.RectangleContainsScreenPoint(_mainPanelRect, touchPos, Camera.main))
                    OnUICloseRequrested?.Invoke(UIName.FoodDescriptionUI);
            }
        }

        public void SetFoodDescription(PopupDescription description)
        {
            _foodImage.sprite = description.Sprite;
            _foodNameText.text = description.Name;
            _foodEffectText.text = description.Effect;
            _foodDescriptionText.text = description.Description;
        }
    }
}