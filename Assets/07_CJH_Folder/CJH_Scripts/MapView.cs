using System.Collections.Generic;
using System.Linq; // Linq ����� ���� �߰�
using UnityEngine;

public class MapView : MonoBehaviour
{

    public Transform mapParent;


    // ����� �� ���ø� �������� ���⿡ ����
    [Header("Template Settings")]
    public GameObject mapTemplatePrefab;


    private GameObject currentMapInstance; // ���� ������ �� �ν��Ͻ��� ������ ����
    private MapData currentMap;


    // CreateMapView�� ���ø� ������� ������ ���� ����
    public void CreateMapView(MapData map)
    {
        ClearMap(); // ���� �� �ν��Ͻ� �ʱ�ȭ

        if (mapTemplatePrefab == null)
        {
            Debug.LogError("Map Template Prefab�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // 1. ���ø� �������� ���� ����
        currentMapInstance = Instantiate(mapTemplatePrefab, mapParent);
        currentMap = map;

        // 2. ������ ���ø� ���� ��� ��� �ĺ���(Identifier)�� ������
        var nodePlaceholders = currentMapInstance.GetComponentsInChildren<MapNodeIdentifier>();

        // 3. �� �������� ���� ������ ������
        var dataNodes = map.Nodes;

        // 4. �� �������� ��带 ��ȸ�ϸ� ���ø��� �� ��ġ�� ��Ī
        foreach (var dataNode in dataNodes)
        {
            // ������ ����� ��ġ(row, column)�� �´� �÷��̽�Ȧ���� ã��
            // ���⼭�� floorIndex�� nodeIndexInFloor�� ���������,
            // �� ���� ������ ���� ��Ī ������ �����ϰ� �����ؾ� �� �� �ֽ��ϴ�.
            // ���� MapGenerator�� ��带 ���������� ������ �����Ƿ�,
            // ���⼭�� �ӽ÷� ������� ��Ī�մϴ�. ���� ������Ʈ������ �� ������ ��Ī�� �ʿ��� �� �ֽ��ϴ�.

            // ����: dataNode.point.x (row)�� dataNode.point.y (column)�� ����� ��Ī
            MapNodeIdentifier placeholder = nodePlaceholders
                .FirstOrDefault(p => p.floorIndex == dataNode.point.x && p.nodeIndexInFloor == dataNode.point.y);

            // FirstOrDefault�� �����ϴٸ�, �����ϰ� �̸��̳� ������ ��Ī�� ���� �ֽ��ϴ�.
            // ������ �����ϰ� dataNodes ����Ʈ ������ placeholder ����Ʈ ������ ���ٰ� �����ϰ� �����մϴ�.
        }

        // �ӽ÷�, ���� ������ ����� '�������' ��Ī�� �����մϴ�.
        for (int i = 0; i < dataNodes.Count; i++)
        {
            if (i < nodePlaceholders.Length)
            {
                MapNode mapNode = nodePlaceholders[i].GetComponent<MapNode>();
                if (mapNode != null)
                {
                    mapNode.Setup(dataNodes[i]);
                    nodePlaceholders[i].gameObject.SetActive(true); // ��带 Ȱ��ȭ
                }
            }
        }

        // �����Ϳ� ���� ���ø� ���� ��Ȱ��ȭ
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