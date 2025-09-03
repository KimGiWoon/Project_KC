using System;
using System.Collections.Generic;

namespace SDW
{
    public struct BattleStageDataFileData
    {
        public int BattleEventID;
        public int IncludedChapter;
        public int IncludedStage;
        public BattleEventType Type;
        public List<SpawnMonster> Monsters;

        public BattleStageDataFileData(string[] fields)
        {
            BattleEventID = int.Parse(fields[0]);
            IncludedChapter = int.Parse(fields[1]);
            IncludedStage = int.Parse(fields[2]);
            Type = (BattleEventType)Enum.Parse(typeof(BattleEventType), fields[3]);

            //# 몬스터 1
            Monsters = new List<SpawnMonster>();
            Monsters.Add(new SpawnMonster
            {
                MonsterID = int.Parse(fields[4]),
                MonsterLevel = int.Parse(fields[5]),
                MonsterNum = int.Parse(fields[6])
            });

            //# 몬스터 2
            int id = int.Parse(fields[7]);
            if (id == 0) return;
            Monsters.Add(new SpawnMonster
            {
                MonsterID = id,
                MonsterLevel = int.Parse(fields[8]),
                MonsterNum = int.Parse(fields[9])
            });
        }
    }
}