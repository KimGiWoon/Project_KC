using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [Header("Map Settings")]
    public GameObject mapNodePrefab;
    public Transform mapParent;

    private List<MapNode> renderedNodes = new();
    private MapData currentMap;

    //  CreateMapView 정의
    public void CreateMapView(MapData map)
    {
        ClearMap(); // 이전 맵 초기화

        currentMap = map;

        foreach (var node in map.Nodes)
        {
            GameObject nodeObj = Instantiate(mapNodePrefab, mapParent);
            MapNode mapNode = nodeObj.GetComponent<MapNode>();
            mapNode.Setup(node);
            renderedNodes.Add(mapNode);

            // 배치
            RectTransform rt = nodeObj.GetComponent<RectTransform>();
            rt.anchoredPosition = GetNodePosition(node);
        }

        // 이후 선 연결 등 처리 가능
        DrawConnections();
    }

    //  기존 노드 제거
    private void ClearMap()
    {
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        renderedNodes.Clear();
    }

    //  노드 위치 계산
    private Vector2 GetNodePosition(Node node)
    {
        float x = node.column * 100f;
        float y = -node.row * 100f;
        return new Vector2(x, y);
    }

    //  노드 간 연결선 (선택 사항)
    private void DrawConnections()
    {
        foreach (MapNode node in renderedNodes)
        {
            foreach (Node next in node.Node.nextNodes)
            {
                MapNode to = renderedNodes.Find(n => n.Node == next);
                if (to != null)
                {
                    // 선 그리는 로직 여기에 (LineRenderer 등)
                }
            }
        }
    }
}