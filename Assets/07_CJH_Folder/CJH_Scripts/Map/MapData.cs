using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapData
{
    public readonly List<List<Node>> Map;       // 2D Grid 구조 (floors x width)
    public readonly List<Node> Nodes;           // 실제로 배치된 모든 유효 노드 (NotAssgined 제외)
    public readonly Node BossNode;
    public readonly Node StartNode;
    public readonly List<List<Node>> AllPath;   // 모든 가능한 경로 (디버깅, 통계용)

    public readonly List<Node> Path;

    public Node CurrentNode => Path.LastOrDefault();

    public MapData(List<List<Node>> map, List<List<Node>> allPath, Node startNode, Node bossNode)
    {
        this.Map = map;
        this.AllPath = allPath;
        this.StartNode = startNode;
        this.BossNode = bossNode;

        // 유효한 노드만 포함
        Nodes = map.SelectMany(floor => floor)
                   .Where(node => node.nodeType != NodeType.NotAssigned)
                   .ToList();

        Path = new List<Node> { StartNode };
    }

    public Node GetNodeByPoint(int row, int column)
    {
        if (row < 0 || column < 0 || row >= Map.Count || column >= Map[0].Count)
            return null;

        return Map[row][column];
    }
}