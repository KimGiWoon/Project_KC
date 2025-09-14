using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SDW;
using JJY;

namespace CJH
{
    public class EventStart : MonoBehaviour
    {
        [Header("프리팹 내부 UI 연결")]
        public TextMeshProUGUI eventTitleText;
        public Image eventImage;
        public TextMeshProUGUI encounterText;
        public Transform buttonContainer;

        public GameObject resultPanel;
        public TextMeshProUGUI resultText;

        public GameObject choiceButtonPrefab;

        private EventManager _eventManager;

        public class EncounterSpriteMapping
        {
            public EncounterType typeEnum;
            public Sprite sprite;
        }

        public List<EncounterSpriteMapping> encounterSprites;

  

        public void Initialize(EncounterTable data)
        {
            Debug.LogWarning(">>>>> 이 로그를 실행하는 오브젝트: " + this.gameObject.name, this.gameObject);
            Debug.LogWarning($"--- EventStart 데이터 수신 ---");
            Debug.Log($"ID: {data.EncounterID}, 타입: {data.Type}");
            Debug.Log($"내용: '{data.EncounterText}'");
            Debug.Log($"선택지 개수: {data.ChoiceCount}");
            Debug.LogWarning($"--------------------------");
            _eventManager = FindObjectOfType<EventManager>();

            SetTitleByType(data.Type);

            var mapping = encounterSprites.Find(m => m.typeEnum == data.Type);
            if (mapping != null && eventImage != null)
            {
                eventImage.sprite = mapping.sprite;
                eventImage.gameObject.SetActive(true);
            }
            else if (eventImage != null)
            {
                eventImage.gameObject.SetActive(false);
            }

            encounterText.text = data.EncounterText;

            foreach (Transform child in buttonContainer)
                Destroy(child.gameObject);

            for (int i = 0; i < data.ChoiceCount; i++)
            {
                var buttonObj = Instantiate(choiceButtonPrefab, buttonContainer);
                int choiceIndex = i;

                var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && choiceIndex < data.ChoiceTexts.Count)
                    buttonText.text = string.IsNullOrEmpty(data.ChoiceTexts[choiceIndex]) ? "확인" : data.ChoiceTexts[choiceIndex];

                var button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => OnChoiceSelected(data, choiceIndex));
            }

            resultPanel.SetActive(false);
        }

        private void OnChoiceSelected(EncounterTable data, int choiceIndex)
        {
            foreach (var btn in buttonContainer.GetComponentsInChildren<Button>())
                btn.interactable = false;

            if (EventBranches.Map.ContainsKey((data.EncounterID, choiceIndex)))
            {
                Debug.Log($"[Branch Detected] ID: {data.EncounterID}, Choice: {choiceIndex}");
                var branch = EventBranches.Map[(data.EncounterID, choiceIndex)];
                Initialize(branch);
                return;
            }

            if (choiceIndex < data.EncounterExitText.Count && !string.IsNullOrEmpty(data.EncounterExitText[choiceIndex]))
            {
                resultPanel.SetActive(true);
                resultText.text = data.EncounterExitText[choiceIndex];

                if (eventTitleText != null) eventTitleText.gameObject.SetActive(false);
                eventImage.gameObject.SetActive(false);
                encounterText.gameObject.SetActive(false);
                buttonContainer.gameObject.SetActive(false);
            }
            else
            {
                _eventManager.EndEncounter();
            }
        }

        //코드 기반 분기 이벤트용
        public void Initialize(EventBranchData branch)
        {
            _eventManager = FindObjectOfType<EventManager>();

            SetTitleByType(branch.Type ?? EncounterType.None);
            eventImage.gameObject.SetActive(false); // 분기 이벤트는 이미지 없음

            encounterText.text = branch.Text;

            foreach (Transform child in buttonContainer)
                Destroy(child.gameObject);

            for (int i = 0; i < branch.Choices.Count; i++)
            {
                var buttonObj = Instantiate(choiceButtonPrefab, buttonContainer);
                int choiceIndex = i;

                var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)

                    buttonText.text = branch.Choices[choiceIndex];
                var button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => OnBranchChoiceSelected(branch, choiceIndex));
            }

            resultPanel.SetActive(false);
        }

        private void OnBranchChoiceSelected(EventBranchData branch, int index)
        {
            foreach (var btn in buttonContainer.GetComponentsInChildren<Button>())
                btn.interactable = false;

            if (branch.Rewards != null && index < branch.Rewards.Count)
                ApplyRewards(branch.Rewards[index]);

            resultPanel.SetActive(true);
            resultText.text = branch.Results[index];

            if (eventTitleText != null) eventTitleText.gameObject.SetActive(false);
            eventImage.gameObject.SetActive(false);
            encounterText.gameObject.SetActive(false);
            buttonContainer.gameObject.SetActive(false);
        }

        private void SetTitleByType(EncounterType type)
        {
            Debug.Log($"SetTitleByType called with type: {type}");

            if (eventTitleText == null) return;

            switch (type)
            {
                case EncounterType.Money:
                case EncounterType.Relic:
                case EncounterType.RelicSel:
                case EncounterType.Luck:
                    eventTitleText.text = "긍정적 사건 발생!";
                    break;

                case EncounterType.MoneyFight:
                case EncounterType.BadRelic:
                case EncounterType.RelicDel:
                    eventTitleText.text = "부정적 사건 발생!";
                    break;

                default:
                    eventTitleText.text = "사건 발생!";
                    break;
            }
        }

        private void ApplyRewards(List<BranchReward> rewards)
        {
            var coinManager = FindObjectOfType<CoinManager>();
            //todo 렐릭 연동 코드에 맞게 수정
            //var relicManager = FindObjectOfType<RelicManager>();

            foreach (var reward in rewards)
            {
                switch (reward.Type)
                {
                    case BranchReward.RewardType.Coin:
                        if (coinManager != null)
                            coinManager.AddYeopjeon(reward.Amount);
                        else
                            Debug.LogError("CoinManager 인스턴스를 찾을 수 없습니다");
                        break;

                   //case BranchReward.RewardType.BuffRelic:
                   //    if (relicManager != null)
                   //        relicManager.GiveRandomRelic(true);
                   //    break;
                   //
                   //case BranchReward.RewardType.DebuffRelic:
                   //    if (relicManager != null)
                   //        relicManager.GiveRandomRelic(false);
                   //    break;
                }
            }
        }
    }
}