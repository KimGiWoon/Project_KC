using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using TableForge.Demo;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class MonsterController : UnitBaseData
{
    [Header("Monster Data Setting")]
    [SerializeField]
    private MonsterDataSO _monsterData; // 몬스터 데이터
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
    public bool _isApplyPassive;
    private float _skill1Timer;
    private float _skill2Timer;
    private float _breakCount;
    private RecallPointProvider _recallPointProvider;
    private MonsterController _monster;
    private float _saveAttackValue;
    private Coroutine _bossBreakRoutine;
    public BattleManager Battle;
    private Animator _monAnimatior;

    // 체력 절반 이벤트
    public event Action OnHalfHp;
    public event Action OnRelicMonsterDeath;

    // 몬스터 애니메이션
    public readonly int Idle_Hash = Animator.StringToHash("Idle");
    public readonly int Walk_Hash = Animator.StringToHash("Walk");
    public readonly int Attack_Hash = Animator.StringToHash("Attack");
    public readonly int Death_Hash = Animator.StringToHash("Death");
    public readonly int Skill1_Hash = Animator.StringToHash("Skill1");
    public readonly int Skill2_Hash = Animator.StringToHash("Skill2");

    protected override void Awake()
    {
        base.Awake();

        _monster = GetComponent<MonsterController>();
        _monAnimatior = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (_monsterData.MonType == MonsterType.Boss)
            _battleManager.Buff.OnUseGroggyItem += ApplyGroggy;
    }

    private void OnDisable()
    {
        if (_monsterData.MonType == MonsterType.Boss)
            _battleManager.Buff.OnUseGroggyItem -= ApplyGroggy;
    }

    protected override void Update()
    {
        if (_battleManager._isGameOver || _battleUI._isOnMenu || _isStern) return;
        base.Update();


        UseSkill();
    }

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
        _monsterState._monEnName = _monsterData.MonEnName;
        _monsterState._monType = _monsterData.MonType;
        _monsterState._monLevel = _monsterData.MonLv;
        _monsterState._monBreak = _monsterData.MonBreak;
        _monsterState._monbreakGage = _monsterData.MonBreakGage;
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
        _monsterState._reductionUpValue = 0f;
        _monsterState._reductionDownValue = 0f;

        _monsterState._monHPIncrase = _monsterData.MonHPIncrase;
        _monsterState._monAttackIncrease = _monsterData.MonAttackIncrease;
        _monsterState._monArmorIncrease = _monsterData.MonArmorIncrease;
        _monsterState._monAvoidIncrease = _monsterData.MonAvoidIncrease;

        _monsterState._monActiveSkill_1 = _monsterData._monActiveSkill_1;
        _monsterState._monActiveSkill_2 = _monsterData._monActiveSkill_2;

        // 캐릭터 레벨 업 스텟 적용
        LevelUpStatUpdate();

        _moveDir = Vector3.left;
        _isAlive = true;
        _isApplyPassive = false;
        _skill1Timer = 0f;
        _skill2Timer = 0f;
        _breakCount = 0f;
        _monData = _monsterData;
        _attackCoolTimer = _monsterState._monAtkSpeed;
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

            _monAnimatior.Play(Idle_Hash);
        }
        else
        {
            // 탐색 대상이 없으면 
            if (_researchTarget == null)
            {
                // 이동 애니메이션
                _monAnimatior.Play(Walk_Hash);

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
                    // 이동 애니메이션
                    _monAnimatior.Play(Walk_Hash);

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
            if (attackDistance <= attackSpareDistance && _attackCoolTimer <= 0f)
            {
                // 캐릭터가 살아있으면 공격
                if (_attackTarget != null && _attackTarget._isAlive)
                {
                    // 공격 애니메이션
                    _monAnimatior.Play(Attack_Hash);

                    // 몬스터의 데미지로 캐릭터에 주기
                    _attackTarget.TakeDamage(_monsterState._monAttack, _monsterState._monAccuracy);

                    _isAttack = true;

                    if (_attackTarget != null)
                    {
                        // 캐릭터의 공격 타겟 전환
                        _attackTarget.AttackTargetChange(_monster);
                    }

                    // 공격 쿨타임 초기화
                    _attackCoolTimer = _monsterState._monAtkSpeed / _gameSpeed;
                }
            }
            else
            {
                _isAttack = false;
            }
        }
    }

    // 몬스터 레벨 업 스텟 적용
    private void LevelUpStatUpdate()
    {
        _monsterState._monCurrentHP = _monsterState._monCurrentHP +
                                      (_monsterState._monLevel - 1) * _monsterState._monHPIncrase * _monsterState._monCurrentHP;
        _monsterState._monMaxHP = _monsterState._monMaxHP +
                                  (_monsterState._monLevel - 1) * _monsterState._monHPIncrase * _monsterState._monMaxHP;
        _monsterState._monAttack = _monsterState._monAttack +
                                   (_monsterState._monLevel - 1) * _monsterState._monAttackIncrease * _monsterState._monAttack;
        _monsterState._monArmor = _monsterState._monArmor +
                                  (_monsterState._monLevel - 1) * _monsterState._monArmorIncrease * _monsterState._monArmor;
        _monsterState._monAvoid = _monsterState._monAvoid + (_monsterState._monLevel - 1) * _monsterState._monAvoidIncrease;
    }

    // 몬스터의 스킬
    public void UseSkill()
    {
        // 정예 몬스터 이상만 스킬 사용 가능
        if (_monsterData.MonType == MonsterType.Normal) return;
        // 타겟이 없으면 미사용
        if (_attackTarget == null) return;

        _skill1Timer += Time.deltaTime * _gameSpeed;
        _skill2Timer += Time.deltaTime * _gameSpeed;

        // 액티브 스킬1을 보유하고 있는지 확인
        if (_monsterState._monActiveSkill_1)
        {
            // 액티브 스킬1의 쿨타임 시간
            if (_monsterState._monActiveSkill_1._monSkillCd <= _skill1Timer)
            {
                Debug.Log("액티브 스킬 1");

                _isUseSkill = true;

                // 스킬1 애니메이션
                _monAnimatior.Play(Skill1_Hash);
                // 액티브 스킬1 사용
                _monsterState._monActiveSkill_1.UseSkill(_monster, _monsterState._monActiveSkill_1, _attackTarget);

                // 타이머 초기화
                _skill1Timer = 0f;
            }
            _isUseSkill = false;
        }
        // 액티브 스킬2을 보유하고 있는지 확인
        if (_monsterState._monActiveSkill_2)
        {
            // 액티브 스킬2의 쿨타임 시간
            if (_monsterState._monActiveSkill_2._monSkillCd <= _skill2Timer)
            {
                Debug.Log("액티브 스킬 2 사용");

                _isUseSkill = true;

                // 스킬1 애니메이션
                _monAnimatior.Play(Skill2_Hash);
                // 액티브 스킬2 사용
                _monsterState._monActiveSkill_2.UseSkill(_monster, _monsterState._monActiveSkill_2, _attackTarget);

                // 타이머 초기화
                _skill2Timer = 0f;
            }
            _isUseSkill = false;
        }
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

            // 스킬 사용
            _monsterData._recallSkills.UseSkill(_monster, _researchTarget, recallPoint);
        }
    }

    public override void TakeDamage(float damage, float hitRate)
    {
        // 공격 회피
        if (AttackEvasion(_monsterState._monAvoid, hitRate))
        {
            Debug.Log($"{_monsterState._monEnName}가 공격을 회피했습니다.");
            return;
        }

        // 캐릭터 넉백
        base.TakeDamage(damage, hitRate);

        // 데미지 받기 전 체력 저장
        float saveCurHp = _monsterState._monCurrentHP;

        // 최종데미지로 체력 감소
        _monsterState._monCurrentHP -= FinalDamage(damage, _monsterState._reductionUpValue, _monsterState._reductionDownValue);

        // 체력이 0이 됨
        if (_monsterState._monCurrentHP <= 0)
        {
            _monAnimatior.Play(Death_Hash);
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
            // 그로기 수치 상승
            _breakCount++;
            // 보스 그로기 확인
            BossBreakCheck();

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
        if (_battleManager._battleType == BattleEventType.Boss || _battleManager._battleType == BattleEventType.BossFinal) return;

        // 실제 줄어든 체력
        float decreaseHp = MathF.Max(0f, saveCurHp - _monsterState._monCurrentHP);

        // 실제 줄어든 체력 전달
        _battleManager.ReportMonsterDamage(decreaseHp);
    }

    // 몬스터 사망
    protected override void Death()
    {
        base.Death();
        OnRelicMonsterDeath?.Invoke();
        if (gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            _isDetect = false;

            // 매니저에 사망 보고
            _battleManager.MonsterDeathCheck();
        }
        else
        {
            // 보스전에서는 몬스터는 사망보고 하지 않음
            if (_battleManager._battleType == BattleEventType.Boss ||
                _battleManager._battleType == BattleEventType.BossFinal) return;

            // 매니저에 사망 보고
            _battleManager.MonsterDeathCheck();
        }
    }

    // 공격 대상 변경
    public void AttackTargetChange(MyCharacterController chaData)
    {
        // 공격한 캐릭터가 죽었으면 넘어가기
        if (chaData == null || !chaData.isActiveAndEnabled) return;

        // 공격 대상 전환
        _attackTarget = chaData;
    }

    // 전체 몬스터 회복
    public void MonsterHealApply(float healValue)
    {
        // 데미지 받기 전 체력 저장
        float saveCurHp = _monsterState._monCurrentHP;

        // 체력 회복
        _monsterState._monCurrentHP += healValue;

        // 현재 체력이 최대 체력보다 크면 최대 체력으로 세팅
        if (_monsterState._monCurrentHP >= _monsterState._monMaxHP)
        {
            _monsterState._monCurrentHP = _monsterState._monMaxHP;
        }

        // 보스가 아니면 개인 체력바 변화
        if (gameObject.layer != LayerMask.NameToLayer("Boss"))
        {
            // 체력 변화에 체력바 변화
            _monsterHp.value = _monsterState._monCurrentHP / _monsterState._monMaxHP;
        }

        // 실제 증가한 체력
        float increaseHp = _monsterState._monCurrentHP - saveCurHp;

        // 실제 증가한 체력 전달
        _battleManager.ReportMonsterHeal(increaseHp);
    }

    // 최종데미지 계산
    private float FinalDamage(float damage, float reducUpValue, float reducDownValue)
    {
        // 데미지 계산
        float reduction = _monsterState._monArmor / (_monsterState._monArmor + 100);
        float buffReduction = _monsterState._reductionUpValue - _monsterState._reductionDownValue;
        float finalReduction = MathF.Min(reduction + buffReduction, 0.95f);
        float finalDamage = damage * (1 - finalReduction);

        Debug.Log($"몬스터 방어력 : {_monsterState._monArmor}");
        Debug.Log(
            $"몬스터가 받은 데미지 계산 Reduction : {reduction}, BuffReduction : {buffReduction}, FinalReduction : {finalReduction}, FinalDamage : {finalDamage}");
        return finalDamage;
    }

    // 공격 회피
    private bool AttackEvasion(float avoid, float hitRate)
    {
        float evasionRate = (avoid - (hitRate - 100)) * 0.01f;

        // 회피율 0이하 1초과 금지
        if (evasionRate < 0)
        {
            evasionRate = 0f;
        }
        else if (evasionRate >= 1)
        {
            evasionRate = 1f;
        }

        Debug.Log($"{_monsterState._monEnName} 회피율 : {evasionRate}");

        // 회피 가능 확인
        bool isEvasion = UnityEngine.Random.value < evasionRate ? true : false;

        return isEvasion;
    }

    // 보스 몬스터 그로기 확인
    private void BossBreakCheck()
    {
        if (_breakCount >= _monsterState._monbreakGage)
        {
            _isStern = true;
            // 보스 그로기 타임
            _bossBreakRoutine = StartCoroutine(BossBreakCoroutine());
        }
    }

    // 보스 그로기 코루틴
    private IEnumerator BossBreakCoroutine()
    {
        Debug.Log("보스가 그로기 상태 입니다.");

        yield return new WaitForSeconds(2f);

        Debug.Log("보스가 그로기 상태가 끝났습니다.");

        // 그로기 초기화
        _isStern = false;
        _breakCount = 0f;
    }

    private void ApplyGroggy(float value)
    {
        _breakCount *= 1 + value;
        BossBreakCheck();
    }

    #region 캐릭터의 패시브 스킬 효과

    // 사기 저하 패시브 스킬
    public void AttackDownPassive(float saveAttack, float attackDownValue, float duration)
    {
        // 지속 시간 중 중복 적용 방지
        if (!_isApplyPassive)
        {
            _saveAttackValue = saveAttack;
            // 공격한 몬스터의 공격력 감소
            _monsterState._monAttack -= _monsterState._monAttack * attackDownValue;
            _isApplyPassive = true;

            // 사기 저하 원복
            Invoke(nameof(AttackDownPassiveRestoration), duration / _gameSpeed);
        }
    }

    // 사기 저하 효과 원복
    private void AttackDownPassiveRestoration()
    {
        if (_isApplyPassive)
        {
            _isApplyPassive = false;
            _monsterState._monAttack = _saveAttackValue;
        }
    }

    #endregion
}