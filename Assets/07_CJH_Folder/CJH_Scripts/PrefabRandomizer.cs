using UnityEngine;
using System.Collections.Generic;

public class PrefabRandomizer : MonoBehaviour
{
    [Header("연결할 컴포넌트")]
    public MapGenerator mapGenerator; // 인스펙터에서 MapGenerator 오브젝트를 연결할 변수
    public MapView mapView;             // 인스펙터에서 MapView 오브젝트를 연결할 변수

    [Header("랜덤으로 선택될 프리팹 목록")]
    public List<GameObject> mapNodePrefabs; // 5개의 맵 노드 프리팹을 담을 리스트

    // 게임이 시작될 때 Awake() 함수가 가장 먼저 호출됩니다.
    void Awake()
    {
        // 프리팹 목록이 비어있지 않은지 확인합니다.
        if (mapNodePrefabs != null && mapNodePrefabs.Count > 0)
        {
            // 0부터 프리팹 목록의 개수-1 사이의 랜덤한 숫자를 선택합니다.
            int randomIndex = Random.Range(0, mapNodePrefabs.Count);

            // 랜덤하게 선택된 프리팹을 변수에 저장합니다.
            GameObject selectedPrefab = mapNodePrefabs[randomIndex];

            // MapGenerator와 MapView의 mapTemplatePrefab 변수에 선택된 프리팹을 할당합니다.
            if (mapGenerator != null)
            {
                mapGenerator.mapTemplatePrefab = selectedPrefab;
            }

            if (mapView != null)
            {
                mapView.mapTemplatePrefab = selectedPrefab;
            }

            Debug.Log($"선택된 맵 프리팹: {selectedPrefab.name}");
        }
        else
        {
            Debug.LogError("맵 노드 프리팹 목록이 비어있습니다!");
        }
    }
}
