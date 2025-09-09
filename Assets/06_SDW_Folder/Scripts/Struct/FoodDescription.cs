using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public struct FoodDescription
    {
        public Sprite Sprite;
        public string FoodName;
        public string FoodEffect;
        public string Description;

        public FoodDescription(Sprite sprite, string foodName, string foodEffect, string foodDescription)
        {
            Sprite = sprite;
            FoodName = foodName;
            FoodEffect = foodEffect;
            Description = foodDescription;
        }
    }
}