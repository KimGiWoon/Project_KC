namespace SDW
{
    public struct CharacterUpgrade
    {
        public int ChaId;
        public string ChaUpID;
        public int ChaUpgrade;
        public float ChaHp;
        public float ChaMP;
        public float ChaArmor;
        public float ChaAccuracy;
        public float ChaAvoid;
        public float ChaCrit;
        public float ChaCritDmg;
        public float ChaReg;
        public int ChaCkillID;

        public CharacterUpgrade(string[] fields)
        {
            ChaId = int.Parse(fields[0]);
            ChaUpID = fields[1];
            ChaUpgrade = int.Parse(fields[2]);
            ChaHp = float.Parse(fields[3]);
            ChaMP = float.Parse(fields[4]);
            ChaArmor = float.Parse(fields[5]);
            ChaAccuracy = float.Parse(fields[6]);
            ChaAvoid = float.Parse(fields[7]);
            ChaCrit = float.Parse(fields[8]);
            ChaCritDmg = float.Parse(fields[9]);
            ChaReg = float.Parse(fields[10]);
            ChaCkillID = int.Parse(fields[11]);
        }
    }
}