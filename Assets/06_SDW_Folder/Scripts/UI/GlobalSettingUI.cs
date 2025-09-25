using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class GlobalSettingUI : BaseUI
    {
        [SerializeField] private GameObject _backgroundPanelObject;
        private TweenAlpha_Image _backgroundPanel;
        [Header("Top Component")]
        [SerializeField] private TextMeshProUGUI _userNameText;
        [SerializeField] private TextMeshProUGUI _uidText;

        [Header("Center Component")]
        [SerializeField] private List<Slider> _volumeSlider;

        [Header("Volume Component")]
        [SerializeField] private List<Button> _muteButtonList;
        [SerializeField] private List<GameObject> _muteObjectList;

        [Header("Buttons")]
        [SerializeField] private Button _deleteAccountButton;
        [SerializeField] private Button _signOutButton;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _saveButton;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _tweenAnimation;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        public Action OnSignOutButtonClicked;
        private Coroutine _coroutine;
        private bool _isRoguelikeScene;
        private RectTransform _rectTransform;

        private AudioManager _audio;
        private GameManager _gameManager;
        private bool _isProgress;

        private List<float> _originalVolumeList = new List<float>();
        private List<bool> _originalMuteList = new List<bool>();

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _backgroundPanel = _backgroundPanelObject.GetComponent<TweenAlpha_Image>();
            _backgroundPanelObject.SetActive(false);
            _rectTransform = _panelContainer.GetComponent<RectTransform>();
        }

        protected override void Start()
        {
            if (GameManager.Instance.UI.UiDic.ContainsKey(UIName.GlobalSettingUI)) return;

            _gameManager = GameManager.Instance;
            base.Start();
            StartCoroutine(LoadCoroutine());
        }

        private IEnumerator LoadCoroutine()
        {
            while (true)
            {
                yield return null;
                if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected ||
                    !_gameManager.PrefabAndSoConnected) continue;

                break;
            }
            _audio = GameManager.Instance.Audio;
            InitializeSettings();
            CheckSceneName();
        }

        private void OnDisable()
        {
            // foreach (var slider in _volumeSlider)
            // {
            //     slider.onValueChanged.RemoveListener((value) =>
            //     {
            //         var buttonId = slider.GetComponent<ButtonId>();
            //         slider.minValue = 0f;
            //         slider.maxValue = 100f;
            //         slider.wholeNumbers = true;
            //         VolumeSliderChanged(buttonId.Id, value);
            //     });
            // }
            // foreach (var muteButton in _muteButtonList)
            // {
            //     muteButton.onClick.RemoveListener(() =>
            //     {
            //         var buttonId = muteButton.GetComponent<ButtonId>();
            //         MuteButtonClicked(buttonId.Id);
            //     });
            // }
            // _deleteAccountButton.onClick.RemoveListener(DeleteAccountButtonClicked);
            // _signOutButton.onClick.RemoveListener(SignOutButtonClicked);
            // _giveUpButton.onClick.RemoveListener(GiveUpButtonClicked);
            // _saveButton.onClick.RemoveListener(SaveButtonClicked);
            //
            // if (_coroutine != null) StopCoroutine(_coroutine);
        }

        protected override void OnDestroy()
        {
        }

        private void Update()
        {
            if (!_panelContainer.activeSelf || _isProgress) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (!RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, touchPos))
                {
                    CancelToChange();
                    _isProgress = true;
                    OnUICloseRequested?.Invoke(UIName.GlobalSettingUI);
                }
            }
        }

        public override void Open()
        {
            _isProgress = false;
            _backgroundPanelObject.SetActive(true);
            _gameManager.Firebase.RequestUserInfo();
            SetupInitialVolumeState();
            CheckSceneName();
            _tweenAnimation.moveAway();
            base.Open();
        }

        public override void Close()
        {
            _backgroundPanel.FadeOut();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            _tweenAnimation.moveBack();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            _originalVolumeList.Clear();
            _originalMuteList.Clear();
            base.Close();
        }

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

            _deleteAccountButton.onClick.AddListener(DeleteAccountButtonClicked);
            _signOutButton.onClick.AddListener(SignOutButtonClicked);
            _giveUpButton.onClick.AddListener(GiveUpButtonClicked);
            _saveButton.onClick.AddListener(SaveButtonClicked);
        }

        private void SetupInitialVolumeState()
        {
            for (int i = 0; i < _volumeSlider.Count; i++)
            {
                _volumeSlider[i].value = _audio.VolumeList[i];
                _muteObjectList[i].SetActive(_audio.VolumeMuteList[i]);
                _originalVolumeList.Add(_volumeSlider[i].value);
                _originalMuteList.Add(_muteObjectList[i].activeSelf);
            }
        }

        #region Update User Info

        /// <summary>
        /// 사용자 정보를 UI 컴포넌트에 업데이트
        /// </summary>
        /// <param name="user">사용자 정보</param>
        public void UpdateUserInfo(UserInfo user)
        {
            _userNameText.text = $"셰프명: {user.Nickname}";
            _uidText.text = $"UID : {user.UserId}";
        }

        #endregion

        private void CheckSceneName()
        {
            var sceneName = (SceneName)Enum.Parse(typeof(SceneName), GameManager.Instance.Scene.GetActiveScene());

            if (sceneName == SceneName.SDW_RoguelikeScene)
            {
                _signOutButton.gameObject.SetActive(false);
                _deleteAccountButton.gameObject.SetActive(false);
                _giveUpButton.gameObject.SetActive(true);
                _isRoguelikeScene = false;
            }
            else
            {
                _signOutButton.gameObject.SetActive(true);
                _deleteAccountButton.gameObject.SetActive(true);
                _giveUpButton.gameObject.SetActive(false);
                _isRoguelikeScene = true;
            }
        }

        private void CancelToChange()
        {
            for (int i = 0; i < _volumeSlider.Count; i++)
            {
                _volumeSlider[i].value = _originalVolumeList[i];
                SetMuteState(_originalMuteList[i], i);
            }
        }

        #region Slider Methods

        private void VolumeSliderChanged(int buttonId, float value)
        {
            _audio.SetVolume((VolumeType)buttonId, value);
        }

        #endregion

        #region Button Methods

        private void MuteButtonClicked(int buttonId)
        {
            bool currentState = _muteObjectList[buttonId].activeSelf;
            _muteObjectList[buttonId].SetActive(!currentState);

            //# id 0 = Master, id 1 = bgm, id 2 = sfx
            _audio.SetMute((VolumeType)buttonId, _muteObjectList[buttonId].activeSelf);
        }

        private void SetMuteState(bool state, int buttonId)
        {
            _muteObjectList[buttonId].SetActive(state);
            _audio.SetMute((VolumeType)buttonId, _muteObjectList[buttonId].activeSelf);
        }

        /// <summary>
        /// Delete Account 버튼 클릭 이벤트 핸들러
        /// </summary>
        private void DeleteAccountButtonClicked()
        {
            CancelToChange();
            OnUIOpenRequested?.Invoke(UIName.DeleteAccountUI);
        }

        /// <summary>
        /// 호출된 경우 사용자 정보 UI에서로그아웃 기능을 실행하는 이벤트 핸들러 메소드
        /// </summary>
        private void SignOutButtonClicked()
        {
            CancelToChange();
            OnSignOutButtonClicked?.Invoke();
            OnUICloseRequested?.Invoke(UIName.GlobalSettingUI);
        }

        private void GiveUpButtonClicked()
        {
            CancelToChange();
            OnUIOpenRequested?.Invoke(UIName.RoguelikeClosingUI);
            OnUICloseRequested?.Invoke(UIName.GlobalSettingUI);
        }

        private void SaveButtonClicked()
        {
            _isProgress = true;
            PlayerPrefs.SetInt("MasterVolume", (int)_volumeSlider[0].value);
            PlayerPrefs.SetInt("BGMVolume", (int)_volumeSlider[1].value);
            PlayerPrefs.SetInt("SFXVolume", (int)_volumeSlider[2].value);

            PlayerPrefs.SetInt("MasterVolumeMute", _muteObjectList[0].activeSelf ? 1 : 0);
            PlayerPrefs.SetInt("BGMVolumeMute", _muteObjectList[1].activeSelf ? 1 : 0);
            PlayerPrefs.SetInt("SFXVolumeMute", _muteObjectList[2].activeSelf ? 1 : 0);

            OnUICloseRequested?.Invoke(UIName.GlobalSettingUI);
        }

        public void DeactiveDeleteButton()
        {
            _deleteAccountButton.interactable = false;
            _coroutine = StartCoroutine(ActiveDeleteButton());
        }

        private IEnumerator ActiveDeleteButton()
        {
            yield return new WaitForSeconds(3f);
            _deleteAccountButton.interactable = false;
            _coroutine = null;
        }

        #endregion
    }
}