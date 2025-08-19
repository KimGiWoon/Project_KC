using System;

namespace SDW
{
    public struct MonsterAnimationFileData
    {
        public int MonAnimID;
        public AnimationType MonAnimType;
        public string MonName;
        public string Description;

        /// <summary>
        /// MonsterAnimationFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public MonsterAnimationFileData(string[] fields)
        {
            MonAnimID = int.Parse(fields[0]);
            MonAnimType = (AnimationType)Enum.Parse(typeof(AnimationType), fields[1]);
            MonName = fields[2];
            Description = fields[3];
        }
    }
}