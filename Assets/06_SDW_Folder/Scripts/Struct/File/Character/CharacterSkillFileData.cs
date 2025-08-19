using System;

namespace SDW
{
    public struct CharacterSkillFileData
    {
        public int ChaSkillID;
        public string ChaSkillName;
        public string ChaSkillNameEn;
        public SkillType ChaSkillType;
        public SkillTargetType ChaSkillTargetType;
        public SkillCC ChaSkillCC;
        public SkillEffectType ChaSkillChance;
        public AttackRange ChaSkillRange;
        public float ChaSkillDuration;
        public float ChaSkillTick;
        public float CharSkillHit;
        public float ChaSkillValue;
        public int ChaSkillAnim;
        public string ChaSkillImg;
        public string ChaSkillEffect;

        public CharacterSkillFileData(string[] fields)
        {
            ChaSkillID = int.Parse(fields[0]);
            ChaSkillName = fields[1];
            ChaSkillNameEn = fields[2];
            ChaSkillType = (SkillType)Enum.Parse(typeof(SkillType), fields[3]);
            ChaSkillTargetType = (SkillTargetType)Enum.Parse(typeof(SkillTargetType), fields[4]);
            ChaSkillCC = (SkillCC)Enum.Parse(typeof(SkillCC), fields[5]);
            ChaSkillChance = (SkillEffectType)Enum.Parse(typeof(SkillEffectType), fields[6]);
            ChaSkillRange = (AttackRange)Enum.Parse(typeof(AttackRange), fields[7]);
            ChaSkillDuration = float.Parse(fields[8]);
            ChaSkillTick = float.Parse(fields[9]);
            CharSkillHit = float.Parse(fields[10]);
            ChaSkillValue = float.Parse(fields[11]);
            ChaSkillAnim = int.Parse(fields[12]);
            ChaSkillImg = fields[13];
            ChaSkillEffect = fields[14];
        }
    }
}