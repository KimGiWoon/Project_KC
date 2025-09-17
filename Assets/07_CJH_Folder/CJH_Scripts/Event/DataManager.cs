using UnityEngine;
using System.Linq;
using SDW;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    private EncounterDataManager _encounterData;
    public EventGroupData eventGroupData;

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
        return default;
    }

    public List<EncounterTable> GetEncountersByGroupID(int groupID)
    {
        // 1. ScriptableObject에서 ID 목록을 가져옵니다.
        List<int> idsInGroup = eventGroupData.GetEncounterIDsByGroupID(groupID);

        List<EncounterTable> encounters = new List<EncounterTable>();

        // 2. 가져온 ID로 실제 이벤트 데이터를 하나씩 조회합니다.
        foreach (int id in idsInGroup)
        {
            var encounter = GetEncounterByID(id);
            if (encounter.EncounterID != 0)
            {
                encounters.Add(encounter);
            }
        }

        // 3. 완성된 리스트를 반환합니다.
        return encounters;
    }

    public int GetRandomEncounterID(EncounterSentiment sentiment, int stage)
    {
        var key = (sentiment, stage);
        if (_encounterData.EncountersBySentimentAndStage.TryGetValue(key, out var idList) && idList.Count > 0)
        {
            return idList[Random.Range(0, idList.Count)];
        }

        var commonKey = (sentiment, 0);
        if (_encounterData.EncountersBySentimentAndStage.TryGetValue(commonKey, out var commonIdList) && commonIdList.Count > 0)
        {
            return commonIdList[Random.Range(0, commonIdList.Count)];
        }


        var anySentimentIds = _encounterData.EncountersBySentimentAndStage.Where(pair => pair.Key.Item2 == stage)
            .SelectMany(pair => pair.Value)
            .ToList();
        if (anySentimentIds.Count > 0)
        {
            return anySentimentIds[Random.Range(0, anySentimentIds.Count)];
        }

        var anyCommonSentimentIds = _encounterData.EncountersBySentimentAndStage.Where(pair => pair.Key.Item2 == 0)
            .SelectMany(pair => pair.Value)
            .ToList();
        if (anyCommonSentimentIds.Count > 0)
        {
            return anyCommonSentimentIds[Random.Range(0, anyCommonSentimentIds.Count)];
        }

        return 0;
    }
}