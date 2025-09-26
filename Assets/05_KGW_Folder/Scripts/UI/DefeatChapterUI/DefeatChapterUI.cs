using System;
using UnityEngine;
using UnityEngine.UI;
using SDW;

// 클리어 실패 UI
public class DefeatChapterUI : BaseUI
{
    [Header("Battle Manager Reference")]
    private Button _confirmButton; // 로비 이동 버튼

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    // 컴포넌트의 할당
    private void Awake()
    {
        _panelContainer.SetActive(false); // 패널 비활성화
        _confirmButton = _panelContainer.GetComponentInChildren<Button>();
        _confirmButton.onClick.AddListener(LobbyButtonClick);
    }

    public override void Open()
    {
        base.Open();

        // 패배 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.BattleLose);
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick()
    {
        OnUIOpenRequested?.Invoke(UIName.RoguelikeClosingUI);
        OnUICloseRequested?.Invoke(UIName.DefeatChapterUI);
    }
}