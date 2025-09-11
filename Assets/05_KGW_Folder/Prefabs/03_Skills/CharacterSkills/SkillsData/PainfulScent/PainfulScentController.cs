using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainfulScentController : MonoBehaviour
{
    private float _skillDamage;
    private float _decreaseDownValue;
    private float _skillAttackHit;
    private float _skillTick;
    private float _reductionDownValue;
    private float _saveArmor;
    private float _activeSkillDuration;
    private bool _isArmorDown;
    private MyCharacterController _character;
    private MonsterController _monster;
    private Coroutine _attackRoutine;
    private WaitForSeconds _time;

    private Dictionary<MonsterController, float> _originalReduction = new Dictionary<MonsterController, float>();

    public void Init(MyCharacterController caster, MonsterController target, float hit, float damageValue, float reductionValue, float duration, float tick)
    {
        _skillDamage = damageValue * caster._characterState._chaAttack;
        _skillAttackHit = hit;
        _skillTick = tick;
        _reductionDownValue = reductionValue;
        _activeSkillDuration = duration;
        _saveArmor = target._monsterState._monArmor;
        _isArmorDown = false;
        _character = caster;
        _monster = target;
        _time = new WaitForSeconds(_skillTick);

        AllMonsterAttackSkill();
    }

    // 전체 몬스터에게 괴로운 향기 공격
    public void AllMonsterAttackSkill()
    {
        if (_isArmorDown) return;

        foreach (var mon in _monster._battleManager._monsters)
        {
            if (!mon._isAlive) continue;

            // 몬스터의 피해 감소 하강 값 저장
            _originalReduction[mon] = mon._monsterState._reductionDownValue;

            // 데미지 코루틴 시작
            _attackRoutine = StartCoroutine(MonsterAttackCoroutine(mon));

        }
        _isArmorDown = true;

        // 지속시간 후 피해 감소 원복
        Invoke(nameof(MonsterArmorReset), _activeSkillDuration / _character._gameSpeed);

        // 게임 종료가 되면 감소된 피래 감소 원복
        if (_character._battleManager._isGameOver)
        {
            MonsterArmorReset();
            AttackCoroutineStop();
        }
    }

    // 지속시간 후 감소된 방어력 원복 
    public void MonsterArmorReset()
    {
        foreach (var mon in _monster._battleManager._monsters)
        {
            if (_originalReduction.ContainsKey(mon))
            {
                mon._monsterState._reductionDownValue = _originalReduction[mon];
            }

            _isArmorDown = false;
        }

        Destroy(gameObject);
    }

    // 몬스터 스킬 다단 히트 공격 코루틴
    public IEnumerator MonsterAttackCoroutine(MonsterController mon)
    {
        float count = 0;

        while (count < _skillAttackHit)
        {
            mon.TakeDamage(_skillDamage, _character._characterState._chaAccuracy);
            count++;

            yield return _time;
        }

        // 몬스터의 피해 감소 하강
        mon._monsterState._reductionDownValue -= _reductionDownValue;
    }

    // 코루틴 정지
    public void AttackCoroutineStop()
    {
        if(_attackRoutine != null)
        {
            StopCoroutine( _attackRoutine );
            _attackRoutine = null;
        }
    }
}
