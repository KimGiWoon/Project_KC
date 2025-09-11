using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SDW;

namespace CJH
{
    public class EventStart : MonoBehaviour
    {
        [Header("프리팹 내부 UI 연결")]
        public TextMeshProUGUI eventTitleText; // 이벤트 종류 텍스트 (예: 긍정적 사건)
        public Image eventImage;
        public TextMeshProUGUI encounterText;
        public Transform buttonContainer;
        public GameObject resultPanel;
        public TextMeshProUGUI resultText;
        public GameObject choiceButtonPrefab;
        private EventManager _eventManager;

        [System.Serializable]
        public class EncounterSpriteMapping
        {
            public EncounterType typeEnum;
            public Sprite sprite;
        }

        public List<EncounterSpriteMapping> encounterSprites;

        public void Initialize(EncounterTable data)
        {
            _eventManager = FindObjectOfType<EventManager>();

            // --- 이벤트 종류에 따라 제목 텍스트 설정 ---
            if (eventTitleText != null)
            {
                switch (data.Type)
                {
                    case EncounterType.Money:
                        eventTitleText.text = "긍정적 사건 발생!";
                        break;
                    case EncounterType.Relic:
                        eventTitleText.text = "긍정적 사건 발생!";
                        break;
                    case EncounterType.RelicSel:
                        eventTitleText.text = "긍정적 사건 발생!";
                        break;
                    case EncounterType.Luck:
                        eventTitleText.text = "긍정적 사건 발생!";
                        break;

                    case EncounterType.MoneyFight:
                        eventTitleText.text = "부정적 사건 발생!";
                        break;
                    case EncounterType.BadRelic:
                        eventTitleText.text = "부정적 사건 발생!";
                        break;
                    case EncounterType.RelicDel:
                        eventTitleText.text = "부정적 사건 발생!";
                        break;
                    default:
                        eventTitleText.text = "사건 발생!";
                        break;
                }
            }

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
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < data.ChoiceCount; i++)
            {
                var buttonObj = Instantiate(choiceButtonPrefab, buttonContainer);
                int choiceIndex = i;

                var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && choiceIndex < data.ChoiceTexts.Count)
                {
                    if (string.IsNullOrEmpty(data.ChoiceTexts[choiceIndex]) || data.ChoiceTexts[choiceIndex].ToLower() == "null")
                    {
                        buttonText.text = "확인";
                    }
                    else
                    {
                        buttonText.text = data.ChoiceTexts[choiceIndex];
                    }
                }

                var button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => OnChoiceSelected(data, choiceIndex));
            }

            resultPanel.SetActive(false);
        }

        private void OnChoiceSelected(EncounterTable data, int choiceIndex)
        {
            if (choiceIndex < data.EncounterExitText.Count && !string.IsNullOrEmpty(data.EncounterExitText[choiceIndex]))
            {
                resultPanel.SetActive(true);
                resultText.text = data.EncounterExitText[choiceIndex];

                // 결과창이 표시될 때 기존 UI 요소들을 비활성화합니다.
                if (eventTitleText != null) eventTitleText.gameObject.SetActive(false);
                eventImage.gameObject.SetActive(false);
                encounterText.gameObject.SetActive(false);
                buttonContainer.gameObject.SetActive(false);
            }
            else
            {
                _eventManager.EndEncounter();
            }

            foreach (var btn in buttonContainer.GetComponentsInChildren<Button>())
            {
                btn.interactable = false;
            }
        }
    }
}