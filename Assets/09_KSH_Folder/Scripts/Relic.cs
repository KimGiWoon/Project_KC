using UnityEngine;
using System.Collections.Generic;

namespace KSH
{
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
        
        [Header("유물 효과 타입")]
        public RelicEffectType relicEffectType;
        
        [Header("유물 등급")]
        public RelicRarity relicRarity;

        [Header("발동 조건")] 
        public bool isPassive;
    
        [Header("유물 발동 타입")]
        public RelicType relicType;
    
        [Header("발동 가능 역할군")]
        public RelicRole relicRole;
        
        [Header("발동 대상")]
        public RelicTarget relicTarget;
    
        [Header("유물 효과")]
        public List<RelicEffectValue> relicEffectValues;
    }
    
    [System.Serializable]
    public struct RelicEffectValue
    {
        public RelicEffect effect; 
        public int value;          
    }

    public enum RelicEffectType
    {
        BuffType,
        DeburffType,
    }

    public enum RelicRarity
    {
        Normal,
        Rare,
        None
    }

    public enum RelicType
    {
        None,
        ActiveSkill
    }

    public enum RelicRole
    {
        None
    }

    public enum RelicTarget
    {
        Character,
        Monster,
        Store
    }

    [System.Flags]
    public enum RelicEffect
    {
        None = 0,
        chaAttack = 1 << 0,
        chaArmor = 1 << 1,
        chaHP = 1 << 2,
        chaCritDmg  = 1 << 3,
        chaAtkSpeed = 1 << 4,
        chaMPRecovery = 1 << 5,
    }
}