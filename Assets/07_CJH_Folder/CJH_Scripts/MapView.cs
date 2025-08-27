using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapView : MonoBehaviour
{
    public Transform mapParent;
    public GameObject mapTemplatePrefab;
    public MapConfig mapConfig;

    private GameObject currentMapInstance;
    private MapData currentMap;
    private Dictionary<Vector2Int, MapNode> nodeObjects;
    public static MapView Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CreateMapView(MapData map)
    {
        ClearMap();
        currentMap = map;
        currentMapInstance = Instantiate(mapTemplatePrefab, mapParent);
        nodeObjects = new Dictionary<Vector2Int, MapNode>();

        foreach (var placeholder in currentMapInstance.GetComponentsInChildren<MapNodeIdentifier>())
        {
            var point = new Vector2Int(placeholder.floorIndex, placeholder.nodeIndexInFloor);
            if (!nodeObjects.ContainsKey(point))
            {
                nodeObjects.Add(point, placeholder.GetComponent<MapNode>());
            }
            placeholder.gameObject.SetActive(false);
        }

        foreach (var dataNode in map.Nodes)
        {
            if (nodeObjects.TryGetValue(dataNode.point, out MapNode mapNode))
            {
                mapNode.Setup(dataNode, mapConfig);
                mapNode.gameObject.SetActive(true);
            }
        }

        // 게임 시작 시 초기 맵 상태 업데이트
        UpdateMapState();
    }

    // [수정!] SelectNode는 이제 경로를 업데이트하고 맵 상태를 갱신
    public void SelectNode(MapNode selectedNode)
    {
        // 이미 방문한 노드는 다시 선택할 수 없음
        if (currentMap.Path.Contains(selectedNode.nodeData)) return;

        // 플레이어의 경로에 현재 노드를 추가
        currentMap.Path.Add(selectedNode.nodeData);

        // 맵 전체의 상태를 새로고침
        UpdateMapState();
    }

    // [추가!] 맵의 모든 노드 상태를 업데이트하는 핵심 함수
    private void UpdateMapState()
    {
        // 현재 플레이어가 위치한 노드를 가져옵니다.
        Node currentNode = currentMap.CurrentNode;
        if (currentNode == null) return;

        // 맵의 모든 노드를 순회합니다.
        foreach (var mapNode in nodeObjects.Values)
        {
            // 1. 다음으로 갈 수 있는 노드(자식 노드)인지 확인
            bool isNextNode = currentNode.nextNodes.Contains(mapNode.nodeData);

            // 2. 해당 노드를 공개(Reveal)하고 선택 가능(Selectable) 상태로 만듭니다.
            if (isNextNode)
            {
                mapNode.Reveal();
                mapNode.SetSelectable(true);
            }
            // 3. 그 외 모든 노드는 선택 불가능 상태로 만듭니다.
            else
            {
                mapNode.SetSelectable(false);
            }
        }
    }

    private void ClearMap()
    {
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
        }
    }
}