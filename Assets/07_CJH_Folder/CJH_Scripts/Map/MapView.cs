using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class MapView : MonoBehaviour
{
    [Header("맵 & 플레이어 프리팹")]
    public GameObject mapTemplatePrefab;
    public GameObject playerCharacterPrefab; // 플레이어 캐릭터 프리팹 연결
    public MapConfig mapConfig;
    public GameObject arrowPrefab; // 깜빡이는 화살표 프리팹

    [Header("캐릭터 이동 애니메이션")]
    public float playerMoveDuration = 0.5f; // 캐릭터가 이동하는 데 걸리는 시간
    public Ease playerMoveEase = Ease.OutQuad; // 캐릭터 이동 애니메이션 방식


    [Header("카메라 이동 애니메이션")]
    public float cameraMoveDuration = 0.8f;
    public Ease cameraMoveEase = Ease.OutCubic;
    public MapData CurrentMapData => currentMap;

    [Header("카메라 컴포넌트 연결")]
    public CameraScrollLinker cameraScrollLinker;

    [Header("하단 패널 UI")]
    public Transform bottomPanelContainer;
    public GameObject nodeButtonPrefab;

    private GameObject currentMapInstance;
    private MapData currentMap;
    private Dictionary<Vector2Int, MapNode> nodeObjects;
    private List<GameObject> lineArrows = new List<GameObject>();

    // 생성된 플레이어 캐릭터를 담을 변수
    private GameObject playerCharacterInstance;

    private Transform cameraTransform;

    public static MapView Instance;

    private void Awake()
    {
        DOTween.Init();

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
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

        Debug.Log($"[MapView] SelectNode 실행: {selectedNode.gameObject.name} 선택됨");

        if (currentMap.Path.Contains(selectedNode.nodeData)) return;
        currentMap.Path.Add(selectedNode.nodeData);
        UpdateMapState();
    }

    private void UpdateMapState()
    {
        Node currentNode = currentMap.CurrentNode;
        if (currentNode == null) return;

        // 기존 화살표들 삭제
        foreach (var arrow in lineArrows)
        {
            Destroy(arrow);
        }
        lineArrows.Clear();

        // 현재 노드로 플레이어 캐릭터 이동
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

            // 다음 노드일 경우 화살표 생성
            if (isNextNode && arrowPrefab != null && nodeObjects.TryGetValue(currentNode.point, out MapNode currentNodeObject))
            {
                CreateArrow(currentNodeObject, mapNode);
            }
        }
        UpdateBottomPanel();
    }

    // 두 노드 사이에 화살표를 생성하는 함수
    private void CreateArrow(MapNode from, MapNode to)
    {
        GameObject arrow = Instantiate(arrowPrefab, transform);
        lineArrows.Add(arrow); // 리스트에 추가하여 관리

        Vector3 startPos = from.transform.position;
        Vector3 endPos = to.transform.position;

        // 화살표의 위치를 두 노드의 중간으로 설정
        arrow.transform.position = Vector3.Lerp(startPos, endPos, 0.5f);

        // 화살표가 노드를 바라보도록 회전
        Vector3 direction = (endPos - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 위를 향하는 화살표 스프라이트에 맞게 회전 값을 보정합니다. (-90도)
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void UpdateBottomPanel()
    {
        // 기존에 있던 버튼들을 모두 삭제
        foreach (Transform child in bottomPanelContainer)
        {
            Destroy(child.gameObject);
        }

        Node currentNode = currentMap.CurrentNode;
        if (currentNode == null || currentNode.nextNodes == null || !currentNode.nextNodes.Any()) return;

        // 다음 노드에 해당하는 MapNode들을 리스트 전달
        List<MapNode> nextMapNodes = new List<MapNode>();
        foreach (Node nextNodeData in currentNode.nextNodes)
        {
            if (nodeObjects.TryGetValue(nextNodeData.point, out MapNode targetMapNode))
            {
                nextMapNodes.Add(targetMapNode);
            }
        }

        // MapNode들을 월드 좌표의 x값을 기준으로 정렬 (왼쪽 -> 오른쪽).
        List<MapNode> sortedNextMapNodes = nextMapNodes.OrderBy(mapNode => mapNode.transform.position.x).ToList();

        // 정렬된 순서대로 버튼을 생성하고 기능을 연결
        foreach (MapNode targetMapNode in sortedNextMapNodes)
        {
            GameObject buttonObj = Instantiate(nodeButtonPrefab, bottomPanelContainer);
            buttonObj.transform.localScale = Vector3.one;
            Button button = buttonObj.GetComponent<Button>();

            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
            {
                // MapNode에게 Node 전달 받음
                buttonImage.sprite = targetMapNode.GetSpriteForNodeType();
            }

            button.onClick.AddListener(() =>
            {
                Debug.Log($"[MapView] UI 버튼 클릭 '{targetMapNode.gameObject.name}'으로 이동 시도.");
                SelectNode(targetMapNode);
            });
        }
    }

    // 플레이어 캐릭터를 현재 노드 위치로 이동시키는 함수
    private void UpdatePlayerPosition()
    {
        if (playerCharacterInstance == null || currentMap.CurrentNode == null) return;
        // 현재 노드의 게임 오브젝트를 찾습니다.
        if (nodeObjects.TryGetValue(currentMap.CurrentNode.point, out MapNode currentNodeObject))
        {
            // DoTween을 사용해 부드럽게 이동
            playerCharacterInstance.transform.DOKill();
            playerCharacterInstance.transform
                .DOMove(currentNodeObject.transform.position, playerMoveDuration)
                .SetEase(playerMoveEase);

            if (cameraTransform != null && cameraScrollLinker != null)
            {

                // 처음 시작 시 스크롤 기능 해제
                cameraScrollLinker.SetManualScroll(false);

                Vector3 targetPosition = currentNodeObject.transform.position;
                targetPosition.z = cameraTransform.position.z;

                cameraTransform.DOKill();

                cameraTransform.DOMoveY(currentNodeObject.transform.position.y, cameraMoveDuration)
                    .SetEase(cameraMoveEase)

                // 작업이 끝나면
                .OnComplete(() =>
                {
                    // 위치 동기화
                    cameraScrollLinker.CameraPositon();
                });
            }
        }
    }

    private void ClearMap()
    {
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
        }

        // 맵 클리어 시 화살표도 모두 제거
        foreach (var arrow in lineArrows)
        {
            Destroy(arrow);
        }
        lineArrows.Clear();
    }
}