using System.Collections;
using System.Collections.Generic;
using KSH;
using UnityEngine;

public class BuffRelicManager : MonoBehaviour
{
    [SerializeField] private CharacterState playerData;
    //TODO : 플레이어 데이터 받아와야 함
    public void ApplyStat(CharacterState cha, RelicEffect effect, int value) //기본 스탯 ++
    {
        if ((effect & RelicEffect.chaAttack) != 0)
        {
            cha._chaAttack += value;
            Debug.Log($"공격력 적용 +{value}");
        }

        if ((effect & RelicEffect.chaArmor) != 0)
        {
            cha._chaArmor += value;
            Debug.Log($"방어력 적용 +{value}");
        }

        if ((effect & RelicEffect.chaAtkSpeed) != 0)
        {
            cha._chaAtkSpeed += value;
            Debug.Log($"치명타 데미지 적용 +{value}");
        }

        if ((effect & RelicEffect.chaHP) != 0)
        {
            cha._chaMaxHP += value;
            Debug.Log($"최대 체력 적용 +{value}");
        }

        if ((effect & RelicEffect.chaMPRecovery) != 0)
        {
            cha._chaMaxMP += value;
            Debug.Log($"마나 적용 +{value}");
        }

        if ((effect & RelicEffect.chaCritDmg) != 0)
        {
            cha._chaCritDmg += value;
            Debug.Log($"치명타 데미지 적용 + {value}");
        }
    }

    public void ApplyRelicEffect(CharacterState cha, List<RelicEffectValue> relicEffect) //기본 스탯 적용
    {
        foreach (var effect in relicEffect)
        {
            ApplyStat(playerData, effect.effect, effect.value);
        }
    }
}
