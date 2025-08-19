using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : UnitBaseData
{
    [Header("Character Data Setting")]
    [SerializeField] public CharacterDataSO _characterData;  // 캐릭터 데이터

    [Header("Attack Unit List")]
    public List<MonsterController> _attackTargets = new();    // 사거리에 들어온 몬스터 데이터
    public MonsterController _attackTarget;    // 현재 공격 대상

    [Header("Research Unit Target")]
    public MonsterController _researchTarget;    // 현재 탬색 대상

    // 캐릭터 생성 초기화
    protected override void Init()
    {
        base._currentHp = _characterData._maxHp;
        base._moveDir = Vector3.right;
        base._isAlive = true;
        base._isAttack = false;
    }

    // 캐릭터 이동
    protected override void Movement()
    {
        // 타겟이 없으면 
        if (_researchTarget == null)
        {
            // 오른쪽으로 이동
            transform.Translate(_moveDir * _characterData._moveSpeed * Time.deltaTime);
        }
        else    // 탐색 대상이 있으면
        {
            // 공격 중이면 이동 정지
            if (_attackTarget != null && base._isAttack) return;

            // 탐색한 대상과 거리 확인
            float moveDistance = Vector3.Distance(transform.position, _researchTarget.transform.position);

            // 탐색 대상과의 거리가 공격 사거리 안에 들어올 때까지 접근
            if (moveDistance > _characterData._attackRange)
            {
                base._isAttack = false;
                // 탐색 대상으로 이동
                transform.position = Vector3.MoveTowards(transform.position, _researchTarget.transform.position, _characterData._moveSpeed * Time.deltaTime);
            }
        }
    }

    // 공격
    protected override void Attack()
    {
        // 타겟이 없으면 공격하지 않기
        if (_attackTarget == null) return;

        // 공격 전 대상 확인
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

        // 공격 대상의 거리가 캐릭터의 공격 사거리에 들어오면 타겟 공격
        if (attackDistance <= _characterData._attackRange)
        {
            if(base._attackCoolTimer <= 0f)
            {
                // 캐릭터의 데미지로 몬스터에 주기
                _attackTarget.TakeDamage(_characterData._attackDamage);

                base._isAttack = true;

                // 공격 쿨타임 초기화
                base._attackCoolTimer = _characterData._attackSpeed;
            }           
        }
    }

    // 캐릭터 스킬사용 (버튼으로 사용)
    private void UseSkill()
    {
        // 레어 캐릭터만 스킬 사용 가능
        if(_characterData._characterRating == characterRating.Rare)
        {
            // 보유한 스킬이 없거나 타겟이 없으면 미사용
            if (_characterData._skills == null || _attackTarget == null) return;

            // 보유한 스킬을 순회
            foreach(var skill in _characterData._skills)
            {
                if (skill._canUseSkill)
                {
                    // 스킬 사용
                    skill.UseSkill(transform, _attackTarget);

                    // 스킬 사용 쿨타임이 있으면 계산 
                }
            }
        }
    }

    // 캐릭터 사망
    protected override void Death()
    {
        base.Death();

        // 매니저에 사망 보고
        base._battleManager.CharacterDeathCheck();
    }
}
