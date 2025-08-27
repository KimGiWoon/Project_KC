using System;
using UnityEngine;
using UnityEngine.UI;
using SDW;

// 스테이지를 클리어하여 보상을 선택하는 UI
public class ClearStageUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private Button _confirmButton; // 랜덤 인카운터로 이동 버튼

    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
    }

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(ConfirmButtonClicked);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
    }

    private void ConfirmButtonClicked()
    {
        GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene, UIName.KGW_StageSelectUI);
        OnUICloseRequested?.Invoke(UIName.ClearStageUI);
    }
}