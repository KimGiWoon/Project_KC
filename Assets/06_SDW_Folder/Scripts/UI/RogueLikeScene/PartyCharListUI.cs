using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class PartyCharListUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private GameObject _scrollContents;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _partyWarningPanel;
        [SerializeField] private GameObject _backgroundObject;
        private TweenAnimation _tweenAnimation;
        private TeamFormationManager _teamManager;

        public Action<UIName, CharacterDataSO> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
            _infoButton.onClick.AddListener(InfoButtonClicked);
            _closeButton.onClick.AddListener(CloseButtonClicked);
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
            _infoButton.onClick.RemoveListener(InfoButtonClicked);
            _closeButton.onClick.RemoveListener(CloseButtonClicked);
        }

        public override void Open()
        {
            _backgroundObject.SetActive(true);
            _teamManager = TeamFormationManager.Instance;
            _teamManager.Initialize();
            base.Open();
            _tweenAnimation.moveAway();
        }

        public override void Close()
        {
            _tweenAnimation.moveBack();
            _teamManager.ClearPrevFinalTeamSlots();
            StartCoroutine(DelayedClose());
            _backgroundObject.SetActive(false);
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void ConfirmButtonClicked()
        {
            //todo 현재 편성 정보 적용
            if (_teamManager.OnConfirm())
            {
                OnUICloseRequested?.Invoke(UIName.PartyCharListUI);
            }
        }

        private void InfoButtonClicked()
        {
            var characterData = _teamManager.GetLastSelectedCharacterInfo();
            if (characterData == null) return;
            OnUIOpenRequested?.Invoke(UIName.CharInfoUI, characterData);
        }

        private void CloseButtonClicked()
        {
            _teamManager.OnClose();
            //todo 이전 편성 정보로 복구해야 함
            OnUICloseRequested?.Invoke(UIName.PartyCharListUI);
        }

        public void ShowWarningPopup() => _partyWarningPanel.SetActive(true);
    }
}