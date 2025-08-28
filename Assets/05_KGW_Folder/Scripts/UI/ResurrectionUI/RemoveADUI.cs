using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDW;
using UnityEngine.UI;

public class RemoveADUI : BaseUI
{
    [Header("Battle Manager Reference")]
    [SerializeField] private BattleManager _battleManager;

    [Header("UI Components")]
    [SerializeField] private GameObject _isOkayContainer;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    [SerializeField] private Button _retryButton; // 즉시 부활 버튼
    [SerializeField] private Button _noButton; // 로비 이동 버튼

    [SerializeField] private GameObject _popupBackground;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _isOkayContainer.SetActive(false);
    }

    private void OnEnable()
    {
        // 버튼, 슬라이드 등록
        _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        _cancelButton.onClick.AddListener(CancelButtonClicked);
        _retryButton.onClick.AddListener(RetryButtonClick);
        _noButton.onClick.AddListener(NoButtonClick);
    }

    private void OnDisable()
    {
        // 버튼, 슬라이드 등록
        _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
        _cancelButton.onClick.RemoveListener(CancelButtonClicked);
        _retryButton.onClick.RemoveListener(RetryButtonClick);
        _noButton.onClick.RemoveListener(NoButtonClick);
    }

    // 즉시 부활 버튼 클릭
    private void RetryButtonClick()
    {
        OnUICloseRequested?.Invoke(UIName.NonRemoveADUI);
        _popupBackground.SetActive(false);

        // 캐릭터 부활
        CharacterResurrection();
    }

    // 캐릭터 부활
    private void CharacterResurrection()
    {
        // 게임 진행 상황 초기화
        _battleManager._canResurrection = false;
        _battleManager._isClear = false;
        _battleManager._isGameOver = false;
        _battleManager._characters.Clear();
        //_battleManager.Wall.gameObject.SetActive(false);
        //_battleManager._battleUI._count = 3f;

        _battleManager.CharacterSpawn();
    }

    // 노 버튼 클릭
    private void NoButtonClick()
    {
        // TODO : 로비 확인 UI 오픈

        _isOkayContainer.SetActive(true);
    }

    private void ConfirmButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.DefeatChapterUI);
        OnUICloseRequested?.Invoke(UIName.RemoveADUI);
        _isOkayContainer.SetActive(false);
    }
    private void CancelButtonClicked() => _isOkayContainer.SetActive(false);
}