using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject mapTemplatePrefab;

    private int _floors;
    private int _mapWidth;
    private List<List<Node>> _map;

    public MapData GenerateMap(MapConfig config)
    {
        Debug.Log("--- 맵 생성 시작 ---");

        InitSetting(config);
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
        Debug.Log("--- 단계 1 프리팹에서 노드 활성화 ---");
        int maxFloorIndex = identifiers.Max(id => id.floorIndex);

        foreach (var id in identifiers)
        {
            Node node = _map[id.floorIndex][id.nodeIndexInFloor];

            if (id.floorIndex == 0) node.nodeType = NodeType.Boss;
            else if (id.floorIndex == maxFloorIndex) node.nodeType = NodeType.Start;
            else node.nodeType = NodeType.Event;

            Debug.Log($"Node({node.point.x}, {node.point.y}) 활성화, 초기 타입: {node.nodeType}");
        }
    }

    private void BuildConnectionsFromPrefab(MapNodeIdentifier[] identifiers)
    {
        Debug.Log("--- 단계 2 프리팹에서 연결 정보 읽기 시작 ---");
        var identifierToNodeMap = identifiers.ToDictionary(
            id => id,
            id => _map[id.floorIndex][id.nodeIndexInFloor]
        );

        int totalConnectionsFound = 0;

        foreach (var parentId in identifiers)
        {
            // 각 부모 노드가 프리팹에 몇 개의 자식을 연결하고 있는지 로그로 확인합니다.
            if (parentId.connections != null && parentId.connections.Any())
            {
                Debug.Log($"프리팹 정보 확인: 부모 {parentId.gameObject.name} ({parentId.floorIndex},{parentId.nodeIndexInFloor}) / 연결된 자식 수: {parentId.connections.Count}");

                Node parentNode = identifierToNodeMap[parentId];

                foreach (var childId in parentId.connections)
                {
                    if (childId == null) continue;

                    Node childNode = identifierToNodeMap[childId];
                    if (!parentNode.nextNodes.Contains(childNode))
                    {
                        parentNode.nextNodes.Add(childNode);
                        childNode.previousNodes.Add(parentNode);
                        totalConnectionsFound++;
                    }
                }
            }
        }
        Debug.Log($"프리팹으로부터 총 {totalConnectionsFound}개의 연결을 설정했습니다.");
    }

    // MapGenerator.cs 파일에서 이 함수도 찾아서 교체해주세요. (훨씬 간단해집니다)
    private void AssignNodeTypesToPaths(Node start, Node end)
    {
        Debug.Log("--- 단계 3 노드 타입(색상) 할당 시작 ---");

        // 고정 규칙: 1층은 전투, 4층은 이벤트
        foreach (var node in _map[start.point.x - 1].Where(n => n.nodeType == NodeType.Event))
            node.nodeType = NodeType.Battle;

        foreach (var node in _map[1].Where(n => n.nodeType == NodeType.Event))
            node.nodeType = NodeType.Event; // 이 규칙은 그대로 Event로 두겠습니다.

        Debug.Log("고정 규칙 적용 완료: 1층(Battle), 4층(Event)");

        // 2층(부모) 노드들의 타입을 모두 무작위로 결정
        Debug.Log("--- 2층(부모) 노드 타입 무작위 결정 ---");
        var floor2Nodes = _map[3].Where(n => n.nodeType == NodeType.Event).ToList();
        foreach (var node2 in floor2Nodes)
        {
            node2.nodeType = (Random.value > 0.5f) ? NodeType.Battle : NodeType.Event;
            Debug.Log($"Node ({node2.point.x}, {node2.point.y}) 타입 무작위 결정: {node2.nodeType}");
        }

        // 3층(자식) 노드들의 타입을 결정 (이제 childToParentsMap이 필요 없습니다!)
        Debug.Log("--- 3층(자식) 노드 최종 타입 결정 ---");
        var floor3Nodes = _map[2].Where(n => n.nodeType != NodeType.NotAssigned && n.nodeType != NodeType.Start && n.nodeType != NodeType.Boss).ToList();

        foreach (var childNode in floor3Nodes)
        {
            // 각 자식 노드가 직접 기억하고 있는 부모 리스트(previousNodes)를 사용합니다.
            List<Node> parents = childNode.previousNodes;

            string parentTypes = string.Join(", ", parents.Select(p => p.nodeType.ToString()));
            Debug.Log($"처리 시작: Node({childNode.point.x}, {childNode.point.y}). 연결된 부모 수: {parents.Count}. 부모 타입: [{parentTypes}]");

            if (parents.Any())
            {
                bool hasEventParent = parents.Any(p => p.nodeType == NodeType.Event);
                if (hasEventParent)
                {
                    childNode.nodeType = NodeType.Battle;
                    Debug.Log($"===> 결과: 부모 중 Event(파란색) 있음. 최종 타입을 Battle(보라색)으로 결정.");
                }
                else
                {
                    childNode.nodeType = NodeType.Event;
                    Debug.Log($"===> 결과: 모든 부모가 Battle(보라색). 최종 타입을 Event(파란색)으로 결정.");
                }
            }
            else
            {
                Debug.Log($"===> 결과: 연결된 부모 없음. 기본 타입 유지.");
            }
        }

        Debug.Log("--- 이벤트 노드 세부 타입 랜덤 할당 ---");
        // 맵에 있는 모든 'Event' 타입 노드를 찾습니다.
        var eventNodes = _map.SelectMany(floor => floor)
                              .Where(node => node.nodeType == NodeType.Event);

        foreach (var eventNode in eventNodes)
        {
            // EventType Enum에서 NotAssigned(0)를 제외하고 1, 2, 3 중에서 랜덤 선택
            int randomIndex = Random.Range(1, System.Enum.GetNames(typeof(EventType)).Length);
            eventNode.eventType = (EventType)randomIndex;
            Debug.Log($"Node ({eventNode.point.x}, {eventNode.point.y})의 이벤트 타입을 '{eventNode.eventType}'로 결정");
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