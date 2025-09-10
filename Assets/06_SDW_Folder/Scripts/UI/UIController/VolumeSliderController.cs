using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI _volumeValueText;
    [SerializeField] private Button _muteButton;
    [SerializeField] private Color _nonMutedColor;
    [SerializeField] private Color _mutedColor;

    private Slider _volumeSlider;

    private bool _currentMuteState;
    private float _originalVolume;
    private bool _originalMuteState;

    private void Awake() => _volumeSlider = GetComponent<Slider>();

    private void OnEnable()
    {
        _volumeSlider.onValueChanged.AddListener(SliderValueChanged);
        _muteButton.onClick.AddListener(MuteButtonClicked);

        _originalVolume = _volumeSlider.value;
    }

    private void OnDisable()
    {
        _volumeSlider.onValueChanged.RemoveListener(SliderValueChanged);
        _muteButton.onClick.RemoveListener(MuteButtonClicked);
    }

    private void SliderValueChanged(float value)
    {
        int volume = (int)(value * 100);
        _volumeValueText.text = volume.ToString();
    }

    private void MuteButtonClicked()
    {
        _originalMuteState = _currentMuteState;
        _currentMuteState = !_currentMuteState;

        var colorBlock = _muteButton.colors;

        colorBlock.normalColor = _currentMuteState ? _mutedColor : _nonMutedColor;
        colorBlock.highlightedColor = _currentMuteState ? _mutedColor : _nonMutedColor;
        colorBlock.pressedColor = _currentMuteState ? _mutedColor : _nonMutedColor;
        colorBlock.selectedColor = _currentMuteState ? _mutedColor : _nonMutedColor;
        _muteButton.colors = colorBlock;

        //todo 추후 mute 기능 구현 시 연동
        //# 버튼 상태 변해야 함
    }

    public void Cancel()
    {
        _volumeSlider.value = _originalVolume;
        _currentMuteState = _originalMuteState;
        //# 버튼 상태 변해야 함
    }
}