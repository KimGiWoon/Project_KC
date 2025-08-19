using System;

namespace SDW
{
    public struct MonsterDataFileData
    {
        public int MonId;
        public string MonName;
        public string MonNameEn;
        public MonsterType MonType;
        public AttackRange MonAtkRange;
        public float MonMoveSpeed;
        public int MonSkill1;
        public int MonSkill2;
        public int MonSkill3;
        public int MonItem;
        public int MonAniIdle;
        public int MonAniAttack;
        public int MonAnimDeath;

        /// <summary>
        /// MonsterDataFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public MonsterDataFileData(string[] fields)
        {
            MonId = int.Parse(fields[0]);
            MonName = fields[1];
            MonNameEn = fields[2];
            MonType = (MonsterType)Enum.Parse(typeof(MonsterType), fields[3]);
            MonAtkRange = (AttackRange)Enum.Parse(typeof(AttackRange), fields[4]);
            MonMoveSpeed = float.Parse(fields[5]);
            MonSkill1 = int.Parse(fields[6]);
            MonSkill2 = int.Parse(fields[7]);
            MonSkill3 = int.Parse(fields[8]);
            MonItem = int.Parse(fields[9]);
            MonAniIdle = int.Parse(fields[10]);
            MonAniAttack = int.Parse(fields[11]);
            MonAnimDeath = int.Parse(fields[12]);
        }
    }
}