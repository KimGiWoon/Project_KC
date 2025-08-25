using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public NodeType nodeType;
    public Vector2Int point;
    public List<Node> nextNodes;
    public List<Node> previousNodes;

    public Node(int row, int column)
    {
        point = new Vector2Int(row, column);
        nextNodes = new List<Node>();
        previousNodes = new List<Node>();
    }

    // C#���� Node ��ü�� ��ǥ �������� ���ϵ��� �˷��ִ� �ʼ� �ڵ��Դϴ�.
    public override bool Equals(object obj)
    {
        return Equals(obj as Node);
    }

    public bool Equals(Node other)
    {
        return other != null &&
               this.point.x == other.point.x &&
               this.point.y == other.point.y;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(this.point.x, this.point.y);
    }

    // [������ �κ�] column ��� point.y�� ����մϴ�.
    public override string ToString()
    {
        return $"{point.y} ({nodeType.ToString()[0]})";
    }
}