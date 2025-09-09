using System.Collections.Generic;
using UnityEngine;

namespace JJY
{
    [CreateAssetMenu(menuName = "ScriptableObjects/IngredientData/IngredientDatabase")]
    public class IngredientDatabase : ScriptableObject
    {
        public List<IngredientData> ingredients = new List<IngredientData>();

        public IngredientData Get(Ingredient ing)
        {
            if (ingredients == null) return null;
            for (int i = 0; i < ingredients.Count; i++)
            {
                var d = ingredients[i];
                if (d != null && d.ingredient == ing) return d;
            }
            return null;
        }
    }
}