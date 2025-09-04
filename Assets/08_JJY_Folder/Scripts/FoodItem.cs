using System;

namespace JJY
{
    [Serializable]
    public class FoodItem
    {
        public RecipeData recipe;    // ScriptableObject 참조
        public bool isNew;           // 인벤토리에서 'new' 표시 여부

        public FoodItem(RecipeData recipe, bool isNew = true)
        {
            this.recipe = recipe;
            this.isNew = isNew;
        }
    }
}
