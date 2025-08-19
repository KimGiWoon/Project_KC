using System;

namespace SDW
{
    public struct MonsterSkillFileData
    {
        public int MonSkillID;
        public string MonSkillName;
        public string MonSkillNameEn;
        public SkillType MonSkillType;
        public SkillTargetType MonSkillTargetType;
        public SkillEffectType MonSkillEffectType;
        public float MonSkillCd;
        public SkillCC MonSkillCC;
        public float MonSkillChance;
        public AttackRange MonSkillRange;
        public float MonSkillDuration;
        public float MonSkillTick;
        public int MonSkillHit;
        public float MonSkillValue;
        public int MonSkillAnim;
        public string MonSkillImg;
        public string MonSkillEffect;

        /// <summary>
        /// MonsterSkillData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public MonsterSkillFileData(string[] fields)
        {
            MonSkillID = int.Parse(fields[0]);
            MonSkillName = fields[1];
            MonSkillNameEn = fields[2];
            MonSkillType = (SkillType)Enum.Parse(typeof(SkillType), fields[3]);
            MonSkillTargetType = (SkillTargetType)Enum.Parse(typeof(SkillTargetType), fields[4]);
            MonSkillEffectType = (SkillEffectType)Enum.Parse(typeof(SkillEffectType), fields[5]);
            MonSkillCd = float.Parse(fields[6]);
            MonSkillCC = (SkillCC)Enum.Parse(typeof(SkillCC), fields[7]);
            MonSkillChance = float.Parse(fields[8]);
            MonSkillRange = (AttackRange)Enum.Parse(typeof(AttackRange), fields[9]);
            MonSkillDuration = float.Parse(fields[10]);
            MonSkillTick = float.Parse(fields[11]);
            MonSkillHit = int.Parse(fields[12]);
            MonSkillValue = float.Parse(fields[13]);
            MonSkillAnim = int.Parse(fields[14]);
            MonSkillImg = fields[15];
            MonSkillEffect = fields[16];
        }
    }
}