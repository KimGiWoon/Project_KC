using System;

namespace SDW
{
    public struct CharacterAnimationFileData
    {
        public int ChaAnimID;
        public AnimationType ChaAnimType;
        public string ChaName;
        public string Description;

        public CharacterAnimationFileData(string[] fields)
        {
            ChaAnimID = int.Parse(fields[0]);
            ChaAnimType = (AnimationType)Enum.Parse(typeof(AnimationType), fields[1]);
            ChaName = fields[2];
            Description = fields[3];
        }
    }
}