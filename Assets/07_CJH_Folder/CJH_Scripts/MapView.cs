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

        int maxFloorIndex = map.Map.Count - 1;

        foreach (var dataNode in map.Nodes)
        {
            int visualFloor = maxFloorIndex - dataNode.point.x;
            var visualPoint = new Vector2Int(visualFloor, dataNode.point.y);

            if (nodeObjects.TryGetValue(visualPoint, out MapNode mapNode))
            {
                mapNode.Setup(dataNode, mapConfig);
                mapNode.gameObject.SetActive(true);

                // 타입이 Battle이 되었을 경우 다시 Sprite 적용
                if (dataNode.nodeType == NodeType.Battle || dataNode.nodeType == NodeType.Start || dataNode.nodeType == NodeType.Boss)
                {
                    mapNode.Reveal(); 
                }
            }
        }

        int startVisualFloor = maxFloorIndex - map.StartNode.point.x;
        var startVisualPoint = new Vector2Int(startVisualFloor, map.StartNode.point.y);

        if (nodeObjects.TryGetValue(startVisualPoint, out MapNode startMapNode))
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
        // 현재 맵의 최대 층 인덱스를 가져옵니다.
        int maxFloorIndex = currentMap.Map.Count - 1;
        Debug.Log(selectedNode.nodeData.point + " 노드를 선택했습니다. 다음 노드들을 공개합니다.");

        foreach (var nextNodeData in selectedNode.nodeData.nextNodes)
        {
            // 다음 노드의 논리적 좌표를 프리팹의 시각적 좌표로 변환합니다.
            int visualFloor = maxFloorIndex - nextNodeData.point.x;
            var visualPoint = new Vector2Int(visualFloor, nextNodeData.point.y);

            // 변환된 시각적 좌표로 Dictionary에서 다음 노드를 찾습니다.
            if (nodeObjects.TryGetValue(visualPoint, out MapNode nextMapNode))
            {
                nextMapNode.Reveal();
            }
        }
    }
}