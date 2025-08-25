using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : UnitBaseData
{
    [Header("Boss Data Setting")]
    [SerializeField] public MonsterDataSO _BossData;  // 보스 데이터

    [Header("Monster Recall Setting")]
    [SerializeField] Transform[] _monsterRecallPoint;
    [SerializeField] List<MonsterDataSO> _recallMonsterList;
    [SerializeField] float _recallTime;

    public bool _targetResearch;

    protected override void Init()
    {
        base._currentHp = _BossData._maxHp;
        base._isAlive = true;
        base._monData = _BossData;
        _targetResearch = false;
    }

    protected override void Attack()
    {
        if (!_targetResearch) return;

        float timer = 0f;

        timer += Time.deltaTime;

        Debug.Log($"소환 시간 : {timer}");

        // 소환 가능 시간이 되면
        if (timer >= _recallTime)
        {
            // 몬스터 스폰
            MonsterSpawn();
            // 타이머 초기화
            timer = 0f;
        }
    }

    // 보스는 움직이지 않음
    protected override void Movement()
    {

    }

    // 몬스터 스폰
    private void MonsterSpawn()
    {
        // 일반 몬스터 스테이지, 정예 몬스터 스테이지인지 확인하는 조건 필요


        for (int i = 0; i < _recallMonsterList.Count; i++)
        {
            // 생성을 위한 몬스터의 정보 확인
            MonsterDataSO monsterData = _recallMonsterList[i];

            //// 몬스터 스폰위치 설정
            Transform spawnPoint = _monsterRecallPoint[i];

            // 몬스터 생성
            GameObject monster = Instantiate(monsterData._prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public override void TakeDamage(float damage)
    {
        // 데미지를 받음, 방어력에 대한 것은??
        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            // 유닛의 죽음
            Death();
        }
    }
}
