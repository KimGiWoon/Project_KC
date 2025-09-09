using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDW;

public class InventoryUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private GameObject _contents;
    [SerializeField] private Button _foodButton;
    [SerializeField] private Button _relicButton;
    [SerializeField] private RectTransform _inventoryPanelRect;
    [SerializeField] private RectTransform[] _buttonsRect;
    private TweenAnimation _tweenAnimation;
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
    private bool _canInteract;

    public Action<UIName, bool> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _tweenAnimation = GetComponent<TweenAnimation>();
    }

    private void OnEnable()
    {
        _foodButton.onClick.AddListener(FoodButtonClicked);
        _relicButton.onClick.AddListener(RelicButtonClicked);
    }

    private void OnDisable()
    {
        _foodButton.onClick.RemoveListener(FoodButtonClicked);
        _relicButton.onClick.RemoveListener(RelicButtonClicked);
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
            if (RectTransformUtility.RectangleContainsScreenPoint(_inventoryPanelRect, touchPos)) return;

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

    private IEnumerator DelayedCloseCall()
    {
        _canInteract = false;
        StartCoroutine(InteractDelay());
        yield return new WaitForSeconds(0.1f);

        OnUICloseRequested?.Invoke(UIName.InventoryUI, false);
    }

    //todo 인벤토리 관련 설정

    private void FoodButtonClicked()
    {
        throw new NotImplementedException();
    }
    private void RelicButtonClicked()
    {
        throw new NotImplementedException();
    }
}