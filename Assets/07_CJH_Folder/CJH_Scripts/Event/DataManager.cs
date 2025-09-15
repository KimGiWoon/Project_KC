using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SDW;

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
        return default;
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