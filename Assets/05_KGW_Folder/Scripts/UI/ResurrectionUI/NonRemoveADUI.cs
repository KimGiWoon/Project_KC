using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;
using UnityEngine.UI;

public class NonRemoveADUI : BaseUI
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
        // TODO : 광고 보기
        PlayAdvertisement();

        // TODO : 캐릭터의 부활관련 코드는 배틀매니저의 스폰 메서드로 사용하여 선택한 캐릭터를 체력 100%으로 소환
    }

    // 노 버튼 클릭
    private void NoButtonClick()
    {
        // TODO : 로비 확인 UI 오픈
        // OnUIOpenRequested?.Invoke(UIName.LobbyConfirmUI);
    }

    // 광고 보기
    private void PlayAdvertisement()
    {
        // TODO : 광고 시청
        // 광고를 봤다고 치고 재시작

        OnUICloseRequested?.Invoke(UIName.NonRemoveADUI);
        _popupBackground.SetActive(false);

        // 캐릭터 부활
        CharacterResurrection();
    }

    // 캐릭터 부활
    private void CharacterResurrection()
    {
        // 게임 진행 상황 초기화
        _battleManager._isClear = false;
        _battleManager._isGameOver = false;
        _battleManager._characters.Clear();
        _battleManager.Wall.gameObject.SetActive(false);
        _battleManager._battleUI._count = 3f;

        _battleManager.CharacterSpawn();
    }

    private void ConfirmButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.DefeatChapterUI);
        OnUICloseRequested?.Invoke(UIName.NonRemoveADUI);
        _isOkayContainer.SetActive(false);
    }
    private void CancelButtonClicked() => _isOkayContainer.SetActive(false);
}