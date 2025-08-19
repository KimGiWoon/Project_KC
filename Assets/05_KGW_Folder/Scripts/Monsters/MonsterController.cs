using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class MonsterController : UnitBaseData
{
    [Header("Monster Data Setting")]
    [SerializeField] public MonsterDataSO _monsterData;  // 몬스터 데이터

    [Header("Attack Unit List")]
    public List<CharacterController> _attackTargets = new();    // 공격 사거리에 들어온 캐릭터 데이터
    public CharacterController _attackTarget;    // 현재 공격 대상

    [Header("Research Unit List")]
    public CharacterController _researchTarget;    // 현재 탬색 대상


    // 몬스터 생성 초기화
    protected override void Init()
    {
        base._currentHp = _monsterData._maxHp;
        base._moveDir = Vector3.left;
        base._isAlive = true;
        base._isAttack = false;
    }

    // 몬스터 이동
    protected override void Movement()
    {
        // 탐색 대상이 없으면 
        if (_researchTarget == null)
        {
            // 왼쪽으로 이동
            transform.Translate(_moveDir * _monsterData._moveSpeed * Time.deltaTime);
        }
        else    // 탐색 대상이 있으면
        {
            // 공격 중이면 이동 정지
            if (_attackTarget != null && base._isAttack) return;

            // 탐색한 대상과 거리 확인
            float moveDistance = Vector3.Distance(transform.position, _researchTarget.transform.position);

            // 공격 대상과의 거리가 공격 사거리 안에 들어올 때까지 접근
            if (moveDistance > _monsterData._attackRange)
            {
                base._isAttack = false;
                // 탐색 대상으로 이동
                transform.position = Vector3.MoveTowards(transform.position, _researchTarget.transform.position, _monsterData._moveSpeed * Time.deltaTime);
            }
        }
    }

    // 공격
    protected override void Attack()
    {
        // 타겟이 없으면 공격하지 않기
        if (_attackTarget == null) return;

        // 공격 후 캐릭터가 사망
        if (!_attackTarget._isAlive)
        {
            // 공격 대상에서 삭제
            _attackTargets.Remove(_attackTarget);
            // 공격 타겟 갱신
            _attackTarget = _attackTargets.Count > 0 ? _attackTargets[0] : null;
        }

        // 공격 쿨타임 계산
        base._attackCoolTimer -= Time.deltaTime;

        // 공격 타겟과 거리 비교
        float attackDistance = Vector3.Distance(transform.position, _attackTarget.transform.position);

        // 공격 대상의 거리가 몬스터의 공격 사거리에 들어오면 타겟 공격
        if (attackDistance <= _monsterData._attackRange && base._attackCoolTimer <= 0f)
        {
            // 몬스터의 데미지로 캐릭터에 주기
            _attackTarget.TakeDamage(_monsterData._attackDamage);

            base._isAttack = true;

            // 공격 쿨타임 초기화
            base._attackCoolTimer = _monsterData._attackSpeed;
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
