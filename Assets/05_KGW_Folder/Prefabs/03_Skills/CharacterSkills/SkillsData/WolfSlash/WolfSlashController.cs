using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSlashController : MonoBehaviour
{
    private float _skillDamage;
    private float _skillAttackHit;
    private float _skillTick;
    private MyCharacterController _character;
    private MonsterController _monster;
    private Coroutine _attackRoutine;
    private WaitForSeconds _time;

    public void Init(MyCharacterController caster, MonsterController target, float hit, float damageValue, float armorValue, float tick)
    {
        _skillDamage = damageValue * caster._characterState._chaAttack;
        _skillAttackHit = hit;
        _skillTick = tick;
        _character = caster;
        _monster = target;
        _time = new WaitForSeconds(_skillTick);

        AllMonsterWolfSlash();
    }

    // 전체 몬스터에게 늑대 베기 사용
    public void AllMonsterWolfSlash()
    {
        foreach (var mon in _monster._battleManager._monsters)
        {
            if (!mon._isAlive) continue;
            // 데미지 코루틴 시작
            _attackRoutine = StartCoroutine(MonsterAttackCoroutine(mon));
        }

        if (_character._battleManager._isGameOver)
        {
            AttackCoroutineStop();
        }

    }

    // 몬스터 스킬 다단 히트 공격 코루틴
    public IEnumerator MonsterAttackCoroutine(MonsterController mon)
    {
        float count = 0;

        while (count < _skillAttackHit)
        {
            mon.TakeDamage(_skillDamage, _character._characterState._chaAccuracy);
            count++;

            // 공격한 만큼 캐릭터의 체력 회복
            _character.CharacterHealApply(_skillDamage);

            yield return _time;
        }

        Destroy(gameObject, 0.5f);
    }

    // 코루틴 정지
    public void AttackCoroutineStop()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }
}
