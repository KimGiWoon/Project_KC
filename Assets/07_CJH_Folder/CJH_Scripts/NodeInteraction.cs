using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class NodeInteraction : MonoBehaviour, IPointerClickHandler
{
    public float punchScale = 0.2f;
    public float duration = 0.5f;
    public int vibrato = 10;
    public float elasticity = 1f;

    private MapNode mapNode;

    private void Awake()
    {
        mapNode = GetComponent<MapNode>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // [수정!] 선택 가능한 상태가 아니면 아무것도 하지 않음
        if (mapNode == null || !mapNode.isSelectable)
        {
            return;
        }

        Debug.Log(gameObject.name + " 노드 선택됨!");

        // 애니메이션 실행
        transform.DOPunchScale(new Vector3(punchScale, punchScale, 0), duration, vibrato, elasticity);

        // MapView에게 이 노드가 선택되었음을 알림
        if (MapView.Instance != null)
        {
            MapView.Instance.SelectNode(mapNode);
        }
    }
}