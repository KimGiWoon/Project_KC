using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CJH
{
    public class EventStart : MonoBehaviour
    {
        public static EventManager Instance; 

        [Header("UI 요소")]
        public TextMeshProUGUI encounterText;
        public GameObject choiceButtonPrefab;
        public Transform buttonContainer;
        public GameObject resultPanel;
        public TextMeshProUGUI resultText;
        public Button closeResultButton; // 결과 패널 닫기 버튼

        private Dictionary<int, EncounterData> allEncounters;

        void Awake()
        {
            LoadAllEncounterData();
        }

        void Start()
        {
            if (closeResultButton != null)
            {
                // 버튼이 클릭되면 OnCloseResultButtonClicked 함수를 호출합니다.
                closeResultButton.onClick.AddListener(OnCloseResultButtonClicked);
            }
        }

        // EncounterData를 Resources 폴더에서 불러옴
        void LoadAllEncounterData()
        {
            allEncounters = new Dictionary<int, EncounterData>();
            EncounterData[] encounters = Resources.LoadAll<EncounterData>("Data/EncounterData");
            foreach (var encounter in encounters)
            {
                if (!allEncounters.ContainsKey(encounter.EncounterID))
                    allEncounters.Add(encounter.EncounterID, encounter);
            }
        }

        // GameManager가 이 함수를 호출하여 이벤트를 시작
        public void StartEncounter(int encounterID)
        {
            if (!allEncounters.ContainsKey(encounterID))
            {
                Debug.LogError($"Encounter ID {encounterID}에 해당하는 데이터를 찾을 수 없습니다.");
                return;
            }
            EncounterData currentEncounter = allEncounters[encounterID];
            UpdateUI(currentEncounter);
        }

        void UpdateUI(EncounterData encounter)
        {
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
            resultPanel.SetActive(false);
            encounterText.text = encounter.EncounterText;

            for (int i = 0; i < encounter.ChoiceCount; i++)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, buttonContainer);
                int choiceIndex = i;

                var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && choiceIndex < encounter.ChoiceTexts.Count)
                {
                    buttonText.text = encounter.ChoiceTexts[choiceIndex];
                }

                var button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => OnChoiceSelected(encounter, choiceIndex));
            }
        }

        void OnChoiceSelected(EncounterData encounter, int choiceIndex)
        {
            if (choiceIndex < encounter.EncounterExitText.Count)
            {
                resultText.text = encounter.EncounterExitText[choiceIndex];
                resultPanel.SetActive(true);
            }

            foreach (var btn in buttonContainer.GetComponentsInChildren<Button>())
            {
                btn.interactable = false;
            }
        }

        // 결과 패널의 닫기 버튼이 클릭되면 GameManager에게 이벤트 종료 호출
        public void OnCloseResultButtonClicked()
        {
            resultPanel.SetActive(false);
            EventManager.Instance.EndEncounter();
        }
    }
}