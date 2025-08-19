namespace SDW
{
    public struct MonsterStatFileData
    {
        public int MomID;
        public string MonName;
        public int MonLv;
        public bool MonBreak;
        public float BreakGage;
        public float MonHP;
        public float MonMP;
        public float MonAtkSpeed;
        public float MonAttack;
        public float MonArmor;
        public float MonAccuracy;
        public float MonAvoid;
        public float MonReg;

        public MonsterStatFileData(string[] fields)
        {
            MomID = int.Parse(fields[0]);
            MonName = fields[1];
            MonLv = int.Parse(fields[2]);
            MonBreak = bool.Parse(fields[3]);
            BreakGage = float.Parse(fields[4]);
            MonHP = float.Parse(fields[5]);
            MonMP = float.Parse(fields[6]);
            MonAtkSpeed = float.Parse(fields[7]);
            MonAttack = float.Parse(fields[8]);
            MonArmor = float.Parse(fields[9]);
            MonAccuracy = float.Parse(fields[10]);
            MonAvoid = float.Parse(fields[11]);
            MonReg = float.Parse(fields[12]);
        }
    }
}