using System.Collections;
using System.Collections.Generic;
using KSH;
using UnityEngine;

public class BuffRelicManager : MonoBehaviour
{
    public void ApplyStat(CharacterDataSO cha, RelicEffect effect, int value)
    {
        if ((effect & RelicEffect.chaAttack) != 0) cha._attackDamage += value;
        if ((effect & RelicEffect.chaArmor) != 0) cha._attackDefense += value;
        if ((effect & RelicEffect.chaAtkSpeed) != 0) cha._attackSpeed += value;
        if ((effect & RelicEffect.chaHP) != 0) cha._maxHp += value;
        if ((effect & RelicEffect.chaMPRecovery) != 0) cha._maxMp += value;
        //TODO : 플레이어 치명타 데미지 연결해야함
        //if ((effect & RelicEffect.chaCritDmg) != 0)
    }

    public void ApplyRelicEffect(CharacterDataSO cha, List<RelicEffectValue> relicEffect)
    {
        foreach (var effect in relicEffect)
        {
            ApplyStat(cha, effect.effect, effect.value);
        }
    }
}
