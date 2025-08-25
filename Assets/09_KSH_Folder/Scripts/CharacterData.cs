using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Gacha/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName; //캐릭터 이름
    public Sprite characterImage; //캐릭터 사진
    public Rarity rarity; //캐릭터 등급
}

public enum Rarity
{
    Common,
    Rare
}
