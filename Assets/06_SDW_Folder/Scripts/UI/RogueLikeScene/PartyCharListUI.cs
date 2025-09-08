using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDW;

public class PartyCharListUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private GameObject _scrollContents;
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _closeButton;
    private TweenAnimation _tweenAnimation;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _tweenAnimation = GetComponent<TweenAnimation>();
    }

    private void OnEnable()
    {
        _infoButton.onClick.AddListener(InfoButtonClicked);
        _closeButton.onClick.AddListener(CloseButtonClicked);
    }

    private void OnDisable()
    {
        _infoButton.onClick.RemoveListener(InfoButtonClicked);
        _closeButton.onClick.RemoveListener(CloseButtonClicked);
    }

    public override void Open()
    {
        base.Open();
        _tweenAnimation.moveAway();
    }

    public override void Close()
    {
        _tweenAnimation.moveBack();
        base.Close();
    }

    private void InfoButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.CharInfoUI);
    }

    private void CloseButtonClicked()
    {
        OnUICloseRequested?.Invoke(UIName.PartyCharListUI);
    }
}