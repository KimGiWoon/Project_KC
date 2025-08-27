using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : BaseUI
{
    Slider _slider_BGM; // BGM 슬라이더
    Slider _slider_SFX; // SFX 슬라이더
    Button _ContinueButton; // 계속하기 버튼
    Button _lobbyButton;    // 로비 이동 버튼

    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _slider_BGM = _panelContainer.GetComponentInChildren<Slider>();
        _slider_SFX = _panelContainer.GetComponentInChildren<Slider>();
        _ContinueButton = _panelContainer.GetComponentInChildren<Button>();
        _lobbyButton = _panelContainer.GetComponentInChildren<Button>();

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
        // 메뉴 패널 비활성화
        //OnUICloseRequested?.Invoke(UIName.MenuUI);
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick()
    {
        // TODO : 김기운 : 추후에 UI매니저에서 관리
        //OnUIOpenRequested?.Invoke(UIName.LobbyConfirmUI);
    }
}
