using System;
using System.Collections.Generic;
using CJH;
using KSH;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI _selectText;
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
            DetailUI.SetActive(false);
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
            GameManager.Instance.Coin.OnYeopjeonBonus += UpdateYeopjeonText;
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
            GameManager.Instance.Coin.OnYeopjeonBonus -= UpdateYeopjeonText;
        }

        private void UpdateYeopjeonText(int yeopjeon)
        {
            _yeopjeonText.text = $"{GameManager.Instance.Coin.bonus} 엽전을 획득하였습니다.";
        }

        public override void Open()
        {
            base.Open();

            // 승리 사운드 플레이
            GameManager.Instance.Audio.Play2DSFX(AudioClipName.BattleVictory);

            if (MapView.Instance._eventManager.IsCombatRewardYeopjeon)
            {
                int getYeopjeon = MapView.Instance._eventManager.CombatRewardAmount;

                _selectText.text = "전투에서 승리했습니다!";
                _confirmButton.interactable = true;
                content.gameObject.SetActive(false);
                GameManager.Instance.Coin.AddYeopjeon(getYeopjeon);
                UpdateYeopjeonText(GameManager.Instance.Coin.bonus);
                MapView.Instance._eventManager.SetCombatReward(false, 0);
            }
            else
            {
                _confirmButton.interactable = false;
                GameManager.Instance.Coin.OnRelicChanged?.Invoke();
                GameManager.Instance.Coin.AddYeopjeon(100);
                UpdateYeopjeonText(GameManager.Instance.Coin.bonus);
                GameManager.Instance.DailyQuest.CompleteQuestInRoguelikeScene(QuestType.RoguelikeClear, 1); //클리어시 퀘스트 완료
                GameManager.Instance.Firebase.SetQuestState(QuestType.RoguelikeClear, true, 1);
            }

            // 보스 클리어 시
            if (RoguelikeManager.Instance.MonsterType == BattleEventType.Boss ||
                RoguelikeManager.Instance.MonsterType == BattleEventType.BossFinal)
            {
                Debug.Log("보스 클리어");
                // 다음 스테이지 이동
                NextStage();
            }
        }

        public override void Close()
        {
            DetailUI.SetActive(false);
            base.Close();
        }

        private void ConfirmButtonClicked()
        {
            if (!MapView.Instance._eventManager.IsCombatRewardYeopjeon) GetRelic();

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
                GameManager.Instance.NextStage();
                //todo 스테이지가 완료되었을 때, 초기화 필요?
                // GameManager.Instance.InGameItem.ClearItemCounts();
                // GameManager.Instance.InGameItem.foodInventory.Clear();
                // GameManager.Instance.InGameItem.relicInventory.Clear();

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