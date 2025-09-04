using System;
using KSH;

namespace JJY
{
    [Serializable]
    public class InventoryItem
    {
        public RecipeData recipe;    // ScriptableObject 참조
        public Relic relic;
        public bool isNew;           // 인벤토리에서 'new' 표시 여부

        public InventoryItem(RecipeData recipe, bool isNew = true)
        {
            this.recipe = recipe;
            this.isNew = isNew;
        }
        public InventoryItem(Relic relic, bool isNew = true)
        {
            this.relic = relic;
            this.isNew = isNew;
        }
    }
}
