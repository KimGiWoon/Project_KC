using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDW;
using TMPro;
using UnityEngine.UI;

public class StageGlobalUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _cookButton;

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
        OnUIOpenRequested?.Invoke(UIName.RouteSelectUI);
    }

    public override void Close()
    {
        OnUICloseRequested?.Invoke(UIName.RouteSelectUI);
        base.Close();
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
}