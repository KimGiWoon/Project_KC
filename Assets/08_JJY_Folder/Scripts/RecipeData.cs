using System;
using UnityEngine;

namespace JJY
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RecipeData")]
    public class RecipeData : ScriptableObject
    {
        public string recipeName;                  // 레시피 이름 (인스펙터 표시)
        public Ingredient[] requiredIngredients;   // 레시피를 구성하는 재료들
        public Sprite image;                       // TODO : Addressable
        public string description;                 // 설명 (CSV?)
        public string description2;                // 설명 (CSV?)
        // 이 아이템의 효과(= description2)를 전투중 적용 가능하게

        // 편의용: requiredIngredients -> mask 자동 저장
        [HideInInspector] public Ingredient mask;

        // 에디터에서 배열 바꿀 때마다 mask를 갱신
        void OnValidate()
        {
            Ingredient m = Ingredient.None;
            if (requiredIngredients != null)
            {
                foreach (var ing in requiredIngredients) m |= ing;
            }
            mask = m;
        }
    }
}
