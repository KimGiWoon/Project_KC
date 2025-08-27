using System;
using SDW;
using UnityEngine;
using UnityEngine.UI;

public class KGW_StageSelectUI : BaseUI
{
    [Header("UI Setting")]
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _selectButton; // 배치 버튼
    [SerializeField] private Button _bettleButton; // 전투 버튼

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _bettleButton.onClick.AddListener(OnBettleStartClick);
        _selectButton.onClick.AddListener(OnSelectButtonClick);
    }

    private void OnMainMenuButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.MainLobbyUI);
        OnUICloseRequested?.Invoke(UIName.KGW_StageSelectUI);
    }

    // 전투 스타트
    private void OnBettleStartClick()
    {
        if (!CharacterSelectManager.Instance._canBettle)
        {
            Debug.Log("전투 참가 인원이 부족합니다.");
        }
        else
        {
            // 전투 시작
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.KGW_TestIngameScene);
            OnUICloseRequested?.Invoke(UIName.KGW_StageSelectUI);
        }
    }

    // 인원 배치 버튼 클릭
    public void OnSelectButtonClick()
    {
        OnUIOpenRequested?.Invoke(UIName.KGW_CharacterSelectUI);
        OnUICloseRequested?.Invoke(UIName.KGW_StageSelectUI);
    }
}