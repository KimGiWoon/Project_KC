using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

// 챕터 클리어 UI
public class ClearChapterUI : BaseUI
{
    [Header("Battle Manager Reference")]
    // private BattleManager _battleManager;
    [SerializeField] private TMP_Text _expText; // 겅험치 텍스트
    [SerializeField] private TMP_Text _growthPointText; // 성장 포인트 텍스트
    private Button _confirmButton; // 로비 이동 버튼

    public Action<UIName> OnUICloseRequested;

    // 컴포넌트의 할당
    private void Awake()
    {
        _panelContainer.SetActive(false); // 패널 비활성화
        // _expText = _panelContainer.GetComponentInChildren<TMP_Text>();
        // _growthPointText = _panelContainer.GetComponentInChildren<TMP_Text>();
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
        _expText.text = GameManager.Instance.Score.ToString();
        base.Open();
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick()
    {
        //todo 경험치 레시피로 변환
        GameManager.Instance.ClearScore();
        GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene);
        OnUICloseRequested?.Invoke(UIName.ClearChapterUI);
    }
}