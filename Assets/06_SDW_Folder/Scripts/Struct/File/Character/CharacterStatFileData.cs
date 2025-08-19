namespace SDW
{
    public struct CharacterStatFileData
    {
        public int ChaId;
        public string ChaName;
        public int ChaLevel;
        public int ChaLevelPoint;
        public bool ChaSkillUnlock;
        public float ChaHP;
        public float ChaMP;
        public float ChaAtkSpeed;
        public float ChaAttack;
        public float ChaArmor;
        public float ChaAccuracy;
        public float ChaAvoid;
        public float ChaCrit;
        public float ChaCritDmg;
        public float ChaReg;

        /// <summary>
        /// CharacterStatFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterStatFileData(string[] fields)
        {
            ChaId = int.Parse(fields[0]);
            ChaName = fields[1];
            ChaLevel = int.Parse(fields[2]);
            ChaLevelPoint = int.Parse(fields[3]);
            ChaSkillUnlock = bool.Parse(fields[4]);
            ChaHP = float.Parse(fields[5]);
            ChaMP = float.Parse(fields[6]);
            ChaAtkSpeed = float.Parse(fields[7]);
            ChaAttack = float.Parse(fields[8]);
            ChaArmor = float.Parse(fields[9]);
            ChaAccuracy = float.Parse(fields[10]);
            ChaAvoid = float.Parse(fields[11]);
            ChaCrit = float.Parse(fields[12]);
            ChaCritDmg = float.Parse(fields[13]);
            ChaReg = float.Parse(fields[14]);
        }
    }
}