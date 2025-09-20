using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class MainLobbyUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private Button _userInfoButton;
        [SerializeField] private Button _dailyQuestButton;
        [SerializeField] private Button _gachaButton;
        //todo 나중에 이동될 수 있음
        [SerializeField] private Button _growthButton;
        [SerializeField] private Button _levelUpButton;

        [SerializeField] private Image _userIcon;
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private TextMeshProUGUI _cashStarText;
        [SerializeField] private TextMeshProUGUI _rainbowStarText;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        public Action<int> OnIconRequested;
        private GameManager _gameManager;
        private bool _isLoaded;
        private Coroutine _iconCoroutine;
        private int _iconNumber;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _gameManager = GameManager.Instance;
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            _gameStartButton.onClick.AddListener(GameStartButtonClicked);
            _userInfoButton.onClick.AddListener(UserInfoButtonClicked);
            _dailyQuestButton.onClick.AddListener(DailyQuestButtonClicked);
            _gachaButton.onClick.AddListener(GachaButtonClicked);
            _growthButton.onClick.AddListener(GrowthButtonClicked);
            _levelUpButton.onClick.AddListener(LevelUpButtonClicked);

            GameManager.Instance.Reward.OnStarCandyChange += UpdateRainbowStar;
            GameManager.Instance.DailyQuest.OnStarCandyChange += UpdateRainbowStar;
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _gameStartButton.onClick.RemoveListener(GameStartButtonClicked);
            _userInfoButton.onClick.RemoveListener(UserInfoButtonClicked);
            _dailyQuestButton.onClick.RemoveListener(DailyQuestButtonClicked);
            _gachaButton.onClick.RemoveListener(GachaButtonClicked);
            _growthButton.onClick.RemoveListener(GrowthButtonClicked);
            _levelUpButton.onClick.RemoveListener(LevelUpButtonClicked);

            GameManager.Instance.Reward.OnStarCandyChange -= UpdateRainbowStar;
            GameManager.Instance.DailyQuest.OnStarCandyChange -= UpdateRainbowStar;
        }

        public override void Open()
        {
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

        #region Button Methods

        /// <summary>
        /// GameStartButtonClicked 핸들러 메서드 호출로 사용자가 GameStart 버튼을 클릭했을 때 StageUI를 활성화
        /// </summary>
        private void GameStartButtonClicked()
        {
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_RoguelikeScene);
            OnUICloseRequested?.Invoke(UIName.MainLobbyUI);
        }

        /// <summary>
        /// UserInfoButtonClicked 핸들러 메서드 호출로 사용자가 UserInfo 버튼을 눌렀을 때 UserInfoUI를 활성화
        /// </summary>
        private void UserInfoButtonClicked() => OnUIOpenRequested?.Invoke(UIName.UserInfoUI);

        /// <summary>
        /// DailyQuestButtonClicked 핸들러 메서드 호출로 사용자가 Quest 버튼을 눌렀을 때 DailyQuestUI를 활성화
        /// </summary>
        private void DailyQuestButtonClicked() => OnUIOpenRequested?.Invoke(UIName.DailyQuestUI);

        /// <summary>
        /// GachaButtonClicked 핸들러 메서드 호출로 사용자가 Gacha 버튼을 눌렀을 때 GachaMainUI를 활성화
        /// </summary>
        private void GachaButtonClicked() => OnUIOpenRequested?.Invoke(UIName.GachaMainUI);

        private void GrowthButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.PermanentGrowthUI);
            OnUICloseRequested?.Invoke(UIName.MainLobbyUI);
        }

        private void LevelUpButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.CharLevelUpMainUI);
            OnUICloseRequested?.Invoke(UIName.MainLobbyUI);
        }

        #endregion

        #region Update User Info

        /// <summary>
        /// UserInfoUI의 Nickname을 업데이트
        /// </summary>
        /// <param name="user">사용자의 정보</param>
        public void UpdateUserInfo(UserInfo user)
        {
            _nicknameText.text = user.Nickname;
            _iconNumber = user.IconNumber;
            OnIconRequested?.Invoke(_iconNumber);
            // StartCoroutine(DelayedInvoke(user.IconNumber));
        }

        // private IEnumerator DelayedInvoke(int iconNumber)
        // {
        //     yield return new WaitForEndOfFrame();
        //
        //     while (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected)
        //     {
        //         yield return new WaitForSeconds(0.1f);
        //     }
        //     OnIconRequested?.Invoke(iconNumber);
        // }

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
    }
}