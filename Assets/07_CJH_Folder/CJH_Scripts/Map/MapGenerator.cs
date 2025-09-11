using SDW;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject mapTemplatePrefab;

    public MapConfig config;

    [SerializeField] private DataManager _dataManager;

    private int _floors;
    private int _mapWidth;
    private List<List<Node>> _map;
    private List<EncounterData> availableEncounters;

    public MapData GenerateMap(MapConfig configToGenerate)
    {
        // Resources 폴더에서 EncounterData를 불러옵니다.
        availableEncounters = Resources.LoadAll<EncounterData>("Data/EncounterData").ToList();

        // 전달받은 config를 이 컴포넌트의 config 변수에 저장합니다.
        config = configToGenerate;
        // Debug.Log("--- 맵 생성 시작 ---");

        InitSetting(config);
        _map = GenerateInitialGrid();

        var nodeIdentifiers = mapTemplatePrefab.GetComponentsInChildren<MapNodeIdentifier>(true);
        ActivateNodesFromPrefab(nodeIdentifiers);
        BuildConnectionsFromPrefab(nodeIdentifiers);

        var startNode = _map.SelectMany(f => f).FirstOrDefault(n => n.nodeType == NodeType.Start);
        var bossNode = _map.SelectMany(f => f).FirstOrDefault(n => n.nodeType == NodeType.Boss);

        if (startNode == null || bossNode == null)
        {
            Debug.LogError("프리팹에서 시작 또는 보스 노드를 찾을 수 없습니다. floorIndex를 확인하세요.");
            return null;
        }

        AssignNodeTypesToPaths(startNode, bossNode);

        // Debug.Log("--- 모든 맵 생성 과정 완료 ---");
        var allPaths = GetAllPaths(startNode, bossNode);
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
            var node = _map[id.floorIndex][id.nodeIndexInFloor];

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
            var parentNode = identifierToNodeMap[parentId];
            foreach (var childId in parentId.connections)
            {
                if (childId == null) continue;
                var childNode = identifierToNodeMap[childId];
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
        foreach (var node in _map[start.point.x + 1].Where(n => n.nodeType == NodeType.Event))
        {
            node.nodeType = NodeType.Battle;
        }

        foreach (var node in _map[4].Where(n => n.nodeType == NodeType.Battle))
        {
            node.nodeType = NodeType.Event;
        }

        var floor2Nodes = _map[2].Where(n => n.nodeType == NodeType.Event).ToList();
        foreach (var node2 in floor2Nodes)
        {
            node2.nodeType = Random.value > 0.5f ? NodeType.Battle : NodeType.Event;
        }

        var floor3Nodes = _map[3].Where(n =>
            n.nodeType != NodeType.NotAssigned && n.nodeType != NodeType.Start && n.nodeType != NodeType.Boss).ToList();
        foreach (var childNode in floor3Nodes)
        {
            var parents = childNode.previousNodes;
            if (parents.Any())
            {
                childNode.nodeType = parents.Any(p => p.nodeType == NodeType.Event) ? NodeType.Battle : NodeType.Event;
            }
        }

        var eventNodes = _map.SelectMany(floor => floor).Where(node => node.nodeType == NodeType.Event);
        foreach (var eventNode in eventNodes)
        {
            // 1. 노드에 긍정/부정/중립/미묘 감정 타입을 랜덤으로 할당
            // EventTypeKC enum에서 실제 감정을 나타내는 값들만 추립니다.
            var sentimentTypes = new List<EventTypeKC>
                { EventTypeKC.Positive, EventTypeKC.Negative, EventTypeKC.Neutral, EventTypeKC.Subtlety };
            eventNode.EventTypeKC = sentimentTypes[Random.Range(0, sentimentTypes.Count)];

            // 2. 위에서 만든 변환 함수를 사용해 안전하게 Sentiment 타입을 얻습니다.
            var sentiment = ConvertEventTypeToSentiment(eventNode.EventTypeKC);

            // 3. DataManager에게 노드의 감정 타입과 현재 스테이지에 맞는 랜덤 사건 ID를 요청하여 저장
            int currentStage = eventNode.point.x; // 노드의 floor index를 스테이지로 간주
            if (_dataManager != null)
            {
                eventNode.EncounterID = _dataManager.GetRandomEncounterID(sentiment, currentStage);
                // Debug.Log(
                //     $"노드 ({eventNode.point.x}, {eventNode.point.y})에 감정({sentiment}), 스테이지({currentStage})에 따른 사건 ID {eventNode.EncounterID} 할당됨.");
            }
            else
            {
                Debug.LogError("DataManager.Instance가 없습니다! Script Execution Order를 확인해주세요.");
            }
        }
    }

    private EncounterSentiment ConvertEventTypeToSentiment(EventTypeKC eventType)
    {
        switch (eventType)
        {
            case EventTypeKC.Positive:
                return EncounterSentiment.Good;
            case EventTypeKC.Negative:
                return EncounterSentiment.Bad;
            case EventTypeKC.Neutral:
                return EncounterSentiment.Fixed;
            case EventTypeKC.Subtlety:
                return EncounterSentiment.Subtlety;
            default:
                return EncounterSentiment.None; // 그 외의 경우는 없음(None) 처리
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
            foreach (var next in current.nextNodes)
            {
                // 순환 구조가 있을 때 무한 루프를 방지
                if (!path.Contains(next))
                {
                    DFS(next, end, path, allPaths);
                }
            }
        }

        path.RemoveAt(path.Count - 1);
    }
}