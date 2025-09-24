using System;
using CJH;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class ClearIncidentBattleUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _confirmButton; // 랜덤 인카운터로 이동 버튼
        [SerializeField] private TextMeshProUGUI _yeopjeonText;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
        }

        public override void Open()
        {
            // 이벤트 매니저에 저장된 획득한 엽전 출력
            _yeopjeonText.text = $"{MapView.Instance._eventManager.CombatRewardAmount} 엽전을 획득하였습니다.";
            base.Open();
        }

        private void ConfirmButtonClicked()
        {
            // 획득한 엽전 증가
            GameManager.Instance.Coin.AddYeopjeon(MapView.Instance._eventManager.CombatRewardAmount);
            // 전투 끝 
            RoguelikeManager.Instance.OnBattleEnd?.Invoke();
            // UI 닫기
            //OnUICloseRequested?.Invoke(UIName.ClearIncidentBattleUI);
        }
    }
}

