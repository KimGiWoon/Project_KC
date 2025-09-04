using System;
using System.Collections.Generic;

namespace SDW
{
    public struct RelicDataFileData
    {
        public int RelicID;
        public RelicEnName RelicEnName;
        public string RelicName;
        public RelicKind RelicKind;
        public RelicGrade RelicGrade;
        public List<string> RelicDescription;
        public RelicTarget RelicTarget;
        public bool isPassive;
        public RelicType RelicType;
        public RelicRole RelicRole;
        public int ChaAttack;
        public int ChaArmor;
        public int ChaHP;
        public int ChaCritDmg;
        public int ChaAtkSpeed;
        public int ChaAccuracy;
        public int ChaAvoid;
        public int ChaMPRecovery;
        public int ChaDrain;
        public int MonArmor;
        public int MonHP;
        public int MonAttack;
        public int MonAtkSpeed;
        public int AddReward;
        public int StoreDiscount;

        public RelicDataFileData(string[] fields)
        {
            RelicID = int.Parse(fields[0]);
            RelicEnName = (RelicEnName)Enum.Parse(typeof(RelicEnName), fields[1]);
            RelicName = fields[2];
            RelicKind = (RelicKind)Enum.Parse(typeof(RelicKind), fields[3]);
            RelicGrade = (RelicGrade)Enum.Parse(typeof(RelicGrade), fields[4]);

            RelicDescription = new List<string>();
            RelicDescription.Add(fields[5]);
            RelicDescription.Add(fields[6]);

            RelicTarget = (RelicTarget)Enum.Parse(typeof(RelicTarget), fields[7]);
            isPassive = bool.Parse(fields[8]);
            RelicType = (RelicType)Enum.Parse(typeof(RelicType), fields[9]);
            RelicRole = (RelicRole)Enum.Parse(typeof(RelicRole), fields[10]);
            ChaAttack = int.Parse(fields[11]);
            ChaArmor = int.Parse(fields[12]);
            ChaHP = int.Parse(fields[13]);
            ChaCritDmg = int.Parse(fields[14]);
            ChaAtkSpeed = int.Parse(fields[15]);
            ChaAccuracy = int.Parse(fields[16]);
            ChaAvoid = int.Parse(fields[17]);
            ChaMPRecovery = int.Parse(fields[18]);
            ChaDrain = int.Parse(fields[19]);
            MonArmor = int.Parse(fields[20]);
            MonHP = int.Parse(fields[21]);
            MonAttack = int.Parse(fields[22]);
            MonAtkSpeed = int.Parse(fields[23]);
            AddReward = int.Parse(fields[24]);
            StoreDiscount = int.Parse(fields[25]);
        }
    }
}