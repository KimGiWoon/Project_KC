using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public NodeType nodeType;
    public EventTypeKC EventTypeKC;
    public Vector2Int point;
    public List<Node> nextNodes;
    public List<Node> previousNodes;

    public Node(int row, int column)
    {
        point = new Vector2Int(row, column);
        nextNodes = new List<Node>();
        previousNodes = new List<Node>();
    }

    //Node 객체를 좌표 기준으로 비교하도록 알려주는 필수 코드
    public override bool Equals(object obj) => Equals(obj as Node);

    public bool Equals(Node other) => other != null &&
                                      point.x == other.point.x &&
                                      point.y == other.point.y;

    public override int GetHashCode() => System.HashCode.Combine(point.x, point.y);

    public override string ToString() => $"{point.y} ({nodeType.ToString()[0]})";
}