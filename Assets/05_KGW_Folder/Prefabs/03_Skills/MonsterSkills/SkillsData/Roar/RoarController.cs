using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarController : MonoBehaviour
{
    private float _attackValue;
    private float _damageReductionDownValue;
    private float _activeSkillDuration;
    private MonsterController _monster;
    private MyCharacterController _character;

    private Dictionary<MyCharacterController, float> _originalReductionValue = new Dictionary<MyCharacterController, float>();

    public void Init(MonsterController caster, MyCharacterController target, float duration, float attackValue, float reductionDownValue)
    {
        _attackValue = caster._monsterState._monAttack * attackValue;
        _damageReductionDownValue = caster._monsterState._monAttack * reductionDownValue;
        _activeSkillDuration = duration;
        _monster = caster;
        _character = target;

        AllCharacterRoarAttack();
    }

    // 전체 포효 캐릭터 공격
    public void AllCharacterRoarAttack()
    {
        foreach (var cha in _character._battleManager._characters)
        {
            if (!cha._isAlive) continue;

            // 몬스터의 원래 피해감소 저장
            _originalReductionValue[cha] = cha._characterState._chaArmor;

            cha.TakeDamage(_attackValue);

            // 피해감소율 감소 추가 예정
        }

        Invoke(nameof(CharacterReductionReset), _activeSkillDuration);

        // 게임 종료가 되면 상승된 공격력 원복
        if (_monster._battleManager._isGameOver)
        {
            CharacterReductionReset();
        }
    }

    // 지속시간 후 감소된 피해감소율 원복
    public void CharacterReductionReset()
    {
        foreach (var cha in _character._battleManager._characters)
        {
            if (_originalReductionValue.ContainsKey(cha))
            {
                cha._characterState._chaArmor = _originalReductionValue[cha];
            }
        }

        Destroy(gameObject);
    }
}
