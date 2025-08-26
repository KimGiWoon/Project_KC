using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{

    public Transform mapParent;


    // 사용할 맵 템플릿 프리팹을 여기에 연결
    [Header("Template Settings")]
    public GameObject mapTemplatePrefab;


    private GameObject currentMapInstance; // 현재 생성된 맵 인스턴스를 저장할 변수
    private MapData currentMap;
    private Dictionary<Vector2Int, MapNode> nodeObjects; // 좌표로 MapNode를 빠르게 찾기 위한 Dictionary
    public static MapView Instance; // 다른 스크립트에서 MapView에 쉽게 접근하기 위한 변수

    private void Awake()
    {
        // 싱글톤 패턴: 이 클래스의 인스턴스가 단 하나만 존재하도록 보장
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void CreateMapView(MapData map)
    {
        ClearMap();

        if (mapTemplatePrefab == null)
        {
            Debug.LogError("Map Template Prefab이 지정되지 않았습니다!");
            return;
        }

        currentMapInstance = Instantiate(mapTemplatePrefab, mapParent);
        currentMap = map;

        nodeObjects = new Dictionary<Vector2Int, MapNode>(); // Dictionary 초기화

        // 템플릿의 모든 노드를 Dictionary에 저장
        foreach (var placeholder in currentMapInstance.GetComponentsInChildren<MapNodeIdentifier>())
        {
            var point = new Vector2Int(placeholder.floorIndex, placeholder.nodeIndexInFloor);
            if (!nodeObjects.ContainsKey(point))
            {
                nodeObjects.Add(point, placeholder.GetComponent<MapNode>());
            }
            placeholder.gameObject.SetActive(false); // 우선 모든 노드 비활성화
        }

        // 맵 데이터의 노드를 순회하며 활성화 및 Setup
        foreach (var dataNode in map.Nodes)
        {
            if (nodeObjects.TryGetValue(dataNode.point, out MapNode mapNode))
            {
                mapNode.Setup(dataNode);
                mapNode.gameObject.SetActive(true);
            }
        }

        // 시작 노드를 찾아 즉시 '선택'하여 다음 노드들을 밝힙니다.
        var startMapNode = nodeObjects[map.StartNode.point];
        if (startMapNode != null)
        {
            SelectNode(startMapNode);
        }
    }

    private void ClearMap()
    {
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
        }
    }

    public void SelectNode(MapNode selectedNode)
    {
        Debug.Log(selectedNode.nodeData.point + " 노드를 선택했습니다. 다음 노드들을 공개합니다.");

        // 선택된 노드에서 갈 수 있는 모든 다음 노드(자식 노드)들을 순회합니다.
        foreach (var nextNodeData in selectedNode.nodeData.nextNodes)
        {
            // 데이터에 해당하는 MapNode 게임 오브젝트를 Dictionary에서 찾습니다.
            if (nodeObjects.TryGetValue(nextNodeData.point, out MapNode nextMapNode))
            {
                // 해당 MapNode의 Reveal() 함수를 호출합니다.
                nextMapNode.Reveal();
            }
        }
    }
}