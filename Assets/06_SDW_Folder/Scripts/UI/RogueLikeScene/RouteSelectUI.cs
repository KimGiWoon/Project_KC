using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDW;
using TMPro;

public class RouteSelectUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private Button _moveLeftButton;
    [SerializeField] private Button _moveStraightButton;
    [SerializeField] private Button _moveRightButton;
    [SerializeField] private Button _partyButton;
    private TextMeshProUGUI _partyButtonText;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        // _panelContainer.SetActive(false);
        _tweenAnimation = GetComponent<TweenAnimation>();
        _partyButtonText = _partyButton.GetComponentInChildren<TextMeshProUGUI>();
        _partyButtonText.text = "파티 편성";
    }

    private void OnEnable()
    {
        _moveLeftButton.onClick.AddListener(LeftButtonClicked);
        _moveStraightButton.onClick.AddListener(CenterButtonClicked);
        _moveRightButton.onClick.AddListener(RightButtonClicked);
        _partyButton.onClick.AddListener(PartyButtonClicked);
    }

    private void OnDisable()
    {
        _moveLeftButton.onClick.RemoveListener(LeftButtonClicked);
        _moveStraightButton.onClick.RemoveListener(CenterButtonClicked);
        _moveRightButton.onClick.RemoveListener(RightButtonClicked);
        _partyButton.onClick.RemoveListener(PartyButtonClicked);
    }

    private TweenAnimation _tweenAnimation;

    public override void Open()
    {
        base.Open();
        _tweenAnimation.moveBack();
    }

    public override void Close()
    {
        _tweenAnimation.moveAway();
        StartCoroutine(DelayedClose());
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(_tweenAnimation.tweenTime);
        base.Close();
    }

    private void LeftButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void CenterButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void RightButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void PartyButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.PartyUI);
        OnUICloseRequested?.Invoke(UIName.RouteSelectUI);
    }
}