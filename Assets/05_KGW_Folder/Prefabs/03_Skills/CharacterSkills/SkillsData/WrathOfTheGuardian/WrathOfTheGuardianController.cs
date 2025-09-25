using System.Collections;
using SDW;
using UnityEngine;

public class WrathOfTheGuardianController : MonoBehaviour
{
    private float _skillDamage;
    private float _skillAttackHit;
    private float _skillTick;
    private MonsterController _monster;
    private MyCharacterController _character;
    private Coroutine _attackRoutine;
    private WaitForSeconds _time;

    public void Init(MyCharacterController caster, MonsterController target, float hit, float damageValue, float tick)
    {
        _skillDamage = damageValue * caster._characterState._chaAttack;
        _skillAttackHit = hit;
        _skillTick = tick;
        _character = caster;
        _monster = target;
        _time = new WaitForSeconds(_skillTick);

        SingleMonsterAttack();
    }

    // 단일 몬스터에게 스킬 공격
    public void SingleMonsterAttack()
    {
        if (!_monster._isAlive) return;

        // 수호령의 분노 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.GSEActiveSkill);

        // 데미지 코루틴 시작
        _attackRoutine = StartCoroutine(MonsterAttackCoroutine(_monster));

        // 게임 종료가 되면 공격 코루틴 정지
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
