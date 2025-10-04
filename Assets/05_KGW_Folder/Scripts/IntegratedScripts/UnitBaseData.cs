using System.Collections;
using SDW;
using UnityEngine;

// 유닛의 공통의기능 정보 추상 클래스
public abstract class UnitBaseData : MonoBehaviour
{
    [Header("Knockback Setting")]
    [SerializeField] private float _knockbackForce = 1f; // 넉백 파워
    [SerializeField] private float _knockbackDuraction = 0.1f; // 넉백 지속 시간

    [Header("Unit State")]
    public bool _isAlive; // 유닛의 생존 여부
    public bool _isAttack; // 유닛의 공격 여부
    public bool _isHalfHpSkill; // 유닛의 체력 절반 여부
    public bool _isStern; // 유닛의 그로기 상태확인
    public bool _playSkillAni;  // 스킬 애니메이션 플레이
    public bool _isUseSkill; // 유닛의 스킬사용 확인
    public bool _isUseSkill2;
    public Vector3 _moveDir; // 유닛의 이동 방향
    public int _gameSpeed; // 게임 속도

    private Coroutine _knockbackRoutine;
    public BattleManager _battleManager;
    protected BattleUI _battleUI;
    protected CharacterDataSO _chaData;
    protected MonsterDataSO _monData;

    protected virtual void Awake()
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
        if (_battleManager._isGameOver || _battleUI._isOnMenu || _isStern || !_isAlive) return;

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
    public virtual void TakeDamage(float damage, float hitRate)
    {
        // 1~5까지 랜덤
        int randomValue = Random.Range(1, 6);

        // 피격 사운드 플레이
        switch (randomValue)
        {
            case 1: GameManager.Instance.Audio.Play2DSFX(AudioClipName.HitSound_1); break;
            case 2: GameManager.Instance.Audio.Play2DSFX(AudioClipName.HitSound_2); break;
            case 3: GameManager.Instance.Audio.Play2DSFX(AudioClipName.HitSound_3); break;
            case 4: GameManager.Instance.Audio.Play2DSFX(AudioClipName.HitSound_4); break;
            case 5: GameManager.Instance.Audio.Play2DSFX(AudioClipName.HitSound_5); break;
        }
        
        // 보스가 아니면 넉백 가능
        if (gameObject?.layer != LayerMask.NameToLayer("Boss"))
        {
            // 크리티컬이나 스킬 사용 시 넉백
            if (_isUseSkill || _isUseSkill2)
            {
                // 넉백 코루틴 null 체크
                if (_knockbackRoutine != null)
                {
                    StopCoroutine(_knockbackRoutine);
                    _knockbackRoutine = null;
                }

                _knockbackRoutine = StartCoroutine(KnockBackCoroutine());
            }
        }
    }

    // 유닛의 사망
    protected virtual void Death()
    {
        // 유닛의 사망처리
        _isAlive = false;

        Invoke(nameof(MonsterDeath), 0.7f);
    }

    // 몬스터 사망
    private void MonsterDeath()
    {
        gameObject.SetActive(false);
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