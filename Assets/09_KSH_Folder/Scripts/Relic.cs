using UnityEngine;

namespace KSH
{
    
}
[CreateAssetMenu(fileName = "Relic", menuName = "Relic/Relic Data")]
public class Relic : ScriptableObject
{
    [Header("유물 이름")]
    public string relicName;
    
    [Header("유물 이미지")]
    public Sprite relicImage;
    
    [Header("유물 설명")]
    public string relicDescription1;
    public string relicDescription2;
    
    [Header("유물 등급")]
    public RelicRarity relicRarity;
}

public enum RelicRarity
{
    Normal,
    Rare,
    Deburff
}

public enum RelicType
{
    Buff,
    Debuff,
}