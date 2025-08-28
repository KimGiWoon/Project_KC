using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapData
{
    public readonly List<List<Node>> Map;       // 2D Grid ���� (floors x width)
    public readonly List<Node> Nodes;           // ������ ��ġ�� ��� ��ȿ ��� (NotAssgined ����)
    public readonly List<Vector2Int> Path;      // �÷��̾ ���� ���
    public readonly Node BossNode;
    public readonly Node StartNode;
    public readonly List<List<Node>> AllPath;   // ��� ������ ��� (�����, ����)

    public Vector2Int CurrentNode => Path.Count > 0 ? Path[Path.Count - 1] : StartNode.point;

    public MapData(List<List<Node>> map, List<List<Node>> allPath, Node startNode, Node bossNode)
    {
        this.Map = map;
        this.AllPath = allPath;
        this.StartNode = startNode;
        this.BossNode = bossNode;

        // ��ȿ�� ��常 ����
        Nodes = map.SelectMany(floor => floor)
                   .Where(node => node.nodeType != NodeType.NotAssigned)
                   .ToList();

        Path = new List<Vector2Int>();
    }

    public Node GetNodeByPoint(int row, int column)
    {
        if (row < 0 || column < 0 || row >= Map.Count || column >= Map[0].Count)
            return null;

        return Map[row][column];
    }
}