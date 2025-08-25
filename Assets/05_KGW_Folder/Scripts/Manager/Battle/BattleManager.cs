using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Character Spawn Point Setting")]
    [SerializeField] Transform[] _characterSpawnPoint;

    [Header("Monster Spawn Point Setting")]
    [SerializeField] Transform[] _monsterSpawnPoint;

    [Header("Boss Spawn Point Setting")]
    [SerializeField] Transform _bossSpawnPoint;

    [Header("Monster List Setting")]
    [SerializeField] public List<MonsterDataSO> _monsterList;

    [Header("Boss List Setting")]
    [SerializeField] public List<MonsterDataSO> _bossList;

    [Header("Battle Type Setting")]
    [SerializeField] public bool _isBoss;

    // 생성된 캐릭터 보관
    List<CharacterController> _characters = new();
    List<MonsterController> _monsters = new();

    BattleUIManager _battleUIManager;
    List<CharacterDataSO> _selectCharacters;
    public int _monsterCount;
    public int _characterCount;
    public bool _isClear;
    public bool _isGameOver;
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
        _battleUIManager = FindObjectOfType<BattleUIManager>();

        CharacterSpawn();
        
        if (_isBoss)
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
        _timer = 20;
        _characters.Clear();
        _monsters.Clear();
    }

    // 캐릭터 스폰
    private void CharacterSpawn()
    {
        // 스폰 포인트 리스트 전달
        var Points = new List<Transform>(_characterSpawnPoint);

        // 스폰위치 섞기
        SpawnPointShuffle(Points);

        // 선택된 캐릭터의 수와 스폰 포인트의 수를 비교하여 작은 쪽으로 배치
        int count = Mathf.Min(_selectCharacters.Count, Points.Count);

        for (int i = 0; i < count; i++)
        {
            // 생성을 위한 선택한 캐릭터의 정보 확인
            CharacterDataSO characterData = _selectCharacters[i];

            // 캐릭터 스폰위치 설정
            Transform spawnPoint = Points[i];

            // 캐릭터 생성
            GameObject character = Instantiate(characterData._prefab, spawnPoint.position, spawnPoint.rotation);

            // 생성된 캐릭터 저장
            CharacterController createCharacter = character.GetComponent<CharacterController>();
            _characters.Add(createCharacter);

            // 캐릭터 데이터 전달
            _battleUIManager._infoSlot[i].GetCharacterData(characterData);
            _battleUIManager._infoSlot[i].GetCharacterController(createCharacter);
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
            MonsterDataSO monsterData = _monsterList[i];

            //// 몬스터 스폰위치 설정
            Transform spawnPoint = _monsterSpawnPoint[i];

            // 몬스터 생성
            GameObject monster = Instantiate(monsterData._prefab, spawnPoint.position, spawnPoint.rotation);

            // 생성된 캐릭터 저장
            MonsterController createMonster = monster.GetComponent<MonsterController>();
            _monsters.Add(createMonster);

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

        }
    }

    // 몬스터의 개별 데미지를 확인
    public void ReportMonsterDamage(float damage)
    {
        // 통합 체력 계산
        _monsterTotalCurrentHp -= damage;

        // 현재 체력이 0보다 작으면
        if(_monsterTotalCurrentHp < 0)
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
