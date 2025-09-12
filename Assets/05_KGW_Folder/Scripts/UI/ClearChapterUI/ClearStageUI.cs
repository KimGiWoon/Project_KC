using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KSH;
using TMPro;

// 스테이지를 클리어하여 보상을 선택하는 UI
namespace SDW
{
    public class ClearStageUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _confirmButton; // 랜덤 인카운터로 이동 버튼
        [SerializeField] private Transform content;
        [SerializeField] private RelicUI relicPrefab;
        [SerializeField] public GameObject RelicWindow;
        [SerializeField] private TextMeshProUGUI ignoreText;
        [SerializeField] private RelicDetailUI relicDetailUI;
        private RelicUI currentRelicUI;
        private RelicDatas relic;
        private GameObject DetailUI;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            DetailUI = relicDetailUI.gameObject;
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
        }

        private void ConfirmButtonClicked()
        {
            GetRelic();
            RoguelikeManager.Instance.OnBattleEnd?.Invoke();
            OnUICloseRequested?.Invoke(UIName.ClearStageUI);
        }

        private void GetRelic()
        {
            if (currentRelicUI == null)
            {
                ignoreText.gameObject.SetActive(true);
                return;
            }

            var relic = currentRelicUI.GetRelic();
            RelicDropManager.Instance.GetRelic(relic);
            _confirmButton.interactable = false;
            RelicWindow.SetActive(false);
        }

        public void ShowRelic(List<RelicDatas> relics, RelicGrade grade)
        {
            RelicWindow.SetActive(true);
            _confirmButton.interactable = true;
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < relics.Count; i++)
            {
                var relic = Instantiate(relicPrefab, content);
                relic.SetData(relics[i], OnClickRelic, relicDetailUI);
            }
            currentRelicUI = null;
        }

        private void OnClickRelic(RelicUI relicUI)
        {
            currentRelicUI = relicUI;
        }
    }
}