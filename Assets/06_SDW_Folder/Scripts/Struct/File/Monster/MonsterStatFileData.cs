using System;

namespace SDW
{
    public struct MonsterStatFileData
    {
        public int MomID;
        public string MonName;
        public MonsterEnName MonEnName;
        public int MonLv;
        public bool MonBreak;
        public float BreakGage;
        public float MonHP;
        //# Increase는 % 증가(명중률은 +)
        public float MonHPIncrase;
        public float MonAtkSpeed;
        public float MonAttack;
        public float MonAttackIncrease;
        public float MonArmor;
        public float MonArmorIncrease;
        public float MonAccuracy;
        public float MonAvoid;
        public float MonAvoidIncrease;
        public float MonReg;

        /// <summary>
        /// MonsterStatFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public MonsterStatFileData(string[] fields)
        {
            MomID = int.Parse(fields[0]);
            MonName = fields[1];
            MonEnName = (MonsterEnName)Enum.Parse(typeof(MonsterEnName), fields[2]);
            MonLv = int.Parse(fields[3]);
            MonBreak = bool.Parse(fields[4]);
            BreakGage = int.Parse(fields[5]);
            MonHP = float.Parse(fields[6]);
            MonHPIncrase = float.Parse(fields[7]);
            MonAtkSpeed = float.Parse(fields[8]);
            MonAttack = float.Parse(fields[9]);
            MonAttackIncrease = float.Parse(fields[10]);
            MonArmor = float.Parse(fields[11]);
            MonArmorIncrease = float.Parse(fields[12]);
            MonAccuracy = float.Parse(fields[13]);
            MonAvoid = float.Parse(fields[14]);
            MonAvoidIncrease = float.Parse(fields[15]);
            MonReg = float.Parse(fields[16]);
        }
    }
}