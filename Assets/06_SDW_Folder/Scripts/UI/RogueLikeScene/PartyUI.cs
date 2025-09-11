using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CJH;

namespace SDW
{
    public class PartyUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private GameObject _selectedCharContainer;
        [SerializeField] private Button _charChangeButton;
        [SerializeField] private Button _enterStageButton;
        [SerializeField] private Button _moveButton;
        [SerializeField] private MapView _mapView;
        [SerializeField] private GameObject _battleBackgroundObject;
        private TweenAnimation _tweenAnimation;
        private TweenAnimation[] _selectedCharTweens;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        private BattleEventType _eventType;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
            _enterStageButton.interactable = false;
            _battleBackgroundObject.SetActive(false);
            //todo 추후 선택된 Character 추가 시 TweenAnimation을 설정해야 함
            _selectedCharTweens = _selectedCharContainer.GetComponentsInChildren<TweenAnimation>();
        }

        private void OnEnable()
        {
            _charChangeButton.onClick.AddListener(CharChangeButtonClicked);
            _enterStageButton.onClick.AddListener(EnterStageButtonClicked);
            _moveButton.onClick.AddListener(MoveButtonClicked);
            _mapView.OnCharacterMoved += CharacterMoved;
            _mapView.OnEventTypeChanged += SetEventType;
            RoguelikeManager.Instance.OnBattleStart += () =>
            {
                OnUIOpenRequested?.Invoke(UIName.BattleUI);
                OnUICloseRequested.Invoke(UIName.PartyUI);
            };

            RoguelikeManager.Instance.OnBattleEnd += BattleEnd;
        }

        private void OnDisable()
        {
            _charChangeButton.onClick.RemoveListener(CharChangeButtonClicked);
            _enterStageButton.onClick.RemoveListener(EnterStageButtonClicked);
            _moveButton.onClick.RemoveListener(MoveButtonClicked);
            _mapView.OnCharacterMoved -= CharacterMoved;
            _mapView.OnEventTypeChanged -= SetEventType;
            RoguelikeManager.Instance.OnBattleStart -= () =>
            {
                OnUIOpenRequested?.Invoke(UIName.BattleUI);
                OnUICloseRequested.Invoke(UIName.PartyUI);
            };
            RoguelikeManager.Instance.OnBattleEnd -= BattleEnd;
        }

        public override void Open()
        {
            base.Open();
            _tweenAnimation.moveAway();
        }

        public override void Close()
        {
            _tweenAnimation.moveBack();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            base.Close();
        }

        private void CharChangeButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.PartyCharListUI);
        }

        private void EnterStageButtonClicked()
        {
            RoguelikeManager.Instance.SetBattleEventType(_eventType);
            RoguelikeManager.Instance.OnBattleStart?.Invoke();
            _enterStageButton.interactable = false;
        }

        private void MoveButtonClicked()
        {
            OnUIOpenRequested(UIName.RouteSelectUI);
            OnUICloseRequested(UIName.PartyUI);
        }

        public void UISecondPositionMoveAway()
        {
            _charChangeButton.gameObject.SetActive(false);
            _enterStageButton.gameObject.SetActive(false);
            _moveButton.gameObject.SetActive(false);

            foreach (var charTween in _selectedCharTweens)
            {
                charTween.moveAway();
            }

            _tweenAnimation.moveAway2();
        }

        public void UIMoveBack()
        {
            _charChangeButton.gameObject.SetActive(true);
            _enterStageButton.gameObject.SetActive(true);
            _moveButton.gameObject.SetActive(true);

            foreach (var charTween in _selectedCharTweens)
            {
                charTween.moveBack();
            }

            _tweenAnimation.moveAway();
        }

        private void CharacterMoved(bool isBattle)
        {
            _battleBackgroundObject.SetActive(isBattle);
            _enterStageButton.interactable = isBattle;
            _moveButton.interactable = !isBattle;
        }

        private void SetEventType(BattleEventType type) => _eventType = type;

        private void BattleEnd()
        {
            _battleBackgroundObject.SetActive(false);
            _moveButton.interactable = true;
            _enterStageButton.interactable = false;
        }
    }
}