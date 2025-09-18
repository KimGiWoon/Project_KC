using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class EncounterDataManager : MonoBehaviour
    {
        //# 사건 ID별 정리
        private Dictionary<int, EncounterTable> _encounterIDDataTable = new Dictionary<int, EncounterTable>();
        public Dictionary<int, EncounterTable> EncounterIDDataTable => _encounterIDDataTable;

        //# Stage 별로 일어날 수 있는 일 정리
        private Dictionary<int, List<EncounterTable>> _encounterStageDataTables = new Dictionary<int, List<EncounterTable>>();
        public Dictionary<int, List<EncounterTable>> EncounterStageDataTables => _encounterStageDataTables;

        //# EncounterSentiment+Stage - EncounterID
        private Dictionary<(EncounterSentiment, int), List<int>> _encountersBySentimentAndStage =
            new Dictionary<(EncounterSentiment, int), List<int>>();
        public Dictionary<(EncounterSentiment, int), List<int>> EncountersBySentimentAndStage => _encountersBySentimentAndStage;

        /// <summary>
        /// Data Table 데이터 연결
        /// </summary>
        private void Start() => LoadEncounterData();

        /// <summary>
        /// EncounterTable을 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadEncounterData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Encounter/EncounterTable");
            var encounterList = HandleCSV.ReadDataFromLines<EncounterTable>(fields);

            for (int i = 1; i < 4; i++)
            {
                if (!_encounterStageDataTables.ContainsKey(i))
                    _encounterStageDataTables[i] = new List<EncounterTable>();
            }

            foreach (var encounter in encounterList)
            {
                _encounterIDDataTable[encounter.EncounterID] = encounter;

                if (encounter.EncounterStage != 0)
                    _encounterStageDataTables[encounter.EncounterStage].Add(encounter);
                else
                {
                    for (int i = 1; i < 4; i++)
                    {
                        _encounterStageDataTables[i].Add(encounter);
                    }
                }

                var key = (encounter.Sentiment, encounter.EncounterStage);
                if (!_encountersBySentimentAndStage.ContainsKey(key))
                    _encountersBySentimentAndStage.Add(key, new List<int>());
                _encountersBySentimentAndStage[key].Add(encounter.EncounterID);
            }
        }
    }
}