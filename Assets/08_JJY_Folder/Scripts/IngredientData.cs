using UnityEngine;

namespace JJY
{
    [CreateAssetMenu(menuName = "ScriptableObjects/IngredientData/IngredientData")]
    public class IngredientData : ScriptableObject
    {
        public Ingredient ingredient;
        public Sprite icon;
        public int cost = 100;
        public string name;
        // [TextArea] public string description;
    }
}
