using System;
using System.Collections.Generic;
using SDW;
using TableForge.Demo;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : UnitBaseData
{
    [Header("Monster Data Setting")]
    [SerializeField] public MonsterDataSO _monsterData; // 몬스터 데이터
    [SerializeField] private Slider _monsterHp;

    [Header("Attack Unit List")]
    public List<MyCharacterController> _attackTargets = new List<MyCharacterController>(); // 공격 사거리에 들어온 캐릭터 데이터
    public MyCharacterController _attackTarget; // 현재 공격 대상

    [Header("Research Unit List")]
    public MyCharacterController _researchTarget; // 현재 탬색 대상

    // 몬스터의 상태
    public MonsterState _monsterState;

    public bool _isDetect;
    public bool _isFirst;
    private RecallPointProvider _recallPointProvider;

    // 체력 절반 이벤트
    public event Action OnHalfHp;

    private void OnDestroy()
    {
        // 보스 체력 절반에서의 소환스킬 사용 관련 이벤트 구독 해제
        OnHalfHp -= UseRecallSkill;
    }

    // 몬스터 생성 초기화
    protected override void Init()
    {
        _monsterState._monID = _monsterData.MonId;
        _monsterState._monName = _monsterData.MonName;
        _monsterState._monLevel = _monsterData.MonLv;
        _monsterState._monCurrentHP = _monsterData.MonHP;
        _monsterState._monMaxHP = _monsterData.MonHP;
        _monsterState._monAtkRange = _monsterData.MonAtkRange;
        _monsterState._monAttack = _monsterData.MonAttack;
        _monsterState._monAtkSpeed = _monsterData.MonAtkSpeed;
        _monsterState._monMoveSpeed = _monsterData.MonMoveSpeed;
        _monsterState._monArmor = _monsterData.MonArmor;
        _monsterState._monAccuracy = _monsterData.MonAccuracy;
        _monsterState._monAvoid = _monsterData.MonAvoid;
        _monsterState._monReg = _monsterData.MonReg;

        _moveDir = Vector3.left;
        _isAlive = true;
        _monData = _monsterData;
        _recallPointProvider = GetComponent<RecallPointProvider>();

        // 보스 체력 절반에서의 소환스킬 사용 관련 이벤트 구독
        OnHalfHp += UseRecallSkill;

        if (_monsterData.MonType == MonsterType.Normal || _monsterData.MonType == MonsterType.Elite)
        {
            // 체력 게이지 최소, 최대값 초기화
            if (_monsterHp)
            {
                _monsterHp.minValue = 0;
                _monsterHp.maxValue = 1;
            }

            // 현재 체력으로 세팅
            _monsterHp.value = _monsterState._monCurrentHP / _monsterState._monMaxHP;
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
                transform.Translate(_moveDir * _monsterState._monMoveSpeed * _gameSpeed * Time.deltaTime);
            }
            else // 탐색 대상이 있으면
            {
                // 공격 중이면 이동 정지
                if (_attackTarget != null && _isAttack) return;

                // 이동 여유 거리
                float moveSpareDistance = _monsterState._monAtkRange * 0.8f;

                // 탐색한 대상과 거리 확인
                float moveDistance = Vector3.Distance(transform.position, _researchTarget.transform.position);

                // 공격 대상과의 거리가 공격 사거리 안에 들어올 때까지 접근
                if (moveDistance > moveSpareDistance)
                {
                    // 탐색 대상으로 이동
                    transform.position = Vector3.MoveTowards(transform.position, _researchTarget.transform.position,
                        _monsterState._monMoveSpeed * _gameSpeed * Time.deltaTime);
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
            _attackCoolTimer -= Time.deltaTime;

            // 공격 여유 사거리
            float attackSpareDistance = _monsterState._monAtkRange * 0.9f;

            // 공격 타겟과 거리 비교
            float attackDistance = Vector3.Distance(transform.position, _attackTarget.transform.position);

            // 공격 대상의 거리가 몬스터의 공격 사거리에 들어오면 타겟 공격
            if (attackDistance < attackSpareDistance && _attackCoolTimer <= 0f)
            {
                // 몬스터의 데미지로 캐릭터에 주기
                _attackTarget.TakeDamage(_monsterState._monAttack);

                _isAttack = true;

                // 공격 쿨타임 초기화
                _attackCoolTimer = _monsterState._monAtkSpeed / _gameSpeed;
            }
            else
            {
                _isAttack = false;
            }
        }
    }

    // 몬스터의 스킬
    public void UseSkill()
    {
        
    }

    // 보스 몬스터 소환 스킬사용 (적을 감지 하면 사용)
    public void UseRecallSkill()
    {
        _isFirst = true;

        // 보스 몬스터만 스킬 사용 가능
        if (_monsterData.MonType == MonsterType.Boss)
        {
            var recallPoint = _recallPointProvider._points;
            // 보유한 스킬이 없으면 미사용
            if (_monsterData._recallSkills == null) return;

            // 보유한 스킬을 순회
            foreach (var skill in _monsterData._recallSkills)
            {
                // 스킬 사용
                skill.UseSkill(transform, _researchTarget, recallPoint);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // 데미지 받기 전 체력 저장
        float saveCurHp = _monsterState._monCurrentHP;

        // 데미지를 받음, 방어력에 대한 것은??
        _monsterState._monCurrentHP -= damage;

        // 체력이 0이 됨
        if (_monsterState._monCurrentHP <= 0)
        {
            // 유닛의 죽음
            Death();
        }

        // 보스가 아니면 개인 체력바 변화
        if (gameObject.layer != LayerMask.NameToLayer("Boss"))
        {
            // 체력 변화에 체력바 변화
            _monsterHp.value = _monsterState._monCurrentHP / _monsterState._monMaxHP;
        }
        else // 보스이면 통합 체력 변화
        {
            // 실제 줄어든 체력
            float decreaseBossHp = MathF.Max(0f, saveCurHp - _monsterState._monCurrentHP);

            // 실제 줄어든 체력 전달
            _battleManager.ReportMonsterDamage(decreaseBossHp);

            float halfHp = _monsterState._monMaxHP / 2;

            // 체력 절반 확인
            if (_monsterState._monCurrentHP <= halfHp)
            {
                // 그로기 상태이면 스킬 사용 금지
                if (_isStern) return;

                // 체력이 절반 시 스킬 1회 사용
                if (!_isHalfHpSkill)
                {
                    _isHalfHpSkill = true;

                    // 이벤트 호출
                    OnHalfHp?.Invoke();
                }
            }
        }

        // 보스전이면 생성된 몬스터는 통합체력에 영향을 주면 안됨
        if (_battleManager._isLastBoss || _battleManager._isLocalBoss) return;

        // 실제 줄어든 체력
        float decreaseHp = MathF.Max(0f, saveCurHp - _monsterState._monCurrentHP);

        // 실제 줄어든 체력 전달
        _battleManager.ReportMonsterDamage(decreaseHp);
    }

    // 몬스터 사망
    protected override void Death()
    {
        base.Death();

        if (gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            _isDetect = false;

            // 매니저에 사망 보고
            _battleManager.MonsterDeathCheck();
        }
        else
        {
            // 보스전에서는 몬스터는 사망보고 하지 않음
            if (_battleManager._isLastBoss || _battleManager._isLocalBoss) return;

            // 매니저에 사망 보고
            _battleManager.MonsterDeathCheck();
        }
    }
}