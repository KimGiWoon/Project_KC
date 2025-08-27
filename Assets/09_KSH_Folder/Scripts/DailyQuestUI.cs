using System;
using UnityEngine;
using SDW;
using UnityEngine.UI;

public class DailyQuestUI : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private Button _rewardButton;
    [SerializeField] private Button _backButton;

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
    }

    private void OnDisable()
    {
        _rewardButton.onClick.RemoveListener(RewardButtonClicked);
        _backButton.onClick.RemoveListener(BackButtonClicked);
    }

    private void Update()
    {
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
}