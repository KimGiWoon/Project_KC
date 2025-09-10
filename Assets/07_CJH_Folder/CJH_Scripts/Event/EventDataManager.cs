using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SDW; // EncounterTable을 사용하기 위해 네임스페이스 추가

public class EventDataManager : MonoBehaviour
{
    public static EventDataManager Instance;

    private List<EncounterTable> allEncounters;

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

    // Resources 폴더에서 CSV 파일을 로드하고 파싱하는 함수
    private void LoadEncounterData()
    {
        allEncounters = new List<EncounterTable>();
        TextAsset textAsset = Resources.Load<TextAsset>("EncounterTable");

        if (textAsset == null)
        {
            Debug.LogError("'EncounterTable.csv' 파일을 Resources 폴더에서 찾을 수 없습니다!");
            return;
        }

        // 줄 단위로 자르고, 실제 데이터가 시작되는 4번째 줄부터 읽음
        var lines = textAsset.text.Split('\n').Skip(3).ToArray();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var fields = line.Split('\t');
            if (fields.Length < 16) continue;

            // EncounterTable.cs의 생성자를 사용하여 데이터 파싱
            allEncounters.Add(new EncounterTable(fields));
        }

        Debug.Log($"[EventDataManager] 총 {allEncounters.Count}개의 인카운터 데이터를 CSV에서 불러왔습니다.");
    }

    // 지정된 스테이지에 맞는 인카운터를 무작위로 반환하는 함수
    public EncounterTable GetRandomEncounter(int stage)
    {
        // 현재 스테이지와 일치하거나, 모든 스테이지 공용(0)인 사건들을 필터링
        var stageEncounters = allEncounters.Where(e => e.EncounterStage == stage || e.EncounterStage == 0).ToList();

        if (stageEncounters.Count == 0)
        {
            Debug.LogWarning($"{stage} 스테이지에 해당하는 인카운터를 찾을 수 없습니다!");
            return default;
        }

        int randomIndex = Random.Range(0, stageEncounters.Count);
        return stageEncounters[randomIndex];
    }
}