using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapView : MonoBehaviour
{
    [Header("맵 & 플레이어 프리팹")]
    public GameObject mapTemplatePrefab;
    public GameObject playerCharacterPrefab; // [추가!] 플레이어 캐릭터 프리팹 연결
    public MapConfig mapConfig;

    [Header("캐릭터 이동 애니메이션")]
    public float playerMoveDuration = 0.5f; // 캐릭터가 이동하는 데 걸리는 시간
    public Ease playerMoveEase = Ease.OutQuad; // 캐릭터 이동 애니메이션 방식

    public MapData CurrentMapData => currentMap;

    private GameObject currentMapInstance;
    private MapData currentMap;
    private Dictionary<Vector2Int, MapNode> nodeObjects;

    // [추가!] 생성된 플레이어 캐릭터를 담을 변수
    private GameObject playerCharacterInstance;

    public static MapView Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CreateMapView(MapData map)
    {
        ClearMap();
        currentMap = map;
        currentMapInstance = Instantiate(mapTemplatePrefab);
        nodeObjects = new Dictionary<Vector2Int, MapNode>();

        // 플레이어 캐릭터 생성 (씬에 없으면 새로 생성)
        if (playerCharacterInstance == null && playerCharacterPrefab != null)
        {
            playerCharacterInstance = Instantiate(playerCharacterPrefab, transform);
        }

        foreach (var placeholder in currentMapInstance.GetComponentsInChildren<MapNodeIdentifier>())
        {
            var point = new Vector2Int(placeholder.floorIndex, placeholder.nodeIndexInFloor);
            if (!nodeObjects.ContainsKey(point))
            {
                nodeObjects.Add(point, placeholder.GetComponent<MapNode>());
            }
            placeholder.gameObject.SetActive(false);
        }

        foreach (var dataNode in map.Nodes)
        {
            if (nodeObjects.TryGetValue(dataNode.point, out MapNode mapNode))
            {
                mapNode.Setup(dataNode, mapConfig);
                mapNode.gameObject.SetActive(true);
            }
        }

        UpdateMapState();
    }

    public void SelectNode(MapNode selectedNode)
    {
        if (currentMap.Path.Contains(selectedNode.nodeData)) return;
        currentMap.Path.Add(selectedNode.nodeData);
        UpdateMapState();
    }

    private void UpdateMapState()
    {
        Node currentNode = currentMap.CurrentNode;
        if (currentNode == null) return;

        // [추가!] 현재 노드로 플레이어 캐릭터 이동
        UpdatePlayerPosition();

        foreach (var mapNode in nodeObjects.Values)
        {
            bool isNextNode = currentNode.nextNodes.Contains(mapNode.nodeData);

            if (isNextNode && !mapNode.isRevealed)
            {
                mapNode.Reveal();
            }
            mapNode.SetSelectable(isNextNode);
            mapNode.UpdateVisuals();
        }
    }

    // [추가!] 플레이어 캐릭터를 현재 노드 위치로 이동시키는 함수
    private void UpdatePlayerPosition()
    {
        if (playerCharacterInstance == null || currentMap.CurrentNode == null) return;

        // 현재 노드의 게임 오브젝트를 찾습니다.
        if (nodeObjects.TryGetValue(currentMap.CurrentNode.point, out MapNode currentNodeObject))
        {
            // DoTween을 사용해 부드럽게 이동!
            playerCharacterInstance.transform
                .DOMove(currentNodeObject.transform.position, playerMoveDuration)
                .SetEase(playerMoveEase);
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