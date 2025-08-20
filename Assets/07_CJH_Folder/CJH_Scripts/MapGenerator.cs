using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // �� ��ǥ ��ġ ���� ������
    private int _xDist;
    private int _yGap;
    private int _placementRandomness;

    // ���� ũ�� ���� ������
    private int _floors;
    private int _mapWidth;

    // ���� ��� ���� ������
    private int _battleCount = 0;
    private const int MaxBattleCount = 3;
    private bool _forceMidBattleUsed = false; // �߰��� ���� Battle ��� ����


    // ��ü �� ���� (���� ���)
    private List<List<Node>> _map;

    // ���� �� ���������� ������ ��ε�
    private List<List<Node>> _allPath;

    // ���� ������: �� ���� ����
    public MapData GenerateMap(MapConfig config)
    {
        InitSetting(config);
        _map = GenerateInitialGrid();

        // 4��: ���� �� 3�� ����
        Node startNode = SetUpStartAndFloorOne();

        // 3�� �� 2�� ����
        ConnectFloorToNext(3);

        // 2�� �� 1��(Event) �� 0��(Boss) ����
        Node bossNode = SetUpBeforBossAndBoss();

        // ��� ��� ���� (DFS)
        _allPath = GetAllPaths(startNode, bossNode);

        return new MapData(_map, _allPath, startNode, bossNode);
    }

    // ������ �ʱ�ȭ
    private void InitSetting(MapConfig config)
    {
        _xDist = config.xDist;
        _yGap = config.yGap;
        _placementRandomness = config.placementRandomness;

        _floors = config.floors;
        _mapWidth = config.mapWidth;
    }

    // 2D �� �׸��� �ʱ� ����
    private List<List<Node>> GenerateInitialGrid()
    {
        List<List<Node>> grid = new(_floors);

        for (int i = 0; i < _floors; i++)
        {
            List<Node> floorRooms = new List<Node>(_mapWidth);
            for (int j = 0; j < _mapWidth; j++)
            {
                Node node = new Node(i, j);
                node.nodeType = NodeType.NotAssgined;   // �ʱ� ����
                node.nextNodes.Clear();
                floorRooms.Add(node);
            }
            grid.Add(floorRooms);
        }

        return grid;
    }

    // �������� Battle �Ǵ� Event ��� ��ȯ
    private NodeType GetRandomRoomType()
    {
        if (_battleCount >= MaxBattleCount - 1)
            return NodeType.Event;

        NodeType type = (Random.value < 0.5f) ? NodeType.Battle : NodeType.Event;

        if (type == NodeType.Battle)
            _battleCount++;

        return type;
    }

    // ������(4��)�� 3�� ����
    private Node SetUpStartAndFloorOne()
    {
        int middle = _mapWidth / 2;

        // 4�� �߾ӿ� ���� ��� ����
        Node start = _map[4][middle];
        start.nodeType = NodeType.Battle;
        _battleCount++; // ���� ���� 1ȸ

        // 3��: 1~3���� ���� ��� ����
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

        // ��尡 ���� ��� �߾ӿ� 1�� ���� ����
        if (floor3Nodes.Count == 0)
        {
            Node fallback = _map[3][middle];
            fallback.nodeType = GetRandomRoomType();
            floor3Nodes.Add(fallback);
        }

        // ���� ��� �� 3�� ���� ����
        foreach (var node in floor3Nodes)
            start.nextNodes.Add(node);

        return start;
    }

    // upperRow �� upperRow - 1 ���� (ex. 3�� �� 2��)
    private void ConnectFloorToNext(int upperRow)
    {
        List<Node> upperNodes = _map[upperRow].Where(n => n.nodeType != NodeType.NotAssgined).ToList();
        List<Node> lowerNodes = new();

        // 2��: 1~3�� ���� ��� ����
        for (int j = 0; j < _mapWidth; j++)
        {
            if (Random.value < 0.4f && lowerNodes.Count < 3)
            {
                Node node = _map[upperRow - 1][j];
                node.nodeType = GetRandomRoomType();
                lowerNodes.Add(node);
            }
        }

        // ��� ���� �� �߾� ��� ���� ����
        if (lowerNodes.Count == 0)
        {
            Node fallback = _map[upperRow - 1][_mapWidth / 2];
            fallback.nodeType = GetRandomRoomType();
            lowerNodes.Add(fallback);
        }

        // ������ �� ������ ����
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

    // ������(0)�� �̺�Ʈ��(1), 2�� ����
    private Node SetUpBeforBossAndBoss()
    {
        int middle = _mapWidth / 2;

        // 0�� �߾ӿ� ���� ��� ����
        Node boss = _map[0][middle];
        boss.nodeType = NodeType.Boss;
        _battleCount++; // ���� ���� 1ȸ

        // 1��: 1~3���� �̺�Ʈ ��� ����
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

        // ������ �߾ӿ� 1�� ���� ����
        if (beforeBossNodes.Count == 0)
        {
            Node fallback = _map[1][middle];
            fallback.nodeType = NodeType.Event;
            fallback.nextNodes.Add(boss);
            beforeBossNodes.Add(fallback);
        }

        // 2�� ���� �� 1�� �̺�Ʈ ��� ����
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

    // DFS�� ���� ���� �� ���� ��� Ž��
    private List<List<Node>> GetAllPaths(Node start, Node end)
    {
        List<List<Node>> paths = new();
        List<Node> currentPath = new();
        DFS(start, end, currentPath, paths);
        return paths;
    }

    // ���� �켱 Ž��
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