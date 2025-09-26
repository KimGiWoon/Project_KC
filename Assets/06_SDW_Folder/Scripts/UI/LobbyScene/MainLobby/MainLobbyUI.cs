using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SDW
{
    public class MainLobbyUI : BaseUI
    {
        [Header("Top Components")]
        [SerializeField] private TextMeshProUGUI _menuTitle;
        private string _prevTitle;
        [SerializeField] private Image _userIcon;
        [SerializeField] private Button _userInfoButton;
        [SerializeField] private TextMeshProUGUI _cashStarText;
        [SerializeField] private Button _cashStarButton;
        [SerializeField] private TextMeshProUGUI _rainbowStarText;
        [SerializeField] private Button _rainbowStarButton;
        [SerializeField] private Button _optionButton;
        //todo Game Start 눌렀을 때 뜨는 팝업 창으로 이동해야 함
        // [SerializeField] private Button _growthButton;

        [Header("Right Components")]
        [SerializeField] private Image _curerntChaImage;
        [SerializeField] private ExpandMenu _expandMenu;
        [SerializeField] private List<Button> _memoryButtonList;
        [SerializeField] private List<Image> _memoryImageList;

        [Header("Center Components")]
        [SerializeField] private Button _stageSelectButton;

        [Header("Animations")]
        [SerializeField] private TweenAnimation _mainLobbyTweenAnimation;
        [SerializeField] private TweenAlpha _backgroundVideoTweenAnimation;
        [SerializeField] private float delayTime = 1.35f;

        public Action<UIName> OnUIOpenRequested;
        public Action<int> OnIconRequested;
        public Action<bool> OnButtonInteractableChanged;
        private GameManager _gameManager;
        private bool _isLoaded;
        private Coroutine _iconCoroutine;
        private int _iconNumber;
        private bool _isBusy = false;
        private VideoPlayer _videoPlayer;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _gameManager = GameManager.Instance;
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            _cashStarButton.onClick.AddListener(CashStarButtonClicked);
            _rainbowStarButton.onClick.AddListener(RainbowStartButtonClicked);
            _optionButton.onClick.AddListener(OptionButtonClicked);
            _userInfoButton.onClick.AddListener(UserInfoButtonClicked);
            _stageSelectButton.onClick.AddListener(StageSelectButtonClicked);

            foreach (var memoryButton in _memoryButtonList)
            {
                var buttonId = memoryButton.GetComponent<ButtonId>();
                memoryButton.onClick.AddListener(() => MemoryButtonClicked(buttonId.Id));
            }

            _gameManager.Coin.OnShiningStarCandyChanged += UpdateShiningStarCandy;
            _gameManager.Coin.OnStarCandyChanged += UpdateRainbowStar;
            _gameManager.Reward.OnStarCandyChange += UpdateRainbowStar;
            _gameManager.Reward.OnShiningStarCandyChange += UpdateShiningStarCandy;
            _gameManager.DailyQuest.OnStarCandyChange += UpdateRainbowStar;
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _cashStarButton.onClick.RemoveListener(CashStarButtonClicked);
            _rainbowStarButton.onClick.RemoveListener(RainbowStartButtonClicked);
            _optionButton.onClick.RemoveListener(OptionButtonClicked);
            _userInfoButton.onClick.RemoveListener(UserInfoButtonClicked);
            _stageSelectButton.onClick.RemoveListener(StageSelectButtonClicked);

            foreach (var memoryButton in _memoryButtonList)
            {
                var buttonId = memoryButton.GetComponent<ButtonId>();
                memoryButton.onClick.RemoveListener(() => MemoryButtonClicked(buttonId.Id));
            }
            _gameManager.Coin.OnShiningStarCandyChanged -= UpdateShiningStarCandy;
            _gameManager.Coin.OnStarCandyChanged -= UpdateRainbowStar;

            _gameManager.Reward.OnStarCandyChange -= UpdateRainbowStar;
            _gameManager.Reward.OnShiningStarCandyChange -= UpdateShiningStarCandy;
            _gameManager.DailyQuest.OnStarCandyChange -= UpdateRainbowStar;
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(LoadCoroutine());
        }

        private IEnumerator LoadCoroutine()
        {
            while (true)
            {
                yield return null;
                if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                    !_gameManager.Firebase.IsLoaded || !_gameManager.Video.IsLoaded || !_gameManager.Audio.IsLoaded) continue;

                break;
            }

            //todo 추후 선택지도 DB에 저장하면 Load해서 설정, 초기 HSR로 설정
            _curerntChaImage.sprite = _memoryImageList[0].sprite;
            _videoPlayer.clip = _gameManager.Video.VideoDictionary[VideoClipName.VideoHSR].Video;
            _gameManager.Audio.PlayBGM(AudioClipName.MemoryHSR);
        }

        public override void Open()
        {
            SetMainText("메인 로비");
            _prevTitle = _menuTitle.text;
            OnUIOpenRequested?.Invoke(UIName.MainLobbyBottomUI);
            base.Open();
            StartCoroutine(UpdateCoroutine());
        }

        private IEnumerator UpdateCoroutine()
        {
            while (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                   !_gameManager.Firebase.IsLoaded || _isLoaded)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            UpdateUserInfo(_gameManager.Firebase.GetUserInfo());
            UpdateRainbowStar(GameManager.Instance.Coin.starCandy);
            UpdateShiningStarCandy(GameManager.Instance.Coin.shiningStarCandy);

            _isLoaded = true;
        }

        public override void Close()
        {
            _mainLobbyTweenAnimation.moveAway();
            _backgroundVideoTweenAnimation.FadeOut();

            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_mainLobbyTweenAnimation.tweenTime);
            base.Close();
        }

        #region Button Methods

        private void CashStarButtonClicked()
        {
            SetMainText("유료상점");
            _cashStarButton.interactable = false;
            StartCoroutine(DelayedActivateButton(_cashStarButton));
            OnUIOpenRequested?.Invoke(UIName.PaidStoreUI);
        }

        private void RainbowStartButtonClicked()
        {
            SetMainText("별사탕 교환");
            _rainbowStarButton.interactable = false;
            StartCoroutine(DelayedActivateButton(_rainbowStarButton));
            OnUIOpenRequested?.Invoke(UIName.SugarStarExchangeUI);
        }

        private void OptionButtonClicked()
        {
            SetMainText("환경설정");
            _optionButton.interactable = false;
            StartCoroutine(DelayedActivateButton(_optionButton));
            OnUIOpenRequested?.Invoke(UIName.GlobalSettingUI);
        }

        /// <summary>
        /// UserInfoButtonClicked 핸들러 메서드 호출로 사용자가 UserInfo 버튼을 눌렀을 때 UserInfoUI를 활성화
        /// </summary>
        private void UserInfoButtonClicked()
        {
            SetMainText("내 정보");
            _userInfoButton.interactable = false;
            StartCoroutine(DelayedActivateButton(_userInfoButton));
            OnUIOpenRequested?.Invoke(UIName.UserInfoUI);
        }
        private void MemoryButtonClicked(int index)
        {
            //todo 기본적으로 index로 적용하면 되지만 현재는 4번까지만 나왔으므로
            if (index < 4)
            {
                _curerntChaImage.sprite = _memoryImageList[index].sprite;
                _videoPlayer.clip = _gameManager.Video.VideoDictionary[(VideoClipName)index].Video;
                _gameManager.Audio.PlayBGM((AudioClipName)index);
            }
            else
            {
                _curerntChaImage.sprite = _memoryImageList[index].sprite;
                _videoPlayer.clip = _gameManager.Video.VideoDictionary[(VideoClipName)3].Video;
                _gameManager.Audio.PlayBGM((AudioClipName)3);
            }
        }

        /// <summary>
        /// GameStartButtonClicked 핸들러 메서드 호출로 사용자가 GameStart 버튼을 클릭했을 때 StageUI를 활성화
        /// </summary>
        private void StageSelectButtonClicked()
        {
            SetMainText("스테이지 선택");
            _stageSelectButton.interactable = false;
            StartCoroutine(DelayedActivateButton(_stageSelectButton));
            OnUIOpenRequested?.Invoke(UIName.StageSelectUI);
        }

        private IEnumerator DelayedActivateButton(Button button)
        {
            yield return new WaitForSeconds(2f);
            button.interactable = true;
        }

        #endregion

        #region Update User Info

        /// <summary>
        /// UserInfoUI의 Nickname을 업데이트
        /// </summary>
        /// <param name="user">사용자의 정보</param>
        public void UpdateUserInfo(UserInfo user)
        {
            _iconNumber = user.IconNumber;
            OnIconRequested?.Invoke(_iconNumber);
        }

        /// <summary>
        /// Icon을 설정하기 위한 메서드
        /// </summary>
        /// <param name="sprite">설정할 Icon</param>
        public void SetIcon(Sprite sprite)
        {
            if (sprite == null && _iconCoroutine == null)
                _iconCoroutine = StartCoroutine(UpdateIcon());
            else if (_iconCoroutine != null)
                StopCoroutine(_iconCoroutine);

            _userIcon.sprite = sprite;
            Canvas.ForceUpdateCanvases();
        }

        private IEnumerator UpdateIcon()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                OnIconRequested?.Invoke(_iconNumber);
            }
        }

        #endregion

        /// <summary>
        /// RainbowStar의 정보를 가져와서 Text 업데이트
        /// </summary>
        /// <param name="numOfStars">업데이트할 Rainbow Start 수</param>
        private void UpdateRainbowStar(int numOfStars)
        {
            _rainbowStarText.text = numOfStars.ToString();
        }

        private void UpdateShiningStarCandy(int numOfStars)
        {
            _cashStarText.text = numOfStars.ToString();
        }

        public void SetMainText(string value)
        {
            _menuTitle.text = value;
        }

        public void SetPrevText(string value) => _prevTitle = value;

        public void MainLobbyMoveBack()
        {
            _mainLobbyTweenAnimation.moveBack();
            _backgroundVideoTweenAnimation.FadeIn();

            DOVirtual.DelayedCall(delayTime, () => { OnButtonInteractableChanged?.Invoke(true); });
        }

        public void ResetMainText() => SetMainText(_prevTitle);
    }
}