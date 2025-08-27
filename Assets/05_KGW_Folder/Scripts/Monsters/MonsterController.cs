using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : UnitBaseData
{
    [Header("Monster Data Setting")]
    [SerializeField] public MonsterDataSO _monsterData;  // 몬스터 데이터
    [SerializeField] Slider _monsterHp;

    [Header("Attack Unit List")]
    public List<MyCharacterController> _attackTargets = new();    // 공격 사거리에 들어온 캐릭터 데이터
    public MyCharacterController _attackTarget;    // 현재 공격 대상

    [Header("Research Unit List")]
    public MyCharacterController _researchTarget;    // 현재 탬색 대상

    public bool _isDetect;
    float _time;
    public bool _isFirst;
    RecallPointProvider _recallPointProvider;

    protected override void Update()
    {
        base.Update();

        // 보스몬스터만 사용
        if (_isDetect && gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            _time += Time.deltaTime;

            // 스킬 쿨타임이 지나면
            if(_time >= (_monsterData._useSkillTime / _gameSpeed))
            {
                if (_isFirst)
                {
                    UseSkill();
                }

                // 타이머 초기화
                _time = 0f;
            }
        }
    }

    // 몬스터 생성 초기화
    protected override void Init()
    {
        base._currentHp = _monsterData._maxHp;
        base._moveDir = Vector3.left;
        base._isAlive = true;
        base._isAttack = false;
        base._monData = _monsterData;
        _recallPointProvider = GetComponent<RecallPointProvider>();

        _isDetect = false;
        _time = 0f;
        _isFirst = false;

        if (_monsterData._monsterRating == MonsterRating.Common || _monsterData._monsterRating == MonsterRating.Elite)
        {
            // 체력 게이지 최소, 최대값 초기화
            if (_monsterHp) { _monsterHp.minValue = 0; _monsterHp.maxValue = 1; }

            // 현재 체력으로 세팅
            _monsterHp.value = _currentHp / _monsterData._maxHp;
        }
    }

    // 몬스터 이동
    protected override void Movement()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            // 보스는 움직이지 않습니다.
            _moveDir = Vector3.zero;
        }
        else
        {
            // 탐색 대상이 없으면 
            if (_researchTarget == null)
            {
                // 왼쪽으로 이동
                transform.Translate(_moveDir * _monsterData._moveSpeed * _gameSpeed * Time.deltaTime);
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
                    transform.position = Vector3.MoveTowards(transform.position, _researchTarget.transform.position, _monsterData._moveSpeed * _gameSpeed * Time.deltaTime);
                }
            }
        }
    }

    // 공격
    protected override void Attack()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            // 보스 몬스터는 공격하지 않는다
            return;
        }
        else
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
                base._attackCoolTimer = _monsterData._attackSpeed / _gameSpeed;
            }
        }
    }

    // 보스 몬스터 스킬사용 (적을 감지 하면 사용)
    public void UseSkill()
    {
        _isFirst = true;

        // 보스 몬스터만 스킬 사용 가능
        if (_monsterData._monsterRating == MonsterRating.LocalBoss || _monsterData._monsterRating == MonsterRating.FinalBoss)
        {
            Transform[] recallPoint = _recallPointProvider._points;
            // 보유한 스킬이 없으면 미사용
            if (_monsterData._skills == null) return;

            // 보유한 스킬을 순회
            foreach (var skill in _monsterData._skills)
            {
                Debug.Log("몬스터 소환 스킬 사용");

                // 스킬 사용
                skill.UseSkill(transform, _researchTarget, recallPoint);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        // 데미지 받기 전 체력 저장
        float saveCurHp = _currentHp;

        base.TakeDamage(damage);

        // 보스가 아니면 개인 체력바 변화
        if(gameObject.layer != LayerMask.NameToLayer("Boss"))
        {
            // 체력 변화에 체력바 변화
            _monsterHp.value = _currentHp / _monsterData._maxHp;
        }
        else    // 보스이면 통합 체력 변화
        {
            // 실제 줄어든 체력
            float decreaseBossHp = MathF.Max(0f, saveCurHp - _currentHp);

            // 실제 줄어든 체력 전달
            _battleManager.ReportMonsterDamage(decreaseBossHp);
        }

        // 보스전이면 생성된 몬스터는 통합체력에 영향을 주면 안됨
        if (_battleManager._isBoss) return;

        // 실제 줄어든 체력
        float decreaseHp = MathF.Max(0f, saveCurHp - _currentHp);

        // 실제 줄어든 체력 전달
        _battleManager.ReportMonsterDamage(decreaseHp);
    }

    // 몬스터 사망
    protected override void Death()
    {
        base.Death();

        if(gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            _isDetect = false;

            // 매니저에 사망 보고
            base._battleManager.MonsterDeathCheck();
        }
        else
        {
            if(_battleManager._isBoss) return;

            // 매니저에 사망 보고
            base._battleManager.MonsterDeathCheck();
        }
    }
}
