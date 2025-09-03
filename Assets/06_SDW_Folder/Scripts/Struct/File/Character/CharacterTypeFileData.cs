using System;

namespace SDW
{
    [Serializable]
    public struct CharacterTypeFileData
    {
        public CharacterRole ChaRole;
        public int ChaAtkIsMelee;
        public float ChaAccuracy;
        public float ChaAvoid;
        public float ChaCrit;
        public float ChaCritDmg;
        public float ChaReg;
        public float ChaMoveSpeed;

        /// <summary>
        /// CharacterTypeFileData를 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public CharacterTypeFileData(string[] fields)
        {
            ChaRole = (CharacterRole)Enum.Parse(typeof(CharacterRole), fields[0]);
            ChaAtkIsMelee = int.Parse(fields[1]);
            ChaAccuracy = float.Parse(fields[2]);
            ChaAvoid = float.Parse(fields[3]);
            ChaCrit = float.Parse(fields[4]);
            ChaCritDmg = float.Parse(fields[5]);
            ChaReg = float.Parse(fields[6]);
            ChaMoveSpeed = float.Parse(fields[7]);
        }
    }
}