using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyBlade1Controller : MonoBehaviour
{
    private float _skillDamage;
    private MyCharacterController _character;
    private MonsterController _monster;

    public void Init(MonsterController caster, MyCharacterController target, float damageValue)
    {
        _skillDamage = caster._monsterState._monAttack * damageValue;
        _character = target;
        _monster = caster;

        AllCharacterMonkeyBlade();
    }

    // 전체 캐릭터에게 원숭이 검술 사용
    public void AllCharacterMonkeyBlade()
    {
        foreach (var cha in _character._battleManager._characters)
        {
            if (!cha._isAlive) continue;

            cha.TakeDamage(_skillDamage, _monster._monsterState._monAccuracy);
        }

        Destroy(gameObject);
    }
}
