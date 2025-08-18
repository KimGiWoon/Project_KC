using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class MapGenerator : MonoBehaviour
{
    private int _xDist;
    private int _yGap;
    private int _placementRandomness;

    private int _floors;
    private int _mapWidth;
    private int _paths;

    private float _BattleNodeWeight;
    private float _ShopNodeWeight;
    private float _EventNodeWeight;

    private List<float> _randomWeightList = new()
        {
            0f, // NotAssigned
            0f, // Battle
            0f, // Shop
            0f, // Event
            0f  // Boss
        };

    private float _totalWeight = 0f;

    private List<List<Node>> _map;
    private List<List<Node>> _allPath;

    //  MapConfig 기반으로 맵 생성
    public MapData GenerateMap(MapConfig config)
    {
        InitSetting(config);
        _map = GenerateInitialGrid();

        List<int> startingPoints = GetRandomStartingPoints();

        for (int j = 0; j < startingPoints.Count; j++)
        {
            int currentJ = startingPoints[j];
            for (int i = 2; i < _floors - 2; i++)
            {
                currentJ = SetUpConnection(i, currentJ);
            }
        }

        Node startNode = SetUpStartAndFloorOne();
        Node bossNode = SetUpBeforBossAndBoss();

        _allPath = GetAllPaths(startNode, bossNode);

        SetUpRandomNodeWeights();
        SetUpNodeType();

        return new MapData(_map, _allPath, startNode, bossNode);
    }

    //  설정 초기화
    private void InitSetting(MapConfig config)
    {
        _xDist = config.xDist;
        _yGap = config.yGap;
        _placementRandomness = config.placementRandomness;

        _floors = config.floors;
        _mapWidth = config.mapWidth;
        _paths = config.paths;

        _BattleNodeWeight = config.BattleNodeWeight;
        _EventNodeWeight = config.EventNodeWeight;
    }

    //  초기 Grid 생성
    private List<List<Node>> GenerateInitialGrid()
    {
        List<List<Node>> grid = new(_floors);

        for (int i = 0; i < _floors; i++)
        {
            List<Node> floorRooms = new List<Node>(_mapWidth);
            for (int j = 0; j < _mapWidth; j++)
            {
                Node node = new Node(i, j);
                node.nextNodes.Clear();
                floorRooms.Add(node);
            }
            grid.Add(floorRooms);
        }

        return grid;
    }

    //  랜덤한 시작 포인트 선정
    private List<int> GetRandomStartingPoints()
    {
        List<int> yCoordinates = new();
        int uniquePoints = 0;

        while (uniquePoints < 2)
        {
            uniquePoints = 0;
            yCoordinates.Clear();

            for (int i = 0; i < _paths; i++)
            {
                int point = Random.Range(0, _mapWidth);
                if (!yCoordinates.Contains(point)) uniquePoints++;
                yCoordinates.Add(point);
            }
        }

        return yCoordinates;
    }

    //  노드 연결 설정
    private int SetUpConnection(int i, int j)
    {
        Node currentNode = _map[i][j];
        Node nextNode = null;

        while (nextNode == null || IsCrossingPath(i, j, nextNode))
        {
            int randJ = Mathf.Clamp(Random.Range(j - 1, j + 2), 0, _mapWidth - 1);
            nextNode = _map[i + 1][randJ];
        }

        currentNode.nextNodes.Add(nextNode);
        return nextNode.column;
    }

    private bool IsCrossingPath(int i, int j, Node node)
    {
        Node left = j > 0 ? _map[i][j - 1] : null;
        Node right = j < _mapWidth - 1 ? _map[i][j + 1] : null;

        if (right != null && node.column > j)
        {
            if (right.nextNodes.Any(n => n.column < node.column))
                return true;
        }

        if (left != null && node.column < j)
        {
            if (left.nextNodes.Any(n => n.column > node.column))
                return true;
        }

        return false;
    }

    //  시작 + 1층 설정
    private Node SetUpStartAndFloorOne()
    {
        int middle = _mapWidth / 2;
        Node start = _map[0][middle];
        Node floor1 = _map[1][middle];

        start.nodeType = NodeType.Battle;
        floor1.nodeType = NodeType.Event;
        start.nextNodes.Add(floor1);

        for (int j = 0; j < _mapWidth; j++)
        {
            if (_map[2][j].HasConnections())
                floor1.nextNodes.Add(_map[2][j]);
        }

        return start;
    }

    //  보스층 설정
    private Node SetUpBeforBossAndBoss()
    {
        int middle = _mapWidth / 2;
        Node boss = _map[_floors - 1][middle];
        Node beforeBoss = _map[_floors - 2][middle];

        boss.nodeType = NodeType.Boss;

        beforeBoss.nextNodes.Add(boss);

        for (int j = 0; j < _mapWidth; j++)
        {
            if (_map[_floors - 3][j].HasConnections())
            {
                _map[_floors - 3][j].nextNodes.Clear();
                _map[_floors - 3][j].nextNodes.Add(beforeBoss);
            }
        }

        return boss;
    }

    //  타입 가중치 초기화
    private void SetUpRandomNodeWeights()
    {
        _randomWeightList[(int)NodeType.Battle] = _BattleNodeWeight;
        _randomWeightList[(int)NodeType.Event] = _BattleNodeWeight + _EventNodeWeight;

        _totalWeight = _randomWeightList[(int)NodeType.Event];
    }

    //  노드 타입 랜덤 지정
    private void SetUpNodeType()
    {

        foreach (List<Node> path in _allPath)
        {
            int battleCount = path.Count(n => n.nodeType == NodeType.Battle);
            List<Node> candidates = path.Where(n => n.nodeType == NodeType.NotAssgined).ToList();
            List<Node> shuffled = candidates.OrderBy(_ => Random.value).ToList();

            foreach (Node node in shuffled)
            {
                if (battleCount < 3)
                {
                    node.nodeType = NodeType.Battle;
                    battleCount++;
                }
                else
                {
                    if (node.row == _floors - 3)
                    {
                        node.nodeType = NodeType.Event;
                    }
                    else
                    {
                        NodeType type;
                        do
                        {
                            type = GetRandomRoomTypeByWeight();
                        } while (type == NodeType.Event && HasParentOfType(node, NodeType.Event));

                        node.nodeType = type;
                    }
                }
            }
        }
    }

    private NodeType GetRandomRoomTypeByWeight()
    {
        float roll = Random.Range(0f, _totalWeight);
        for (int i = 0; i < _randomWeightList.Count; i++)
        {
            if (_randomWeightList[i] > roll)
                return (NodeType)i;
        }

        return NodeType.Event;
    }

    private bool HasParentOfType(Node node, NodeType type)
    {
        List<Node> parentCandidates = new();

        if (node.row > 0)
        {
            if (node.column > 0)
                parentCandidates.Add(_map[node.row - 1][node.column - 1]);
            parentCandidates.Add(_map[node.row - 1][node.column]);
            if (node.column < _mapWidth - 1)
                parentCandidates.Add(_map[node.row - 1][node.column + 1]);
        }

        return parentCandidates.Any(p => p.nextNodes.Contains(node) && p.nodeType == type);
    }

    //  모든 경로 찾기 (DFS)
    private List<List<Node>> GetAllPaths(Node start, Node end)
    {
        List<List<Node>> paths = new();
        List<Node> currentPath = new();
        DFS(start, end, currentPath, paths);
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
