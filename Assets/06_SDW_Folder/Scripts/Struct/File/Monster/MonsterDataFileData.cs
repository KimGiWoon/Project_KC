using System;
using System.Collections.Generic;

namespace SDW
{
    public struct MonsterDataFileData
    {
        public int MonId;
        public string MonName;
        public MonsterEnName MonEnName;
        public MonsterType MonType;
        public bool MonSummon;
        public int MonAtkRange;
        public float MonMoveSpeed;
        public List<int> MonSkill;
        public List<int> MonSummonID;
        public string MonTag;
        public List<string> MonDescription;

        /// <summary>
        /// MonsterDataFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public MonsterDataFileData(string[] fields)
        {
            MonId = int.Parse(fields[0]);
            MonName = fields[1];
            MonEnName = (MonsterEnName)Enum.Parse(typeof(MonsterEnName), fields[2]);
            MonType = (MonsterType)Enum.Parse(typeof(MonsterType), fields[3]);
            MonSummon = bool.Parse(fields[4]);
            MonAtkRange = int.Parse(fields[5]);
            MonMoveSpeed = float.Parse(fields[6]);
            MonSkill = new List<int>();
            for (int i = 7; i <= 8; i++)
            {
                int skillId = int.Parse(fields[i]);
                if (skillId == -1) break;

                MonSkill.Add(skillId);
            }
            MonSummonID = new List<int>();
            for (int i = 9; i <= 11; i++)
            {
                int summonId = int.Parse(fields[i]);
                if (summonId == -1) break;

                MonSummonID.Add(summonId);
            }
            MonTag = fields[12];
            MonDescription = new List<string>();
            for (int i = 13; i <= 14; i++)
            {
                string description = fields[i];
                if (description == "null") break;

                MonDescription.Add(description);
            }
        }
    }
}