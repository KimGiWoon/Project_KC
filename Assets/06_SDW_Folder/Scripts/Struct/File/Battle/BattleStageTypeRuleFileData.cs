using System;

namespace SDW
{
    public struct BattleStageTypeRuleFileData
    {
        public BattleEventType Type;
        public int StageTime;

        public BattleStageTypeRuleFileData(string[] fields)
        {
            Type = (BattleEventType)Enum.Parse(typeof(BattleEventType), fields[0]);
            StageTime = int.Parse(fields[1]);
        }
    }
}