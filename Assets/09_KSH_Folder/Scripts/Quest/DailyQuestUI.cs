using System;
using System.Collections;
using KSH;
using UnityEngine;
using SDW;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class DailyQuestUI : BaseUI
{
    [Header("Daily Quest Reward")]
    [SerializeField] private int _rewardAmount = 100;

    [Header("UI Components")]
    [SerializeField] private Button _rewardButton;
    [SerializeField] private GameObject _redmardk;
    [SerializeField] private RectTransform _popupRect;
    [SerializeField] private GameObject _backgroundPanelObject;
    private TweenAlpha_Image _backgroundPanel;

    [Header("Animation")]
    [SerializeField] private TweenAnimation _tweenAnimation;

    public Action<int> OnRewardButtonClicked;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _backgroundPanel = _backgroundPanelObject.GetComponent<TweenAlpha_Image>();
        _backgroundPanelObject.SetActive(false);
        _rewardButton.interactable = false;
    }

    private void OnEnable()
    {
        _rewardButton.onClick.AddListener(RewardButtonClicked);
    }

    private void OnDisable()
    {
        _rewardButton.onClick.RemoveListener(RewardButtonClicked);
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

    public override void Open()
    {
        _backgroundPanelObject.SetActive(true);
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
        base.Close();
        _backgroundPanelObject.SetActive(false);
    }

    private void RewardButtonClicked() => OnRewardButtonClicked?.Invoke(_rewardAmount);
}