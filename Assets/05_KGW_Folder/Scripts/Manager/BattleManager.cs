using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Character Spawn Point Setting")]
    [SerializeField] Transform[] _characterSpawnPoint;

    [Header("Monster Spawn Point Setting")]
    [SerializeField] Transform[] _monsterSpawnPoint;

    [Header("Monster List")]
    [SerializeField] public List<MonsterDataSO> _monsterList;

    [Header("UI Setting")]
    [SerializeField] GameObject _clearPanel;

    List<CharacterDataSO> _selectCharacters;
    public int _monsterCount;
    public int _characterCount;
    public bool _isClear = false;
    Coroutine _clearRoutine;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        CharacterSpawn();
        MonsterSpawn();
    }

    // 초기화
    private void Init()
    {
        _selectCharacters = CharacterSelectManager.Instance._characterSelectList;
        _monsterCount = _monsterList.Count;
        _characterCount = _selectCharacters.Count;

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
        }
    }

    // 몬스터 스폰
    private void MonsterSpawn()
    {
        for (int i = 0; i < _monsterList.Count; i++)
        {
            // 생성을 위한 몬스터의 정보 확인
            MonsterDataSO mosterData = _monsterList[i];

            //// 몬스터 스폰위치 설정
            Transform spawnPoint = _monsterSpawnPoint[i];

            // 몬스터 생성
            GameObject monster = Instantiate(mosterData._prefab, spawnPoint.position, spawnPoint.rotation);
        }
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
        _monsterCount = (int)MathF.Max(0, _monsterCount - 1);

        // 클리어 체크
        BattleClearCheck();
    }

    // 캐릭터 사망 체크
    public void CharacterDeathCheck()
    {
        _characterCount = (int)MathF.Max(0, _characterCount - 1);

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

            // 코루틴 null 확인
            if(_clearRoutine != null)
            {
                StopCoroutine(_clearRoutine);
                _clearRoutine = null;
            }

            // 클리어 루틴 스타트
            _clearRoutine = StartCoroutine(ClearPanelCoroutine());
        }

        // 남아있는 플레이어가 없으면
        if (_characterCount == 0)
        {
            Debug.Log("클리어 실패!");
            //SceneManager.LoadScene("KGW_TestLobbyScene");
        }
    }

    // 클리어 패널 코루틴
    private IEnumerator ClearPanelCoroutine()
    {
        _clearPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        _isClear = false;
        SceneManager.LoadScene("KGW_TestLobbyScene");
    }
}
