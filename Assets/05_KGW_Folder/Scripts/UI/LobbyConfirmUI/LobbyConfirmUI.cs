using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyConfirmUI : MonoBehaviour
{
    [Header("Battle Manager Reference")]
    [SerializeField] BattleUI _battleUIManager;

    Button _yesButton;   // 예스 버튼
    Button _noButton;    // 노 버튼

    private void Awake()
    {
        //_yesButton = _panelContainer.GetComponentInChildren<Button>();
        //_noButton = _panelContainer.GetComponentInChildren<Button>();

        _yesButton = GetComponentInChildren<Button>();
        _noButton = GetComponentInChildren<Button>();

        // 버튼 등록
        _yesButton.onClick.AddListener(YesButtonClick);
        _noButton.onClick.AddListener(NoButtonClick);
    }

    // 예스 버튼 클릭
    private void YesButtonClick()
    {
        // TODO : 김기운 : 추후에 마이씬 매니저 교체 예정
        SceneManager.LoadScene("KGW_TestLobbyScene");

        // 해당 UI 비활성화
        //_panelContainer.SetActive(false);

        _battleUIManager._menuUI.SetActive(false);
    }

    // 노 버튼 클릭
    private void NoButtonClick()
    {
    }
}
