using System;

namespace SDW
{
    public struct IngredientDataFileData
    {
        public int IngredientID;
        public IngredientEnName IngredientEnName;
        public string IngredientName;
        public int IngredientPrice;
        public string IngredientDescription;

        /// <summary>
        /// IngredientDataFileData 초기화
        /// </summary>
        /// <param name="fields">초기화 데이터</param>
        public IngredientDataFileData(string[] fields)
        {
            IngredientID = int.Parse(fields[0]);
            IngredientEnName = (IngredientEnName)Enum.Parse(typeof(IngredientEnName), fields[1]);
            IngredientName = fields[2];
            IngredientPrice = int.Parse(fields[3]);
            IngredientDescription = fields[4];
        }
    }
}