using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

namespace KSH
{
    public class GachaMainUI : BaseUI
    {
        [Header("CharacterGacha")]
        [SerializeField] private CharacterGacha gacha;
        [Header("버튼")]
        [SerializeField] private Button singleButton; //1회 뽑기 버튼
        [SerializeField] private Button multipleButton; //10회 뽑기 버튼
        [SerializeField] private Button _backButton;
        [Header("메인UI")]
        [SerializeField] private GameObject GatchaUI; //메인 UI
        [Header("별사탕 UI")]
        [SerializeField] private TextMeshProUGUI starCandyText;

        public int starCandyCount { get; private set; }

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            gacha = GameManager.Instance.Gacha;

            singleButton.onClick.AddListener(() =>
            {
                if (GameManager.Instance.Reward.StarCandy >= 150) //별사탕이 150개 이상 가지고 있으면 1회 뽑기
                {
                    GatchaUI.SetActive(false);
                    GameManager.Instance.Reward.AddStarCandy(-150);
                    gacha.SetGachaType(true);
                    OnUIOpenRequested?.Invoke(UIName.GachaResultUI);
                    OnUICloseRequested?.Invoke(UIName.GachaMainUI);
                }
                else
                {
                    Debug.Log("별사탕이 부족합니다.");
                }
            });
            multipleButton.onClick.AddListener(() =>
            {
                if (GameManager.Instance.Reward.StarCandy >= 1500) //별사탕을 1500개 이상 가지고 있으면 10회 뽑기
                {
                    GatchaUI.SetActive(false);
                    GameManager.Instance.Reward.AddStarCandy(-1500);
                    gacha.SetGachaType(false);
                    OnUIOpenRequested?.Invoke(UIName.GachaResultUI);
                    OnUICloseRequested?.Invoke(UIName.GachaMainUI);
                }
                else
                {
                    Debug.Log("별사탕이 부족합니다.");
                }
            });

            CandyUpdate(GameManager.Instance.Reward.StarCandy);
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Reward.OnStarCandyChange -= CandyUpdate;
            _backButton.onClick.RemoveListener(BackButtonClicked);
        }

        public override void Open()
        {
            base.Open();
            Initialize();
        }

        private void Initialize()
        {
            GameManager.Instance.Reward.OnStarCandyChange += CandyUpdate;
            CandyUpdate(GameManager.Instance.RainbowStarCandy);
            _backButton.onClick.AddListener(BackButtonClicked);
        }

        private void BackButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.MainLobbyUI);
            OnUICloseRequested?.Invoke(UIName.GachaMainUI);
        }

        private void CandyUpdate(int value)
        {
            starCandyText.text = value.ToString();
        }
    }
}