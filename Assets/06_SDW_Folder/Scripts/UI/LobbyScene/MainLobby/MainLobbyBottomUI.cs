using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class MainLobbyBottomUI : BaseUI
    {
        [Header("Top Component")]
        [Header("Bottom Button Components")]
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private Button _dailyQuestButton;
        [SerializeField] private Button _lobbyButton;
        [SerializeField] private Button _collectionButton;
        [SerializeField] private Button _gachaButton;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _bottomButtonsTweenAnimation;
        [SerializeField] private float delayTime = 1.35f;

        public Action<UIName, string> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        private void OnEnable()
        {
            _dailyQuestButton.onClick.AddListener(DailyQuestButtonClicked);
            _gachaButton.onClick.AddListener(GachaButtonClicked);
            _collectionButton.onClick.AddListener(CollectionButtonClicked);
            _levelUpButton.onClick.AddListener(LevelUpButtonClicked);
            GameManager.Instance.OnCanGachaChanged += SetGachaButton;
        }

        private void OnDisable()
        {
            _dailyQuestButton.onClick.RemoveListener(DailyQuestButtonClicked);
            _gachaButton.onClick.RemoveListener(GachaButtonClicked);
            _collectionButton.onClick.RemoveListener(CollectionButtonClicked);
            _levelUpButton.onClick.RemoveListener(LevelUpButtonClicked);
            GameManager.Instance.OnCanGachaChanged -= SetGachaButton;
        }

        public override void Open()
        {
            StartCoroutine(DelayedCheck());
            base.Open();
        }

        private IEnumerator DelayedCheck()
        {
            yield return new WaitForSeconds(0.5f);
            if (GameManager.Instance.CanGacha) _gachaButton.interactable = true;
            else _gachaButton.interactable = false;
        }

        /// <summary>
        /// DailyQuestButtonClicked 핸들러 메서드 호출로 사용자가 Quest 버튼을 눌렀을 때 DailyQuestUI를 활성화
        /// </summary>
        private void DailyQuestButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.DailyQuestUI, "일일 퀘스트");
        }

        /// <summary>
        /// GachaButtonClicked 핸들러 메서드 호출로 사용자가 Gacha 버튼을 눌렀을 때 GachaMainUI를 활성화
        /// </summary>
        private void GachaButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.GachaMainUI, "미식가 초대");
        }

        private void CollectionButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.StoryCollectionUI, "미식가의 추억");
        }

        private void LevelUpButtonClicked()
        {
            SetButtonsInteractable(false);

            DOVirtual.DelayedCall(delayTime, () => { SetButtonsInteractable(true); });

            OnUIOpenRequested?.Invoke(UIName.CharLevelUpMainUI, "미식가");
            OnUICloseRequested?.Invoke(UIName.MainLobbyUI);
        }

        public void ButtonsMoveAway()
        {
            _bottomButtonsTweenAnimation.moveAway();
        }

        public void ButtonsMoveBack()
        {
            _bottomButtonsTweenAnimation.moveBack();
        }

        public void SetButtonsInteractable(bool value)
        {
            _levelUpButton.interactable = value;
            _dailyQuestButton.interactable = value;
            _lobbyButton.interactable = value;
            _collectionButton.interactable = value;
            if (GameManager.Instance.CanGacha) _gachaButton.interactable = value;
            else _gachaButton.interactable = false;
        }

        private void SetGachaButton(bool canGacha) => _gachaButton.interactable = canGacha;
    }
}