using System;

namespace SDW
{
    [Serializable]
    public struct CharacterBaseDataFileData
    {
        public int ChaID;
        public string ChaName;
        public CharacterEnName ChaEnName;
        public CharacterGrade ChaGrade;
        public CharacterRole ChaRole;
        public float ChaMP;
        public float ChaMPRecovery;
        public float ChaHP;
        public float ChaAtkSpeed;
        public float ChaAttack;
        public float ChaArmor;
        public int ChaPassiveSkill;
        public int ChaActiveSkill;
        public int ChaPassiveSkillEffect;
        public int ChaActiveSkillEffect;
        public string ChaIntroduction;

        /// <summary>
        /// CharacterDataFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterBaseDataFileData(string[] fields)
        {
            ChaID = int.Parse(fields[0]);
            ChaName = fields[1];
            ChaEnName = (CharacterEnName)Enum.Parse(typeof(CharacterEnName), fields[2]);
            ChaGrade = (CharacterGrade)Enum.Parse(typeof(CharacterGrade), fields[3]);
            ChaRole = (CharacterRole)Enum.Parse(typeof(CharacterRole), fields[4]);
            ChaMP = float.Parse(fields[5]);
            ChaMPRecovery = float.Parse(fields[6]);
            ChaHP = float.Parse(fields[7]);
            ChaAtkSpeed = float.Parse(fields[8]);
            ChaAttack = float.Parse(fields[9]);
            ChaArmor = float.Parse(fields[10]);
            ChaPassiveSkill = int.Parse(fields[11]);
            ChaActiveSkill = int.Parse(fields[12]);
            ChaPassiveSkillEffect = int.Parse(fields[13]);
            ChaActiveSkillEffect = int.Parse(fields[14]);
            ChaIntroduction = fields[15];
        }
    }
}