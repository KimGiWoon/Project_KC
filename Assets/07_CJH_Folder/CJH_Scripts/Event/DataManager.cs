using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SDW; // EncounterTable을 사용하기 위해 네임스페이스 추가

public class DataManager : MonoBehaviour
{
    private EncounterDataManager _encounterData;

    private void Start()
    {
        _encounterData = GameManager.Instance.Encounter;
    }

    public EncounterTable GetEncounterByID(int id)
    {
        if (_encounterData.EncounterIDDataTable.TryGetValue(id, out var data))
        {
            return data;
        }
        // Debug.LogError($"[DataManager] ID: {id}에 해당하는 사건을 찾을 수 없습니다!");
        return default;
    }

    public int GetRandomEncounterID(EncounterSentiment sentiment, int stage)
    {
        // 1순위: 해당 스테이지의 특정 감정 사건 검색
        var key = (sentiment, stage);
        if (_encounterData.EncountersBySentimentAndStage.TryGetValue(key, out var idList) && idList.Count > 0)
        {
            return idList[Random.Range(0, idList.Count)];
        }

        // 2순위: 해당 감정의 공용(스테이지 0) 사건 검색
        var commonKey = (sentiment, 0);
        if (_encounterData.EncountersBySentimentAndStage.TryGetValue(commonKey, out var commonIdList) && commonIdList.Count > 0)
        {
            return commonIdList[Random.Range(0, commonIdList.Count)];
        }

        // 3순위 (비상 대책): 요청한 감정의 사건이 없으면, 해당 스테이지의 '아무 감정' 사건이라도 검색
        // Debug.LogWarning($"[DataManager] 감정({sentiment})에 해당하는 사건이 없어, 스테이지({stage})의 다른 사건을 대신 탐색합니다.");
        var anySentimentIds = _encounterData.EncountersBySentimentAndStage.Where(pair => pair.Key.Item2 == stage)
            .SelectMany(pair => pair.Value)
            .ToList();
        if (anySentimentIds.Count > 0)
        {
            return anySentimentIds[Random.Range(0, anySentimentIds.Count)];
        }

        // 4순위 (최후의 보루): 그것마저 없으면, 공용(스테이지 0)의 '아무 감정' 사건이라도 검색
        var anyCommonSentimentIds = _encounterData.EncountersBySentimentAndStage.Where(pair => pair.Key.Item2 == 0)
            .SelectMany(pair => pair.Value)
            .ToList();
        if (anyCommonSentimentIds.Count > 0)
        {
            return anyCommonSentimentIds[Random.Range(0, anyCommonSentimentIds.Count)];
        }

        // Debug.LogError($"[DataManager] 대체할 사건을 찾지 못했습니다! CSV 파일에 이벤트가 충분한지 확인해주세요.");
        return 0; // 최악의 경우에만 0 반환
    }
}