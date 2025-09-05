using System;

namespace SDW
{
    public struct BattleStageRewardData
    {
        public int IncludedStage;
        public BattleEventType Type;
        public int RewardScore;

        /// <summary>
        /// BattleStageRewardData 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public BattleStageRewardData(string[] fields)
        {
            IncludedStage = int.Parse(fields[0]);
            Type = (BattleEventType)Enum.Parse(typeof(BattleEventType), fields[1]);
            RewardScore = int.Parse(fields[2]);
        }
    }
}