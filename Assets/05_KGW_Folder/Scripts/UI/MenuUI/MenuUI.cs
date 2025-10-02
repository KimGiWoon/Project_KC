using System;
using System.Collections.Generic;
using SDW;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : BaseUI
{
    [Header("Battle UI Reference")]
    [SerializeField] private BattleUI _battleUI;

    [Header("UI Container")]
    [SerializeField] private GameObject _isOkayContainer;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    [Header("Volume Component")]
    [SerializeField] private List<Slider> _volumeSlider;
    [SerializeField] private List<Button> _muteButtonList;
    [SerializeField] private List<GameObject> _muteObjectList;
    [SerializeField] private List<TextMeshProUGUI> _volumeValueList;

    [Header("Buttons")]
    [SerializeField] private Button _continueButton; // 계속하기 버튼
    [SerializeField] private Button _lobbyButton; // 로비 이동 버튼

    [SerializeField] private GameObject _popupBackground;
    [SerializeField] private BattleManager _battleManager;

    private RectTransform _panelRect;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private List<float> _originalVolumeList = new List<float>();
    private List<bool> _originalMuteList = new List<bool>();

    private AudioManager _audio;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _panelRect = _panelContainer.GetComponent<RectTransform>();
        _audio = GameManager.Instance.Audio;

        _isOkayContainer.SetActive(false);
    }

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        _cancelButton.onClick.AddListener(CancelButtonClicked);
        _continueButton.onClick.AddListener(ContinueButtonClick);
        _lobbyButton.onClick.AddListener(LobbyButtonClick);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
        _cancelButton.onClick.RemoveListener(CancelButtonClicked);
        _continueButton.onClick.RemoveListener(ContinueButtonClick);
        _lobbyButton.onClick.RemoveListener(LobbyButtonClick);
    }

    public override void Open()
    {
        InitializeSettings();
        SetupInitialVolumeState();
        base.Open();
    }

    public override void Close()
    {
        _originalVolumeList.Clear();
        _originalMuteList.Clear();
        base.Close();
    }

    private void Update()
    {
        if (!_panelContainer.activeSelf) return;

        //# 안드로이드 터치 감지
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var touchPos = Input.GetTouch(0).position;

            //# 패널 안에 터치가 있는지 확인
            if (!RectTransformUtility.RectangleContainsScreenPoint(_panelRect, touchPos, Camera.main))
            {
                if (_isOkayContainer.activeSelf)
                {
                    _isOkayContainer.SetActive(false);
                    return;
                }

                CancelToChange();
                OnUICloseRequested?.Invoke(UIName.MenuUI);
                _popupBackground.SetActive(false);
                _battleUI._isOnMenu = false;
            }
        }
    }

    // 사운드 볼륨 초기화
    private void InitializeSettings()
    {
        foreach (var slider in _volumeSlider)
        {
            slider.onValueChanged.AddListener((value) =>
            {
                var buttonId = slider.GetComponent<ButtonId>();
                slider.minValue = 0f;
                slider.maxValue = 100f;
                slider.wholeNumbers = true;
                VolumeSliderChanged(buttonId.Id, value);
            });
        }
        foreach (var muteButton in _muteButtonList)
        {
            muteButton.onClick.AddListener(() =>
            {
                var buttonId = muteButton.GetComponent<ButtonId>();
                MuteButtonClicked(buttonId.Id);
            });
        }
    }
    private void SetupInitialVolumeState()
    {
        for (int i = 0; i < _volumeSlider.Count; i++)
        {
            _volumeSlider[i].value = _audio.VolumeList[i];
            _muteObjectList[i].SetActive(_audio.VolumeMuteList[i]);
            _originalVolumeList.Add(_volumeSlider[i].value);
            _originalMuteList.Add(_muteObjectList[i].activeSelf);
            _volumeValueList[i].text = _volumeSlider[i].value.ToString();
        }
    }

    private void VolumeSliderChanged(int buttonId, float value)
    {
        _volumeValueList[buttonId].text = _volumeSlider[buttonId].value.ToString();
        _audio.SetVolume((VolumeType)buttonId, value);
    }

    private void CancelToChange()
    {
        for (int i = 0; i < _volumeSlider.Count; i++)
        {
            _volumeSlider[i].value = _originalVolumeList[i];
            SetMuteState(_originalMuteList[i], i);
        }
    }

    private void SetMuteState(bool state, int buttonId)
    {
        _muteObjectList[buttonId].SetActive(state);
        _audio.SetMute((VolumeType)buttonId, _muteObjectList[buttonId].activeSelf);
    }

    private void MuteButtonClicked(int buttonId)
    {
        bool currentState = _muteObjectList[buttonId].activeSelf;
        _muteObjectList[buttonId].SetActive(!currentState);

        //# id 0 = Master, id 1 = bgm, id 2 = sfx
        _audio.SetMute((VolumeType)buttonId, _muteObjectList[buttonId].activeSelf);
    }

    // 계속하기 버튼 클릭
    private void ContinueButtonClick()
    {
        PlayerPrefs.SetInt("MasterVolume", (int)_volumeSlider[0].value);
        PlayerPrefs.SetInt("BGMVolume", (int)_volumeSlider[1].value);
        PlayerPrefs.SetInt("SFXVolume", (int)_volumeSlider[2].value);

        PlayerPrefs.SetInt("MasterVolumeMute", _muteObjectList[0].activeSelf ? 1 : 0);
        PlayerPrefs.SetInt("BGMVolumeMute", _muteObjectList[1].activeSelf ? 1 : 0);
        PlayerPrefs.SetInt("SFXVolumeMute", _muteObjectList[2].activeSelf ? 1 : 0);

        OnUICloseRequested?.Invoke(UIName.MenuUI);
        _popupBackground.SetActive(false);
        _battleUI._isOnMenu = false;
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick() => _isOkayContainer.SetActive(true);

    private void ConfirmButtonClicked()
    {
        _battleManager.CharacterStatSave();
        OnUIOpenRequested?.Invoke(UIName.DefeatChapterUI);
        OnUICloseRequested?.Invoke(UIName.MenuUI);
        _isOkayContainer.SetActive(false);
    }
    private void CancelButtonClicked() => _isOkayContainer.SetActive(false);
}