using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

namespace CJH
{
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
            switch (selectedNode.nodeData.nodeType)
            {
                case NodeType.Event:
                    if (EncounterStageManager.Instance == null)
                    {
                        Debug.LogError("EncounterStageManager.Instance가 없습니다! Hierarchy에 EncounterStageManager 오브젝트가 있는지, '활성화'되어 있는지, Script Execution Order가 설정되었는지 확인해주세요.");
                        return; // Manager가 없으므로 여기서 실행을 멈춥니다.
                    }
                    if (EventManager.Instance == null)
                    {
                        Debug.LogError("EventManager.Instance가 없습니다! Hierarchy에 EventManager 오브젝트가 있는지, '활성화'되어 있는지, Script Execution Order가 설정되었는지 확인해주세요.");
                        return;
                    }
                    // 이벤트 노드일 경우,EventManager에게 사건 시작을 요청
                    EventManager.Instance.StartEncounterForStage(EncounterStageManager.Instance.currentStage);
                    break;

                case NodeType.Battle:
                    // TODO: 전투 시작 로직 호출 (예: GameManager.Instance.StartBattle(...))
                    UpdateMapState(); // 임시로 맵 상태만 업데이트
                    break;


                default:
                    // 그 외의 노드는 즉시 맵 상태를 업데이트
                    UpdateMapState();
                    break;
            }
        }

        public void UpdateMapState()
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
            foreach (Transform child in bottomPanelContainer)
            {
                Destroy(child.gameObject);
            }

            Node currentNode = currentMap.CurrentNode;
            if (currentNode == null || currentNode.nextNodes == null || !currentNode.nextNodes.Any()) return;

            List<MapNode> nextMapNodes = new();
            foreach (var next in currentNode.nextNodes)
            {
                if (nodeObjects.TryGetValue(next.point, out var mapNode))
                    nextMapNodes.Add(mapNode);
            }

            foreach (var targetNode in nextMapNodes)
            {
                GameObject arrowBtn = Instantiate(nodeButtonPrefab, bottomPanelContainer);
                arrowBtn.transform.localScale = Vector3.one;

                // 화살표 방향 계산
                Vector3 from = nodeObjects[currentNode.point].transform.position;
                Vector3 to = targetNode.transform.position;
                float angle = Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;

                Image arrowImage = arrowBtn.GetComponentInChildren<Image>();
                if (arrowImage != null)
                {
                    arrowImage.transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // Sprite 기준 보정
                }

                Button button = arrowBtn.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => SelectNode(targetNode));
                }
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
}