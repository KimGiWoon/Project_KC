using System;

namespace SDW
{
    public struct MonsterAnimationFileData
    {
        public int MonAnimID;
        public AnimationType MonAnimType;
        public string MonName;
        public string Description;

        public MonsterAnimationFileData(string[] fields)
        {
            MonAnimID = int.Parse(fields[0]);
            MonAnimType = (AnimationType)Enum.Parse(typeof(AnimationType), fields[1]);
            MonName = fields[2];
            Description = fields[3];
        }
    }
}