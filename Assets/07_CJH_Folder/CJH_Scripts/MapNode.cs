using UnityEngine;

public class MapNode : MonoBehaviour
{
    public Node nodeData { get; private set; }
    public bool isRevealed { get; private set; } = false;

    [Header("노드 프리팹 설정")]
    [Tooltip("시작 지점에 생성될 노드 프리팹")]
    public GameObject startNodePrefab;
    [Tooltip("보스 지점에 생성될 노드 프리팹")]
    public GameObject bossNodePrefab;
    [Tooltip("전투 지점에 생성될 노드 프리팹")]
    public GameObject battleNodePrefab;
    [Tooltip("숨겨진 상태일 때 보여줄 프리팹 (물음표)")]
    public GameObject mysteryNodePrefab;

    [Header("이벤트 노드 프리팹 (랜덤 등장)")]
    [Tooltip("긍정적 이벤트(보상)에 해당하는 프리팹 목록")]
    public GameObject[] positiveEventPrefabs;
    [Tooltip("부정적 이벤트(패널티)에 해당하는 프리팹 목록")]
    public GameObject[] negativeEventPrefabs;
    [Tooltip("중립적 이벤트(선택)에 해당하는 프리팹 목록")]
    public GameObject[] neutralEventPrefabs;

    // 현재 생성된 노드의 비주얼(프리팹 인스턴스)을 저장하는 변수
    private GameObject currentNodeVisual;

    // 맵 생성 시, 노드의 데이터를 설정하고 초기 상태의 프리팹을 생성
    public void Setup(Node dataNode)
    {
        nodeData = dataNode;
        isRevealed = false;

        // 이전에 생성된 비주얼이 있다면 삭제
        if (currentNodeVisual != null) Destroy(currentNodeVisual);

        // 시작, 전투, 보스 노드는 처음부터 바로 공개
        if (nodeData.nodeType == NodeType.Start || nodeData.nodeType == NodeType.Battle || nodeData.nodeType == NodeType.Boss)
        {
            Reveal();
        }
        // 그 외(이벤트 노드)는 미스터리 상태로 시작
        else
        {
            InstantiatePrefab(mysteryNodePrefab);
        }
    }

    // 노드의 실제 타입을 해당하는 프리팹으로 교체하여 공개
    public void Reveal()
    {
        if (isRevealed) return;
        isRevealed = true;

        if (currentNodeVisual != null) Destroy(currentNodeVisual);

        GameObject prefabToInstantiate = null;

        switch (nodeData.nodeType)
        {
            case NodeType.Start:
                prefabToInstantiate = startNodePrefab;
                break;
            case NodeType.Boss:
                prefabToInstantiate = bossNodePrefab;
                break;
            case NodeType.Battle:
                prefabToInstantiate = battleNodePrefab;
                break;
            case NodeType.Event:
                switch (nodeData.EventTypeKc)
                {
                    case EventTypeKC.Positive:
                        // 긍정적 프리팹 배열에서 하나를 랜덤으로 선택
                        if (positiveEventPrefabs != null && positiveEventPrefabs.Length > 0)
                        {
                            int randomIndex = Random.Range(0, positiveEventPrefabs.Length);
                            prefabToInstantiate = positiveEventPrefabs[randomIndex];
                        }
                        break;
                    case EventTypeKC.Negative:
                        // 부정적 프리팹 배열에서 하나를 랜덤으로 선택
                        if (negativeEventPrefabs != null && negativeEventPrefabs.Length > 0)
                        {
                            int randomIndex = Random.Range(0, negativeEventPrefabs.Length);
                            prefabToInstantiate = negativeEventPrefabs[randomIndex];
                        }
                        break;
                    case EventTypeKC.Neutral:
                        // 중립적 프리팹 배열에서 하나를 랜덤으로 선택
                        if (neutralEventPrefabs != null && neutralEventPrefabs.Length > 0)
                        {
                            int randomIndex = Random.Range(0, neutralEventPrefabs.Length);
                            prefabToInstantiate = neutralEventPrefabs[randomIndex];
                        }
                        break;
                }
                break;
        }

        InstantiatePrefab(prefabToInstantiate);
    }

    // 프리팹을 생성하고 위치를 맞추는 함수
    private void InstantiatePrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            // 이 MapNode 오브젝트의 자식으로 프리팹을 생성
            currentNodeVisual = Instantiate(prefab, transform);

            // 시작/보스 노드의 경우 특별히 크기를 키워줌
            if (nodeData.nodeType == NodeType.Start || nodeData.nodeType == NodeType.Boss)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1f); // 크기는 원하는 대로 조절
            }
        }
        else
        {
            Debug.LogWarning($"{nodeData.nodeType} 타입에 해당하는 프리팹이 비어있습니다!");
        }
    }
}