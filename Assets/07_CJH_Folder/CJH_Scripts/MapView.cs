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

    //  CreateMapView ����
    public void CreateMapView(MapData map)
    {
        ClearMap(); // ���� �� �ʱ�ȭ

        currentMap = map;

        foreach (var node in map.Nodes)
        {
            GameObject nodeObj = Instantiate(mapNodePrefab, mapParent);
            MapNode mapNode = nodeObj.GetComponent<MapNode>();
            mapNode.Setup(node);
            renderedNodes.Add(mapNode);

            // ��ġ
            RectTransform rt = nodeObj.GetComponent<RectTransform>();
            rt.anchoredPosition = GetNodePosition(node);
        }

        // ���� �� ���� �� ó�� ����
        DrawConnections();
    }

    //  ���� ��� ����
    private void ClearMap()
    {
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        renderedNodes.Clear();
    }

    //  ��� ��ġ ���
    private Vector2 GetNodePosition(Node node)
    {
        float x = node.column * 100f;
        float y = -node.row * 100f;
        return new Vector2(x, y);
    }

    //  ��� �� ���ἱ (���� ����)
    private void DrawConnections()
    {
        foreach (MapNode node in renderedNodes)
        {
            foreach (Node next in node.Node.nextNodes)
            {
                MapNode to = renderedNodes.Find(n => n.Node == next);
                if (to != null)
                {
                    // �� �׸��� ���� ���⿡ (LineRenderer ��)
                }
            }
        }
    }
}