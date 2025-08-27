using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Map/NodeTemplate")]
public class NodeTemplate : ScriptableObject
{
    [Header("기본 설정")]
    public NodeType nodeType;

    [Header("단일 스프라이트 (이벤트 외 노드용)")]
    [Tooltip("전투, 시작, 보스, 미스터리 노드 등은 여기 스프라이트를 사용합니다.")]
    public Sprite sprite;

    [Header("이벤트 스프라이트 목록 (이벤트 노드용)")]
    [Tooltip("긍정적 이벤트(보상)일 때 랜덤으로 보여줄 스프라이트 목록")]
    public List<Sprite> positiveEventSprites;

    [Tooltip("부정적 이벤트(패널티)일 때 랜덤으로 보여줄 스프라이트 목록")]
    public List<Sprite> negativeEventSprites;

    [Tooltip("중립적 이벤트(선택)일 때 랜덤으로 보여줄 스프라이트 목록")]
    public List<Sprite> neutralEventSprites;
}