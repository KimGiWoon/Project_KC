using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public Image icon;
    public Node Node { get; private set; }

    public void Setup(Node node)
    {
        this.Node = node;

        // ����� Ÿ�Կ� ���� �̹��� ���� ���� (����)
        switch (node.nodeType)
        {
            case NodeType.Battle:
                icon.color = Color.red;
                break;
            case NodeType.Event:
                icon.color = Color.blue;
                break;
            case NodeType.Boss:
                icon.color = Color.magenta;
                break;
            default:
                icon.color = Color.gray;
                break;
        }
    }
}
