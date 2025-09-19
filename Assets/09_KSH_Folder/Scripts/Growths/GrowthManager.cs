using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthManager : MonoBehaviour
{
    [SerializeField] private Transform nodeContent; //노드들 들어있는 부모 오브젝트
    //노드 데이터 저장 딕셔너리
    private Dictionary<int, GrowthDatas> growthDataDic = new Dictionary<int, GrowthDatas>();
    //노드 UI 저장 딕셔너리
    private Dictionary<int, GameObject> growthUIDic = new Dictionary<int, GameObject>();
    //해금된 노드 ID 중복없이 리스트에 저장 
    private HashSet<int> growthUnlockNodes = new HashSet<int>();

    private void Awake()
    {
        LoadGrowthSO();
        ConnectUIAndData();
        UpdateAllNode();
        UnlockNode(80201); //처음 노드만 활성화
    }

    private void LoadGrowthSO() //스크립터블오브젝트 자동으로 딕셔너리에 넣어주는 기능
    {
        var growths = Resources.LoadAll<GrowthDatas>("Growths"); //리소스 파일에 있는 스크립터블 모두 받아와서 저장

        foreach (var growthData in growths)
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
            GrowthNodeUI growthNodeUI = child.GetComponent<GrowthNodeUI>();
            if(growthNodeUI == null) continue;

            if (int.TryParse(child.name, out int nodeID) && growthDataDic.TryGetValue(nodeID, out var growthData))
            {
                growthNodeUI.Init(growthData);
                growthUIDic[nodeID] = child.gameObject;
            }
        }
    }

    private bool IsUnlockNode(GrowthDatas growthDatas) //해금 노드 Bool
    {
        if (growthDatas.nodeActiveCondition == -1) return true;
        
        return growthUnlockNodes.Contains(growthDatas.nodeActiveCondition);
    }

    private void UpdateAllNode() //모든 노드 업데이트 
    {
        foreach (var growth in growthUIDic)
        {
            int nodeId = growth.Key; //해당 노드ID
            GameObject nodeGO = growth.Value; //해당 노드 UI
            
            //노드 데이터가 있으면 True
            if (!growthDataDic.TryGetValue(nodeId, out var growthData)) continue;
            
            GrowthNodeUI growthNodeUI = nodeGO.GetComponent<GrowthNodeUI>(); //오브젝트에서 GrowthNodeUi 가져옴
            if(growthNodeUI == null) continue;

            bool isUnlocked = growthUnlockNodes.Contains(nodeId);
            bool isCompleted = IsUnlockNode(growthData);
            
            growthNodeUI.NodeUIUpdate(isUnlocked, isCompleted);
        }
    }
    
    public void UnlockNode(int nodeId)
    {
        if (growthUnlockNodes.Contains(nodeId)) return;
        
        growthUnlockNodes.Add(nodeId); //해금딕셔너리에 추가
        UpdateAllNode(); //업데이트
    }
}
