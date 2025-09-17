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


    [Header("이벤트 아이콘 설정")]
    public List<EventIconMapping> eventIconMappings;

    public Sprite GetIconForEventGroup(int groupID)
    {
        var mapping = eventIconMappings.Find(m => m.eventGroupID == groupID);
        if (mapping != null)
        {
            return mapping.iconSprite;
        }
        return null; // 해당하는 아이콘이 없을 경우 null 반환
    }
}

[System.Serializable]
public class EventIconMapping
{
    public int eventGroupID; // 이벤트 데이터 시트의 그룹 ID와 일치시킬 값
    public Sprite iconSprite; // 해당 그룹이 맵 노드에서 사용할 아이콘
}