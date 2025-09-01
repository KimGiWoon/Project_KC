namespace SDW
{
    public struct CharacterLevelUpStatFileData
    {
        public int ChaLevel;
        public int ChaLevelPoint;
        public bool ChaSkillUnlock;
        //# Increase - 모두 %
        public float ChaHPIncrease;
        public float ChaAttackIncrease;
        public float ChaArmorIncrease;

        /// <summary>
        /// CharacterLevelUpStatFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterLevelUpStatFileData(string[] fields)
        {
            ChaLevel = int.Parse(fields[0]);
            ChaLevelPoint = int.Parse(fields[1]);
            ChaSkillUnlock = bool.Parse(fields[2]);
            ChaHPIncrease = float.Parse(fields[3]);
            ChaAttackIncrease = float.Parse(fields[4]);
            ChaArmorIncrease = float.Parse(fields[5]);
        }
    }
}