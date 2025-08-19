using System;

namespace SDW
{
    public struct CharacterAnimationFileData
    {
        public int ChaAnimID;
        public AnimationType ChaAnimType;
        public string ChaName;
        public string Description;

        /// <summary>
        /// CharacterAnimationFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterAnimationFileData(string[] fields)
        {
            // 초기화
            ChaAnimID = int.Parse(fields[0]);
            ChaAnimType = (AnimationType)Enum.Parse(typeof(AnimationType), fields[1]);
            ChaName = fields[2];
            Description = fields[3];
        }
    }
}