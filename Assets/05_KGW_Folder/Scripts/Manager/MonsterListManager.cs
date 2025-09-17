using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

public class MonsterListManager : MonoBehaviour
{
    [Header("Monster List")]
    [SerializeField] private MonsterDataSO[] _monsterList;

    private Dictionary<int, MonsterDataSO> _monListDic = new Dictionary<int, MonsterDataSO>();

    private void Awake()
    {
        foreach(var mon in _monsterList)
        {
            _monListDic[mon.MonId] = mon;
        }
    }

    // 몬스터 가져오기
    public MonsterDataSO GetMonster(int monID) => _monListDic.TryGetValue(monID, out var mon) ? mon : null;

}
