using System;
using System.Collections.Generic;
using CJH;
using KSH;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

// 스테이지를 클리어하여 보상을 선택하는 UI
namespace SDW
{
    public class ClearStageUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _confirmButton; // 랜덤 인카운터로 이동 버튼
        [SerializeField] private TextMeshProUGUI _yeopjeonText;
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

        public override void Open()
        {
            _yeopjeonText.text = "100 엽전을 획득하였습니다.";
            base.Open();
            _confirmButton.interactable = false;

            // 보스 클리어 시
            if (RoguelikeManager.Instance.MonsterType == BattleEventType.Boss ||
                RoguelikeManager.Instance.MonsterType == BattleEventType.BossFinal)
            {
                Debug.Log("보스 클리어");
                // 다음 스테이지 이동
                NextStage();
            }
        }

        private void ConfirmButtonClicked()
        {
            GetRelic();
            GameManager.Instance.Coin.AddYeopjeon(100);
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

        // 다음 스테이지 이동
        public void NextStage()
        {
            if (RoguelikeManager.Instance.MonsterType == BattleEventType.Boss)
            {
                // 스테이지 클리어
                GameManager.Instance._isStageClear = true;
                // 스테이지 증가
                GameManager.Instance.AddStageCount();

                Debug.Log($"보스 클리어 후 {GameManager.Instance.Stage} 스테이지로 이동");

                var generator = FindObjectOfType<MapGenerator>();
                var config = MapView.Instance.mapConfig;

                if (generator != null && config != null)
                {
                    var newMapData = generator.GenerateMap(config);
                    MapView.Instance.CreateMapView(newMapData);
                }
            }
        }

        private void OnClickRelic(RelicUI relicUI)
        {
            _confirmButton.interactable = true;
            currentRelicUI = relicUI;
        }
    }
}