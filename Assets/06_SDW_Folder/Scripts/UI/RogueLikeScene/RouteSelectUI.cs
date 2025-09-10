using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SDW
{
    public class RouteSelectUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _partyButton;
        private TextMeshProUGUI _partyButtonText;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
            _partyButtonText = _partyButton.GetComponentInChildren<TextMeshProUGUI>();
            _partyButtonText.text = "파티 편성";
        }

        private void OnEnable()
        {
            _partyButton.onClick.AddListener(PartyButtonClicked);
        }

        private void OnDisable()
        {
            _partyButton.onClick.RemoveListener(PartyButtonClicked);
        }

        private TweenAnimation _tweenAnimation;

        public override void Open()
        {
            base.Open();
            _tweenAnimation.moveBack();
        }

        public override void Close()
        {
            _tweenAnimation.moveAway();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void PartyButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.PartyUI);
            OnUICloseRequested?.Invoke(UIName.RouteSelectUI);
        }
    }
}