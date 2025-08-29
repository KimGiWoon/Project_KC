using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        
        private void OnEnable()
        {
            var manager = RewardChangeManager.Instance;
            if (manager != null)
            {
                manager.OnStarCandyGained += SetStarCandy;
                manager.OnBeadGained += SetBead;
            }
        }

        private void OnDisable()
        {
            var manager = RewardChangeManager.Instance;
            if (manager != null)
            {
                manager.OnStarCandyGained -= SetStarCandy;
                manager.OnBeadGained -= SetBead;
            }
        }
        public void SetData(CharacterData data)
        {
            characterImage.sprite = data.characterImage;
            characterName.text = data.characterName;
            characterName.color = GetRarityColor(data.rarity);
        }

        private Color GetRarityColor(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    Color commonColor;
                    ColorUtility.TryParseHtmlString("#C4F1FF", out commonColor);
                    return commonColor;
                case Rarity.Rare:
                    Color rareColor;
                    ColorUtility.TryParseHtmlString("#FFF6C6", out rareColor);
                    return rareColor;
                default:
                    return Color.white;
            }
        }

        public void SetStarCandy(int amount)
        {
            starCandy.SetActive(true);
            starCandyText.text = amount.ToString();
            Debug.Log("별사탕 획득!");
        }

        public void SetBead(int amount)
        {
            bead.SetActive(true);
            beadText.text = amount.ToString();
            Debug.Log("구슬 획득!");
        }
    }
}