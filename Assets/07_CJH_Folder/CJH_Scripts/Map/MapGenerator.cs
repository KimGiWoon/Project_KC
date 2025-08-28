using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject mapTemplatePrefab;


    public MapConfig config;

    private int _floors;
    private int _mapWidth;
    private List<List<Node>> _map;

    public MapData GenerateMap(MapConfig configToGenerate)
    {
        // 전달받은 config를 이 컴포넌트의 config 변수에 저장합니다.
        this.config = configToGenerate;
        Debug.Log("--- 맵 생성 시작 ---");

        InitSetting(this.config);
        _map = GenerateInitialGrid();

        var nodeIdentifiers = mapTemplatePrefab.GetComponentsInChildren<MapNodeIdentifier>();
        ActivateNodesFromPrefab(nodeIdentifiers);
        BuildConnectionsFromPrefab(nodeIdentifiers);

        Node startNode = _map.SelectMany(f => f).FirstOrDefault(n => n.nodeType == NodeType.Start);
        Node bossNode = _map.SelectMany(f => f).FirstOrDefault(n => n.nodeType == NodeType.Boss);

        if (startNode == null || bossNode == null)
        {
            Debug.LogError("프리팹에서 시작 또는 보스 노드를 찾을 수 없습니다. floorIndex를 확인하세요.");
            return null;
        }

        AssignNodeTypesToPaths(startNode, bossNode);

        Debug.Log("--- 모든 맵 생성 과정 완료 ---");
        List<List<Node>> allPaths = GetAllPaths(startNode, bossNode);
        return new MapData(_map, allPaths, startNode, bossNode);
    }

    private void InitSetting(MapConfig config)
    {
        _floors = config.floors;
        _mapWidth = config.mapWidth;
    }

    private List<List<Node>> GenerateInitialGrid()
    {
        var grid = new List<List<Node>>(_floors);
        for (int i = 0; i < _floors; i++)
        {
            var floor = new List<Node>(_mapWidth);
            for (int j = 0; j < _mapWidth; j++)
            {
                floor.Add(new Node(i, j) { nodeType = NodeType.NotAssigned });
            }
            grid.Add(floor);
        }
        return grid;
    }

    private void ActivateNodesFromPrefab(IEnumerable<MapNodeIdentifier> identifiers)
    {
        int maxFloorIndex = identifiers.Max(id => id.floorIndex);
        foreach (var id in identifiers)
        {
            Node node = _map[id.floorIndex][id.nodeIndexInFloor];

            if (id.floorIndex == 0) node.nodeType = NodeType.Start;
            else if (id.floorIndex == maxFloorIndex) node.nodeType = NodeType.Boss;
            else node.nodeType = NodeType.Event;
        }
    }

    private void BuildConnectionsFromPrefab(MapNodeIdentifier[] identifiers)
    {
        var identifierToNodeMap = identifiers.ToDictionary(
            id => id,
            id => _map[id.floorIndex][id.nodeIndexInFloor]
        );

        foreach (var parentId in identifiers)
        {
            if (parentId.connections == null || !parentId.connections.Any()) continue;
            Node parentNode = identifierToNodeMap[parentId];
            foreach (var childId in parentId.connections)
            {
                if (childId == null) continue;
                Node childNode = identifierToNodeMap[childId];
                if (!parentNode.nextNodes.Contains(childNode))
                {
                    parentNode.nextNodes.Add(childNode);
                    childNode.previousNodes.Add(parentNode);
                }
            }
        }
    }

    private void AssignNodeTypesToPaths(Node start, Node end)
    {
        foreach (var node in _map[start.point.x +1].Where(n => n.nodeType == NodeType.Event))
            node.nodeType = NodeType.Battle;
        
        foreach (var node in _map[4].Where(n => n.nodeType == NodeType.Battle))
            node.nodeType = NodeType.Event;

        var floor2Nodes = _map[2].Where(n => n.nodeType == NodeType.Event).ToList();
        foreach (var node2 in floor2Nodes)
        {
            node2.nodeType = (Random.value > 0.5f) ? NodeType.Battle : NodeType.Event;
        }

        var floor3Nodes = _map[3].Where(n => n.nodeType != NodeType.NotAssigned && n.nodeType != NodeType.Start && n.nodeType != NodeType.Boss).ToList();
        foreach (var childNode in floor3Nodes)
        {
            List<Node> parents = childNode.previousNodes;
            if (parents.Any())
            {
                childNode.nodeType = parents.Any(p => p.nodeType == NodeType.Event) ? NodeType.Battle : NodeType.Event;
            }
        }

        var eventNodes = _map.SelectMany(floor => floor).Where(node => node.nodeType == NodeType.Event);
        foreach (var eventNode in eventNodes)
        {
            int randomIndex = Random.Range(1, System.Enum.GetNames(typeof(EventTypeKC)).Length);
            eventNode.EventTypeKC = (EventTypeKC)randomIndex;
        }
    }

    private List<List<Node>> GetAllPaths(Node start, Node end)
    {
        var paths = new List<List<Node>>();
        DFS(start, end, new List<Node>(), paths);
        return paths;
    }

    private void DFS(Node current, Node end, List<Node> path, List<List<Node>> allPaths)
    {
        path.Add(current);
        if (current == end)
        {
            allPaths.Add(new List<Node>(path));
        }
        else
        {
            foreach (Node next in current.nextNodes)
            {
                DFS(next, end, path, allPaths);
            }
        }
        path.RemoveAt(path.Count - 1);
    }
}