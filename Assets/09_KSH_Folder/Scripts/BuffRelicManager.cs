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
        if((effect & RelicEffect.chaHP) != 0) cha._maxHp += value;
       
    }

    private void Start()
    {
        
    }
}
