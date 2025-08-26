using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChangeManager : SingletonManager<RewardChangeManager>
{
    private Dictionary<string, bool> ownedCharacters = new Dictionary<string, bool>();
    private Dictionary<string, int> beadsInventory = new Dictionary<string, int>();

    public int starCandy = 0;
    private int beadMax = 6; //구슬 최대 갯수

    private int normalReward = 30;
    private int RareReward = 2000;

    protected override void Awake()
    {
        base.Awake();
    }
    public void ProcessCharacter(CharacterData character)
    {
        if (ownedCharacters.ContainsKey(character.characterName))
        {
            if(!beadsInventory.ContainsKey(character.characterName))
                beadsInventory[character.characterName] = 0;
            
            beadsInventory[character.characterName]++;
            
            if (beadsInventory[character.characterName] > beadMax)
            {
                beadsInventory[character.characterName] = beadMax;
                
                int reward = (character.rarity == Rarity.Rare) ? RareReward : normalReward;
                starCandy += reward;
                Debug.Log($"{character.characterName} 구슬 6개 초과하였으므로 별사탕 {reward}개 획득!");
            }
            else
            {
                Debug.Log($"{character.characterName}이 중복이므로 구슬 1개 획득!");
            }
        }
        else
        {
            ownedCharacters[character.characterName] = true;
            Debug.Log($"{character.characterName} 획득!");
        }
    }
}
    