using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // 맵 좌표 배치 관련 설정값
    private int _xDist;
    private int _yGap;
    private int _placementRandomness;

    // 맵의 크기 관련 설정값
    private int _floors;
    private int _mapWidth;

    // 전투 노드 제한 설정값
    private int _battleCount = 0;
    private const int MaxBattleCount = 3;
    private bool _forceMidBattleUsed = false; // 중간층 강제 Battle 사용 여부


    // 전체 맵 구조 (층별 노드)
    private List<List<Node>> _map;

    // 시작 → 보스까지의 가능한 경로들
    private List<List<Node>> _allPath;

    // 메인 진입점: 맵 생성 실행
    public MapData GenerateMap(MapConfig config)
    {
        InitSetting(config);
        _map = GenerateInitialGrid();

        // 4층: 시작 → 3층 연결
        Node startNode = SetUpStartAndFloorOne();

        // 3층 → 2층 연결
        ConnectFloorToNext(3);

        // 2층 → 1층(Event) → 0층(Boss) 연결
        Node bossNode = SetUpBeforBossAndBoss();

        // 모든 경로 수집 (DFS)
        _allPath = GetAllPaths(startNode, bossNode);

        return new MapData(_map, _allPath, startNode, bossNode);
    }

    // 설정값 초기화
    private void InitSetting(MapConfig config)
    {
        _xDist = config.xDist;
        _yGap = config.yGap;
        _placementRandomness = config.placementRandomness;

        _floors = config.floors;
        _mapWidth = config.mapWidth;
    }

    // 2D 맵 그리드 초기 생성
    private List<List<Node>> GenerateInitialGrid()
    {
        List<List<Node>> grid = new(_floors);

        for (int i = 0; i < _floors; i++)
        {
            List<Node> floorRooms = new List<Node>(_mapWidth);
            for (int j = 0; j < _mapWidth; j++)
            {
                Node node = new Node(i, j);
                node.nodeType = NodeType.NotAssgined;   // 초기 상태
                node.nextNodes.Clear();
                floorRooms.Add(node);
            }
            grid.Add(floorRooms);
        }

        return grid;
    }

    // 랜덤으로 Battle 또는 Event 노드 반환
    private NodeType GetRandomRoomType()
    {
        if (_battleCount >= MaxBattleCount - 1)
            return NodeType.Event;

        NodeType type = (Random.value < 0.5f) ? NodeType.Battle : NodeType.Event;

        if (type == NodeType.Battle)
            _battleCount++;

        return type;
    }

    // 시작층(4층)과 3층 설정
    private Node SetUpStartAndFloorOne()
    {
        int middle = _mapWidth / 2;

        // 4층 중앙에 시작 노드 생성
        Node start = _map[4][middle];
        start.nodeType = NodeType.Battle;
        _battleCount++; // 시작 전투 1회

        // 3층: 1~3개의 랜덤 노드 생성
        List<Node> floor3Nodes = new();
        for (int j = 0; j < _mapWidth; j++)
        {
            if (Random.value < 0.4f && floor3Nodes.Count < 3)
            {
                Node node = _map[3][j];
                node.nodeType = GetRandomRoomType();
                floor3Nodes.Add(node);
            }
        }

        // 노드가 없을 경우 중앙에 1개 강제 생성
        if (floor3Nodes.Count == 0)
        {
            Node fallback = _map[3][middle];
            fallback.nodeType = GetRandomRoomType();
            floor3Nodes.Add(fallback);
        }

        // 시작 노드 → 3층 노드들 연결
        foreach (var node in floor3Nodes)
            start.nextNodes.Add(node);

        return start;
    }

    // upperRow → upperRow - 1 연결 (ex. 3층 → 2층)
    private void ConnectFloorToNext(int upperRow)
    {
        List<Node> upperNodes = _map[upperRow].Where(n => n.nodeType != NodeType.NotAssgined).ToList();
        List<Node> lowerNodes = new();

        // 2층: 1~3개 랜덤 노드 생성
        for (int j = 0; j < _mapWidth; j++)
        {
            if (Random.value < 0.4f && lowerNodes.Count < 3)
            {
                Node node = _map[upperRow - 1][j];
                node.nodeType = GetRandomRoomType();
                lowerNodes.Add(node);
            }
        }

        // 노드 없을 시 중앙 노드 강제 생성
        if (lowerNodes.Count == 0)
        {
            Node fallback = _map[upperRow - 1][_mapWidth / 2];
            fallback.nodeType = GetRandomRoomType();
            lowerNodes.Add(fallback);
        }

        // 상위층 → 하위층 연결
        foreach (var from in upperNodes)
        {
            foreach (var to in lowerNodes)
            {
                from.nextNodes.Add(to);
            }
        }

        if (_battleCount < MaxBattleCount && !_forceMidBattleUsed)
        {
            Node forceBattle = lowerNodes[Random.Range(0, lowerNodes.Count)];
            forceBattle.nodeType = NodeType.Battle;
            _battleCount++;
            _forceMidBattleUsed = true;
        }
    }

    // 보스층(0)과 이벤트층(1), 2층 연결
    private Node SetUpBeforBossAndBoss()
    {
        int middle = _mapWidth / 2;

        // 0층 중앙에 보스 노드 생성
        Node boss = _map[0][middle];
        boss.nodeType = NodeType.Boss;
        _battleCount++; // 보스 전투 1회

        // 1층: 1~3개의 이벤트 노드 생성
        List<Node> beforeBossNodes = new();
        for (int j = 0; j < _mapWidth; j++)
        {
            if (Random.value < 0.4f && beforeBossNodes.Count < 3)
            {
                Node node = _map[1][j];
                node.nodeType = NodeType.Event;
                node.nextNodes.Add(boss);
                beforeBossNodes.Add(node);
            }
        }

        // 없으면 중앙에 1개 강제 생성
        if (beforeBossNodes.Count == 0)
        {
            Node fallback = _map[1][middle];
            fallback.nodeType = NodeType.Event;
            fallback.nextNodes.Add(boss);
            beforeBossNodes.Add(fallback);
        }

        // 2층 노드들 → 1층 이벤트 노드 연결
        for (int j = 0; j < _mapWidth; j++)
        {
            Node node = _map[2][j];
            if (node.nodeType != NodeType.NotAssgined)
            {
                Node selected = beforeBossNodes[Random.Range(0, beforeBossNodes.Count)];
                node.nextNodes.Add(selected);
            }
        }

        return boss;
    }

    // DFS를 통한 시작 → 보스 경로 탐색
    private List<List<Node>> GetAllPaths(Node start, Node end)
    {
        List<List<Node>> paths = new();
        List<Node> currentPath = new();
        DFS(start, end, currentPath, paths);
        return paths;
    }

    // 깊이 우선 탐색
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