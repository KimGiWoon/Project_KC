using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SDW;

public class GrowthManager : MonoBehaviour
{
    [SerializeField] private List<GrowthDatas> _growthsDatas;
    [SerializeField] private Transform nodeContent; //노드들 들어있는 부모 오브젝트
    [SerializeField] private PermanentGrowthUI _permanentGrowthUI;
    //노드 데이터 저장 딕셔너리
    private Dictionary<int, GrowthDatas> growthDataDic = new Dictionary<int, GrowthDatas>();
    //노드 UI 저장 딕셔너리
    private Dictionary<int, GameObject> growthUIDic = new Dictionary<int, GameObject>();
    //해금된 노드 ID 중복없이 리스트에 저장 
    private GameManager _gameManager;
    private bool _isLoaded;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
            _isLoaded) return;
        LoadGrowthSO();
        ConnectUIAndData();
        if (!_gameManager.GrowthUnlockNodes.Contains(80201))
            UnlockNode(80201); //처음 노드만 활성화
        UpdateAllNode();
        _isLoaded = true;
    }

    private void LoadGrowthSO() //스크립터블오브젝트 자동으로 딕셔너리에 넣어주는 기능
    {
        foreach (var growthData in _growthsDatas)
        {
            if (growthDataDic.ContainsKey(growthData.nodeID)) //딕셔너리에 중복된 아이디가 있다면
                continue;
            growthDataDic[growthData.nodeID] = growthData; //없다면 넣기
        }
        Debug.Log($"{growthDataDic.Count}개의 노드를 로드");
    }

    private void ConnectUIAndData() //성장 데이터와 UI 연결시켜주는 기능
    {
        foreach (Transform child in nodeContent)
        {
            var growthNodeUI = child.GetComponent<GrowthNodeUI>();
            if (growthNodeUI == null) continue;

            if (int.TryParse(child.name, out int nodeID) && growthDataDic.TryGetValue(nodeID, out var growthData))
            {
                growthNodeUI.Init(growthData);
                growthUIDic[nodeID] = child.gameObject;
                _permanentGrowthUI.AddNode(growthNodeUI);
            }
        }
    }

    private bool IsCompleteNode(int nodeId) //해금 노드 Bool
        // if (growthDatas.nodeActiveCondition == -1) return true;
        => _gameManager.GrowthCompleteNodes.Contains(nodeId);

    //# unlock - 현재 노드가 활성화 가능해진 노드(Complete에 있으면 이미 해금)
    //# 하나가 complete가 되면, 다음 노드가 unlock 되어야 함
    //# 문제점 1. activate를 한 node가 계속 activate가 가능하다고 뜨고 있음
    //# 문제점 2. 다음 노드가 unlock되지 않음

    public void UpdateAllNode() //모든 노드 업데이트 
    {
        foreach (var growth in growthUIDic)
        {
            int nodeId = growth.Key; //해당 노드ID
            var nodeGO = growth.Value; //해당 노드 UI

            //노드 데이터가 있으면 True
            if (!growthDataDic.TryGetValue(nodeId, out var growthData)) continue;

            var growthNodeUI = nodeGO.GetComponent<GrowthNodeUI>(); //오브젝트에서 GrowthNodeUi 가져옴
            if (growthNodeUI == null) continue;

            bool canActivate;
            int activeCondition = growthData.nodeActiveCondition;

            bool isUnlocked;
            if (activeCondition == -1)
            {
                //todo 첫 번째 노드인 경우
                canActivate = !IsCompleteNode(nodeId);
                isUnlocked = _gameManager.GrowthUnlockNodes.Contains(nodeId);
                //can / unlocked
            }
            else
            {
                //# 이전꺼가 unlock이면서 canActivate가 false
                //false, true -> 다음 노드 활성화할 차례
                //true , true -> 해당 노드가 활성화할 차례
                canActivate = !IsCompleteNode(nodeId);
                isUnlocked = _gameManager.GrowthUnlockNodes.Contains(activeCondition) &&
                             IsCompleteNode(growthDataDic[activeCondition].nodeID);

                if (canActivate && isUnlocked) UnlockNode(nodeId);
            }

            // 노드 1 true true
            // 노드 2 -> 이 경우 반응하면 안됨
            // 노드 1 false true
            // 노드 2 -> 아 내 차례다

            growthNodeUI.NodeUIUpdate(isUnlocked, canActivate);
        }
    }

    private void UnlockNode(int nodeId)
    {
        if (_gameManager.GrowthUnlockNodes.Contains(nodeId)) return;

        _gameManager.AddGrowthUnlockNode(nodeId); //해금딕셔너리에 추가
    }
}