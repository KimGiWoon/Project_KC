using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;
using SDW;

namespace KSH
{
    public class GachaUI : MonoBehaviour
    {
        [SerializeField] private Image characterImage; //캐릭터 이미지
        [SerializeField] private TextMeshProUGUI characterName; //캐릭터 이름
        [SerializeField] private Color rarityColor; //등급에 따른 이름 색
        [SerializeField] private GameObject starCandy;
        [SerializeField] private TextMeshProUGUI starCandyText;
        [SerializeField] private GameObject bead;
        [SerializeField] private TextMeshProUGUI beadText;
        private RewardChangeManager manager;
        private CharacterDataManager _data;

        private bool isSet = false;

        private void OnEnable()
        {
            manager = GameManager.Instance.Reward;
            _data = GameManager.Instance.CharacterData;
            if (manager != null)
            {
                manager.OnStarCandyGained += SetStarCandy;
                manager.OnBeadGained += SetBead;
            }
        }

        private void OnDisable()
        {
            manager.OnStarCandyGained -= SetStarCandy;
            manager.OnBeadGained -= SetBead;
        }

        // public void SetData(CharacterData data)
        public void SetData(
            CharacterDataSO data,
            int starCandy,
            int bead,
            int currentBead,
            bool isFirstCharacter
        )
        {
            characterImage.sprite = data.GachaBackground;
            characterName.text = data._chaBaseData.ChaName;
            characterName.color = GetRarityColor(data._chaBaseData.ChaGrade);

            isSet = isFirstCharacter;

            if (currentBead > 6)
            {
                //SetStarCandy(RewardChangeManager.Instance.gainedStarCandy);    
                Debug.Log($"{data._chaBaseData.ChaName}스타캔디트루");
                Debug.Log($"스타캔디트루 {manager.isStarCandy[data._chaBaseData.ChaName]}");
                SetStarCandy(starCandy);
            }
            else
            {
                //SetBead(RewardChangeManager.Instance.gainedBead);
                Debug.Log($"{data._chaBaseData.ChaName}스타캔디펄스");
                Debug.Log($"스타캔디트루 {manager.isStarCandy[data._chaBaseData.ChaName]}");
                SetBead(bead);
            }
            _data.OwnedCharacters[data._chaBaseData.ChaName] = true;
            // }
        }

        private Color GetRarityColor(CharacterGrade rarity)
        {
            switch (rarity)
            {
                case CharacterGrade.Normal:
                    Color commonColor;
                    ColorUtility.TryParseHtmlString("#C4F1FF", out commonColor);
                    return commonColor;
                case CharacterGrade.Rare:
                    Color rareColor;
                    ColorUtility.TryParseHtmlString("#FFF6C6", out rareColor);
                    return rareColor;
                default:
                    return Color.white;
            }
        }

        public void SetStarCandy(int amount)
        {
            if (isSet) return;
            starCandy.gameObject.SetActive(true);
            starCandyText.text = amount.ToString();
            Debug.Log("별사탕 획득!");
        }

        public void SetBead(int amount)
        {
            if (isSet) return;
            bead.gameObject.SetActive(true);
            beadText.text = amount.ToString();
            Debug.Log("구슬 획득!");
        }
    }
}