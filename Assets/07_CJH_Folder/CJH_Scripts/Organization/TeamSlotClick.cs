using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CJH
{
    public class TeamSlotClick : MonoBehaviour
    {
        [Header("캐릭터 정보 UI")]
        public Image characterImage;
        public TMP_Text characterNameText;
        public Button removeButton;

        private CharacterDataSO currentCharacter;
        private int slotIndex;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnSlotClick);
            if (removeButton != null)
            {
                removeButton.onClick.AddListener(OnRemoveClick);
            }
        }

        public void Initialize(int index)
        {
            slotIndex = index;
        }

        public void UpdateSlot(CharacterDataSO characterData)
        {
            currentCharacter = characterData;
            if (currentCharacter != null)
            {
                characterImage.sprite = currentCharacter._characterSprite;
                characterNameText.text = currentCharacter._chaBaseData.ChaName;
                characterImage.gameObject.SetActive(true);
                if (removeButton != null) removeButton.gameObject.SetActive(true);
            }
            else
            {
                // 빈 슬롯 처리
                characterImage.gameObject.SetActive(false);
                characterNameText.text = "Empty";
                if (removeButton != null) removeButton.gameObject.SetActive(false);
            }
        }

        private void OnSlotClick()
        {
            // 슬롯 클릭 시 동작 
            if (currentCharacter != null)
            {
                Debug.Log(currentCharacter._chaBaseData.ChaName + " 슬롯 클릭");
            }
        }

        private void OnRemoveClick()
        {
            // 팀에서 캐릭터 제거
            if (TeamManager.Instance != null)
            {
                TeamManager.Instance.RemoveCharacterFromTeam(slotIndex);
            }
        }
    }
}