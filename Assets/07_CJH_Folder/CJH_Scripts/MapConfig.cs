using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("노드 템플릿")]
    public List<NodeTemplate> NodeTemplates;

    [Header("노드 타입 (랜덤 선택용)")]
    public List<NodeType> randomNodes = new List<NodeType>
    {
        NodeType.Battle,
        NodeType.Boss,
        NodeType.Event
    };

    [Header("노드 위치 설정")]
    public int xDist = 30;
    public int yGap = 25;
    public int placementRandomness = 5;

    [Header("맵 크기 설정")]
    public int floors = 16;
    public int mapWidth = 7;
    public int paths = 6;

    [Header("노드 생성 확률 (가중치)")]
    public float BattleNodeWeight = 2.0f;
    public float EventNodeWeight = 4.0f;
}