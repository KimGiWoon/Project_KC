using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    [SerializeField] private Image characterImage; //캐릭터 이미지
    [SerializeField] private TextMeshProUGUI characterName; //캐릭터 이름
    [SerializeField] private Color rarityColor; //등급에 따른 이름 색
    
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
}
