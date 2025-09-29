using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

public class MyCharacterController : UnitBaseData
{
    [Header("Character Data Setting")]
    [SerializeField]
    private CharacterDataSO _characterData; // 캐릭터 데이터
    // 스폰된 위치 저장
    [SerializeField] public Transform _spawnedPoint;

    [Header("Attack Unit List & Controller")]
    [SerializeField]
    private CharacterAttackController _attackController;
    public List<MonsterController> _attackTargets = new List<MonsterController>(); // 사거리에 들어온 몬스터 데이터
    public MonsterController _attackTarget; // 현재 공격 대상

    [Header("Research Unit Target")]
    public MonsterController _researchTarget; // 현재 탬색 대상

    // 캐릭터의 상태
    public CharacterState _characterState;

    private Coroutine _manaRoutine;
    private float _manaChangeValue;
    private bool _isFirstAttack;
    private MyCharacterController _character;
    private Animator _chaAnimatior;

    // 체력과 마나의 변화 이벤트
    public event Action<float> OnHpChange;
    public event Action<float> OnMpChange;

    // 스킬 사용 모드 변화 이벤트
    public event Action<bool> OnSkillModeChange;

    // 유물 효과 적용 이벤트
    public event Action OnRelicEffect;
    public event Action OnRelicAttack;

    // 캐릭터 애니메이션
    public readonly int Idle_Hash = Animator.StringToHash("Idle");
    public readonly int Walk_Hash = Animator.StringToHash("Walk");
    public readonly int Attack_Hash = Animator.StringToHash("Attack");
    public readonly int Critical_Hash = Animator.StringToHash("Critical");

    public EffectController effectController;

    /// <summary>
    /// 캐릭터별 UI 부모 Transform을 담는 간단한 데이터 클래스입니다.
    /// 캐릭터 프리팹의 루트에 추가하고, 자식으로 만든 위치 오브젝트들을 연결합니다.
    /// </summary>
    public class CharacterUIParents : MonoBehaviour
    {
        [Header("UI 아이콘 생성 위치")]
        public Transform buff; // 우상단 (버프)
        public Transform debuff; // 좌상단 (디버프)
        public Transform instant; // 하단 (회복, 부활 등 즉시효과)
        public Transform barrier; // 좌측 (보호막)
    }

    private CharacterUIParents uiParents;

    protected override void Awake()
    {
        base.Awake();

        _character = GetComponent<MyCharacterController>();
        _chaAnimatior = GetComponentInChildren<Animator>();

        uiParents = GetComponent<CharacterUIParents>();
        effectController = GetComponentInChildren<EffectController>();
    }

    // 캐릭터 생성 초기화
    protected override void Init()
    {
        _characterState._chaLevel = _characterData.GetModifiedCharacterState()._chaLevel;
        _characterState._chaUpgrade = _characterData.GetModifiedCharacterState()._chaUpgrade;
        _characterState._chaID = _characterData.GetModifiedCharacterState()._chaID;

        _characterState._chaName = _characterData.GetModifiedCharacterState()._chaName;
        _characterState._chaEnName = _characterData.GetModifiedCharacterState()._chaEnName;
        _characterState._chaGrade = _characterData.GetModifiedCharacterState()._chaGrade;
        _characterState._chaRole = _characterData.GetModifiedCharacterState()._chaRole;
        _characterState._chaCurrentHP = _characterData.GetModifiedCharacterState()._chaCurrentHP;
        _characterState._chaMaxHP = _characterData.GetModifiedCharacterState()._chaMaxHP;
        _characterState._chaCurrentMP = _characterData.GetModifiedCharacterState()._chaCurrentMP;
        _characterState._chaMaxMP = _characterData.GetModifiedCharacterState()._chaMaxMP;
        _characterState._chaMPRecovery = _characterData.GetModifiedCharacterState()._chaMPRecovery;
        _characterState._chaAtkSpeed = _characterData.GetModifiedCharacterState()._chaAtkSpeed;
        _characterState._chaAttack = _characterData.GetModifiedCharacterState()._chaAttack;
        _characterState._chaArmor = _characterData.GetModifiedCharacterState()._chaArmor;
        _characterState._chaAtkIsMelee = _characterData.GetModifiedCharacterState()._chaAtkIsMelee;
        _characterState._chaAccuracy = _characterData.GetModifiedCharacterState()._chaAccuracy;
        _characterState._chaAvoid = _characterData.GetModifiedCharacterState()._chaAvoid;
        _characterState._chaCrit = _characterData.GetModifiedCharacterState()._chaCrit;
        _characterState._chaCritDmg = _characterData.GetModifiedCharacterState()._chaCritDmg;
        _characterState._chaReg = _characterData.GetModifiedCharacterState()._chaReg;
        _characterState._chaMoveSpeed = _characterData.GetModifiedCharacterState()._chaMoveSpeed;
        _characterState._reductionUpValue = _characterData.GetModifiedCharacterState()._reductionUpValue;
        _characterState._reductionDownValue = _characterData.GetModifiedCharacterState()._reductionDownValue;

        _characterState._isBarrier = _characterData.GetModifiedCharacterState()._isBarrier;
        _characterState._groggyDamage = _characterData.GetModifiedCharacterState()._groggyDamage;
        _characterState._chaPassiveSkill = _characterData.GetModifiedCharacterState()._chaPassiveSkill;
        _characterState._isResurrection = _characterData.GetModifiedCharacterState()._isResurrection;
        _moveDir = Vector3.right;
        _isAlive = true;
        _isFirstAttack = true;

        // 캐릭터의 저장된 데이터 불러오기
        CharacterSaveDataLoad();
        // 캐릭터 레벨 업 스텟 적용
        LevelUpStatUpdate();
        // 캐릭터 돌파 스텟 적용
        UpgradeStatUpdate();

        // 체력, 마나 게이지 현재값 초기화
        OnHpChange?.Invoke(_characterState._chaCurrentHP / _characterState._chaMaxHP);
        OnMpChange?.Invoke(_characterState._chaCurrentMP / _characterState._chaMaxMP);
        // 타임오버에 대한 캐릭터 삭제 이벤트 구독
        _battleUI.OnTimeOver += TimeDeath;
        _battleManager.OnAniChange += CharacterAniIdle;

        _attackCoolTimer = _characterState._chaAtkSpeed;

        _manaChangeValue = _characterState._chaMPRecovery;

        // 마나 충전 
        ManaRecovery();
    }

    private void OnDestroy()
    {
        // 타임오버에 대한 캐릭터 삭제 이벤트 구독 해제
        _battleUI.OnTimeOver -= TimeDeath;
        _battleManager.OnAniChange -= CharacterAniIdle;
    }

    // 캐릭터 이동
    protected override void Movement()
    {
        // 타겟이 없으면 
        if (_researchTarget == null)
        {
            // 오른쪽으로 이동
            transform.Translate(_moveDir * _characterState._chaMoveSpeed * _gameSpeed * Time.deltaTime);

            _chaAnimatior.speed = _gameSpeed;
            // 이동 애니메이션
            _chaAnimatior.Play(Walk_Hash);
        }
        else // 탐색 대상이 있으면
        {
            // 공격 중이면 이동 정지
            if (_attackTarget != null && _isAttack) return;

            // 이동 여유 거리
            float moveSpareDistance = _characterState._chaAtkIsMelee * 0.8f;

            // 탐색한 대상과 거리 확인
            float moveDistance = Vector3.Distance(transform.position, _researchTarget.transform.position);

            // 탐색 대상과의 거리가 공격 사거리 안에 들어올 때까지 접근
            if (moveDistance > moveSpareDistance)
            {
                // 탐색 대상으로 이동
                transform.position = Vector3.MoveTowards(transform.position, _researchTarget.transform.position,
                    _characterState._chaMoveSpeed * _gameSpeed * Time.deltaTime);

                _chaAnimatior.speed = _gameSpeed;
                // 이동 애니메이션
                _chaAnimatior.Play(Walk_Hash);
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

            // 공격 타겟 재선정
            _attackController.RecheckAttackTarget();
        }

        // 공격 쿨타임 계산
        _attackCoolTimer -= Time.deltaTime;

        // 공격 여유 사거리
        float attackSpareDistance = _characterState._chaAtkIsMelee * 0.9f;

        // 공격 타겟과 거리 비교
        float attackDistance = Vector3.Distance(transform.position, _attackTarget.transform.position);

        // 공격 대상의 거리가 캐릭터의 공격 사거리에 들어오면 타겟 공격
        if (attackDistance <= attackSpareDistance)
        {
            if (_attackCoolTimer <= 0f || _isFirstAttack)
            {
                OnRelicAttack?.Invoke();
                float attackDamage = _characterState._chaAttack;
                float passiveDamage;

                // 사용하려는 패시브와 캐릭터가 사용하는 패시브가 같은지 확인
                if (_characterData._chaPassiveSkill._chaSkillEnName == CharacterSkillEnName.AimForTheWound)
                {
                    passiveDamage =
                        _characterData._chaPassiveSkill.UsePassiveSkill(_character, _characterData._chaPassiveSkill,
                            attackDamage);
                }
                else // 패시브 없으면 원래 공격력
                {
                    passiveDamage = attackDamage;
                }

                // 체력회복 패시브 스킬
                HealHpPassive();
                // 공격력 감소 패시브 확인
                AttackDownPassiveCheck();
                // 몬스터 전체 공격 패시브 스킬
                AllMonsterDamagePassive();

                // 몬스터가 살아있으면 공격
                if (_attackTarget != null && _attackTarget._isAlive)
                {
                    float basicAniSpeed = _characterData._chaBaseData.ChaAtkSpeed;
                    float increaseAniSpeed = _characterState._chaAtkSpeed;

                    _chaAnimatior.speed = ((basicAniSpeed + increaseAniSpeed) * _gameSpeed);
                    // 공격 애니메이션
                    _chaAnimatior.Play(Attack_Hash);

                    // 캐릭터의 데미지로 몬스터에 주기
                    _attackTarget.TakeDamage(passiveDamage, _characterState._chaAccuracy);

                    if (_attackTarget != null)
                    {
                        // 몬스터의 공격 타겟 전환
                        _attackTarget.AttackTargetChange(_character);
                    }
                }

                _isAttack = true;
                _isFirstAttack = false;

                // 공격 쿨타임 초기화
                _attackCoolTimer = _characterState._chaAtkSpeed / _gameSpeed;
            }
        }
        else
        {
            _isAttack = false;
            _isFirstAttack = true;
            // 대기 애니메이션
            //_chaAnimatior.Play(Idle_Hash);
        }
    }

    // 공격 대상 변경
    public void AttackTargetChange(MonsterController monData)
    {
        // 공격한 캐릭터가 죽었으면 넘어가기
        if (monData == null || !monData.isActiveAndEnabled) return;

        // 공격 대상 전환
        _attackTarget = monData;
    }

    // 데미지를 받음
    public override void TakeDamage(float damage, float hitRate)
    {
        // 공격 회피
        if (AttackEvasion(_characterState._chaAvoid, hitRate))
        {
            // Debug.Log($"{_characterState._chaEnName}가 공격을 회피했습니다.");
            return;
        }

        // 배리어 상태일때는 공격을 무시함.
        if (_characterState._isBarrier)
        {
            _characterState._isBarrier = false;

            // 이펙트 전환 (깨짐 이펙트 출력)
            effectController?.PlayBrokenBarrier();

            // Debug.Log($"{_characterState._chaEnName}의 배리어가 사용되었습니다.");
            return;
        }

        base.TakeDamage(damage, hitRate);

        // 최종데미지로 체력 감소
        _characterState._chaCurrentHP -=
            FinalDamage(damage, _characterState._reductionUpValue, _characterState._reductionDownValue);

        // 체력이 0이 됨
        if (_characterState._chaCurrentHP <= 0)
        {
            _characterState._chaCurrentHP = 0f;
            OnHpChange?.Invoke(0f);
            // 유닛의 죽음
            Death();
        }

        // 체력 변화에 대한 이벤트 호출
        OnHpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentHP / _characterState._chaMaxHP));
    }

    // 캐릭터 레벨 업 스텟 적용
    private void LevelUpStatUpdate()
    {
        var levelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[_characterState._chaLevel];

        _characterState._chaCurrentHP *= levelData.ChaHPIncrease;
        _characterState._chaMaxHP *= levelData.ChaHPIncrease;
        _characterState._chaAttack *= levelData.ChaAttackIncrease;
        _characterState._chaArmor *= levelData.ChaArmorIncrease;
    }

    // 캐릭터 돌파 스텟 적용
    private void UpgradeStatUpdate()
    {
        var upgradeData = GameManager.Instance.CharacterData.ChaBeadsData[_characterState._chaUpgrade];

        _characterState._chaCurrentHP *= upgradeData.ChaHP;
        _characterState._chaMaxHP *= upgradeData.ChaHP;
        _characterState._chaAttack *= upgradeData.ChaAttack;
        _characterState._chaArmor *= upgradeData.ChaArmor;
    }

    // 저장된 캐릭터의 데이터 불러오기
    private void CharacterSaveDataLoad()
    {
        var characterHpSaveData = GameManager.Instance.CharacterBattleDataSave._chaHpSave;
        var characterLevelSaveData = GameManager.Instance.CharacterBattleDataSave._chaLevel;
        var characterUpgradeSaveData = GameManager.Instance.CharacterBattleDataSave._chaUpgrade;

        if (characterHpSaveData.ContainsKey(_characterState._chaEnName))
        {
            // 전체 부활하면 저장된 체력 불러오지 않음 
            if (!_battleManager._canResurrection) return;

            // 저장된 데이터 불러오기
            _characterState._chaCurrentHP = characterHpSaveData[_characterState._chaEnName];
        }

        if (characterLevelSaveData.ContainsKey(_characterState._chaEnName))
        {
            _characterState._chaLevel = characterLevelSaveData[_characterState._chaEnName];
        }

        if (characterLevelSaveData.ContainsKey(_characterState._chaEnName))
        {
            _characterState._chaUpgrade = characterUpgradeSaveData[_characterState._chaEnName];
        }
    }

    // 마나 회복
    public void ManaRecovery()
    {
        // 캐릭터의 등급이 레어등급만 마나회복 가능
        if (_characterData._chaBaseData.ChaGrade != CharacterGrade.Rare) return;

        // 마나가 풀이면 회복 불가
        if (_characterState._isManaFull) return;

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
        if (_characterData._chaBaseData.ChaGrade == CharacterGrade.Rare)
        {
            // 타겟이 없으면 미사용
            if (_attackTarget == null) return;

            _isUseSkill = true;
            // 유물 효과 적용
            OnRelicEffect?.Invoke();

            // 스킬 사용
            _characterState._chaActiveSkill.UseSkill(_character, _characterData._chaActiveSkill, _attackTarget);

            CharacterManaState();
        }
    }

    // 캐릭터 애니메이션 전환
    private void CharacterAniIdle()
    {
        // Idle로 전환
        _chaAnimatior.Play(Idle_Hash);
    }

    #region 캐릭터의 패시브 스킬 동작 메서드

    // 체력 회복 패시브 스킬
    public void HealHpPassive()
    {
        float attackDamage = _characterState._chaAttack;
        float healValue = 0f;

        // 체력 회복 패시브 스킬이 있는지 확인
        if (_characterData._chaPassiveSkill._chaSkillEnName == CharacterSkillEnName.RegenerativeStrike)
        {
            healValue = _characterData._chaPassiveSkill.UsePassiveSkill(_character, _characterData._chaPassiveSkill,
                attackDamage);

            // 회복량이 0이면 넘어감
            if (healValue == 0f) return;

            _battleManager.AllCharacterHeal(healValue);
        }
    }

    // 전체 캐릭터 회복
    public void CharacterHealApply(float healValue)
    {
        _characterState._chaCurrentHP = Mathf.Min(_characterState._chaCurrentHP + healValue, _characterState._chaMaxHP);

        // 체력 변화에 대한 이벤트 호출
        OnHpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentHP / _characterState._chaMaxHP));
    }

    // 피해 감소 패시브 스킬
    public void ArmorUpPassive()
    {
        if (_characterData._chaPassiveSkill._chaSkillEnName == CharacterSkillEnName.Vanguard)
        {
            float chaAamor = _characterState._chaArmor;
            float upValue =
                _characterData._chaPassiveSkill.UsePassiveSkill(_character, _characterData._chaPassiveSkill, chaAamor);

            // 전체 캐릭터 피해 감소 상승
            _battleManager.AllCharacterArmorUp(upValue);
        }
    }

    // 전체 피해 감소 상승
    public void AllCharacterArmorUpApply(float upValue)
    {
        _characterState._reductionUpValue += upValue;
    }

    // 공격력 다운 패시브 확인
    public void AttackDownPassiveCheck()
    {
        // 사기 저하 패시브 스킬이 있는지 확인
        if (_characterData._chaPassiveSkill._chaSkillEnName == CharacterSkillEnName.MoraleDecline)
        {
            float saveAttack = _attackTarget._monsterState._monAttack;
            float attackDownValue = _characterData._chaPassiveSkill.UsePassiveSkill(_character, _characterState._chaPassiveSkill,
                _characterState._chaPassiveSkill._chaEffectValue);

            // 감소할 공격력이 0이면 넘어감
            if (attackDownValue == 0f) return;

            // 사기 저하 패시브 스킬
            _attackTarget.AttackDownPassive(saveAttack, attackDownValue, _characterState._chaPassiveSkill._chaSkillDuration);
        }
    }

    // 몬스터 전체 공격 패시브 스킬
    public void AllMonsterDamagePassive()
    {
        float attackDamage = _characterState._chaAttack;
        float allAttackDamage = 0f;

        // 체력 회복 패시브 스킬이 있는지 확인
        if (_characterData._chaPassiveSkill._chaSkillEnName == CharacterSkillEnName.WaveOfSteel)
        {
            allAttackDamage =
                _characterData._chaPassiveSkill.UsePassiveSkill(_character, _characterData._chaPassiveSkill, attackDamage);

            // 데미지가 0이면 넘어감
            if (allAttackDamage == 0f) return;

            _battleManager.AllMonsterDamage(allAttackDamage, _characterState._chaAccuracy);
        }
    }

    #endregion

    // 캐릭터 사망
    protected override void Death()
    {
        base.Death();

        OnSkillModeChange?.Invoke(_isAlive);

        StopManaRecovery();
        // 매니저에 사망 보고
        _battleManager.CharacterDeathCheck();

        // UI 매니저에 사망했음을 알려 모든 아이콘을 제거
        FoodEffectUIManager.Instance.OnCharacterDied(gameObject);
    }

    // 타임오버 시 캐릭터 사망
    public void TimeDeath(bool timeOver)
    {
        if (timeOver)
        {
            base.Death();
        }
    }

    // 최종데미지 계산
    private float FinalDamage(float damage, float reducUpValue, float reducDownValue)
    {
        // 치명타 계산
        float critical = UnityEngine.Random.value < _characterState._chaCrit * 0.01f ? _characterState._chaCritDmg * 0.01f : 1f;

        //if (critical != 1f)
        //{
        //    _chaAnimatior.speed = 1 * _gameSpeed;
        //    _chaAnimatior.Play(Critical_Hash);
        //}

        // 데미지 계산
        float reduction = _characterState._chaArmor / (_characterState._chaArmor + 100);
        float buffReduction = _characterState._reductionUpValue - _characterState._reductionDownValue;
        float finalReduction = MathF.Min(reduction + buffReduction, 0.95f);
        float finalDamage = damage * (1 - finalReduction) * critical;

        // Debug.Log($"캐릭터 방어력 : {_characterState._chaArmor}");
        // Debug.Log(
        //     $"캐릭터가 받은 데미지 계산 Reduction : {reduction}, BuffReduction : {buffReduction}, FinalReduction : {finalReduction}, FinalDamage : {finalDamage}");
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

        // Debug.Log($"{_characterState._chaEnName} 회피율 : {evasionRate}");

        // 회피 가능 확인
        bool isEvasion = UnityEngine.Random.value < evasionRate ? true : false;

        return isEvasion;
    }

    // 캐릭터 마나 상태 확인
    public void CharacterManaState()
    {
        // 마나 초기화
        _characterState._chaCurrentMP = 0f;
        _isUseSkill = false;

        // 마나 변화에 대한 이벤트 호출
        OnMpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentMP / _characterState._chaMaxMP));

        // 마나 제로
        _characterState._isManaFull = false;
        // 스킬 사용 모드 전환 이벤트 호출
        OnSkillModeChange?.Invoke(_characterState._isManaFull);

        // 마나 회복
        ManaRecovery();
    }

    // 마나 회복
    private IEnumerator ManaRecoveryCoroutine()
    {
        while (!_characterState._isManaFull)
        {
            // 게임이 종료되거나 메뉴창을 열면 마나회복 중지
            if (_battleManager._isGameOver)
            {
                StopCoroutine(_manaRoutine);
            }
            else if (_battleUI._isOnMenu)
            {
                yield return null;
                continue;
            }

            // 게임 배속 적용
            yield return _battleUI._playTime;

            _characterState._chaCurrentMP += _manaChangeValue;

            // 마나 변화에 대한 이벤트 호출
            OnMpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentMP / _characterState._chaMaxMP));

            if (_characterState._chaCurrentMP >= _characterState._chaMaxMP)
            {
                _characterState._isManaFull = true;
                _characterState._chaCurrentMP = _characterState._chaMaxMP;

                // 스킬 사용 모드 전환 이벤트 호출
                OnSkillModeChange?.Invoke(_characterState._isManaFull);

                // 마나 변화에 대한 이벤트 호출
                OnMpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentMP / _characterState._chaMaxMP));
            }
        }

        _manaRoutine = null;
    }

    #region 캐릭터 스탯 변화 함수(음식 효과)

    public void HPHeal(float value)
    {
        _characterState._chaCurrentHP += Mathf.Clamp(value, 0, _characterState._chaMaxHP);
        OnHpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentHP / _characterState._chaMaxHP));
    }
    public void MPHeal(float value)
    {
        if (_characterData._chaBaseData.ChaGrade != CharacterGrade.Rare) return;

        _characterState._chaCurrentMP += Mathf.Clamp(value, 0, _characterState._chaMaxMP);
        OnMpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentMP / _characterState._chaMaxMP));
    }
    public void Revive(float value)
    {
        _isAlive = true;
        _battleManager._characterCount++;
        // TODO : gameObject의 위치 맨 왼쪽으로 변경.
        gameObject.transform.position = _spawnedPoint.position;
        gameObject.SetActive(true);
        _characterState._chaCurrentHP = value;
        OnHpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentHP / _characterState._chaMaxHP));
    }
    public void CreateBarrier(bool value)
    {
        _characterState._isBarrier = value;
    }
    public void ApplyGroggyBonus(float value)
    {
        _characterState._groggyDamage += value;
    }

    #endregion
}