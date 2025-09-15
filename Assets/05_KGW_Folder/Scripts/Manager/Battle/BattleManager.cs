using System;
using System.Collections;
using System.Collections.Generic;
using JJY;
using SDW;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Character Spawn Point Setting")]
    [SerializeField]
    private Transform[] _characterSpawnPoint;

    [Header("Character Resurrection Point Setting")]
    [SerializeField]
    private Transform[] _characterResurrectionPoint;

    [Header("Monster Spawn Point Setting")]
    [SerializeField]
    private Transform[] _monsterSpawnPoint;

    [Header("Boss Spawn Point Setting")]
    [SerializeField]
    private Transform _bossSpawnPoint;

    // [Header("Character List Setting")]
    // [SerializeField] public List<CharacterDataSO> _characterList;

    [Header("Monster List Setting")]
    [SerializeField] public List<MonsterDataSO> _monsterList;
    [SerializeField] public List<MonsterDataSO> _eliteList;

    [Header("Boss List Setting")]
    [SerializeField] public List<MonsterDataSO> _bossList;

    [Header("Battle Type Setting")]
    [SerializeField] public bool _isLocalBoss;
    public bool IsLocalBoss => _isLocalBoss;
    [SerializeField] public bool _isLastBoss;
    public bool IsLastBoss => _isLastBoss;
    [SerializeField] public GameObject _wall;
    public GameObject Wall => _wall;

    [Header("BuffManager")]
    [SerializeField] private BuffManager buffManager;
    [Header("RelicInventoryUI")]
    [SerializeField] private RelicInventoryUI _relicInventoryUI;

    // 생성된 캐릭터 보관
    public List<MyCharacterController> _characters = new List<MyCharacterController>();
    public List<MonsterController> _monsters = new List<MonsterController>();
    public List<MonsterController> _bossMonster = new List<MonsterController>();

    public BattleUI _battleUI;
    //private List<CharacterDataSO> _selectCharacters;
    private Coroutine _armorRoutine;
    public int _monsterCount;
    public int _characterCount;
    public bool _isClear;
    public bool _isGameOver;
    public bool _isTimeOver;
    public bool _canResurrection;
    public int _timer;
    public float _monsterTotalMaxHp;
    public float _monsterTotalCurrentHp;
    private bool _isBattleStarted;

    // 게임 결과 확인 이벤트
    public event Action<bool> OnGameResult;

    // 캐릭터 애니메이션 전환 이벤트
    public event Action OnAniChange;

    // 전체 체력 변화 이벤트
    public event Action<float, float> OnTotalHpChange;
    private bool _isSpawned;

    public event Action OnCharacterDeath;

    private GameManager _gameManager;
    private CharacterDataManager _charData;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _charData = GameManager.Instance.CharacterData;
        RoguelikeManager.Instance.OnBattleStart += BattleStart;
        RoguelikeManager.Instance.OnBattleEnd += BattleEnd;
        BuffRelicManager.Instance.BattleStart();
    }

    private void OnDisable()
    {
        RoguelikeManager.Instance.OnBattleStart -= BattleStart;
        RoguelikeManager.Instance.OnBattleEnd -= BattleEnd;
    }

    // private void Start()
    // {
    //     _battleUI = FindObjectOfType<BattleUI>();
    //
    //     //_isLocalBoss = ;
    //     _isLastBoss = GameManager.Instance.LastBoss;
    //     // _monsterList = monsterData[stageName].NormalMonsters;
    //     // _eliteList = monsterData[stageName].EliteMonsters;
    //     // _bossList = monsterData[stageName].BossMonsters;
    //
    //     StartCoroutine(Spwan());
    // }

    private void Update()
    {
        if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
            _isSpawned || !_isBattleStarted) return;
        // if(_isDownloaded) return;

        //_isLocalBoss = ;
        //todo BossFinal과 Boss는 구분되어야 함 - 아래 코드는 Test 코드
        _isLastBoss = RoguelikeManager.Instance.MonsterType == BattleEventType.Boss;
        // _monsterList = monsterData[stageName].NormalMonsters;
        // _eliteList = monsterData[stageName].EliteMonsters;
        // _bossList = monsterData[stageName].BossMonsters;

        StartCoroutine(Spwan());
        _isSpawned = true;
    }

    private IEnumerator Spwan()
    {
        yield return new WaitForSeconds(0.5f);


        CharacterSpawn();

        if (_isLastBoss || _isLocalBoss)
        {
            BossSpawn();
        }
        else
        {
            MonsterSpawn();
        }
    }

    // 초기화
    private void Init()
    {
        //_selectCharacters = CharacterSelectManager.Instance._characterSelectList;
        _isClear = false;
        _isGameOver = false;
        _canResurrection = true;
        _timer = 100;
        _characters.Clear();
        _monsters.Clear();
    }

    // 캐릭터 스폰
    public void CharacterSpawn()
    {
        // 스폰 포인트 리스트 전달
        var Points = _canResurrection ? new List<Transform>(_characterSpawnPoint)
            : new List<Transform>(_characterResurrectionPoint);

        // 스폰위치 섞기
        SpawnPointShuffle(Points);

        // 선택된 캐릭터의 수와 스폰 포인트의 수를 비교하여 작은 쪽으로 배치
        int count = Mathf.Min(_charData.SelectedTeam.Count, Points.Count);

        for (int i = 0; i < count; i++)
        {
            // 생성을 위한 선택한 캐릭터의 정보 확인
            var characterData = _charData.SelectedTeam[i];

            // 캐릭터 스폰위치 설정
            var spawnPoint = Points[i];

            // 캐릭터 생성
            var character = Instantiate(characterData._prefab, spawnPoint.position, spawnPoint.rotation);

            var characterOIL = character.GetComponentInChildren<SpriteRenderer>();

            // 마직막 캐릭터를 맨 앞으로 보여주기
            characterOIL.sortingOrder = 10 + count - i;

            // 생성된 캐릭터 저장
            var createCharacter = character.GetComponent<MyCharacterController>();
            _characters.Add(createCharacter);

            // 캐릭터 데이터 전달
            _battleUI._infoSlot[i].GetCharacterData(characterData);
            _battleUI._infoSlot[i].GetCharacterController(createCharacter);
        }

        // 생성된 캐릭터 수 저장
        _characterCount = _characters.Count;

        _armorRoutine = StartCoroutine(ApplyArmorPassiveCoroutine());
    }

    // 몬스터 스폰
    private void MonsterSpawn()
    {
        for (int i = 0; i < _monsterList.Count; i++)
        {
            // 생성을 위한 몬스터의 정보 확인
            var monsterData = _monsterList[i];

            // 몬스터 스폰위치 설정
            var spawnPoint = _monsterSpawnPoint[i];

            // 몬스터 생성
            var monster = Instantiate(monsterData._prefab, spawnPoint.position, spawnPoint.rotation);

            var monterOIL = monster.GetComponentInChildren<SpriteRenderer>();

            // 마직막 캐릭터를 맨 앞으로 보여주기
            monterOIL.sortingOrder = 10 + i;
            // 생성된 캐릭터 저장
            var createMonster = monster.GetComponent<MonsterController>();
            createMonster.Battle = this;
            _monsters.Add(createMonster);

            // 통합 제력 저장
            _monsterTotalMaxHp += monsterData.MonHP;
        }

        _monsterTotalCurrentHp = _monsterTotalMaxHp;
        // 생성된 몬스터 수 저장
        _monsterCount = _monsters.Count;

        // 통합 체력 초기화
        OnTotalHpChange?.Invoke(_monsterTotalCurrentHp, _monsterTotalMaxHp);
    }

    // 보스 스폰
    private void BossSpawn()
    {
        for (int i = 0; i < _bossList.Count; i++)
        {
            // 보스의 정보 확인
            var bossData = _bossList[i];

            //todo dataSO에서 isLastBoss인지 체크하기 위한 필드 추가해야 함
            //if (_isLastBoss && !monsterData._isLastBoss) continue;

            //todo Stage의 Boss(last든 local이든 일치하는 놈을 소환해야 함)
            //if (_stageMonsterName != bossData._monsterName) continue;

            // 보스의 스폰위치 설정
            var spawnPoint = _bossSpawnPoint;

            // 보스 생성
            var bossMonster = Instantiate(bossData._prefab, spawnPoint.position, spawnPoint.rotation);

            // 성생된 보스 저장
            var createBossMonster = bossMonster.GetComponent<MonsterController>();
            _bossMonster.Add(createBossMonster);

            // 통합 제력 저장
            _monsterTotalMaxHp += bossData.MonHP;

            //# 한 마리만 소환되는 경우
            break;
        }

        _monsterTotalCurrentHp = _monsterTotalMaxHp;
        // 생성된 몬스터 수 저장
        _monsterCount = _bossMonster.Count;

        // 통합 체력 초기화
        OnTotalHpChange?.Invoke(_monsterTotalCurrentHp, _monsterTotalMaxHp);
    }

    // 몬스터의 개별 데미지를 확인
    public void ReportMonsterDamage(float damage)
    {
        // 통합 체력 계산
        _monsterTotalCurrentHp -= damage;

        // 현재 체력이 0보다 작으면
        if (_monsterTotalCurrentHp < 0)
        {
            // 0으로 세팅
            _monsterTotalCurrentHp = 0;
        }

        // 통합 체력 변화
        OnTotalHpChange?.Invoke(_monsterTotalCurrentHp, _monsterTotalMaxHp);
    }

    // 몬스터의 개별 체력 회복 확인
    public void ReportMonsterHeal(float healValue)
    {
        // 통합 체력 계산
        _monsterTotalCurrentHp += healValue;

        // 현재 체력이 최대 체력보다 크면
        if (_monsterTotalCurrentHp >= _monsterTotalMaxHp)
        {
            // 최대 체력으로 세팅
            _monsterTotalCurrentHp = _monsterTotalMaxHp;
        }

        // 통합 체력 변화
        OnTotalHpChange?.Invoke(_monsterTotalCurrentHp, _monsterTotalMaxHp);
    }

    // 스폰위치 섞기 (Fisher Yates Shuffle 알고리즘 사용)
    private void SpawnPointShuffle<T>(IList<T> list)
    {
        // 뒤에서부터 앞으로 오면서 
        for (int i = list.Count - 1; i > 0; i--)
        {
            // 무작위 인덱스와 현재 인덱스를 교환
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // 몬스터 사망 체크
    public void MonsterDeathCheck()
    {
        _monsterCount = Math.Max(0, _monsterCount - 1);

        // 클리어 체크
        BattleClearCheck();
    }

    // 캐릭터 사망 체크
    public void CharacterDeathCheck()
    {
        _characterCount = Math.Max(0, _characterCount - 1);
        OnCharacterDeath?.Invoke();
        // 클리어 체크
        BattleClearCheck();
    }

    // 클리어 체크
    public void BattleClearCheck()
    {
        // 남아있는 몬스터가 없으면
        if (_monsterCount == 0)
        {
            Debug.Log("클리어 성공!");

            _isClear = true;
            _isGameOver = true;

            // 캐릭터의 체력 저장
            CharacterStatSave();

            // 이벤트 호출
            OnAniChange?.Invoke();
            OnGameResult?.Invoke(_isClear);
            BuffRelicManager.Instance.BattleEnd();
        }

        // 남아있는 플레이어가 없으면
        if (_characterCount == 0)
        {
            Debug.Log("클리어 실패!");
            _isClear = false;
            _isGameOver = true;

            foreach (var boss in _bossMonster)
            {
                Debug.Log("보스 몬스터의 공격타겟 삭제");
                boss._attackTarget = null;
                boss._attackTargets.Clear();
            }

            // 이벤트 호출
            OnAniChange?.Invoke();
            OnGameResult?.Invoke(_isClear);
            BuffRelicManager.Instance.BattleEnd();
        }
    }

    private void BattleStart()
    {
        buffManager?.InitFoodIcon();
        _relicInventoryUI?.RelicUIAdd();
        _isBattleStarted = true;
    }

    // 캐릭터의 체력 저장
    public void CharacterStatSave()
    {
        foreach(var cha in _characters)
        {
            // 캐릭터가 죽었으면
            if (!cha._isAlive)
            {
                // 체력의 30%만 저장
                GameManager.Instance.CharacterBattleDataSave._chaHpSave[cha._characterState._chaEnName] = cha._characterState._chaMaxHP * 0.3f;
                Debug.Log($"{cha._characterState._chaEnName}의 전체 체력의 30%인 {cha._characterState._chaCurrentHP}저장");
            }
            else
            {
                // 캐릭터의 현재 남은 체력 저장
                GameManager.Instance.CharacterBattleDataSave._chaHpSave[cha._characterState._chaEnName] = cha._characterState._chaCurrentHP;
                Debug.Log($"{cha._characterState._chaEnName}의 현재 남은 체력 {cha._characterState._chaCurrentHP}저장");
            }

            GameManager.Instance.CharacterBattleDataSave._chaLevel[cha._characterState._chaEnName] = cha._characterState._chaLevel;
            GameManager.Instance.CharacterBattleDataSave._chaUpgrade[cha._characterState._chaEnName] = cha._characterState._chaUpgrade;
        }
    }

    private void BattleEnd()
    {
        foreach (var character in _characters)
        {
            Destroy(character?.gameObject);
        }

        foreach (var monster in _monsters)
        {
            Destroy(monster?.gameObject);
        }

        _characters.Clear();
        _monsters.Clear();

        _isSpawned = false;
        _isBattleStarted = false;
        _isGameOver = false;
        _monsterTotalMaxHp = 0;
    }

    #region 캐릭터의 패시브 스킬 동작

    // 캐릭터 전체 체력 회복
    public void AllCharacterHeal(float healValue)
    {
        foreach (var cha in _characters)
        {
            if (cha._isAlive) cha.CharacterHealApply(healValue);
        }
    }

    // 아머 상승 패시브 유/무 확인
    public void ArmorUpPassiveCheck()
    {
        foreach (var cha in _characters)
        {
            // 전투에 참가한 캐릭터에 아머 상승 패시브를 가지고 있는지 체크
            if (cha._characterState._chaPassiveSkill._chaSkillEnName == CharacterSkillEnName.Vanguard)
            {
                // 아머 상승 패시브를 가지고 있으면 실행
                cha.ArmorUpPassive();
                break;
            }
        }
    }

    // 캐릭터 전체 아머 상승
    public void AllCharacterArmorUp(float upValue)
    {
        foreach (var cha in _characters)
        {
            // 전체 캐릭터 아머 상승 적용
            cha.AllCharacterArmorUpApply(upValue);
        }
    }

    // 방어력 상승 코루틴
    private IEnumerator ApplyArmorPassiveCoroutine()
    {
        yield return null; // 모든 캐릭터가 생성 후 초기화가 끝날 때까지 대기

        ArmorUpPassiveCheck(); // 아머 상승 패시브 체크
    }

    // 몬스터 전체 공격
    public void AllMonsterDamage(float damageValue, float hitRate)
    {
        foreach (var mon in _monsters)
        {
            if (mon._isAlive)
            {
                Debug.Log($"{mon._monsterState._monEnName}가 {damageValue}의 공격받음");

                if (mon != null)
                {
                    mon.TakeDamage(damageValue, hitRate);
                }
            }
        }
    }

    #endregion
}