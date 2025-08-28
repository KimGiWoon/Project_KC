using System;
using System.Collections;
using System.Collections.Generic;
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

    // 생성된 캐릭터 보관
    public List<MyCharacterController> _characters = new List<MyCharacterController>();
    public List<MonsterController> _monsters = new List<MonsterController>();

    public BattleUI _battleUI;
    private List<CharacterDataSO> _selectCharacters;
    public int _monsterCount;
    public int _characterCount;
    public bool _isClear;
    public bool _isGameOver;
    public bool _canResurrection;
    public int _timer;
    public float _monsterTotalMaxHp;
    public float _monsterTotalCurrentHp;

    // 게임 결과 확인 이벤트
    public event Action<bool> OnGameResult;

    // 전체 체력 변화 이벤트
    public event Action<float, float> OnTotalHpChange;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        _battleUI = FindObjectOfType<BattleUI>();

        string stageName = GameManager.Instance.StageName;
        //todo datatable에서 stageName으로 일반 몬스터와 보스 몬스터 정보를 가져와야 함
        //_isLocalBoss = ;
        _isLastBoss = GameManager.Instance.LastBoss;
        // _monsterList = monsterData[stageName].NormalMonsters;
        // _eliteList = monsterData[stageName].EliteMonsters;
        // _bossList = monsterData[stageName].BossMonsters;

        StartCoroutine(Spwan());
    }

    private IEnumerator Spwan()
    {
        yield return new WaitForSeconds(0.5f);


        CharacterSpawn();

        if (_isLastBoss)
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
        _selectCharacters = CharacterSelectManager.Instance._characterSelectList;
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
        var Points = _canResurrection ? new List<Transform>(_characterSpawnPoint) : new List<Transform>(_characterResurrectionPoint);

        // 스폰위치 섞기
        SpawnPointShuffle(Points);

        // 선택된 캐릭터의 수와 스폰 포인트의 수를 비교하여 작은 쪽으로 배치
        int count = Mathf.Min(_selectCharacters.Count, Points.Count);

        for (int i = 0; i < count; i++)
        {
            // 생성을 위한 선택한 캐릭터의 정보 확인
            var characterData = _selectCharacters[i];

            // 캐릭터 스폰위치 설정
            var spawnPoint = Points[i];

            // 캐릭터 생성
            var character = Instantiate(characterData._prefab, spawnPoint.position, spawnPoint.rotation);

            // 생성된 캐릭터 저장
            var createCharacter = character.GetComponent<MyCharacterController>();
            _characters.Add(createCharacter);

            // 캐릭터 데이터 전달
            _battleUI._infoSlot[i].GetCharacterData(characterData);
            _battleUI._infoSlot[i].GetCharacterController(createCharacter);
        }

        // 생성된 캐릭터 수 저장
        _characterCount = _characters.Count;
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

            // 생성된 캐릭터 저장
            var createMonster = monster.GetComponent<MonsterController>();
            _monsters.Add(createMonster);

            // 통합 제력 저장
            _monsterTotalMaxHp += monsterData._maxHp;
        }

        _monsterTotalCurrentHp = _monsterTotalMaxHp;
        // 생성된 몬스터 수 저장
        _monsterCount = _monsters.Count;

        // 통합 체력 초기화
        OnTotalHpChange?.Invoke(_monsterTotalCurrentHp, _monsterTotalMaxHp);
    }

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
            _monsters.Add(createBossMonster);

            // 통합 제력 저장
            _monsterTotalMaxHp += bossData._maxHp;

            //# 한 마리만 소환되는 경우
            break;
        }

        _monsterTotalCurrentHp = _monsterTotalMaxHp;
        // 생성된 몬스터 수 저장
        _monsterCount = _monsters.Count;

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

            // 이벤트 호출
            OnGameResult?.Invoke(_isClear);
        }

        // 남아있는 플레이어가 없으면
        if (_characterCount == 0)
        {
            Debug.Log("클리어 실패!");
            _isClear = false;
            _isGameOver = true;

            // 이벤트 호출
            OnGameResult?.Invoke(_isClear);
        }
    }
}