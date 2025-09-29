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
            public EncounterType Type;
            public Sprite sprite;
        }

        public List<EncounterSpriteMapping> encounterSprites;

        private static int gambleCount = 0;

        [Header("유물 선택 UI")]
        [SerializeField] private GameObject relicChoiceButtonPrefab;
        [SerializeField] private Transform relicChoiceContainer;
        public GameObject closeButton;

        public GameObject failedPanel;

        private Dictionary<EncounterType, List<string>> ButtonColors = new Dictionary<EncounterType, List<string>>();

        private void Awake()
        {
            failedPanel.SetActive(false);
            ButtonColors.Add(EncounterType.MoneySpend, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.RelicSpent, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.FightSel, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.MoneyFight, new List<string> { "#8FDDFF", "#FF8F8F" });
            ButtonColors.Add(EncounterType.Gamb, new List<string> { "#8FDDFF", "#FF8F8F" });
        }

        public void Initialize(EncounterTable data, EventManager eventManager)
        {
            _eventManager = eventManager;


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

            var mapping = encounterSprites.Find(m => m.Type == data.Type);
            if (mapping != null && eventImage != null)
            {
                eventImage.sprite = mapping.sprite;
                eventImage.gameObject.SetActive(true);
            }
            else if (eventImage != null)
            {
                eventImage.gameObject.SetActive(false);
            }

            encounterText.text = data.EncounterText.Replace("\\n", "\n");


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
                    var colors = ButtonColors[data.Type];

                    if (choiceIndex < colors.Count)
                    {
                        string colorHex = colors[choiceIndex];
                        if (!string.IsNullOrEmpty(colorHex) && ColorUtility.TryParseHtmlString(colorHex, out var newColor))
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

                button.onClick.AddListener(() => OnChoiceSelected(data, choiceIndex));
            }

            resultPanel.SetActive(false);
        }

        private List<RelicDatas> GetFilteredRelicList(EncounterTable data)
        {
            var allRelics = _eventManager.allRelicsDatabase;

            // EncounterTable에 있는 StartID와 EndID를 직접 사용합니다.
            int minRelicId = data.StartID;
            int maxRelicId = data.EndID;

            // ID 범위가 유효하게 지정되었는지 확인합니다 (0보다 큰 값).
            if (minRelicId > 0 && maxRelicId > 0)
            {
                // Debug.Log($"유물 ID 범위 필터링: {minRelicId} ~ {maxRelicId}");
                return allRelics
                    .Where(relic => relic.relicID >= minRelicId && relic.relicID <= maxRelicId)
                    .ToList();
            }

            // ID 범위가 지정되지 않은 이벤트의 경우, 전체 유물 목록을 그대로 반환합니다.
            // Debug.LogWarning($"ID 범위가 지정되지 않은 이벤트입니다(ID: {data.EncounterID}). 전체 유물 목록을 사용합니다.");
            return allRelics;
        }

        private void OnChoiceSelected(EncounterTable data, int choiceIndex)
        {
            int requiredMoney = 0;

            if (data.Type == EncounterType.MoneySpend && choiceIndex == 0)
            {
                requiredMoney = 500;
            }
            // 갬블의 첫 번째 선택지
            else if (data.Type == EncounterType.Gamb && choiceIndex == 0)
            {
                requiredMoney = 500;
            }
            else if (data.Type == EncounterType.MoneyFight && choiceIndex == 0)
            {
                requiredMoney = 200;
            }

            // 돈이 필요한데 현재 가진 돈이 부족한 경우
            if (requiredMoney > 0 && GameManager.Instance.Coin.yeopjeon < requiredMoney)
            {
                // 참고 이미지와 같이 "한도 초과" 메시지를 띄웁니다.
                if (failedPanel != null)
                {
                    failedPanel.SetActive(true);
                }
                return; // 엽전이 부족하므로 아래 로직을 실행하지 않고 함수를 종료
            }

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
                    resultType = ChoiceResultType.RelicSel;
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
                        string exitText = data.EncounterExitText[choiceIndex];
                    }
                    else if (choiceIndex == 1)
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
            switch (resultType)
            {
                case ChoiceResultType.GainYeopjeon:
                    GameManager.Instance.Coin.AddYeopjeon(resultMoney);
                    break;

                case ChoiceResultType.LoseYeopjeon:
                    GameManager.Instance.Coin.SubtractYeopjeon(Mathf.Abs(resultMoney));
                    break;

                case ChoiceResultType.BuyRelic:
                    var potentialBuyRelics = GetFilteredRelicList(data);
                    var boughtRelic = GameManager.Instance.InGameItem.AddRandomRelic(potentialBuyRelics);

                    GameManager.Instance.Coin.AddYeopjeon(resultMoney);

                    if (boughtRelic != null)
                    {
                        specificRelicName = boughtRelic.relicName;
                    }
                    break;

                case ChoiceResultType.GainRelic:
                    var potentialGainRelics = GetFilteredRelicList(data);
                    var gainedRelic = GameManager.Instance.InGameItem.AddRandomRelic(potentialGainRelics);

                    if (gainedRelic != null)
                    {
                        specificRelicName = gainedRelic.relicName;
                    }
                    break;

                case ChoiceResultType.LoseRelic:
                    var lostRelic = GameManager.Instance.InGameItem.RemoveRandomRelic();
                    GameManager.Instance.Coin.AddYeopjeon(resultMoney);
                    if (lostRelic != null)
                    {
                        specificRelicName = lostRelic.relicName;
                    }
                    break;

                case ChoiceResultType.GainBadRelic:
                    var debuffRelicsFromData = GetFilteredRelicList(data);

                    var availableDebuffRelics = debuffRelicsFromData
                        .Where(relic => !GameManager.Instance.InGameItem.HasRelic(relic))
                        .ToList();

                    if (availableDebuffRelics.Count > 0)
                    {
                        int randomIndex = Random.Range(0, availableDebuffRelics.Count);
                        var debuffRelicToAdd = availableDebuffRelics[randomIndex];
                        GameManager.Instance.InGameItem.AddItem(debuffRelicToAdd);
                        specificRelicName = debuffRelicToAdd.relicName;
                    }
                    break;

                case ChoiceResultType.Combat:
                    bool isYeopjeonReward = data.ResultMoney > 0;
                    _eventManager.SetCombatReward(isYeopjeonReward, data.ResultMoney);
                    var partyUI = FindObjectOfType<PartyUI>();
                    if (partyUI != null)
                    {
                        eventPanel.SetActive(false);
                        resultPanel.SetActive(false);
                        partyUI.PrepareForBattleEvent(BattleEventType.Elite);
                    }
                    break;

                case ChoiceResultType.RelicSel:
                    var potentialSelectRelics = GetFilteredRelicList(data);

                    var acquirableRelics = potentialSelectRelics
                        .Where(relic => !GameManager.Instance.InGameItem.HasRelic(relic))
                        .OrderBy(x => Random.value)
                        .ToList();

                    if (acquirableRelics.Any())
                    {
                        var relicToGain = acquirableRelics.First();
                        GameManager.Instance.InGameItem.AddItem(relicToGain);
                        specificRelicName = relicToGain.relicName;
                    }
                    break;

                case ChoiceResultType.Continue:
                    int numberOfRelicsToGain = Random.Range(data.ResultMinCount, data.ResultMaxCount + 1);
                    var gainedRelicNames = new List<string>();

                    var potentialLuckRelics = GetFilteredRelicList(data);

                    for (int i = 0; i < numberOfRelicsToGain; i++)
                    {
                        var luckRelic = GameManager.Instance.InGameItem.AddRandomRelic(potentialLuckRelics);
                        if (luckRelic != null)
                        {
                            gainedRelicNames.Add(luckRelic.relicName);
                        }
                    }

                    if (gainedRelicNames.Count > 0)
                    {
                        specificRelicName = string.Join(", ", gainedRelicNames);
                    }

                    GameManager.Instance.Coin.AddYeopjeon(resultMoney);
                    break;

                case ChoiceResultType.None:
                    break;
            }



            // 결과가 있으면 결과창 보여주고 없으면 이벤트 종료
            if (data.EncounterExitText.Count > 0 && !string.IsNullOrEmpty(data.EncounterExitText[0]))
            {
                resultPanel.SetActive(true);

                // 텍스트 색상 지정
                if (data.Sentiment == EncounterSentiment.Good)
                {
                    if (ColorUtility.TryParseHtmlString("#8FDDFF", out var goodColor))
                    {
                        resultText.color = goodColor;
                    }
                }
                else
                {
                    resultText.color = Color.white;
                }

                string finalResultText = "";

                // Luck 타입일 경우, 불러온 텍스트에 유물 이름 목록을 직접 추가합니다.
                if (data.Type == EncounterType.Luck)
                {
                    // CSV에서 불러온 기본 텍스트 ("엽전 + N\n아래의 유물을 획득합니다")
                    string baseText = data.EncounterExitText[0].Replace("\\n", "\n").Trim();

                    string relicInfoLine = "";
                    if (!string.IsNullOrEmpty(specificRelicName))
                    {
                        // 획득한 유물 이름들로 한 줄을 새로 만듭니다.
                        relicInfoLine = $"<color=#FFD700>{specificRelicName}</color>";
                    }

                    // 기본 텍스트와 유물 정보 텍스트를 합칩니다.
                    finalResultText = $"{baseText}\n{relicInfoLine}";
                }
                else if (data.Type == EncounterType.RelicSel || choiceIndex >= 0 && choiceIndex < data.EncounterExitText.Count)
                {
                    // RelicSel 타입은 모든 선택지의 결과 텍스트가 동일하므로, 
                    // choiceIndex 대신 항상 0번 인덱스를 사용하여 데이터가 1개만 있어도 에러가 나지 않도록 합니다.
                    string textToShow = data.Type == EncounterType.RelicSel && data.EncounterExitText.Count > 0 ?
                        data.EncounterExitText[0] :
                        data.EncounterExitText[choiceIndex];

                    string baseResultText = textToShow.Replace("\\n", "\n");

                    if (!string.IsNullOrEmpty(specificRelicName))
                    {
                        // 플레이스홀더가 있는 경우 교체
                        if (baseResultText.Contains("[획득한 유물 이름]"))
                        {
                            baseResultText = baseResultText.Replace("[획득한 유물 이름]", $"<color=#FFD700>{specificRelicName}</color>");
                        }
                        // 플레이스홀더가 없는 경우, 정보 추가
                        else
                        {
                            string relicInfo = resultType == ChoiceResultType.LoseRelic ?
                                $"\n잃어버린 유물: <color=#FFD700>{specificRelicName}</color>" :
                                $"\n획득한 유물: <color=#FFD700>{specificRelicName}</color>";
                            baseResultText += relicInfo;
                        }
                    }
                    finalResultText = baseResultText;
                }

                // 최종적으로 완성된 텍스트를 UI에 적용하고, 불필요한 공백을 제거합니다.
                resultText.text = finalResultText.Trim();

                // UI 정리
                if (eventTitleText != null) eventTitleText.gameObject.SetActive(false);
                eventImage.gameObject.SetActive(false);
                encounterText.gameObject.SetActive(false);
                buttonContainer.gameObject.SetActive(false);
            }
            else
            {
                // 결과 텍스트가 없는 이벤트는 바로 종료
                _eventManager.EndEncounter();
            }
        }
    }
}