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
        public GameObject eventPanel;
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
        public GameObject relicSelectionPanel; // 유물 선택 화면 전체 패널
        [SerializeField] private GameObject relicChoiceButtonPrefab;
        [SerializeField] private Transform relicChoiceContainer;
        public GameObject closeButton;


        private Dictionary<EncounterType, List<string>> ButtonColors = new Dictionary<EncounterType, List<string>>();

        void Awake()
        {
            ButtonColors.Add(EncounterType.MoneySpend, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.RelicSpent, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.FightSel, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.MoneyFight, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.Gamb, new List<string> { "#8FDDFF", "#FF8F8F" });


        }


        public void Initialize(EncounterTable data, EventManager eventManager)
        {
            _eventManager = eventManager;

            if (relicSelectionPanel != null)
            {
                relicSelectionPanel.SetActive(false);
            }

            if (eventTitleText != null)
            {
                switch (data.Sentiment)
                {
                    case EncounterSentiment.Good:
                        eventTitleText.text = "극상의 맛";
                        break;
                    case EncounterSentiment.Bad:
                        eventTitleText.text = "절망의 맛";
                        break;
                    default:
                        eventTitleText.text = "미묘한 맛";
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

                var buttonImage = buttonObj.GetComponent<Image>();

                if (buttonImage != null && ButtonColors.ContainsKey(data.Type))
                {
                    // 여기도 data.EncounterID를 data.Type으로 변경
                    List<string> colors = ButtonColors[data.Type];

                    if (choiceIndex < colors.Count)
                    {
                        string colorHex = colors[choiceIndex];
                        if (!string.IsNullOrEmpty(colorHex) && ColorUtility.TryParseHtmlString(colorHex, out Color newColor))
                        {
                            buttonImage.color = newColor;
                        }
                    }
                }

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
                int requiredMoney = 0;

                // 돈이 필요한 EncounterType인지, 그리고 몇 번째 선택지인지 확인
                if ((data.Type == EncounterType.MoneySpend ||
                     data.Type == EncounterType.MoneyFight) && choiceIndex == 0)
                {
                    // ResultMoney: 데이터 시트에 정의된 필요 엽전
                    requiredMoney = data.ResultMoney;
                }
                else if (data.Type == EncounterType.Gamb && choiceIndex == 0)
                {
                    requiredMoney = 500;
                }

                // 돈이 필요한 선택지일 경우, 현재 엽전과 비교
                if (requiredMoney > 0)
                {
                    // GameManager에서 현재 엽전 가져오기
                    int currentMoney = GameManager.Instance.Coin.yeopjeon;

                    if (currentMoney < requiredMoney)
                    {
                        // 가진 돈이 부족하면 버튼 비활성화
                        button.interactable = false;

                    }
                }
                button.onClick.AddListener(() => OnChoiceSelected(data, choiceIndex));
            }

            resultPanel.SetActive(false);
        }

        private void OnChoiceSelected(EncounterTable data, int choiceIndex)
        {
            var resultType = ChoiceResultType.None;
            int resultMoney = data.ResultMoney;
            //todo 추후 잃거나 얻는 유물의 수가 1개 초과가 되면 수정 필요
            int resultRelicCount = data.ResultNumber;

            // 잃거나 얻은 유물 이름을 저장할 변수 추가
            string specificRelicName = string.Empty;


            if (data.Type == EncounterType.Gamb)
            {
                // 선택지 1: 갬블을 포기하고 떠난다.
                if (choiceIndex == 1)
                {
                    _eventManager.EndEncounter();
                    return;
                }

                // 선택지 0: 갬블을 진행한다.
                if (choiceIndex == 0)
                {
                    // 1. 공통 처리: 비용 지불 및 횟수 증가
                    int betMoney = 500;
                    GameManager.Instance.Coin.SubtractYeopjeon(betMoney);
                    gambleCount++;

                    // 2. 랜덤으로 승패 결정
                    bool isWin = Random.value >= 0.5f;

                    // 3. 3회차 도달 시 처리
                    if (gambleCount >= 3)
                    {
                        if (isWin)
                        {
                            GameManager.Instance.Coin.AddYeopjeon(1000); // 마지막 판 승리 보상
                        }
                        // 공통된 강제 종료 텍스트를 보여주고 인카운터 완전 종료
                        resultType = ChoiceResultType.None;
                    }
                    else // 4. 1~2회차 진행 시 처리
                    {
                        int nextEncounterID;

                        if (isWin)
                        {
                            // [승리]
                            GameManager.Instance.Coin.AddYeopjeon(1000);
                            nextEncounterID = 6015; // 승리 인카운터 ID
                        }
                        else
                        {
                            // [패배]
                            nextEncounterID = 6016; // 패배 인카운터 ID
                        }

                        // 결정된 다음 인카운터 ID를 가지고 이벤트를 새로 시작합니다.
                        _eventManager.StartEncounterByID(nextEncounterID);
                    }
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
                    GameManager.Instance.Coin.AddYeopjeon(resultMoney); // resultValue 변수 사용
                    Debug.Log($"{resultMoney} 엽전 얻음");
                    break;

                case ChoiceResultType.LoseYeopjeon:
                    GameManager.Instance.Coin.AddYeopjeon(resultMoney); // resultValue 변수 사용
                    Debug.Log($"{resultMoney} 엽전 잃음");
                    break;

                case ChoiceResultType.BuyRelic:
                    // EventManager로부터 전체 유물 목록을 받아와서 인자로 전달합니다.
                    var boughtRelic = GameManager.Instance.InGameItem.AddRandomRelic(_eventManager.allRelicsDatabase);

                    GameManager.Instance.Coin.AddYeopjeon(resultMoney);
                    Debug.Log($"{resultMoney} 엽전으로 유물을 구매했습니다.");

                    if (boughtRelic != null)
                    {
                        specificRelicName = boughtRelic.relicName;
                        Debug.Log($"구매한 유물: {boughtRelic.relicName}");
                    }
                    break;

                case ChoiceResultType.GainRelic:
                    var gainedRelic = GameManager.Instance.InGameItem.AddRandomRelic(_eventManager.allRelicsDatabase);

                    Debug.Log("유물을 획득했습니다.");

                    if (gainedRelic != null)
                    {
                        specificRelicName = gainedRelic.relicName;
                        Debug.Log($"획득한 유물: {gainedRelic.relicName}");
                    }
                    break;

                case ChoiceResultType.LoseRelic:
                    var lostRelic = GameManager.Instance.InGameItem.RemoveRandomRelic();
                    GameManager.Instance.Coin.AddYeopjeon(resultMoney); // resultValue 변수 사용
                    Debug.Log($"{resultMoney} 엽전 얻음");

                    // 어떤 유물을 잃었는지 확인하거나, 잃을 유물이 없었는지 확인할 수 있습니다.
                    if (lostRelic != null)
                    {
                        specificRelicName = lostRelic.relicName;
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
                        var debuffRelicToAdd = availableDebuffRelics[randomIndex];

                        // 선택된 디버프 유물을 인벤토리에 추가합니다.
                        GameManager.Instance.InGameItem.AddItem(debuffRelicToAdd);

                        specificRelicName = debuffRelicToAdd.relicName;
                        Debug.Log($"디버프 유물 '{debuffRelicToAdd.relicName}'을(를) 강제로 획득했습니다.");
                    }
                    break;

                case ChoiceResultType.Combat:

                    // 전투의 보상이 엽전인지 bool 값으로 결정합니다.
                    bool isYeopjeonReward = data.ResultMoney > 0;

                    // EventManager에 다음 전투의 보상 정보를 저장합니다.
                    _eventManager.SetCombatReward(isYeopjeonReward, data.ResultMoney);

                    var partyUI = FindObjectOfType<PartyUI>();
                    if (partyUI != null)
                    {
                        eventPanel.SetActive(false);
                        resultPanel.SetActive(false);
                        partyUI.PrepareForBattleEvent(BattleEventType.Elite);
                    }
                    break;

                case ChoiceResultType.Continue:
                    // 2~3 사이의 랜덤한 숫자를 정합니다.
                    int numberOfRelicsToGain = Random.Range(2, 4);
                    Debug.Log($"{numberOfRelicsToGain}개의 유물을 획득합니다.");

                    // 획득한 유물들의 이름을 저장할 리스트를 생성합니다.
                    List<string> gainedRelicNames = new List<string>();

                    // 정해진 숫자만큼 유물 획득을 반복합니다.
                    for (int i = 0; i < numberOfRelicsToGain; i++)
                    {
                        // 랜덤 유물 획득 시도
                        var luckRelic = GameManager.Instance.InGameItem.AddRandomRelic(_eventManager.allRelicsDatabase);

                        // 획득에 성공했다면 (null이 아니라면)
                        if (luckRelic != null)
                        {
                            // 리스트에 유물 이름을 추가합니다.
                            gainedRelicNames.Add(luckRelic.relicName);
                        }
                    }

                    //획득한 유물 이름 리스트를 하나의 문자열로 합칩니다.
                    if (gainedRelicNames.Count > 0)
                    {
                        // 이전에 작업한 specificRelicName 변수를 재활용하여 결과창에 표시합니다.
                        specificRelicName = string.Join(", ", gainedRelicNames.Select(name => $"'{name}'"));
                    }

                    GameManager.Instance.Coin.AddYeopjeon(resultMoney);
                    Debug.Log($"{resultMoney} 엽전 얻음");
                    break;

                case ChoiceResultType.None:
                    Debug.Log(" 이벤트 지나감 ");
                    break;
            }

            //case ChoiceResultType.Continue:
            //
            //        int numberOfChoices = Random.Range(2, 4);
            //
            //        ShowRelicSelection(numberOfChoices);
            //
            //        GameManager.Instance.Coin.AddYeopjeon(resultMoney); // resultValue 변수 사용
            //
            //        Debug.Log($"{resultMoney} 엽전 얻음");
            //
            //        Debug.Log("유물 선택지");
            //
            //        break;

            // 결과가 있으면 결과창 보여주고 없으면 이벤트 종료
            if (choiceIndex < data.EncounterExitText.Count && !string.IsNullOrEmpty(data.EncounterExitText[choiceIndex]))
            {
                resultPanel.SetActive(true);

                // 긍정적 사건일 경우 텍스트 색상 변경
                if (data.Sentiment == EncounterSentiment.Good)
                {
                    if (ColorUtility.TryParseHtmlString("#8FDDFF", out Color goodColor))
                    {
                        resultText.color = goodColor;
                    }
                }
                else
                {
                    // 긍정적이지 않은 다른 이벤트에 대비해 기본 색상(흰색)으로 설정
                    resultText.color = Color.white;
                }

                string processedText = data.EncounterExitText[choiceIndex].Replace("\\n", "\n");

                if (!string.IsNullOrEmpty(specificRelicName))
                {
                    processedText = processedText.Replace("{relicName}", $"'{specificRelicName}'");
                }
                resultText.text = processedText;

                if (eventTitleText != null) eventTitleText.gameObject.SetActive(false);
                eventImage.gameObject.SetActive(false);
                encounterText.gameObject.SetActive(false);
                buttonContainer.gameObject.SetActive(false);
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

                var newButtonObj = Instantiate(relicChoiceButtonPrefab, relicChoiceContainer);
                newButtonObj.SetActive(true);

                var buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = relicData.relicName;
                }

                var button = newButtonObj.GetComponent<Button>();
                if (button != null)
                {
                    var capturedRelic = relicData; // 클로저 문제 방지
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