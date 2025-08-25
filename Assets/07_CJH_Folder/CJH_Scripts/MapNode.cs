using UnityEngine;

public class MapNode : MonoBehaviour
{
    public Node nodeData { get; private set; }

    // Inspector 창에 노출되지 않도록 private으로 선언합니다.
    private SpriteRenderer spriteRenderer;

    // 각 노드 타입에 맞는 색상 정의
    private readonly Color startColor = new Color(0.447f, 1f, 0.438f); // 녹색
    private readonly Color bossColor = new Color(1f, 0.127f, 0.117f);  // 빨간색
    private readonly Color battleColor = new Color(0.8f, 0.4f, 1f);   // 보라색
    private readonly Color eventColor = new Color(0.2f, 0.5f, 1f);    // 파란색

    // 이 스크립트가 활성화될 때 최초 한 번만 실행되는 함수입니다.
    void Awake()
    {
        // 자기 자신이 붙어있는 게임 오브젝트에서 SpriteRenderer 컴포넌트를 스스로 찾습니다.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 만약 SpriteRenderer가 없다면 확실하게 에러를 출력합니다.
        if (spriteRenderer == null)
        {
            Debug.LogError("MapNode 오브젝트에 SpriteRenderer 컴포넌트가 없습니다!", this.gameObject);
        }
    }

    // MapView.cs에서 데이터를 받아와 노드의 외형을 설정하는 함수입니다.
    public void Setup(Node dataNode)
    {
        this.nodeData = dataNode;

        // Awake()에서 찾아온 spriteRenderer를 사용해 색상과 크기를 변경합니다.
        if (spriteRenderer != null)
        {
            // 크기를 먼저 기본값으로 되돌립니다.
            transform.localScale = Vector3.one;

            switch (dataNode.nodeType)
            {
                case NodeType.Start:
                    spriteRenderer.color = startColor;
                    transform.localScale = new Vector3(2f, 2f, 1f); // 시작 노드 크기 변경
                    break;
                case NodeType.Boss:
                    spriteRenderer.color = bossColor;
                    transform.localScale = new Vector3(2f, 2f, 1f); // 보스 노드 크기 변경
                    break;
                case NodeType.Battle:
                    spriteRenderer.color = battleColor;
                    break;
                case NodeType.Event:
                    spriteRenderer.color = eventColor;
                    break;
            }
        }
    }
}