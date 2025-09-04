using System;
using System.Collections.Generic;

namespace SDW
{
    public struct CookDataFileData
    {
        public int CookID;
        public CookEnName CookEnName;
        public string CookName;
        public List<string> CookDescription;
        public List<int> IngredientIDs;
        public float ChaAttack;
        public int ChaArmor;
        public float ChaHP;
        public float ChaMP;
        public float MonAttack;
        public float MonArmor;
        public FoodTarget FoodTarget;
        public int FoodDuration;
        public bool ChaBarrier;
        public float MonBreakGage;

        public CookDataFileData(string[] fields)
        {
            CookID = int.Parse(fields[0]);
            CookEnName = (CookEnName)Enum.Parse(typeof(CookEnName), fields[1]);
            CookName = fields[2];

            CookDescription = new List<string>();
            CookDescription.Add(fields[3]);
            CookDescription.Add(fields[4]);

            IngredientIDs = new List<int>();
            IngredientIDs.Add(int.Parse(fields[5]));
            IngredientIDs.Add(int.Parse(fields[6]));
            int id = int.Parse(fields[7]);
            if (id != -1)
                IngredientIDs.Add(id);

            ChaAttack = float.Parse(fields[8]);
            ChaArmor = int.Parse(fields[9]);
            ChaHP = float.Parse(fields[10]);
            ChaMP = float.Parse(fields[11]);
            MonAttack = float.Parse(fields[12]);
            MonArmor = float.Parse(fields[13]);
            FoodTarget = (FoodTarget)Enum.Parse(typeof(FoodTarget), fields[14]);
            FoodDuration = int.Parse(fields[15]);
            ChaBarrier = bool.Parse(fields[16]);
            MonBreakGage = float.Parse(fields[17]);
        }
    }
}