using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

public class MonsterListManager : MonoBehaviour
{
    [Header("Monster List")]
    [SerializeField] private MonsterDataSO[] _monsterList;

    private Dictionary<int, MonsterDataSO> _monListDic = new Dictionary<int, MonsterDataSO>();
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        StartCoroutine(LoadCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        while (true)
        {
            yield return null;
            if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                !_gameManager.Firebase.IsLoaded) continue;

            break;
        }

        LoadMonsterList();
    }

    private void LoadMonsterList()
    {
        foreach (var mon in _monsterList)
        {
            _monListDic[mon.MonId] = mon;
        }
    }

    // 몬스터 가져오기
    public MonsterDataSO GetMonster(int monID) => _monListDic.TryGetValue(monID, out var mon) ? mon : null;
}