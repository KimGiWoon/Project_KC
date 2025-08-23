using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    // Image(UI)�� SpriteRenderer(2D)�� ��� �õ��ϵ��� ����
    public Image iconImage;
    public SpriteRenderer spriteRenderer;

    public Node Node { get; private set; }

    private void Awake()
    {
        // ������Ʈ �ڵ� ã�ƿ���
        if (iconImage == null) iconImage = GetComponent<Image>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Node node)
    {
        this.Node = node;
        Color nodeColor;

        // ����� Ÿ�Կ� ���� ���� ����
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

        // Image�� ������ Image ���� ����, SpriteRenderer�� ������ �װ��� ���� ����
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