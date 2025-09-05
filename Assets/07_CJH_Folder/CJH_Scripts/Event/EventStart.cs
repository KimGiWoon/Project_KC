using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SDW;
using System.Collections.Generic;

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
        public GameObject choiceButtonPrefab; // 버튼 모양으로 사용할 프리팹

        // 사건 유형(EncounterType)에 따라 보여줄 스프라이트를 인스펙터에서 연결
        [System.Serializable]
        public class EncounterSpriteMapping
        {
            public EncounterType type;
            public Sprite sprite;
        }
        public List<EncounterSpriteMapping> encounterSprites;


        public void Initialize(EncounterTable data)
        {
            // 1. 사건 유형에 맞는 이미지 설정
            EncounterSpriteMapping mapping = encounterSprites.Find(m => m.type == data.Type);
            if (mapping != null && eventImage != null)
            {
                eventImage.sprite = mapping.sprite;
                eventImage.gameObject.SetActive(true);
            }
            else if (eventImage != null)
            {
                eventImage.gameObject.SetActive(false); // 맞는 이미지가 없으면 숨김
            }

            // 2. 사건 설명 텍스트 설정
            encounterText.text = data.EncounterText;

            // 3. 기존 버튼 삭제
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }

            // 4. Choice_Count 만큼 버튼 자동 생성
            for (int i = 0; i < data.ChoiceCount; i++)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, buttonContainer);
                int choiceIndex = i; // 클로저 문제 방지

                // 버튼 텍스트 설정
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && choiceIndex < data.ChoiceTexts.Count)
                {
                    buttonText.text = data.ChoiceTexts[choiceIndex];
                }

                // 버튼 클릭 이벤트 연결
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => OnChoiceSelected(data, choiceIndex));
            }

            resultPanel.SetActive(false);
        }

        void OnChoiceSelected(EncounterTable data, int choiceIndex)
        {
            if (choiceIndex < data.EncounterExitText.Count && !string.IsNullOrEmpty(data.EncounterExitText[choiceIndex]))
            {
                resultPanel.SetActive(true);
                resultText.text = data.EncounterExitText[choiceIndex];
            }
            else
            {
                // 결과 텍스트가 없으면 바로 종료
                EventManager.Instance.EndEncounter();
            }

            // 모든 선택지 버튼 비활성화
            foreach (var btn in buttonContainer.GetComponentsInChildren<Button>())
            {
                btn.interactable = false;
            }
        }
    }
}