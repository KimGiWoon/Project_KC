using UnityEngine;
using DG.Tweening;


public class NodeInteraction : MonoBehaviour
{
    [Header("Animation Settings")]
    public float punchScale = 0.2f;
    public float duration = 0.5f;
    public int vibrato = 10;
    public float elasticity = 1f;

    private MapNode mapNode;

    private void Awake()
    {
        mapNode = GetComponent<MapNode>();
    }

    private void OnMouseDown()
    {
        if (mapNode == null)
        {
            Debug.LogError("MapNode 컴포넌트가 없습니다!", gameObject);
            return;
        }

        // 아직 공개되지 않은 노드는 클릭할 수 없도록 막습니다.
        if (!mapNode.isRevealed)
        {
            Debug.Log($"클릭 실패: {gameObject.name} 노드는 아직 공개되지 않았습니다 (isRevealed: false).");
            return;
        }

        Debug.Log(gameObject.name + " 노드가 클릭되었습니다!");

        transform.DOPunchScale(
            new Vector3(punchScale, punchScale, 0),
            duration,
            vibrato,
            elasticity
        );

        // MapView가 null이 아닐 때만 SelectNode 함수를 호출하도록 안전장치 추가
        if (MapView.Instance != null)
        {
            MapView.Instance.SelectNode(mapNode);
        }
        else
        {
            Debug.LogError("MapView.Instance가 없습니다! 씬에 MapView 오브젝트가 있는지 확인해주세요.");
        }
    }
}