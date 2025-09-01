using System;

namespace SDW
{
    public struct MonsterSkillFileData
    {
        public int MonSkillID;
        public string MonSkillName;
        public MonsterSkillEnName MonSkillEnName;
        public SkillType MonSkillType;
        public SkillTargetType MonSkillTargetType;
        public SkillEffectType MonSkillEffectType;
        public float MonSkillCd;
        public int MonSkillRange;
        public int MonSkillConHP;
        public float MonSkillChance;
        public float MonSkillDuration;
        public float MonSkillTick;
        public int MonSkillHit;
        public float MonSkillValue;
        public string MonEffectValue;
        public SkillCC MonSkillCC;
        public string SkillDescription;

        /// <summary>
        /// MonsterSkillData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public MonsterSkillFileData(string[] fields)
        {
            MonSkillID = int.Parse(fields[0]);
            MonSkillName = fields[1];
            MonSkillEnName = (MonsterSkillEnName)Enum.Parse(typeof(MonsterSkillEnName), fields[2]);
            MonSkillType = (SkillType)Enum.Parse(typeof(SkillType), fields[3]);
            MonSkillTargetType = (SkillTargetType)Enum.Parse(typeof(SkillTargetType), fields[4]);
            MonSkillEffectType = (SkillEffectType)Enum.Parse(typeof(SkillEffectType), fields[5]);
            MonSkillCd = float.Parse(fields[6]);
            MonSkillRange = int.Parse(fields[7]);
            MonSkillConHP = int.Parse(fields[8]);
            MonSkillChance = float.Parse(fields[9]);
            MonSkillDuration = float.Parse(fields[10]);
            MonSkillTick = float.Parse(fields[11]);
            MonSkillHit = int.Parse(fields[12]);
            MonSkillValue = float.Parse(fields[13]);
            MonEffectValue = fields[14];
            MonSkillCC = (SkillCC)Enum.Parse(typeof(SkillCC), fields[15]);
            SkillDescription = fields[16];
            ;
        }
    }
}