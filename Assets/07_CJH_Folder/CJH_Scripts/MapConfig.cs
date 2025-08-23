using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

[CreateAssetMenu(menuName = "Map/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("��� ���ø�")]
    public List<NodeTemplate> NodeTemplates;

    [Header("��� Ÿ�� (���� ���ÿ�)")]
    public List<NodeType> randomNodes = new List<NodeType>
    {
        NodeType.Battle,
        NodeType.Boss,
        NodeType.Event
    };

    [Header("��� ��ġ ����")]
    public int xDist = 30;
    public int yGap = 25;
    public int placementRandomness = 5;

    [Header("�� ũ�� ����")]
    public int floors = 16;
    public int mapWidth = 7;
    public int paths = 6;

    [Header("��� ���� Ȯ�� (����ġ)")]
    public float BattleNodeWeight = 2.0f;
    public float EventNodeWeight = 4.0f;
}