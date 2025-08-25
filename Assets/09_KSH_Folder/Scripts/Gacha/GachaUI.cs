using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private Color rarityColor;
    
    public void SetData(CharacterData data)
    {
        characterImage.sprite = data.characterImage;
        characterName.text = data.characterName;
        characterImage.color = GetRarityColor(data.rarity);
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
