using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SDW; // EncounterTable을 사용하기 위해 네임스페이스 추가

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private Dictionary<int, EncounterTable> encounterDatabase = new Dictionary<int, EncounterTable>();
    private Dictionary<(EncounterSentiment, int), List<int>> encountersBySentimentAndStage = new Dictionary<(EncounterSentiment, int), List<int>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadEncounterData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadEncounterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("EncounterTable");
        if (textAsset == null)
        {
            Debug.LogError("'EncounterTable.csv' 파일을 'Assets/Resources' 폴더에서 찾을 수 없습니다!");
            return;
        }

        var lines = textAsset.text.Split('\n').Skip(3).ToArray();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var fields = line.Split('\t');
            if (fields.Length < 16) continue;

            var encounter = new EncounterTable(fields);

            if (!encounterDatabase.ContainsKey(encounter.EncounterID))
            {
                encounterDatabase.Add(encounter.EncounterID, encounter);
            }

            var key = (encounter.Sentiment, encounter.EncounterStage);
            if (!encountersBySentimentAndStage.ContainsKey(key))
            {
                encountersBySentimentAndStage.Add(key, new List<int>());
            }
            encountersBySentimentAndStage[key].Add(encounter.EncounterID);
        }

        Debug.Log($"[DataManager] 총 {encounterDatabase.Count}개의 인카운터 데이터를 CSV에서 성공적으로 불러왔습니다.");
    }

    public EncounterTable GetEncounterByID(int id)
    {
        if (encounterDatabase.TryGetValue(id, out EncounterTable data))
        {
            return data;
        }
        Debug.LogError($"[DataManager] ID: {id}에 해당하는 사건을 찾을 수 없습니다!");
        return default;
    }

    // ▼▼▼ 이 함수가 더 똑똑하게 변경되었습니다! ▼▼▼
    public int GetRandomEncounterID(EncounterSentiment sentiment, int stage)
    {
        // 1순위: 해당 스테이지의 특정 감정 사건 검색
        var key = (sentiment, stage);
        if (encountersBySentimentAndStage.TryGetValue(key, out List<int> idList) && idList.Count > 0)
        {
            return idList[Random.Range(0, idList.Count)];
        }

        // 2순위: 해당 감정의 공용(스테이지 0) 사건 검색
        var commonKey = (sentiment, 0);
        if (encountersBySentimentAndStage.TryGetValue(commonKey, out List<int> commonIdList) && commonIdList.Count > 0)
        {
            return commonIdList[Random.Range(0, commonIdList.Count)];
        }

        // 3순위 (비상 대책): 요청한 감정의 사건이 없으면, 해당 스테이지의 '아무 감정' 사건이라도 검색
        Debug.LogWarning($"[DataManager] 감정({sentiment})에 해당하는 사건이 없어, 스테이지({stage})의 다른 사건을 대신 탐색합니다.");
        var anySentimentIds = encountersBySentimentAndStage.Where(pair => pair.Key.Item2 == stage)
                                                          .SelectMany(pair => pair.Value)
                                                          .ToList();
        if (anySentimentIds.Count > 0)
        {
            return anySentimentIds[Random.Range(0, anySentimentIds.Count)];
        }

        // 4순위 (최후의 보루): 그것마저 없으면, 공용(스테이지 0)의 '아무 감정' 사건이라도 검색
        var anyCommonSentimentIds = encountersBySentimentAndStage.Where(pair => pair.Key.Item2 == 0)
                                                                 .SelectMany(pair => pair.Value)
                                                                 .ToList();
        if (anyCommonSentimentIds.Count > 0)
        {
            return anyCommonSentimentIds[Random.Range(0, anyCommonSentimentIds.Count)];
        }

        Debug.LogError($"[DataManager] 대체할 사건을 찾지 못했습니다! CSV 파일에 이벤트가 충분한지 확인해주세요.");
        return 0; // 최악의 경우에만 0 반환
    }
}