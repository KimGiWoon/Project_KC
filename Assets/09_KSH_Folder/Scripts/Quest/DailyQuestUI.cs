using System;
using KSH;
using UnityEngine;
using SDW;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class DailyQuestUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private Button _rewardButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private RectTransform _popupRect;

    [Header("Test Buttons")]
    [SerializeField] private Button _q1button;
    [SerializeField] private Button _q2button;
    [SerializeField] private Button _q3button;
    [SerializeField] private Button _q4button;
    [SerializeField] private Button _q5button;
    [SerializeField] private Button _q6button;
    [SerializeField] private Button _q7button;

    public Action OnRewardButtonClicked;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _rewardButton.interactable = false;
    }

    private void OnEnable()
    {
        _rewardButton.onClick.AddListener(RewardButtonClicked);
        _backButton.onClick.AddListener(BackButtonClicked);

        //# Test Code
        _q1button.onClick.AddListener(Q1buttonClicked);
        _q2button.onClick.AddListener(Q2buttonClicked);
        _q3button.onClick.AddListener(Q3buttonClicked);
        _q4button.onClick.AddListener(Q4buttonClicked);
        _q5button.onClick.AddListener(Q5buttonClicked);
        _q6button.onClick.AddListener(Q6buttonClicked);
        _q7button.onClick.AddListener(Q7buttonClicked);
    }

    private void OnDisable()
    {
        _rewardButton.onClick.RemoveListener(RewardButtonClicked);
        _backButton.onClick.RemoveListener(BackButtonClicked);

        //# Test Code
        _q1button.onClick.RemoveListener(Q1buttonClicked);
        _q2button.onClick.RemoveListener(Q2buttonClicked);
        _q3button.onClick.RemoveListener(Q3buttonClicked);
        _q4button.onClick.RemoveListener(Q4buttonClicked);
        _q5button.onClick.RemoveListener(Q5buttonClicked);
        _q6button.onClick.RemoveListener(Q6buttonClicked);
        _q7button.onClick.RemoveListener(Q7buttonClicked);
    }

    private void Update()
    {
        if (_panelContainer.activeSelf)
        {
            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (!RectTransformUtility.RectangleContainsScreenPoint(_popupRect, touchPos))
                {
                    OnUICloseRequested?.Invoke(UIName.DailyQuestUI);
                }
            }
        }

        if (!GameManager.Instance.DailyQuest.CanReward())
        {
            if (_rewardButton.interactable)
                _rewardButton.interactable = false;
            return;
        }

        if (!_rewardButton.interactable)
            _rewardButton.interactable = true;
    }

    private void RewardButtonClicked() => OnRewardButtonClicked?.Invoke();

    private void BackButtonClicked() => OnUICloseRequested?.Invoke(UIName.DailyQuestUI);
    private void Q1buttonClicked() => GameManager.Instance.DailyQuest.CompleteQuest(QuestType.ChallengeDungeon, 1);
    private void Q2buttonClicked() => GameManager.Instance.DailyQuest.CompleteQuest(QuestType.UseFood, 3);
    private void Q3buttonClicked() => GameManager.Instance.DailyQuest.CompleteQuest(QuestType.GetArtifact, 5);
    private void Q4buttonClicked() => GameManager.Instance.DailyQuest.CompleteQuest(QuestType.UseStemina, 100);
    private void Q5buttonClicked() => GameManager.Instance.DailyQuest.CompleteQuest(QuestType.ChargeStemina, 1);
    private void Q6buttonClicked() => GameManager.Instance.DailyQuest.CompleteQuest(QuestType.LevelUp, 1);
    private void Q7buttonClicked() => GameManager.Instance.DailyQuest.CompleteQuest(QuestType.SkillLevelUp, 1);
}