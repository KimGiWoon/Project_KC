using System;

namespace SDW
{
    public struct CharacterDataFileData
    {
        public int ChaID;
        public string ChaName;
        public string ChaNameEn;
        public CharacterGrade ChaGrade;
        public CharacterRole ChaRole;
        public AttackRange ChaAtkRange;
        public float ChaMoveSpeed;
        public int CharSKill1;
        public int CharSkill2;
        public int CharSkill3;
        public bool ChaUpgrade1;
        public bool ChaUpgrade2;
        public bool ChaUpgrade3;
        public bool ChaUpgrade4;
        public bool ChaUpgrade5;
        public bool ChaUpgrade6;
        public int ChaAniIdle;
        public int ChaAniAttack;
        public int ChaAnimDeath;

        public CharacterDataFileData(string[] fields)
        {
            ChaID = int.Parse(fields[0]);
            ChaName = fields[1];
            ChaNameEn = fields[2];
            ChaGrade = (CharacterGrade)Enum.Parse(typeof(CharacterGrade), fields[3]);
            ChaRole = (CharacterRole)Enum.Parse(typeof(CharacterRole), fields[4]);
            ChaAtkRange = (AttackRange)Enum.Parse(typeof(AttackRange), fields[5]);
            ChaMoveSpeed = float.Parse(fields[6]);
            CharSKill1 = int.Parse(fields[7]);
            CharSkill2 = int.Parse(fields[8]);
            CharSkill3 = int.Parse(fields[9]);
            ChaUpgrade1 = bool.Parse(fields[10]);
            ChaUpgrade2 = bool.Parse(fields[11]);
            ChaUpgrade3 = bool.Parse(fields[12]);
            ChaUpgrade4 = bool.Parse(fields[13]);
            ChaUpgrade5 = bool.Parse(fields[14]);
            ChaUpgrade6 = bool.Parse(fields[15]);
            ChaAniIdle = int.Parse(fields[16]);
            ChaAniAttack = int.Parse(fields[17]);
            ChaAnimDeath = int.Parse(fields[18]);
        }
    }
}