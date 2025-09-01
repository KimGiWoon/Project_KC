using System;

namespace SDW
{
    public struct CharacterSkillFileData
    {
        public int ChaSkillID;
        public string ChaSkillName;
        public CharacterSkillEnName ChaSkillEnName;
        public SkillType ChaSkillType;
        public SkillTargetType ChaSkillTargetType;
        public SkillEffectType ChaSkillEffectType;
        public float ChaSkillChance;
        public float ChaSkillDuration;
        public float ChaSkillTick;
        public int ChaSkillHit;
        public float ChaSkillValue;
        public float ChaEffectValue;
        public string ChaSkillDescription;

        /// <summary>
        /// CharacterSkillFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterSkillFileData(string[] fields)
        {
            ChaSkillID = int.Parse(fields[0]);
            ChaSkillName = fields[1];
            ChaSkillEnName = (CharacterSkillEnName)Enum.Parse(typeof(CharacterSkillEnName), fields[2]);
            ChaSkillType = (SkillType)Enum.Parse(typeof(SkillType), fields[3]);
            ChaSkillTargetType = (SkillTargetType)Enum.Parse(typeof(SkillTargetType), fields[4]);
            ChaSkillEffectType = (SkillEffectType)Enum.Parse(typeof(SkillEffectType), fields[5]);
            ChaSkillChance = float.Parse(fields[6]);
            ChaSkillDuration = float.Parse(fields[7]);
            ChaSkillTick = float.Parse(fields[8]);
            ChaSkillHit = int.Parse(fields[9]);
            ChaSkillValue = float.Parse(fields[10]);
            ChaEffectValue = int.Parse(fields[11]);
            ChaSkillDescription = fields[12];
        }
    }
}