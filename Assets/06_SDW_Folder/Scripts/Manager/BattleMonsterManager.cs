using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class BattleMonsterManager : MonoBehaviour
    {
        //# ID - BattleStageDataFileData
        private Dictionary<int, BattleStageDataFileData> _battleIDDataTable = new Dictionary<int, BattleStageDataFileData>();
        public Dictionary<int, BattleStageDataFileData> BattleIDDataTable => _battleIDDataTable;

        //# Chapter-Stage-BattleEventType - BattleStageDataFileData
        private Dictionary<string, List<BattleStageDataFileData>> _battleStageDataTable =
            new Dictionary<string, List<BattleStageDataFileData>>();
        public Dictionary<string, List<BattleStageDataFileData>> BattleStageDataTable => _battleStageDataTable;

        //# Chapter-Stage - BattleStageUIDataFIleData
        private Dictionary<string, BattleStageUIDataFileData> _battleStageUIDataTable =
            new Dictionary<string, BattleStageUIDataFileData>();
        public Dictionary<string, BattleStageUIDataFileData> BattleStageUIDataTable => _battleStageUIDataTable;

        //# BattleEventType - StageTime
        private Dictionary<BattleEventType, int> _battleStageTypeRuleTimeDataTable = new Dictionary<BattleEventType, int>();
        public Dictionary<BattleEventType, int> BattleStageTypeRuleTimeDataTable => _battleStageTypeRuleTimeDataTable;

        //# Stage+BattleEventType - Reward
        private Dictionary<string, int> _battleStageRewardDataTable = new Dictionary<string, int>();
        public Dictionary<string, int> BattleStageRewardDataTable => _battleStageRewardDataTable;

        private void Start()
        {
            LoadBattleStageDataFileData();
            LoadBattleStageUIDataFileData();
            LoadBattleStageTypeRuleFileData();
            LoadBattleStageRewardData();
        }

        private void LoadBattleStageDataFileData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Battle/BattleStageData");
            var battleList = HandleCSV.ReadDataFromLines<BattleStageDataFileData>(fields);

            foreach (var battle in battleList)
            {
                _battleIDDataTable[battle.BattleEventID] = battle;

                string key = $"{battle.IncludedChapter}-{battle.IncludedStage}-{battle.Type}";

                //# 중복 키가 없을 경우 List 초기화
                if (!_battleStageDataTable.ContainsKey(key))
                    _battleStageDataTable[key] = new List<BattleStageDataFileData>();

                _battleStageDataTable[key].Add(battle);
            }
        }

        private void LoadBattleStageUIDataFileData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Battle/BattleStageUIData");
            var battleUIList = HandleCSV.ReadDataFromLines<BattleStageUIDataFileData>(fields);

            foreach (var battleUI in battleUIList)
            {
                string key = $"{battleUI.IncludedChapter}-{battleUI.IncludedStage}";
                _battleStageUIDataTable[key] = battleUI;
            }
        }

        private void LoadBattleStageTypeRuleFileData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Battle/BattleStageTypeRule");
            var battleTypeList = HandleCSV.ReadDataFromLines<BattleStageTypeRuleFileData>(fields);

            foreach (var battleType in battleTypeList)
            {
                _battleStageTypeRuleTimeDataTable[battleType.Type] = battleType.StageTime;
            }
        }

        private void LoadBattleStageRewardData()
        {
            string[] fields = HandleCSV.LoadFromCsv("Battle/BattleStageRewardData");
            var battleRewardList = HandleCSV.ReadDataFromLines<BattleStageRewardData>(fields);

            foreach (var battleReward in battleRewardList)
            {
                string key = $"{battleReward.IncludedStage}-{battleReward.Type}";
                _battleStageRewardDataTable[key] = battleReward.RewardScore;
            }
        }
    }
}