using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

// 챕터 클리어 UI
public class ClearChapterUI : BaseUI
{
    [Header("Battle Manager Reference")]
    [SerializeField] private TextMeshProUGUI _yeopjeonText;
    private Button _confirmButton; // 로비 이동 버튼
    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    // 컴포넌트의 할당
    private void Awake()
    {
        _panelContainer.SetActive(false); // 패널 비활성화
        _confirmButton = _panelContainer.GetComponentInChildren<Button>();
    }

    private void OnEnable()
    {
        // 버튼 등록
        _confirmButton.onClick.AddListener(LobbyButtonClick);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(LobbyButtonClick);
    }

    public override void Open()
    {
        base.Open();

        GameManager.Instance.Coin.OnRelicChanged?.Invoke();
        GameManager.Instance.Coin.AddYeopjeon(100);
        _yeopjeonText.text = $"{GameManager.Instance.Coin.bonus} 엽전을 획득하였습니다.";
        // 승리 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.BattleVictory);
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick()
    {
        OnUIOpenRequested?.Invoke(UIName.RoguelikeClosingUI);
        OnUICloseRequested?.Invoke(UIName.ClearChapterUI);
    }
}