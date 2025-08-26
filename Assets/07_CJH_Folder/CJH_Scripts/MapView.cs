using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{

    public Transform mapParent;


    // ����� �� ���ø� �������� ���⿡ ����
    [Header("Template Settings")]
    public GameObject mapTemplatePrefab;


    private GameObject currentMapInstance; // ���� ������ �� �ν��Ͻ��� ������ ����
    private MapData currentMap;


    public void CreateMapView(MapData map)
    {
        ClearMap();

        if (mapTemplatePrefab == null)
        {
            Debug.LogError("Map Template Prefab�� �������� �ʾҽ��ϴ�!");
            return;
        }

        currentMapInstance = Instantiate(mapTemplatePrefab, mapParent);
        currentMap = map;

        // 1. ���ø��� ��� ��� �ĺ��ڸ� �����ͼ� Dictionary�� ���� (���� ��ȸ�� ����)
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
                Debug.LogWarning($"�ߺ��� ��� �ĺ��� �߰�: ({p.floorIndex}, {p.nodeIndexInFloor})");
            }
            // �켱 ��� ��带 ��Ȱ��ȭ
            p.gameObject.SetActive(false);
        }

        // 2. �� �������� ��带 ��ȸ�ϸ� ���ø��� ��ġ�� ��Ī�ϰ� Ȱ��ȭ
        foreach (var dataNode in map.Nodes)
        {
            var point = new Vector2Int(dataNode.point.x, dataNode.point.y);
            if (placeholders.TryGetValue(point, out MapNodeIdentifier placeholder))
            {
                MapNode mapNode = placeholder.GetComponent<MapNode>();
                if (mapNode != null)
                {
                    mapNode.Setup(dataNode);
                    placeholder.gameObject.SetActive(true); // �����Ͱ� �ִ� ��常 Ȱ��ȭ
                }
            }
            else
            {
                Debug.LogWarning($"�� �������� ��� ({point.x}, {point.y})�� �ش��ϴ� ��ġ�� �����տ��� ã�� �� �����ϴ�.");
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