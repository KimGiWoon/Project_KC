using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PainfulScentController : MonoBehaviour
{
    private float _skillDamage;
    private float _decreaseDownValue;
    private float _skillAttackHit;
    private float _skillTick;
    private float _armorDownValue;
    private float _saveArmor;
    private float _activeSkillDuration;
    private bool _isArmorDown;
    private MyCharacterController _character;
    private MonsterController _monster;
    private Coroutine _attackRoutine;
    private WaitForSeconds _time;

    public void Init(MyCharacterController caster, MonsterController target, float hit, float damageValue, float armorValue, float duration, float tick)
    {
        _skillDamage = damageValue * caster._characterState._chaAttack;
        _skillAttackHit = hit;
        _skillTick = tick;
        _armorDownValue = armorValue;
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

            // 방어력이 마이너스로 가는걸 방지
            _decreaseDownValue = Mathf.Min(_saveArmor, _armorDownValue);

            mon._monsterState._monArmor -= _decreaseDownValue;

            // 데미지 코루틴 시작
            _attackRoutine = StartCoroutine(MonsterAttackCoroutine(mon));
        }
        _isArmorDown = true;

        Invoke(nameof(MonsterArmorReset), _activeSkillDuration);

        // 게임 종료가 되면 감소된 방어력 원복
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
            mon._monsterState._monArmor = _saveArmor;

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
            mon.TakeDamage(_skillDamage);
            count++;

            yield return _time;
        }
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
