using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRacallController : MonoBehaviour
{
    MonsterDataSO[] _recallMonsterList;
    Transform[] _recallPoint;
    BattleManager _battleManager;

    private List<GameObject> _recallMonsters = new List<GameObject>();

    // 데이터 받고 몬스터 소환
    public void Init(MonsterDataSO[] monsterList, Transform[] recallPoint)
    {
        _recallMonsterList = monsterList;
        _recallPoint = recallPoint;
        _battleManager = FindObjectOfType<BattleManager>();

        // 몬스터 소환
        MonsterRecall();
    }

    private void Update()
    {
        if (_battleManager._isGameOver)
        {
            RemoveRecallMonster();
        }
    }

    // 몬스터 소환
    private void MonsterRecall()
    {
        for (int i = 0; i < _recallMonsterList.Length; i++)
        {
            // 생성되는 몬스터 정보 확인
            MonsterDataSO recallMonsterData = _recallMonsterList[i];

            // 몬스터 스폰 위치
            Transform spawnPoint = _recallPoint[i];

            Debug.Log("몬스터 소환");
            // 몬스터 생성
            GameObject recallMonster = Instantiate(recallMonsterData._prefab, spawnPoint.position, spawnPoint.rotation);

            _recallMonsters.Add(recallMonster);
        }
    }

    // 소환한 몬스터 삭제
    public void RemoveRecallMonster()
    {
        Debug.Log("소환된 몬스터 삭제");
        foreach(var mon in _recallMonsters)
        {
            if (mon != null)
            {
                Destroy(mon);
            }
        }
        _recallMonsters.Clear();

        // 0.5초 후 사라짐
        Destroy(gameObject);
    }
}
