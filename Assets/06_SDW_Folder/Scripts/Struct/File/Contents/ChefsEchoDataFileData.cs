using System;

namespace SDW
{
    public struct ChefsEchoDataFileData
    {
        public int NodeID;
        public NodeGrade Grade;
        public string NodeName;
        public NodeEnName EnName;
        public string NodeDescription;
        public int NodeCurrency;
        public int NodeActiveCondition;
        public NodeAbility Ability;
        public float NodeAbilityValuePlus;
        public float NodeAbilityValueMult;

        public ChefsEchoDataFileData(string[] fields)
        {
            NodeID = int.Parse(fields[0]);
            Grade = (NodeGrade)Enum.Parse(typeof(NodeGrade), fields[1]);
            NodeName = fields[2];
            EnName = (NodeEnName)Enum.Parse(typeof(NodeEnName), fields[3]);
            NodeDescription = fields[4];
            NodeCurrency = int.Parse(fields[5]);
            NodeActiveCondition = int.Parse(fields[6]);
            Ability = (NodeAbility)Enum.Parse(typeof(NodeAbility), fields[7]);
            NodeAbilityValuePlus = float.Parse(fields[8]);
            NodeAbilityValueMult = float.Parse(fields[9]);
        }
    }
}