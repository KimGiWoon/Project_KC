using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDW;
using TMPro;
using UnityEngine.UI;

public class CookingUI : BaseUI
{
    [Header("Need Ingredients")]
    [SerializeField] private Image[] _needIngredientImages;
    [SerializeField] private Image _foodImage;

    [Header("Buttons")]
    [SerializeField] private Button _foodInfoButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _cookButton;

    [Header("ETC")]
    [SerializeField] private GameObject _contents;
    [SerializeField] private RectTransform _cookPanelRect;
    [SerializeField] private RectTransform[] _buttonsRect;
    private TweenAnimation _tweenAnimation;
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
    private bool _canInteract;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName, bool> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _tweenAnimation = GetComponent<TweenAnimation>();
    }

    private void OnEnable()
    {
        _foodInfoButton.onClick.AddListener(FoodInfoButtonClicked);
        _resetButton.onClick.AddListener(ResetButtonClicked);
        _cookButton.onClick.AddListener(CookButtonClicked);
    }

    private void OnDisable()
    {
        _foodInfoButton.onClick.RemoveListener(FoodInfoButtonClicked);
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        _cookButton.onClick.RemoveListener(CookButtonClicked);
    }

    public override void Open()
    {
        StartCoroutine(InteractDelay());
        base.Open();
        _tweenAnimation.moveAway();
    }

    public override void Close()
    {
        _tweenAnimation.moveBack();
        StartCoroutine(DelayedClose());
    }

    private IEnumerator InteractDelay()
    {
        yield return _waitForSeconds;
        _canInteract = true;
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(_tweenAnimation.tweenTime);
        base.Close();
    }

    private void Update()
    {
        if (!_panelContainer.activeSelf || !_canInteract) return;

        //# 안드로이드 터치 감지
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var touchPos = Input.GetTouch(0).position;

            //# 패널 안에 터치가 있는지 확인
            if (RectTransformUtility.RectangleContainsScreenPoint(_cookPanelRect, touchPos)) return;

            //# 버튼을 클릭했는지 확인
            foreach (var buttonRect in _buttonsRect)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, touchPos))
                {
                    if (buttonRect.CompareTag("InventoryButton")) return;
                    StartCoroutine(InteractDelay());
                    return;
                }
            }

            StartCoroutine(DelayedCloseCall());
        }
    }

    private IEnumerator DelayedCloseCall(bool uiOnly = false)
    {
        _canInteract = false;
        StartCoroutine(InteractDelay());
        yield return new WaitForSeconds(0.1f);

        OnUICloseRequested?.Invoke(UIName.CookingUI, uiOnly);
    }

    //todo 요리 관련 설정

    private void FoodInfoButtonClicked()
    {
        //todo food 관련 정보도 같이 보내야 함
        OnUIOpenRequested?.Invoke(UIName.FoodDescriptionUI);
    }

    private void ResetButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void CookButtonClicked()
    {
        throw new NotImplementedException();
    }
}