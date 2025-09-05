using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using TableForge.Demo;
using UnityEngine;

public class MyCharacterController : UnitBaseData
{
    [Header("Character Data Setting")]
    [SerializeField] CharacterDataSO _characterData; // 캐릭터 데이터

    [Header("Attack Unit List & Controller")]
    [SerializeField] CharacterAttackController _attackController;
    public List<MonsterController> _attackTargets = new List<MonsterController>(); // 사거리에 들어온 몬스터 데이터
    public MonsterController _attackTarget; // 현재 공격 대상

    [Header("Research Unit Target")]
    public MonsterController _researchTarget; // 현재 탬색 대상

    // 캐릭터의 상태
    public CharacterState _characterState;

    private Coroutine _manaRoutine;
    private float _manaChangeValue;
    private MyCharacterController _chaCon;

    // 체력과 마나의 변화 이벤트
    public event Action<float> OnHpChange;
    public event Action<float> OnMpChange;

    // 스킬 사용 모드 변화 이벤트
    public event Action<bool> OnSkillModeChange;
    // 유물 효과 적용 이벤트
    public event Action OnRelicEffect;

    protected override void Awake()
    {
        base.Awake();

        _attackController.RecheckAttackTarget();
        _chaCon = GetComponent<MyCharacterController>();
    }
    // 캐릭터 생성 초기화
    protected override void Init()
    {
        _characterState._chaID = _characterData._chaBaseData.ChaID;
        _characterState._chaName = _characterData._chaBaseData.ChaName;
        _characterState._chaEnName = _characterData._chaBaseData.ChaEnName;
        _characterState._chaGrade = _characterData._chaBaseData.ChaGrade;
        _characterState._chaRole = _characterData._chaBaseData.ChaRole;
        _characterState._chaLevel = _characterData._chaLv;
        _characterState._chaCurrentHP = _characterData._chaBaseData.ChaHP;
        _characterState._chaMaxHP = _characterData._chaBaseData.ChaHP;
        _characterState._chaCurrentMP = 0f;
        _characterState._chaMaxMP = _characterData._chaBaseData.ChaMP;
        _characterState._chaMPRecovery = _characterData._chaBaseData.ChaMPRecovery;
        _characterState._chaAtkSpeed = _characterData._chaBaseData.ChaAtkSpeed;
        _characterState._chaAttack = _characterData._chaBaseData.ChaAttack;
        _characterState._chaArmor = _characterData._chaBaseData.ChaArmor;
        _characterState._chaAtkIsMelee = _characterData._chaTypeData.ChaAtkIsMelee;
        _characterState._chaAccuracy = _characterData._chaTypeData.ChaAccuracy;
        _characterState._chaAvoid = _characterData._chaTypeData.ChaAvoid;
        _characterState._chaCrit = _characterData._chaTypeData.ChaCrit;
        _characterState._chaCritDmg = _characterData._chaTypeData.ChaCritDmg;
        _characterState._chaReg = _characterData._chaTypeData.ChaReg;
        _characterState._chaMoveSpeed = _characterData._chaTypeData.ChaMoveSpeed;
        _moveDir = Vector3.right;
        _isAlive = true;

        // 체력, 마나 게이지 현재값 초기화
        OnHpChange?.Invoke(_characterState._chaCurrentHP / _characterState._chaMaxHP);
        OnMpChange?.Invoke(_characterState._chaCurrentMP / _characterState._chaMaxMP);
        // 타임오버에 대한 캐릭터 삭제 이벤트 구독
        _battleUI.OnTimeOver += TimeDeath;
        _attackCoolTimer = _characterState._chaAtkSpeed;

        _manaChangeValue = _characterState._chaMPRecovery;
        // 마나 충전 
        ManaRecovery();
    }

    private void OnDestroy()
    {
        // 타임오버에 대한 캐릭터 삭제 이벤트 구독 해제
        _battleUI.OnTimeOver -= TimeDeath;

    }

    // 캐릭터 이동
    protected override void Movement()
    {
        // 타겟이 없으면 
        if (_researchTarget == null)
        {
            // 오른쪽으로 이동
            transform.Translate(_moveDir * _characterState._chaMoveSpeed * _gameSpeed * Time.deltaTime);
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

        Debug.Log($"attackSpareDistance : {attackSpareDistance}, attackDistance : {attackDistance}");
        Debug.Log($"_attackCoolTimer : {_attackCoolTimer}");

        // 공격 대상의 거리가 캐릭터의 공격 사거리에 들어오면 타겟 공격
        if (attackDistance <= attackSpareDistance)
        {
            Debug.Log("사거리 안에 들어옴");

            if(_attackCoolTimer <= 0f)
            {
                Debug.Log("공격 쿨타임 O");

                // 캐릭터의 데미지로 몬스터에 주기
                _attackTarget.TakeDamage(_characterState._chaAttack);
                _attackTarget.AttackTargetChange(_chaCon);

                _isAttack = true;

                // 공격 쿨타임 초기화
                _attackCoolTimer = _characterState._chaAtkSpeed / _gameSpeed;
            }
            else
            {
                Debug.Log("공격 쿨타임 X");
            }
        }
        else
        {
            Debug.Log("사거리 안에 들어오지 못함");
            _isAttack = false;
        }
    }

    // 데미지를 받음
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // 데미지를 받음, 방어력에 대한 것은??
        _characterState._chaCurrentHP -= damage;

        // 체력이 0이 됨
        if (_characterState._chaCurrentHP <= 0)
        {
            // 유닛의 죽음
            Death();
        }

        // 체력 변화에 대한 이벤트 호출
        OnHpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentHP / _characterState._chaMaxHP));
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

            // 유물 효과 적용
            OnRelicEffect?.Invoke();

            // 보유한 스킬을 순회
            foreach (var skill in _characterData._chaSkills)
            {
                Debug.Log("스킬 사용");
                // 스킬 사용
                skill.UseSkill(transform, _attackTarget);

                // 마나 초기화
                _characterState._chaCurrentMP = 0f;
                // 마나 변화에 대한 이벤트 호출
                OnMpChange?.Invoke(Mathf.Clamp01(_characterState._chaCurrentMP / _characterState._chaMaxMP));

                // 마나 제로
                _characterState._isManaFull = false;
                // 스킬 사용 모드 전환 이벤트 호출
                OnSkillModeChange?.Invoke(_characterState._isManaFull);

                // 마나 회복
                ManaRecovery();
            }
        }
    }

    // 캐릭터 사망
    protected override void Death()
    {
        base.Death();

        OnSkillModeChange?.Invoke(_isAlive);

        StopManaRecovery();
        // 매니저에 사망 보고
        _battleManager.CharacterDeathCheck();
    }

    // 타임오버 시 캐릭터 사망
    public void TimeDeath(bool timeOver)
    {
        if (timeOver)
        {
            base.Death();
        }
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
}