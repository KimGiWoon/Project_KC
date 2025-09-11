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
                    // 만약 CSV에서 가져온 선택지 텍스트가 비어있다면, "확인"으로 표시합니다.
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