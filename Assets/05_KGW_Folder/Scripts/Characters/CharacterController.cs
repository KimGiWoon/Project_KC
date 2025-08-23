using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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

    Coroutine _manaRoutine;
    bool _isManaFull;
    float _manaChageValue;
    WaitForSeconds _time;

    // 체력과 마나의 변화 이벤트
    public event Action<float> OnHpChange;
    public event Action<float> OnMpChange;
    // 스킬 사용 모드 변화 이벤트
    public event Action<bool> OnSkillModeChange;

    // 캐릭터 생성 초기화
    protected override void Init()
    {
        base._currentHp = _characterData._maxHp;
        base._currentMp = 0f;
        base._moveDir = Vector3.right;
        base._isAlive = true;
        base._isAttack = false;
        base._chaData = _characterData;

        _isManaFull = false;

        // 체력, 마나 게이지 현재값 초기화
        OnHpChange?.Invoke(_currentHp / _characterData._maxHp);
        OnMpChange?.Invoke(_currentMp / _characterData._maxMp);

        _manaChageValue = _characterData._recoveryMp;
        _time = new WaitForSeconds(1f);

        // 마나 충전 
        ManaRecovery();
    }

    // 캐릭터 이동
    protected override void Movement()
    {
        // 게임이 종료되면 움직이지 않는다.
        if (_battleManager._isGameOver) return;

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

    // 데미지를 받음
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // 체력 변화에 대한 이벤트 호출
        OnHpChange?.Invoke(Mathf.Clamp01(_currentHp / _characterData._maxHp));
    }

    // 마나 회복
    public void ManaRecovery()
    {
        // 캐릭터의 등급이 레어등급만 마나회복 가능
        if (_characterData._characterRating != characterRating.Rare) return;

        // 마나가 풀이면 회복 불가
        if (_isManaFull) return;

        _manaRoutine = StartCoroutine(ManaRecoveryCoroutine());
    }

    // 마나 회복 정지
    public void StopManaRecovery()
    {
        if (_manaRoutine != null)
        {
            StopCoroutine(_manaRoutine);
            _manaRoutine = null;
        }
    }

    // 캐릭터 스킬사용 (버튼으로 사용)
    public void UseSkill()
    {
        // 레어 캐릭터만 스킬 사용 가능
        if (_characterData._characterRating == characterRating.Rare)
        {
            // 보유한 스킬이 없거나 타겟이 없으면 미사용
            if (_characterData._skills == null || _attackTarget == null) return;

            // 보유한 스킬을 순회
            foreach (var skill in _characterData._skills)
            {
                // 스킬 사용
                skill.UseSkill(transform, _attackTarget);

                // 마나 초기화
                _currentMp = 0f;
                // 마나 변화에 대한 이벤트 호출
                OnMpChange?.Invoke(Mathf.Clamp01(_currentMp / _characterData._maxMp));

                // 마나 제로
                _isManaFull = false;
                // 스킬 사용 모드 전환 이벤트 호출
                OnSkillModeChange?.Invoke(_isManaFull);

                // 마나 회복
                _manaRoutine = StartCoroutine(ManaRecoveryCoroutine());
            }
        }
    }

    // 캐릭터 사망
    protected override void Death()
    {
        base.Death();

        StopManaRecovery();
        // 매니저에 사망 보고
        base._battleManager.CharacterDeathCheck();
    }

    // 마나 회복
    private IEnumerator ManaRecoveryCoroutine()
    {
        while (!_isManaFull)
        {
            yield return _time;

            _currentMp += _manaChageValue;

            // 게임이 종료되면 마나회복 중지
            if (_battleManager._isGameOver)
            {
                StopCoroutine(_manaRoutine);
            }

            // 마나 변화에 대한 이벤트 호출
            OnMpChange?.Invoke(Mathf.Clamp01(_currentMp / _characterData._maxMp));

            if (_currentMp >= _chaData._maxMp)
            {
                _isManaFull = true;
                _currentMp = _chaData._maxMp;

                // 스킬 사용 모드 전환 이벤트 호출
                OnSkillModeChange?.Invoke(_isManaFull);

                // 마나 변화에 대한 이벤트 호출
                OnMpChange?.Invoke(Mathf.Clamp01(_currentMp / _characterData._maxMp));
            }
        }

        _manaRoutine = null;
    }
}
