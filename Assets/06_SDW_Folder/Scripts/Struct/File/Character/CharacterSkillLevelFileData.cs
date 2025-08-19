using System;

namespace SDW
{
    public struct CharacterSkillLevelFileData
    {
        public int ChaSkillID;
        public int ChaSkillLevel;
        public int ChaSkillPoint;
        public string ChaSkillName;
        public SkillType ChaSkillType;
        public float ChaSkillChance;
        public float ChaSkillDuration;
        public float ChaSkillTick;
        public float ChaSkillValue;

        /// <summary>
        /// CharacterSkillLevelFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterSkillLevelFileData(string[] fields)
        {
            ChaSkillID = int.Parse(fields[0]);
            ChaSkillLevel = int.Parse(fields[1]);
            ChaSkillPoint = int.Parse(fields[2]);
            ChaSkillName = fields[3];
            ChaSkillType = (SkillType)Enum.Parse(typeof(SkillType), fields[4]);
            ChaSkillChance = float.Parse(fields[5]);
            ChaSkillDuration = float.Parse(fields[6]);
            ChaSkillTick = float.Parse(fields[7]);
            ChaSkillValue = float.Parse(fields[8]);
        }
    }
}