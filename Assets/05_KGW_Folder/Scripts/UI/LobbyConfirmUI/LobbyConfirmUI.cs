using SDW;
using UnityEngine;
using UnityEngine.UI;

public class LobbyConfirmUI : BaseUI
{
    [Header("Battle Manager Reference")]
    [SerializeField]
    private BattleUI _battleUIManager;

    private Button _yesButton; // 예스 버튼
    private Button _noButton; // 노 버튼

    private void Awake()
    {
        _yesButton = _panelContainer.GetComponentInChildren<Button>();
        _noButton = _panelContainer.GetComponentInChildren<Button>();

        // 버튼 등록
        _yesButton.onClick.AddListener(YesButtonClick);
        _noButton.onClick.AddListener(NoButtonClick);
    }

    // 예스 버튼 클릭
    private void YesButtonClick()
    {
        // TODO : 김기운 : 추후에 마이씬 매니저 교체 예정
        //GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene, UIName.KGW_StageSelectUI);
        //OnUICloseRequested?.Invoke(UIName.LobbyConfirmUI);
        //OnUICloseRequested?.Invoke(UIName.MenuUI);
    }

    // 노 버튼 클릭
    private void NoButtonClick()
    {
        // TODO : 김기운 : 추후에 UI매니저에서 관리
        //OnUICloseRequested?.Invoke(UIName.LobbyConfirmUI);
    }
}