using UnityEngine;

public class StompController : MonoBehaviour
{
    private float _skillDamage;
    private MyCharacterController _character;

    public void Init(MonsterController caster, MyCharacterController target, float damageValue)
    {
        _skillDamage = caster._monsterState._monAttack * damageValue;
        _character = target;

        AllCharacterStomp();
    }

    // 전체 캐릭터에게 발구르기 사용
    public void AllCharacterStomp()
    {
        foreach (var cha in _character._battleManager._characters)
        {
            if (!cha._isAlive) continue;

            cha.TakeDamage(_skillDamage);
            Debug.Log($"{cha._characterState._chaEnName}에게 {_skillDamage}의 데미지를 주었습니다.");
        }

        Destroy(gameObject);
    }
}
