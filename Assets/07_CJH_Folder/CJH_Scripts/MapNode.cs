using UnityEngine;

public class MapNode : MonoBehaviour
{
    public Node nodeData { get; private set; }

    // Inspector â�� ������� �ʵ��� private���� �����մϴ�.
    private SpriteRenderer spriteRenderer;

    // �� ��� Ÿ�Կ� �´� ���� ����
    private readonly Color startColor = new Color(0.447f, 1f, 0.438f); // ���
    private readonly Color bossColor = new Color(1f, 0.127f, 0.117f);  // ������
    private readonly Color battleColor = new Color(0.8f, 0.4f, 1f);   // �����
    private readonly Color eventColor = new Color(0.2f, 0.5f, 1f);    // �Ķ���

    // �� ��ũ��Ʈ�� Ȱ��ȭ�� �� ���� �� ���� ����Ǵ� �Լ��Դϴ�.
    void Awake()
    {
        // �ڱ� �ڽ��� �پ��ִ� ���� ������Ʈ���� SpriteRenderer ������Ʈ�� ������ ã���ϴ�.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ���� SpriteRenderer�� ���ٸ� Ȯ���ϰ� ������ ����մϴ�.
        if (spriteRenderer == null)
        {
            Debug.LogError("MapNode ������Ʈ�� SpriteRenderer ������Ʈ�� �����ϴ�!", this.gameObject);
        }
    }

    // MapView.cs���� �����͸� �޾ƿ� ����� ������ �����ϴ� �Լ��Դϴ�.
    public void Setup(Node dataNode)
    {
        this.nodeData = dataNode;

        // Awake()���� ã�ƿ� spriteRenderer�� ����� ����� ũ�⸦ �����մϴ�.
        if (spriteRenderer != null)
        {
            // ũ�⸦ ���� �⺻������ �ǵ����ϴ�.
            transform.localScale = Vector3.one;

            switch (dataNode.nodeType)
            {
                case NodeType.Start:
                    spriteRenderer.color = startColor;
                    transform.localScale = new Vector3(2f, 2f, 1f); // ���� ��� ũ�� ����
                    break;
                case NodeType.Boss:
                    spriteRenderer.color = bossColor;
                    transform.localScale = new Vector3(2f, 2f, 1f); // ���� ��� ũ�� ����
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