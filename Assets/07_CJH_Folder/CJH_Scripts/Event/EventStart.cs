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

        [Header("유물 선택 UI")]
        public GameObject relicSelectionPanel;      // 유물 선택 화면 전체 패널
        public Transform relicChoiceContainer;      // 유물 선택 버튼들이 생성될 부모 Transform
        public GameObject relicChoiceButtonPrefab;  // 유물 선택 버튼의 프리팹

        public void Initialize(EncounterTable data)
        {
            _eventManager = FindObjectOfType<EventManager>();

            if (relicSelectionPanel != null)
            {
                relicSelectionPanel.SetActive(false);
            }

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

                case EncounterType.Money:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.GainYeopjeon; // 첫 번째 선택은 돈 얻기
                    }
                    else
                    {
                        resultType = ChoiceResultType.None;
                    }
                    break;

                case EncounterType.Relic:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.GainYeopjeon; // 첫 번째 선택은 돈 얻기
                    }
                    else
                    {
                        resultType = ChoiceResultType.None;
                    }
                    break;

                case EncounterType.RelicSel:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.GainRelic; // 첫 번째 유물
                    }
                    if (choiceIndex == 1)
                    {
                        resultType = ChoiceResultType.GainRelic; // 두 번째 유물
                    }
                    else
                    {
                        resultType = ChoiceResultType.GainRelic; // 세 번째 유물
                    }
                    break;

                case EncounterType.Luck:
                    if (choiceIndex == 0)
                    {
                        //todo 유물 두 개 선택 작성
                        resultType = ChoiceResultType.Continue; // 확인 후 유물
                    }
                    else
                    {
                        resultType = ChoiceResultType.None;
                    }
                    break;

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

                case EncounterType.BadRelic:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.GainBadRelic; // 첫 번째 선택은 디버프 유물 얻기
                    }
                    else
                    {
                        resultType = ChoiceResultType.None; // 두 번째 선택은 무조건 아무것도 안 함
                    }
                    break;

                case EncounterType.RelicDel:
                    if (choiceIndex == 0)
                    {
                        resultType = ChoiceResultType.LoseRelic; // 첫 번째 선택은 유물 잃기
                    }
                    else
                    {
                        resultType = ChoiceResultType.None; // 두 번째 선택은 무조건 아무것도 안 함
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
                    GameManager.Instance.Coin.SubtractYeopjeon(resultValue); // resultValue 변수 사용
                    Debug.Log($"{resultValue} 엽전 잃음");
                    break;
                case ChoiceResultType.LoseRelic:
                    // GameManager를 통해 InGameItemManager의 새 함수를 호출합니다.
                    RelicDatas lostRelic = GameManager.Instance.InGameItem.RemoveRandomRelic();

                    // 어떤 유물을 잃었는지 확인하거나, 잃을 유물이 없었는지 확인할 수 있습니다.
                    if (lostRelic != null)
                    {
                        Debug.Log($"잃어버린 유물: {lostRelic.relicEnName}");
                    }
                    else
                    {
                        // 잃을 유물이 없었을 경우
                        Debug.Log("잃을 유물이 없어서 아무 일도 일어나지 않았습니다.");
                    }
                    break;

                //case ChoiceResultType.None:
                //  Debug.Log(" 이벤트 지나감 ");
                //  break;
            }

            // 결과가 있으면 결과창 보여주고 없으면 이벤트 종료
            if (choiceIndex < data.EncounterExitText.Count && !string.IsNullOrEmpty(data.EncounterExitText[choiceIndex]))
            {
                resultPanel.SetActive(true);
                string processedText = data.EncounterExitText[choiceIndex].Replace("\\n", "\n");
                resultText.text = processedText;

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