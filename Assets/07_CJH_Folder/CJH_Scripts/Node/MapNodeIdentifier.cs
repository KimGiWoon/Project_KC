using System.Collections.Generic;
using UnityEngine;

// 이 컴포넌트는 템플릿 프리팹 안의 노드 위치를 식별하는 역할만 합니다.
public class MapNodeIdentifier : MonoBehaviour
{
    // 인스펙터에서 직접 설정할 값
    public int floorIndex; // 몇 번째 층에 속하는가 (0부터 시작)
    public int nodeIndexInFloor; // 해당 층에서 몇 번째 노드인가 (0부터 시작)

    public List<MapNodeIdentifier> connections;
}