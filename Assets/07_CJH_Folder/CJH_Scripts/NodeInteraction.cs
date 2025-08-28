using UnityEngine;
using DG.Tweening;


public class NodeInteraction : MonoBehaviour
{
    // 애니메이션 설정값들을 인스펙터에서 편하게 수정할 수 있도록 변수로 만듭니다.
    [Header("Animation Settings")]
    public float punchScale = 0.2f;   // 얼마나 크게 만들 것인가
    public float duration = 0.5f;     // 애니메이션 시간 (초)
    public int vibrato = 10;          // 떨림 횟수
    public float elasticity = 1f;     // 탄성 (0~1 사이)

    private MapNode mapNode;

    private void Awake()
    {
        // 스크립트가 시작될 때 자신의 MapNode 컴포넌트를 찾아 저장해둡니다.
        mapNode = GetComponent<MapNode>();
    }

    // 이 스크립트가 적용된 게임 오브젝트에 마우스 클릭이 감지되면 자동으로 호출되는 함수입니다.
    private void OnMouseDown()
    {

        // 아직 공개되지 않은 미스터리 노드는 클릭할 수 없도록 막습니다.
        if (!mapNode.isRevealed)
        {
            Debug.Log("아직 공개되지 않은 노드입니다!");
            return;
        }

        Debug.Log(gameObject.name + " 노드가 클릭되었습니다!");

        // DoTween의 DOPunchScale 함수를 호출하여 애니메이션을 실행합니다.
        // transform: 이 스크립트가 붙어있는 게임 오브젝트의 Transform 컴포넌트
        transform.DOPunchScale(
            new Vector3(punchScale, punchScale, 0), // x, y 방향으로 punchScale 만큼 크기를 키웁니다.
            duration,                               // 애니메이션 지속 시간
            vibrato,                                // 진동 횟수
            elasticity                              // 탄성 정도
        );

        MapView.Instance.SelectNode(mapNode);
    }
}