using System;

namespace SDW
{
    public struct BattleStageRewardData
    {
        public int IncludedStage;
        public BattleEventType Type;
        public int RewardScore;

        public BattleStageRewardData(string[] fields)
        {
            IncludedStage = int.Parse(fields[0]);
            Type = (BattleEventType)Enum.Parse(typeof(BattleEventType), fields[1]);
            RewardScore = int.Parse(fields[2]);
        }
    }
}