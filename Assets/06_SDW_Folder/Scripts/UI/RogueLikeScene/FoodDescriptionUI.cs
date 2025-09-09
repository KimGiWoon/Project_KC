using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SDW;

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
            if (!RectTransformUtility.RectangleContainsScreenPoint(_mainPanelRect, touchPos))
                OnUICloseRequrested?.Invoke(UIName.FoodDescriptionUI);
        }
    }

    //todo 추후 파라미터 등은 추가 필요
    public void SetFoodDescription(FoodDescription description)
    {
        _foodImage.sprite = description.Sprite;
        _foodNameText.text = description.FoodName;
        _foodEffectText.text = description.FoodEffect;
        _foodDescriptionText.text = description.Description;
    }
}