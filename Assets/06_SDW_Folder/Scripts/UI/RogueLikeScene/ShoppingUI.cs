using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SDW;

public class ShoppingUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private Image[] _ingredientImage;
    [SerializeField] private TextMeshProUGUI[] _ingredientNameText;
    [SerializeField] private TextMeshProUGUI[] _ingredientPriceText;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _buyButton;
    [SerializeField] private RectTransform _mainPanelRect;
    [SerializeField] private RectTransform[] _buttonsRect;
    private TweenAnimation _tweenAnimation;

    public Action<UIName, bool> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _tweenAnimation = GetComponent<TweenAnimation>();
    }

    private void OnEnable()
    {
        _resetButton.onClick.AddListener(ResetButtonClicked);
        _buyButton.onClick.AddListener(BuyButtonClicked);
    }

    private void OnDisable()
    {
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        _buyButton.onClick.RemoveListener(BuyButtonClicked);
    }

    public override void Open()
    {
        base.Open();
        _tweenAnimation.moveAway();
    }

    public override void Close()
    {
        _tweenAnimation.moveBack();
        StartCoroutine(DelayedClose());
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(_tweenAnimation.tweenTime);
        base.Close();
    }

    private void Update()
    {
        //todo 활성화되고 일정 시간은 체크 안하도록 해야할 듯
        if (!_panelContainer.activeSelf) return;

        //todo 하단 버튼 영역일 경우에는 버튼이 선택되어야 함
        //# 안드로이드 터치 감지
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var touchPos = Input.GetTouch(0).position;

            //# 패널 안에 터치가 있는지 확인
            if (RectTransformUtility.RectangleContainsScreenPoint(_mainPanelRect, touchPos)) return;

            foreach (var buttonRect in _buttonsRect)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, touchPos))
                {
                    StartCoroutine(DelayedCloseCall(true));
                    return;
                }
            }

            StartCoroutine(DelayedCloseCall());
        }
    }

    private IEnumerator DelayedCloseCall(bool uiOnly = false)
    {
        yield return new WaitForSeconds(0.2f);

        OnUICloseRequested?.Invoke(UIName.ShoppingUI, uiOnly);
    }

    //todo 상점 관련 설정(재료, 가격, Reset, Buy Button 연동 필요)

    private void ResetButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void BuyButtonClicked()
    {
        throw new NotImplementedException();
    }
}