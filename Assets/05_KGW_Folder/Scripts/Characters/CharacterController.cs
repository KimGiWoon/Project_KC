using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : UnitBaseData
{
    [Header("Character Data Setting")]
    [SerializeField] public CharacterDataSO _characterData;  // 캐릭터 데이터

    public List<MonsterController> _attackTargets = new();    // 사거리에 들어온 몬스터 데이터
    public MonsterController _currentTarget;    // 현재 공격 대상

    // 캐릭터 생성 초기화
    protected override void Init()
    {
        base._currentHp = _characterData._maxHp;
        base._moveDir = Vector3.right;  
    }

    // 캐릭터 이동
    protected override void Movement()
    {
        // 타겟이 없으면 
        if (_currentTarget == null)
        {
            // 오른쪽으로 이동
            transform.Translate(_moveDir * _characterData._moveSpeed * Time.deltaTime);
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
            // 캐릭터의 데미지로 몬스터에 주기
            _currentTarget.TakeDamage(_characterData._attackDamage);

            Debug.Log($"{_currentTarget}을 {_characterData._attackDamage}의 데미지로 공격했습니다.");

            // 공격 쿨타임 초기화
            base._attackCoolTimer = _characterData._attackSpeed;

            // 공격한 몬스터가 사망
            if (_currentTarget == null)
            {
                // 공격 대상에서 삭제
                _attackTargets.Remove( _currentTarget );

                // 공격 타겟 갱신
                _currentTarget = _attackTargets.Count > 0 ? _attackTargets[0] : null;
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
            if (_characterData._skills == null && _currentTarget == null) return;

            // 보유한 스킬을 순회
            foreach(var skill in _characterData._skills)
            {
                if (skill._canUseSkill)
                {
                    // 스킬 사용
                    skill.UseSkill(transform, _currentTarget);

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
