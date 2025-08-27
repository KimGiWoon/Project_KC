using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems; 

public class NodeInteraction : MonoBehaviour, IPointerDownHandler // IPointerDownHandler 인터페이스를 구현
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

    // OnPointerDown을 사용
    // EventSystem이 Physics 2D Raycaster를 통해 호출해줍니다.
    public void OnPointerDown(PointerEventData eventData)
    {
        if (mapNode == null)
        {
            Debug.LogError("MapNode 컴포넌트가 없습니다!", gameObject);
            return;
        }

        if (!mapNode.isRevealed)
        {
            Debug.Log($"클릭 실패: {gameObject.name} 노드는 아직 공개되지 않았습니다 (isRevealed: false).");
            return;
        }

        Debug.Log(gameObject.name + " 노드가 클릭되었습니다! DoTween 애니메이션을 실행합니다.");

        transform.DOPunchScale(
            new Vector3(punchScale, punchScale, 0),
            duration,
            vibrato,
            elasticity
        );

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