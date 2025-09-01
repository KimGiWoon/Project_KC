using System;
using System.Collections;
using UnityEngine;

// 유닛의 공통의기능 정보 추상 클래스
public abstract class UnitBaseData : MonoBehaviour
{
    [Header("Knockback Setting")]
    [SerializeField] private float _knockbackForce = 0.1f; // 넉백 파워
    [SerializeField] private float _knockbackDuraction = 0.1f; // 넉백 지속 시간

    public float _currentHp; // 유닛의 현재 체력
    public float _maxHp; // 유닛의 최대 체력
    public float _currentMp; // 유닛의 현재 마나
    public bool _isAlive; // 유닛의 생존 여부
    public bool _isAttack; // 유닛의 공격 여부
    public bool _isHalfHpSkill; // 유닛의 체력 절반 여부
    public bool _isStern;   // 유닛의 그로기 상태확인
    public int _gameSpeed; // 게임 속도
    protected float _attackCoolTimer; // 공격 쿨타임
    protected Vector3 _moveDir; // 유닛의 이동 방향
    private Coroutine _knockbackRoutine;
    protected BattleManager _battleManager;
    protected BattleUI _battleUI;
    protected CharacterDataSO _chaData;
    protected MonsterDataSO _monData;
    
    // 체력 절반 이벤트
    public event Action OnHalfHp;

    private void Awake()
    {
        _battleManager = FindObjectOfType<BattleManager>();
        _battleUI = FindObjectOfType<BattleUI>();
    }

    // 유닛 생성시 초기화
    protected virtual void Start()
    {
        Init();
    }

    // 유닛의 동작 관리
    protected virtual void Update()
    {
        // 게임이 종료되거나 메뉴창이 오픈되거나 그로기 상태이면 움직이지 않는다.
        if (_battleManager._isGameOver || _battleUI._isOnMenu || _isStern) return;

        Movement();
        Attack();

        // 게임 속도에 따른 유닛의 이동 속도 변경
        _gameSpeed = CharacterSelectManager.Instance._isFastGame ? 2 : 1;
    }

    // 유닛의 초기화
    protected abstract void Init();

    // 유닛의 움직임
    protected abstract void Movement();

    // 유닛의 공격
    protected abstract void Attack();

    // 유닛의 데미지 받음
    public virtual void TakeDamage(float damage)
    {
        // 데미지를 받음, 방어력에 대한 것은??
        _currentHp -= damage;

        // 체력이 0이 됨
        if (_currentHp <= 0)
        {
            // 유닛의 죽음
            Death();
        }

        // 보스가 아니면 넉백 가능
        if (gameObject.layer != LayerMask.NameToLayer("Boss"))
        {
            // 넉백 코루틴 null 체크
            if (_knockbackRoutine != null)
            {
                StopCoroutine(_knockbackRoutine);
                _knockbackRoutine = null;
            }

            _knockbackRoutine = StartCoroutine(KnockBackCoroutine());
        }
        else // 보스이면 체력 절반 확인
        {
            float halfHp = _maxHp / 2;

            if(_currentHp <= halfHp)
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
    }

    // 유닛의 사망
    protected virtual void Death()
    {
        // 유닛의 사망처리
        _isAlive = false;

        // 오브젝트 삭제
        Destroy(gameObject);
    }

    // 캐릭터 넉백
    private IEnumerator KnockBackCoroutine()
    {
        // 이동 방향의 반대방향으로 넉백 방향 설정
        var knockbackDir = -_moveDir;

        float time = 0f;

        while (time < _knockbackDuraction)
        {
            time += Time.deltaTime;

            // 넉백 이동
            transform.Translate(knockbackDir * _knockbackForce * Time.deltaTime);

            yield return null;

            _knockbackRoutine = null;
        }
    }
}