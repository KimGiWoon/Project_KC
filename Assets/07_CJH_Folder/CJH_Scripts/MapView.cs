using System.Collections.Generic;
using System.Linq; // Linq 사용을 위해 추가
using UnityEngine;

public class MapView : MonoBehaviour
{

    public Transform mapParent;


    // 사용할 맵 템플릿 프리팹을 여기에 연결
    [Header("Template Settings")]
    public GameObject mapTemplatePrefab;


    private GameObject currentMapInstance; // 현재 생성된 맵 인스턴스를 저장할 변수
    private MapData currentMap;


    // CreateMapView를 템플릿 기반으로 완전히 새로 정의
    public void CreateMapView(MapData map)
    {
        ClearMap(); // 이전 맵 인스턴스 초기화

        if (mapTemplatePrefab == null)
        {
            Debug.LogError("Map Template Prefab이 지정되지 않았습니다!");
            return;
        }

        // 1. 템플릿 프리팹을 씬에 생성
        currentMapInstance = Instantiate(mapTemplatePrefab, mapParent);
        currentMap = map;

        // 2. 생성된 템플릿 안의 모든 노드 식별자(Identifier)를 가져옴
        var nodePlaceholders = currentMapInstance.GetComponentsInChildren<MapNodeIdentifier>();

        // 3. 맵 데이터의 실제 노드들을 가져옴
        var dataNodes = map.Nodes;

        // 4. 맵 데이터의 노드를 순회하며 템플릿의 각 위치에 매칭
        foreach (var dataNode in dataNodes)
        {
            // 데이터 노드의 위치(row, column)에 맞는 플레이스홀더를 찾음
            // 여기서는 floorIndex와 nodeIndexInFloor를 사용하지만,
            // 맵 생성 로직에 따라 매칭 기준을 유연하게 변경해야 할 수 있습니다.
            // 현재 MapGenerator는 노드를 순차적으로 만들지 않으므로,
            // 여기서는 임시로 순서대로 매칭합니다. 실제 프로젝트에서는 더 정교한 매칭이 필요할 수 있습니다.

            // 예시: dataNode.point.x (row)와 dataNode.point.y (column)를 사용해 매칭
            MapNodeIdentifier placeholder = nodePlaceholders
                .FirstOrDefault(p => p.floorIndex == dataNode.point.x && p.nodeIndexInFloor == dataNode.point.y);

            // FirstOrDefault가 복잡하다면, 간단하게 이름이나 순서로 매칭할 수도 있습니다.
            // 지금은 간단하게 dataNodes 리스트 순서와 placeholder 리스트 순서가 같다고 가정하고 진행합니다.
        }

        // 임시로, 가장 간단한 방식인 '순서대로' 매칭을 구현합니다.
        for (int i = 0; i < dataNodes.Count; i++)
        {
            if (i < nodePlaceholders.Length)
            {
                MapNode mapNode = nodePlaceholders[i].GetComponent<MapNode>();
                if (mapNode != null)
                {
                    mapNode.Setup(dataNodes[i]);
                    nodePlaceholders[i].gameObject.SetActive(true); // 노드를 활성화
                }
            }
        }

        // 데이터에 없는 템플릿 노드는 비활성화
        for (int i = dataNodes.Count; i < nodePlaceholders.Length; i++)
        {
            nodePlaceholders[i].gameObject.SetActive(false);
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