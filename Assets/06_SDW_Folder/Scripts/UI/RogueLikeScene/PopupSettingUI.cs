using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SDW
{
    public class PopupSettingUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _uidText;
        [SerializeField] private VolumeSliderController _masterVolumeSlider;
        [SerializeField] private VolumeSliderController _backgroundVolumeSlider;
        [SerializeField] private VolumeSliderController _effectVolumeSlider;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private GameObject _backgroundObject;
        private TweenAnimation _tweenAnimation;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
        }

        private void OnEnable()
        {
            _giveUpButton.onClick.AddListener(GiveUpButtonClicked);
            _saveButton.onClick.AddListener(SaveButtonClicked);
        }

        private void OnDisable()
        {
            _giveUpButton.onClick.RemoveListener(GiveUpButtonClicked);
            _saveButton.onClick.RemoveListener(SaveButtonClicked);
        }

        public override void Open()
        {
            _backgroundObject.SetActive(true);
            base.Open();
            _tweenAnimation.moveAway();
        }

        public override void Close()
        {
            _tweenAnimation.moveBack();
            StartCoroutine(DelayedClose());
            _backgroundObject.SetActive(false);
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void GiveUpButtonClicked()
        {
            _masterVolumeSlider.Cancel();
            _backgroundVolumeSlider.Cancel();
            _effectVolumeSlider.Cancel();

            //todo 추후 Popup - 진짜 포기할지, 정산창도 띄워야 함
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene);
            OnUICloseRequested?.Invoke(UIName.PopupSettingUI);
        }

        private void SaveButtonClicked()
        {
            //todo 추후 관련 세팅을 저장해야 함
            OnUICloseRequested?.Invoke(UIName.PopupSettingUI);
        }
    }
}