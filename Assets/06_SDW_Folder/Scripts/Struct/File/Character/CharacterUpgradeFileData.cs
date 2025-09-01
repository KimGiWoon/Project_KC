namespace SDW
{
    public struct CharacterUpgradeFileData
    {
        //# Character Beads와 연동
        public int ChaUpgrade;
        public float ChaHP;
        public float ChaAttack;
        public float ChaArmor;

        /// <summary>
        /// CharacterUpgradeFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterUpgradeFileData(string[] fields)
        {
            ChaUpgrade = int.Parse(fields[0]);
            ChaHP = float.Parse(fields[1]);
            ChaAttack = float.Parse(fields[2]);
            ChaArmor = float.Parse(fields[3]);
        }
    }
}