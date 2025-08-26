using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public NodeType nodeType;
    public EventType eventType;
    public Vector2Int point;
    public List<Node> nextNodes;
    public List<Node> previousNodes;

    public Node(int row, int column)
    {
        point = new Vector2Int(row, column);
        nextNodes = new List<Node>();
        previousNodes = new List<Node>();
    }

    // C#에게 Node 객체를 좌표 기준으로 비교하도록 알려주는 필수 코드입니다.
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

    // [수정된 부분] column 대신 point.y를 사용합니다.
    public override string ToString()
    {
        return $"{point.y} ({nodeType.ToString()[0]})";
    }
}