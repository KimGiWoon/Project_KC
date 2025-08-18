using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : UnitBaseData
{
    [Header("Monster Data Setting")]
    [SerializeField] public MonsterDataSO _monsterData;  // 몬스터 데이터

    public List<CharacterController> _attackTargets = new();    // 사거리에 들어온 캐릭터 데이터
    public CharacterController _currentTarget;    // 현재 공격 대상

    // 몬스터 생성 초기화
    protected override void Init()
    {
        base._currentHp = _monsterData._maxHp;
        base._moveDir = Vector3.left;
    }

    // 몬스터 이동
    protected override void Movement()
    {
        // 타겟이 없으면 
        if (_currentTarget == null)
        {
            // 왼쪽으로 이동
            transform.Translate(_moveDir * _monsterData._moveSpeed * Time.deltaTime);
        }
    }

    // 공격
    protected override void Attack()
    {
        // 타겟이 없으면 공격하지 않기
        if (_currentTarget == null) return;

        // 공격 쿨타임 계산
        base._attackCoolTimer -= Time.deltaTime;

        // 타겟 공격
        if (base._attackCoolTimer <= 0f)
        {
            // 몬스터의 데미지로 캐릭터에 주기
            _currentTarget.TakeDamage(_monsterData._attackDamage);

            Debug.Log($"{_currentTarget}을 {_monsterData._attackDamage}의 데미지로 공격했습니다.");

            // 공격 쿨타임 초기화
            base._attackCoolTimer = _monsterData._attackSpeed;

            // 공격한 캐릭터가 사망
            if (_currentTarget == null)
            {
                // 공격 대상에서 삭제
                _attackTargets.Remove(_currentTarget);

                // 공격 타겟 갱신
                _currentTarget = _attackTargets.Count > 0 ? _attackTargets[0] : null;
            }
        }
    }

    // 몬스터 사망
    protected override void Death()
    {
        base.Death();

        // 매니저에 사망 보고
        base._battleManager.MonsterDeathCheck();
    }
}
