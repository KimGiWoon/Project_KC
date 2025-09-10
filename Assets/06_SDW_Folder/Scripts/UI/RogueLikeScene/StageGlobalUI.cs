using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDW;
using UnityEngine.UI;

public class StageGlobalUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _cookButton;
    [SerializeField] private TweenAnimation[] _buttonTwwenAnimations;
    [SerializeField] private TweenAnimation _buttonContainerTweenAnimation;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName, bool> OnUICloseRequested;

    private Stack<UIName> _uiStack = new Stack<UIName>();
    private UIName _prevUIName;

    /// <summary>
    /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
    /// </summary>
    private void Awake()
    {
        _panelContainer.SetActive(false);
        _prevUIName = UIName.RouteSelectUI;
    }

    private void OnEnable()
    {
        _settingButton.onClick.AddListener(SettingButtonClicked);
        _shopButton.onClick.AddListener(ShopButtonClicked);
        _inventoryButton.onClick.AddListener(InventoryButtonClicked);
        _cookButton.onClick.AddListener(CookButtonClicked);
    }

    private void OnDisable()
    {
        _settingButton.onClick.RemoveListener(SettingButtonClicked);
        _shopButton.onClick.RemoveListener(ShopButtonClicked);
        _inventoryButton.onClick.RemoveListener(InventoryButtonClicked);
        _cookButton.onClick.RemoveListener(CookButtonClicked);
    }

    public override void Open()
    {
        base.Open();
        StartCoroutine(DelayedOpenAndClose(true));
    }

    public override void Close()
    {
        StartCoroutine(DelayedOpenAndClose(false));
        base.Close();
    }

    private IEnumerator DelayedOpenAndClose(bool isOpen)
    {
        yield return new WaitForSeconds(0.01f);

        if (isOpen)
        {
            OnUIOpenRequested?.Invoke(_prevUIName);
        }
        else
        {
            OnUICloseRequested?.Invoke(_prevUIName, false);
            _uiStack.Clear();
        }
    }

    private IEnumerator DelayedDeactive(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        target.SetActive(false);
    }

    public void PushPrevUI()
    {
        OnUIOpenRequested?.Invoke(_prevUIName);
        _uiStack.Push(_prevUIName);
    }

    public void SetPrevUI(UIName uiName)
    {
        while (_uiStack.Count > 0)
        {
            _uiStack.Pop();
        }
        _uiStack.Push(uiName);
        _prevUIName = uiName;
    }

    #region Button Methods

    private void SettingButtonClicked()
    {
        var tempUI = _uiStack.Peek();
        if (_uiStack.Count > 0)
        {
            tempUI = _uiStack.Pop();
            if (tempUI != UIName.PopupSettingUI)
                OnUICloseRequested?.Invoke(tempUI, false);
        }
        OnUIOpenRequested?.Invoke(UIName.PopupSettingUI);
    }

    private void ShopButtonClicked()
    {
        var tempUI = _uiStack.Peek();
        if (_uiStack.Count > 0)
        {
            tempUI = _uiStack.Pop();
            if (_prevUIName != UIName.ShoppingUI)
                OnUICloseRequested?.Invoke(tempUI, true);
        }

        _uiStack.Push(UIName.ShoppingUI);
        if (_prevUIName != UIName.ShoppingUI)
            OnUIOpenRequested?.Invoke(UIName.ShoppingUI);
    }

    private void InventoryButtonClicked()
    {
        var tempUI = _uiStack.Peek();
        if (_uiStack.Count > 0)
        {
            tempUI = _uiStack.Pop();
            if (tempUI != UIName.InventoryUI)
                OnUICloseRequested?.Invoke(tempUI, true);
        }

        _uiStack.Push(UIName.InventoryUI);
        if (tempUI != UIName.InventoryUI)
            OnUIOpenRequested?.Invoke(UIName.InventoryUI);
    }

    private void CookButtonClicked()
    {
        var tempUI = _uiStack.Peek();
        if (_uiStack.Count > 0)
        {
            tempUI = _uiStack.Pop();
            if (tempUI != UIName.CookingUI)
                OnUICloseRequested?.Invoke(tempUI, true);
        }

        _uiStack.Push(UIName.CookingUI);
        if (tempUI != UIName.CookingUI)
            OnUIOpenRequested?.Invoke(UIName.CookingUI);
    }

    #endregion

    #region DoTWeen Methods

    public void ButtonContainerMoveAway()
    {
        _buttonContainerTweenAnimation.moveAway();
        StartCoroutine(DelayedDeactive(_buttonContainerTweenAnimation.gameObject, _buttonContainerTweenAnimation.tweenTime));
    }

    public void ButtonToBottomMoveAway()
    {
        foreach (var buttonTween in _buttonTwwenAnimations)
        {
            buttonTween.moveAway();
        }
    }

    public void ButtonContainerMoveBack()
    {
        _buttonContainerTweenAnimation.gameObject.SetActive(true);
        _buttonContainerTweenAnimation.moveBack();
    }

    public void ButtonToMoveBack()
    {
        foreach (var buttonTween in _buttonTwwenAnimations)
        {
            buttonTween.moveBack();
        }
    }

    #endregion
}