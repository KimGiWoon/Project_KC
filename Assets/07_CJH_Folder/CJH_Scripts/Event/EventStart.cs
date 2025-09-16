using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SDW;
using KSH;
using System.Linq;

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
        [SerializeField] private GameObject relicChoiceButtonPrefab;
        [SerializeField] private Transform relicChoiceContainer;

        public GameObject closeButton;

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
                        resultType = ChoiceResultType.GainRelic; // 첫 번째 선택은 돈 얻기
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
                        resultType = ChoiceResultType.BuyRelic; // 첫 번째 선택은 무조건 전투
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
                case ChoiceResultType.GainYeopjeon:
                    GameManager.Instance.Coin.AddYeopjeon(resultValue); // resultValue 변수 사용
                    Debug.Log($"{resultValue} 엽전 얻음");
                    break;

                case ChoiceResultType.LoseYeopjeon:
                    GameManager.Instance.Coin.SubtractYeopjeon(resultValue); // resultValue 변수 사용
                    Debug.Log($"{resultValue} 엽전 잃음");
                    break;

                case ChoiceResultType.BuyRelic:
                    // EventManager로부터 전체 유물 목록을 받아와서 인자로 전달합니다.
                    RelicDatas boughtRelic = GameManager.Instance.InGameItem.AddRandomRelic(_eventManager.allRelicsDatabase);

                    GameManager.Instance.Coin.SubtractYeopjeon(resultValue);
                    Debug.Log($"{resultValue} 엽전으로 유물을 구매했습니다.");

                    if (boughtRelic != null)
                    {
                        Debug.Log($"구매한 유물: {boughtRelic.relicName}");
                    }
                    break;

                case ChoiceResultType.GainRelic:
                    RelicDatas gainedRelic = GameManager.Instance.InGameItem.AddRandomRelic(_eventManager.allRelicsDatabase);

                    Debug.Log("유물을 획득했습니다.");

                    if (gainedRelic != null)
                    {
                        Debug.Log($"획득한 유물: {gainedRelic.relicName}");
                    }
                    break;

                case ChoiceResultType.LoseRelic:
                    RelicDatas lostRelic = GameManager.Instance.InGameItem.RemoveRandomRelic();
                    GameManager.Instance.Coin.AddYeopjeon(resultValue); // resultValue 변수 사용
                    Debug.Log($"{resultValue} 엽전 얻음");

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

                case ChoiceResultType.GainBadRelic:
                    // EventManager가 가지고 있는 전체 유물 목록을 가져옵니다.
                    var allRelics = _eventManager.allRelicsDatabase;

                    // 디버프 유물이고, 아직 플레이어가 가지고 있지 않은 유물만 골라냅니다.
                    var availableDebuffRelics = allRelics
                        .Where(relic => relic.relicGrade == RelicGrade.Debuff && !GameManager.Instance.InGameItem.HasRelic(relic))
                        .ToList();

                    //획득 가능한 디버프 유물이 있는지 확인합니다.
                    if (availableDebuffRelics.Count > 0)
                    {
                        // 획득 가능한 목록 중에서 무작위로 하나를 선택합니다.
                        int randomIndex = Random.Range(0, availableDebuffRelics.Count);
                        RelicDatas debuffRelicToAdd = availableDebuffRelics[randomIndex];

                        // 선택된 디버프 유물을 인벤토리에 추가합니다.
                        GameManager.Instance.InGameItem.AddItem(debuffRelicToAdd);

                        Debug.Log($"디버프 유물 '{debuffRelicToAdd.relicName}'을(를) 강제로 획득했습니다.");
                    }
                    else
                    {
                        Debug.Log("획득할 수 있는 디버프 유물이 더 이상 없습니다. 아무 일도 일어나지 않습니다.");
                    }
                    break;

                case ChoiceResultType.Combat:
                    MapView.Instance.OnCharacterMoved?.Invoke(true);
                    MapView.Instance.OnEventTypeChanged?.Invoke(BattleEventType.Elite);
                    Debug.Log("전투");
                    break;

                case ChoiceResultType.Continue:
                    int numberOfChoices = Random.Range(2, 4);
                    ShowRelicSelection(numberOfChoices);
                    GameManager.Instance.Coin.AddYeopjeon(resultValue); // resultValue 변수 사용
                    Debug.Log($"{resultValue} 엽전 얻음");
                    Debug.Log("유물 선택지");
                    break;


                case ChoiceResultType.None:
                  Debug.Log(" 이벤트 지나감 ");
                  break;
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

        /// <summary>
        /// 플레이어에게 N개의 유물 선택지를 보여줍니다.
        /// </summary>
        /// <param name="numberOfChoices">보여줄 선택지의 개수</param>

        private void ShowRelicSelection(int numberOfChoices)
        {
            relicSelectionPanel.SetActive(true);

            // 기존 UI 숨기기
            buttonContainer.gameObject.SetActive(false);
            encounterText.gameObject.SetActive(false);
            eventImage.gameObject.SetActive(false);
            eventTitleText.gameObject.SetActive(false);
            if (closeButton != null)
                closeButton.SetActive(false); // 닫기 버튼 직접 OFF

            // 이전에 생성된 버튼이 있다면 제거
            foreach (Transform child in relicChoiceContainer)
            {
                Destroy(child.gameObject);
            }

            // 플레이어가 아직 가지지 않은 유물 목록을 무작위로 섞어 만듭니다.
            var acquirableRelics = _eventManager.allRelicsDatabase
                .Where(relic => !GameManager.Instance.InGameItem.HasRelic(relic))
                .OrderBy(x => Random.value)
                .ToList();

            // 보여줄 개수만큼 유물을 선택합니다.
            var relicsToOffer = acquirableRelics.Take(numberOfChoices).ToList();

            // 획득 가능한 유물이 없으면 그냥 이벤트를 종료합니다.
            if (acquirableRelics.Count == 0)
            {
                Debug.Log("제공할 수 있는 새로운 유물이 없어 이벤트가 종료됩니다.");
                _eventManager.EndEncounter();
                return;
            }

            for (int i = 0; i < relicsToOffer.Count; i++)
            {
                var relicData = relicsToOffer[i];

                GameObject newButtonObj = Instantiate(relicChoiceButtonPrefab, relicChoiceContainer);
                newButtonObj.SetActive(true);

                var buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = relicData.relicName;
                }

                var button = newButtonObj.GetComponent<Button>();
                if (button != null)
                {
                    RelicDatas capturedRelic = relicData; // 클로저 문제 방지
                    button.onClick.AddListener(() => OnRelicChosen(capturedRelic));
                }

                Debug.Log($"[RelicSelection] 생성된 버튼 {i} - 유물: {relicData.relicName}");
            }
        }

        /// <summary>
        /// 플레이어가 유물 선택지 중 하나를 클릭했을 때 호출됩니다.
        /// </summary>
        /// <param name="chosenRelic">선택된 유물 데이터</param>
        private void OnRelicChosen(RelicDatas chosenRelic)
        {
            // 선택된 유물을 인벤토리에 추가합니다.
            GameManager.Instance.InGameItem.AddItem(chosenRelic);
            Debug.Log($"플레이어가 유물 '{chosenRelic.relicName}'을(를) 선택했습니다.");

            // 유물 선택 UI를 닫고 이벤트를 완전히 종료합니다.
            relicSelectionPanel.SetActive(false);
            _eventManager.EndEncounter();
        }
    }
}