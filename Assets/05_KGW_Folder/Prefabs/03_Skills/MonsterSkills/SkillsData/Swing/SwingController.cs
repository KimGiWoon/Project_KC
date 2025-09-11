using UnityEngine;

public class SwingController : MonoBehaviour
{
    private float _skillDamage;
    private MyCharacterController _character;
    private MonsterController _monster;

    public void Init(MonsterController caster, MyCharacterController target, float damageValue)
    {
        _skillDamage = caster._monsterState._monAttack * damageValue;
        _character = target;
        _monster = caster;

        AllCharacterSavageSlash();
    }

    // 전체 캐릭터에게 휘두르기 사용
    public void AllCharacterSavageSlash()
    {
        foreach (var cha in _character._battleManager._characters)
        {
            if (!cha._isAlive) continue;

            cha.TakeDamage(_skillDamage, _monster._monsterState._monAccuracy);
        }

        Destroy(gameObject);
    }
}
