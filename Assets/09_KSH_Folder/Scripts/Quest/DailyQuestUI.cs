using System;
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
}