using System;
using SDW;
using UnityEngine;
using UnityEngine.UI;

public class KGW_StageSelectUI : BaseUI
{
    [Header("UI Setting")]
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _selectButton; // 배치 버튼
    [SerializeField] private Button _normalBattleButton; // 전투 버튼
    [SerializeField] private Button _bossBattleButton; // 전투 버튼

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
    }

    private void OnEnable()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _normalBattleButton.onClick.AddListener(OnNormalBattleStartClick);
        _bossBattleButton.onClick.AddListener(OnBossBattleStartClick);
        _selectButton.onClick.AddListener(OnSelectButtonClick);
    }

    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        _normalBattleButton.onClick.RemoveListener(OnNormalBattleStartClick);
        _bossBattleButton.onClick.RemoveListener(OnBossBattleStartClick);
        _selectButton.onClick.RemoveListener(OnSelectButtonClick);
    }

    private void OnMainMenuButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.MainLobbyUI);
        OnUICloseRequested?.Invoke(UIName.KGW_StageSelectUI);
    }

    // 전투 스타트
    private void OnNormalBattleStartClick()
    {
        if (!CharacterSelectManager.Instance._canBettle)
        {
            Debug.Log("전투 참가 인원이 부족합니다.");
        }
        else
        {
            // 전투 시작
            GameManager.Instance.SetStageBoss(false);
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.KGW_TestIngameScene);
            OnUICloseRequested?.Invoke(UIName.KGW_StageSelectUI);
        }
    }

    private void OnBossBattleStartClick()
    {
        if (!CharacterSelectManager.Instance._canBettle)
        {
            Debug.Log("전투 참가 인원이 부족합니다.");
        }
        else
        {
            // 전투 시작
            GameManager.Instance.SetStageBoss(true);
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