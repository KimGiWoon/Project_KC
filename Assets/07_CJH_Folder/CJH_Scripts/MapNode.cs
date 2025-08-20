using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    // Image(UI)와 SpriteRenderer(2D)를 모두 시도하도록 수정
    public Image iconImage;
    public SpriteRenderer spriteRenderer;

    public Node Node { get; private set; }

    private void Awake()
    {
        // 컴포넌트 자동 찾아오기
        if (iconImage == null) iconImage = GetComponent<Image>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Node node)
    {
        this.Node = node;
        Color nodeColor;

        // 노드의 타입에 따라 색상 결정
        switch (node.nodeType)
        {
            case NodeType.Battle:
                nodeColor = Color.red;
                break;
            case NodeType.Event:
                nodeColor = Color.blue;
                break;
            case NodeType.Boss:
                nodeColor = Color.magenta;
                break;
            default:
                nodeColor = Color.gray;
                break;
        }

        // Image가 있으면 Image 색상 변경, SpriteRenderer가 있으면 그것의 색상 변경
        if (iconImage != null)
        {
            iconImage.color = nodeColor;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.color = nodeColor;
        }
    }
}