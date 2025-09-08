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
    [SerializeField] private TweenAnimation _buttonTweenAnimation;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    /// <summary>
    /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
    /// </summary>
    private void Awake()
    {
        _panelContainer.SetActive(false);
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
        // OnUIOpenRequested?.Invoke(UIName.RouteSelectUI);
    }

    public override void Close()
    {
        StartCoroutine(DelayedOpenAndClose(false));
        // OnUICloseRequested?.Invoke(UIName.RouteSelectUI);
        base.Close();
    }

    private IEnumerator DelayedOpenAndClose(bool isOpen)
    {
        yield return new WaitForSeconds(0.01f);

        if (isOpen) OnUIOpenRequested?.Invoke(UIName.RouteSelectUI);
        else OnUICloseRequested?.Invoke(UIName.RouteSelectUI);
    }

    private void SettingButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.PopupSettingUI);
        OnUICloseRequested?.Invoke(UIName.RouteSelectUI);
    }

    private void ShopButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void InventoryButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void CookButtonClicked()
    {
        throw new NotImplementedException();
    }

    public void ButtonMoveAway()
    {
        _buttonTweenAnimation.moveAway();
        StartCoroutine(DelayedDeactive(_buttonTweenAnimation.gameObject, _buttonTweenAnimation.tweenTime));
    }

    public void ButtonMoveBack()
    {
        _buttonTweenAnimation.gameObject.SetActive(true);
        _buttonTweenAnimation.moveBack();
    }

    private IEnumerator DelayedDeactive(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        target.SetActive(false);
    }
}