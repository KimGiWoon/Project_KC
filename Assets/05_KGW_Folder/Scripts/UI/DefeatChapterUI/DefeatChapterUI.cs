using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

// 클리어 실패 UI
public class DefeatChapterUI : BaseUI
{
    [Header("Battle Manager Reference")]
    // private BattleManager _battleManager;
    private TMP_Text _expText; // 겅험치 텍스트
    private TMP_Text _growthPointText; // 성장 포인트 텍스트
    private Button _confirmButton; // 로비 이동 버튼

    public Action<UIName> OnUICloseRequested;

    // 컴포넌트의 할당
    private void Awake()
    {
        _panelContainer.SetActive(false); // 패널 비활성화
        _expText = _panelContainer.GetComponentInChildren<TMP_Text>();
        _growthPointText = _panelContainer.GetComponentInChildren<TMP_Text>();
        _confirmButton = _panelContainer.GetComponentInChildren<Button>();
        _confirmButton.onClick.AddListener(LobbyButtonClick);
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick()
    {
        // 게임 클리어 확인
        // if (!_battleManager._isClear)
        // {
        // TODO : 김기운 : 추후에 마이씬 매니저 교체 예정
        // SceneManager.LoadScene("KGW_TestLobbyScene");
        GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene, UIName.KGW_StageSelectUI);
        OnUICloseRequested?.Invoke(UIName.DefeatChapterUI);
        // }
    }
}