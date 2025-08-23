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

        // 1. 템플릿의 모든 노드 식별자를 가져와서 Dictionary에 저장 (빠른 조회를 위함)
        var placeholders = new Dictionary<Vector2Int, MapNodeIdentifier>();
        foreach (var p in currentMapInstance.GetComponentsInChildren<MapNodeIdentifier>())
        {
            var point = new Vector2Int(p.floorIndex, p.nodeIndexInFloor);
            if (!placeholders.ContainsKey(point))
            {
                placeholders.Add(point, p);
            }
            else
            {
                Debug.LogWarning($"중복된 노드 식별자 발견: ({p.floorIndex}, {p.nodeIndexInFloor})");
            }
            // 우선 모든 노드를 비활성화
            p.gameObject.SetActive(false);
        }

        // 2. 맵 데이터의 노드를 순회하며 템플릿의 위치에 매칭하고 활성화
        foreach (var dataNode in map.Nodes)
        {
            var point = new Vector2Int(dataNode.point.x, dataNode.point.y);
            if (placeholders.TryGetValue(point, out MapNodeIdentifier placeholder))
            {
                MapNode mapNode = placeholder.GetComponent<MapNode>();
                if (mapNode != null)
                {
                    mapNode.Setup(dataNode);
                    placeholder.gameObject.SetActive(true); // 데이터가 있는 노드만 활성화
                }
            }
            else
            {
                Debug.LogWarning($"맵 데이터의 노드 ({point.x}, {point.y})에 해당하는 위치를 프리팹에서 찾을 수 없습니다.");
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