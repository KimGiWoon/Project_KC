using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterGacha : MonoBehaviour
{
    [Header("캐릭터들")] 
    [SerializeField] private List<CharacterData> chracterLists;

    private WeightedRandom<Rarity> rarityPicker;

    private void Awake()
    {
        rarityPicker = new WeightedRandom<Rarity>();
        rarityPicker.Add(Rarity.Common, 98);
        rarityPicker.Add(Rarity.Rare, 2);
    }

    public CharacterData GetRandomCharacter()
    {
        Rarity getRarity = rarityPicker.GetRandom();
        
        List<CharacterData> getChracterList = chracterLists
            .Where(c => c.rarity == getRarity)
            .ToList();
        
        CharacterData selectChracter = getChracterList[Random.Range(0, getChracterList.Count)];
        Debug.Log($"가챠 결과 → {selectChracter.characterName} (등급: {selectChracter.rarity})");
        return selectChracter;
    }
}
