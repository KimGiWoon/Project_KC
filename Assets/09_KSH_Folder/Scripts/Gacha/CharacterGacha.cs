using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterGacha : MonoBehaviour
{
    [Header("캐릭터들")] 
    [SerializeField] private List<CharacterData> chracterLists;
    [Header("UI")]
    [SerializeField] private GachaResultUI gachaResultUI;
    
    private WeightedRandom<Rarity> rarityPicker;

    private int getCount = 0; //누적 뽑기 횟수
    private const int pityStart = 43; // 천장 뽑기
    private const float pityIncrease = 0.14f; //확률 증가

    private void Awake()
    {
        rarityPicker = new WeightedRandom<Rarity>();
        rarityPicker.Add(Rarity.Common, 98);
        rarityPicker.Add(Rarity.Rare, 2);
    }

    public CharacterData GetRandomCharacter()
    {
        getCount++;
        Debug.Log($"누적 {getCount}회");
        
        Rarity getRarity = rarityPicker.GetRandom(); //기본으로 먼저 뽑기

        if (getCount > pityStart && getRarity == Rarity.Common)
        {
            float pity = pityIncrease * (getCount - pityStart) * 100f;
            float roll = Random.Range(0f, 100f);
            if (roll < pity)
            {
                getRarity = Rarity.Rare;
            }
        }

        if (getRarity == Rarity.Rare)
            getCount = 0;
        
        List<CharacterData> getChracterList = chracterLists
            .Where(c => c.rarity == getRarity)
            .ToList();
        
        CharacterData selectChracter = getChracterList[Random.Range(0, getChracterList.Count)];
        
        Debug.Log($"가챠 결과 → {selectChracter.characterName} (등급: {selectChracter.rarity})");
        return selectChracter;
    }

    public void SingleGacha() //1회 뽑기
    {
        CharacterData result = GetRandomCharacter();
        gachaResultUI.Show(new List<CharacterData> { result });
    }

    public void TenGacha() //10회 뽑기
    {
        List<CharacterData> result = new List<CharacterData>();
        for (int i = 0; i < 10; i++)
        {
            result.Add(GetRandomCharacter());
        }
        gachaResultUI.Show(result);
    }
}
