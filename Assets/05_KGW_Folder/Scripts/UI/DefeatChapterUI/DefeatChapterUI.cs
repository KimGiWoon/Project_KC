using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 클리어 실패 UI
public class DefeatChapterUI : MonoBehaviour //BaseUI
{
    [Header("Battle Manager Reference")]
    [SerializeField] BattleManager _battleManager;
    [SerializeField] BattleUIManager _battleUIManager;

    TMP_Text _expText;  // 겅험치 텍스트
    TMP_Text _growthPointText;  // 성장 포인트 텍스트
    Button _confirmButton;    // 로비 이동 버튼

    // 컴포넌트의 할당
    private void Awake()
    {
        //_panelContainer.SetActive(false);   // 패널 비활성화
        //_expText = _panelContainer.GetComponentInChildren<TMP_Text>();
        //_growthPointText = _panelContainer.GetComponentInChildren<TMP_Text>();
        //_confirmButton = _panelContainer.GetComponentInChildren<Button>();
        //_confirmButton.onClick.AddListener(LobbyButtonClick);

        _expText = GetComponentInChildren<TMP_Text>();
        _growthPointText = GetComponentInChildren<TMP_Text>();
        _confirmButton = GetComponentInChildren<Button>();
        _confirmButton.onClick.AddListener(LobbyButtonClick);
    }

    private void Start()
    {
        //// 획득한 경험치와 점수 출력
        //_expText.text = ;
        //_growthPointText.text = ;
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick()
    {
        // 게임 클리어 확인
        if (!_battleManager._isClear)
        {
            // TODO : 김기운 : 추후에 마이씬 매니저 교체 예정
            SceneManager.LoadScene("KGW_TestLobbyScene");

            // 해당 UI 비활성화
            //_panelContainer.SetActive(false);

            _battleUIManager._defeatChapterUI.SetActive(false);
        }
    }
}
