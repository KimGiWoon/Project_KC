using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class Node
{
    public NodeType nodeType;
    public Vector2Int point;
    public int row, column;
    public List<Node> nextNodes;
    public bool selected;

    public Node(int row, int column)
    {
        this.row = row;
        this.column = column;
        point = new Vector2Int(row, column);
        nextNodes = new List<Node>();
        selected = false;
    }

    public bool HasConnections()
    {
        return nextNodes.Count > 0;
    }

    public override string ToString()
    {
        return $"{column} ({nodeType.ToString()[0]})";
    }
}