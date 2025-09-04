using System;
using KSH;
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

        [SerializeField] private Image _userIcon;
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private TextMeshProUGUI _cashStarText;
        [SerializeField] private TextMeshProUGUI _rainbowStarText;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
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

            UpdateRainbowStar(GameManager.Instance.RainbowStarCandy);
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
            GameManager.Instance.Reward.OnStarCandyChange -= UpdateRainbowStar;
            GameManager.Instance.DailyQuest.OnStarCandyChange -= UpdateRainbowStar;
        }

        #region Button Methods

        /// <summary>
        /// GameStartButtonClicked 핸들러 메서드 호출로 사용자가 GameStart 버튼을 클릭했을 때 StageUI를 활성화
        /// </summary>
        private void GameStartButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.KGW_StageSelectUI);
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

        #endregion

        #region Update User Info

        /// <summary>
        /// UserInfoUI의 Nickname을 업데이트
        /// </summary>
        /// <param name="email">사용자의 이메일 주소</param>
        /// <param name="nickname">업데이트할 사용자의 닉네임</param>
        public void UpdateUserInfo(string nickname, string email = null, string uid = null) => _nicknameText.text = nickname;

        /// <summary>
        /// Icon을 설정하기 위한 메서드
        /// </summary>
        /// <param name="sprite">설정할 Icon</param>
        public void SetIcon(Sprite sprite) => _userIcon.sprite = sprite;

        #endregion

        /// <summary>
        /// RainbowStar의 정보를 가져와서 Text 업데이트
        /// </summary>
        /// <param name="numOfStars">업데이트할 Rainbow Start 수</param>
        private void UpdateRainbowStar(int numOfStars)
        {
            _rainbowStarText.text = numOfStars.ToString();
        }
    }
}