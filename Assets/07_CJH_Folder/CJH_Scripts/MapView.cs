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


        foreach (var dataNode in map.Nodes)
        {
            if (nodeObjects.TryGetValue(dataNode.point, out MapNode mapNode))
            {
                mapNode.Setup(dataNode, mapConfig);
                mapNode.gameObject.SetActive(true);
            }
        }

        if (nodeObjects.TryGetValue(map.StartNode.point, out MapNode startMapNode))
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
            if (nodeObjects.TryGetValue(nextNodeData.point, out MapNode nextMapNode))
            {
                nextMapNode.Reveal();
            }
        }
    }
}