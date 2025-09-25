using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

public class BrutalSlashController : MonoBehaviour
{
    private float _skillDamage;
    private float _skillAttackHit;
    private float _skillTick;
    private MyCharacterController _character;
    private MonsterController _monster;
    private Coroutine _attackRoutine;
    private WaitForSeconds _time;

    public void Init(MonsterController caster, MyCharacterController target, float hit, float damageValue, float tick)
    {
        _skillDamage = caster._monsterState._monAttack * damageValue;
        _skillAttackHit = hit;
        _skillTick = tick;
        _character = target;
        _monster = caster;
        _time = new WaitForSeconds(_skillTick);

        AllCharacterSavageSlash();
    }

    // 공격 대상에게 난폭한 베기 사용
    public void AllCharacterSavageSlash()
    {
        if (!_character._isAlive) return;

        // 난폭한 베기 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.BladeMonkeySkill_1);

        // 데미지 코루틴 시작
        _attackRoutine = StartCoroutine(CharacterAttackCoroutine(_character));

        if (_character._battleManager._isGameOver)
        {
            AttackCoroutineStop();
        }

    }

    // 난폭한 베기 스킬 공격 코루틴
    public IEnumerator CharacterAttackCoroutine(MyCharacterController cha)
    {
        float count = 0;

        while (count < _skillAttackHit)
        {
            cha.TakeDamage(_skillDamage, _monster._monsterState._monAccuracy);
            count++;

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
