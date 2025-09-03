using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CJH
{
    public class CharacterInfoPanel : MonoBehaviour
    {
        [Header("UI 요소")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI rarityText;
        // 필요하다면 다른 스탯을 표시할 Text UI들을 추가하세요.
        // [SerializeField] private TextMeshProUGUI statsText;

        [Header("패널 제어")]
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            // 시작할 때 패널을 숨기고, 닫기 버튼에 기능 연결
            gameObject.SetActive(false);
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HidePanel);
            }
        }

        /// <summary>
        /// 캐릭터 데이터를 받아 정보 창을 채우고 보여줍니다.
        /// </summary>
        public void ShowPanel(CharacterData data)
        {
            if (data == null) return;

            // UI 요소들에 캐릭터 데이터 할당
            if (characterNameText != null) characterNameText.text = data.characterName;
            if (characterImage != null) characterImage.sprite = data.characterImage;
            if (rarityText != null) rarityText.text = data.rarity.ToString();

            // 예시: 스탯 텍스트 설정
            // if (statsText != null) statsText.text = $"공격력: {data.attackDamage}\n방어력: {data.defense}";

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 정보 창을 숨깁니다.
        /// </summary>
        public void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}