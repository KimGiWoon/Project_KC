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
        private RewardChangeManager manager;

        private bool isSet = false;

        private void OnEnable()
        {
            manager = RewardChangeManager.Instance;
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

        public void SetData(CharacterData data)
        {
            characterImage.sprite = data.characterImage;
            characterName.text = data.characterName;
            characterName.color = GetRarityColor(data.rarity);

          if (RewardChangeManager.Instance.ownedCharacters[data.characterName] == false)
          {
              RewardChangeManager.Instance.ownedCharacters[data.characterName] = true;
          }
          else
          {
              if (data.beads > 6)
              {
                  //SetStarCandy(RewardChangeManager.Instance.gainedStarCandy);    
                  Debug.Log("스타캔디트루");
                  Debug.Log($"스타캔디트루 {RewardChangeManager.Instance.isStarCandy}");
                  SetStarCandy(1);    
              }
              else
              {
                  //SetBead(RewardChangeManager.Instance.gainedBead);
                  Debug.Log("스타캔디펄스");
                  Debug.Log($"스타캔디트루 {RewardChangeManager.Instance.isStarCandy}");
                  SetBead(1);
              }
              RewardChangeManager.Instance.ownedCharacters[data.characterName] = true;
              isSet = true;
          }
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