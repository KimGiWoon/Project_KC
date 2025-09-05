using System;

namespace SDW
{
    public struct BattleStageTypeRuleFileData
    {
        public BattleEventType Type;
        public int StageTime;

        /// <summary>
        /// BattleStageTypeRuleFileData 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public BattleStageTypeRuleFileData(string[] fields)
        {
            Type = (BattleEventType)Enum.Parse(typeof(BattleEventType), fields[0]);
            StageTime = int.Parse(fields[1]);
        }
    }
}