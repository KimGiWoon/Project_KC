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
        public TextMeshProUGUI eventTitleText;
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
            public EncounterSentiment Sentiment;
            public Sprite sprite;
        }

        public List<EncounterSpriteMapping> encounterSprites;

        private static int gambleCount = 0;

        public void Initialize(EncounterTable data)
        {
            _eventManager = FindObjectOfType<EventManager>();

            if (eventTitleText != null)
            {
                switch (data.Sentiment)
                {
                    case EncounterSentiment.Good:
                        eventTitleText.text = "긍정적 사건 발생!";
                        break;
                    case EncounterSentiment.Bad:
                        eventTitleText.text = "부정적 사건 발생!";
                        break;
                    default:
                        eventTitleText.text = "사건 발생!";
                        break;
                }
            }

            var mapping = encounterSprites.Find(m => m.Sentiment == data.Sentiment);
            if (mapping != null && eventImage != null)
            {
                eventImage.sprite = mapping.sprite;
                eventImage.gameObject.SetActive(true);
            }
            else if (eventImage != null)
            {
                eventImage.gameObject.SetActive(false);
            }

            encounterText.text = data.EncounterText.Replace("\\n", " ");


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
            ChoiceResultType resultType = ChoiceResultType.None; // 기본값
            int resultValue = (choiceIndex < data.ChoiceResultValues.Count) ? data.ChoiceResultValues[choiceIndex] : 0;

            if(data.Type == EncounterType.Gamb)
            {
                if(choiceIndex == 0)
                {
                    gambleCount = 0;
                }

                if(choiceIndex == 1)
                {
                    _eventManager.EndEncounter();
                    return;
                }

                gambleCount++;

                if(gambleCount == 2)
                {
                    //todo 겜블 3번째 때 동작 확인 후 작성
                }
            }

            switch (data.Type)
            {
                case EncounterType.MoneySpend:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.LoseYeopjeon; // 첫 번째 선택은 무조건 전투
                    }
                    else
                    {
                        resultType = ChoiceResultType.None; // 두 번째 선택은 무조건 아무것도 안 함
                    }
                    break;

                case EncounterType.RelicSpent:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.LoseRelic; // 첫 번째 선택은 아이템 잃기
                    }
                    else
                    {
                        resultType = ChoiceResultType.None; // 두 번째 선택은 아무것도 안 함
                    }
                    break;

                case EncounterType.FightSel:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.Combat; // 첫 번째 선택은 무조건 전투
                    }
                    else
                    {
                        resultType = ChoiceResultType.None; // 두 번째 선택은 무조건 아무것도 안 함
                    }
                    break;

                case EncounterType.MoneyFight:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.LoseYeopjeon; // 첫 번째 선택은 돈 잃기
                    }
                    else
                    {
                        resultType = ChoiceResultType.Combat; // 두 번째 선택은 전투
                    }
                    break;


                default:
                    resultType = ChoiceResultType.None;
                    break;
            }
            //todo 이벤트 버튼 분기 시 동작 연결 필요
            switch (resultType)
            {
                case ChoiceResultType.Combat:
                    Debug.Log("전투");
                    break;
                case ChoiceResultType.LoseYeopjeon:
                    Debug.Log("엽전 잃음");
                    break;
                case ChoiceResultType.LoseRelic:
                    Debug.Log("유물 잃음");
                    break;
                case ChoiceResultType.None:
                    Debug.Log(" 이벤트 지나감 ");
                    break;
            }

            // 결과가 있으면 결과창 보여주고 없으면 이벤트 종료
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

            foreach (var btn in buttonContainer.GetComponentsInChildren<Button>())
            {
                btn.interactable = false;
            }
        }
    }
}