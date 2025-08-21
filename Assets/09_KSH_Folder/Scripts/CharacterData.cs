using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Gacha/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterImage;
    public Rarity rarity;
}

public enum Rarity
{
    Common,
    Rare
}
