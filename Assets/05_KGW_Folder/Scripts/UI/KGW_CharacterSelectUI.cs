using System;
using SDW;
using UnityEngine;
using UnityEngine.UI;

public class KGW_CharacterSelectUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private GameObject _slotPanel; // 슬롯 패널
    [SerializeField] private Button _backButton; // 뒤로가기 버튼
    [SerializeField] private Button _okButton; // 확인 버튼

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _backButton.onClick.AddListener(OnBackButtonClick);
        _okButton.onClick.AddListener(OnOkButtonClick);
    }

    // 뒤로가기 버튼 클릭
    private void OnBackButtonClick()
    {
        OnUIOpenRequested?.Invoke(UIName.KGW_StageSelectUI);
        OnUICloseRequested?.Invoke(UIName.KGW_CharacterSelectUI);
    }

    // 완료 버튼 클릭
    private void OnOkButtonClick()
    {
        // 전투 가능 확인
        if (!CharacterSelectManager.Instance._canBettle)
        {
            Debug.Log("전투 참가 인원이 부족합니다.");
        }
        else
        {
            OnUIOpenRequested?.Invoke(UIName.KGW_StageSelectUI);
            OnUICloseRequested?.Invoke(UIName.KGW_CharacterSelectUI);
        }
    }
}