using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour //BaseUI
{
    [Header("Battle Manager Reference")]
    [SerializeField] BattleUIManager _battleUIManager;

    Slider _slider_BGM; // BGM 슬라이더
    Slider _slider_SFX; // SFX 슬라이더
    Button _ContinueButton; // 계속하기 버튼
    Button _lobbyButton;    // 로비 이동 버튼

    private void Awake()
    {
        //_slider_BGM = _panelContainer.GetComponentInChildren<Slider>();
        //_slider_SFX = _panelContainer.GetComponentInChildren<Slider>();
        //_ContinueButton = _panelContainer.GetComponentInChildren<Button>();
        //_lobbyButton = _panelContainer.GetComponentInChildren<Button>();

        _slider_BGM = GetComponentInChildren<Slider>();
        _slider_SFX = GetComponentInChildren<Slider>();
        _ContinueButton = GetComponentInChildren<Button>();
        _lobbyButton = GetComponentInChildren<Button>();

        // 버튼, 슬라이드 등록
        //_slider_BGM.onValueChanged.AddListener()
        //_slider_SFX.onValueChanged.AddListener()
        _ContinueButton.onClick.AddListener(ContinueButtonClick);
        _lobbyButton.onClick.AddListener(LobbyButtonClick);
    }

    private void Start()
    {
        // 사운드 볼륨 초기화
        SoundVolumeinit();
    }

    // 사운드 볼륨 초기화
    private void SoundVolumeinit()
    {
        //_slider_BGM.value = ;
        //_slider_SFX.value = ;

    }

    // 계속하기 버튼 클릭
    private void ContinueButtonClick()
    {
        // TODO : 김기운 : 추후에 UI매니저에서 관리
        //GameManager.Instance.UI.ClosePanel(UIName.MenuUI);

        // 메뉴 패널 비활성화
        _battleUIManager._menuUI.SetActive(false);
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick()
    {
        // 로비 이동 확인 패널

        // TODO : 김기운 : 추후에 UI매니저에서 관리
        //GameManager.Instance.UI.OpenPanel(UIName.LobbyConfirmUI);
    }
}

    

