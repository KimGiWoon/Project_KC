using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJY
{
    // TODO : GameManager와 연결
    public class CookManager : MonoBehaviour
    {
        public static CookManager Instance { get; private set; }
        Dictionary<Ingredient, string> recipes = new Dictionary<Ingredient, string>();
        Ingredient selected;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);

            InitRecipes();
        }

        /// <summary>
        /// Reciepes Dictionary 초기화 작업
        /// </summary>
        void InitRecipes()
        {
            // string 부분도 CSV??
            recipes.Add(Ingredient.마늘 | Ingredient.기름, "구운 마늘 조각");
            recipes.Add(Ingredient.마늘 | Ingredient.물, "마늘 즙");
            recipes.Add(Ingredient.마늘 | Ingredient.빵, "마늘 빵");
            recipes.Add(Ingredient.마늘 | Ingredient.두부, "마늘 두부 조림");
            recipes.Add(Ingredient.마늘 | Ingredient.면 | Ingredient.허브, "알리오 올리오");
            recipes.Add(Ingredient.마늘 | Ingredient.고기 | Ingredient.양파, "마늘 돼지 볶음");
            recipes.Add(Ingredient.마늘 | Ingredient.감자 | Ingredient.우유, "마늘 스프");
            recipes.Add(Ingredient.마늘 | Ingredient.고추 | Ingredient.밥, "마늘 볶음밥");
            recipes.Add(Ingredient.고기 | Ingredient.버터 | Ingredient.허브, "고기 스테이크");
            recipes.Add(Ingredient.면 | Ingredient.우유 | Ingredient.버터, "크림 파스타");
            recipes.Add(Ingredient.밥 | Ingredient.채소, "야채 볶음밥"); // 채소 볶음밥??
            recipes.Add(Ingredient.두부 | Ingredient.채소, "두부 채소 볶음밥");
        }

        void TestCheckIngredient()
        {
            // 선택된 재료를 저장.
            selected = Ingredient.마늘 | Ingredient.기름;

            if (recipes.TryGetValue(selected, out string dish))
            {
                // 레시피에 맞는 요리 완성본 표출.
                Debug.Log("완성된 요리: " + dish);
            }
        }
        /// <summary>
        /// 재료를 레시피에 등록하는 버튼
        /// </summary>
        void AddIngredientToRecipe()
        {
            // TODO : 인벤토리 아이템 클릭 -> 레시피에 올라감.
        }
        /// <summary>
        /// 요리 완성
        /// </summary>
        void SuccesCook()
        {
            // TODO : StageData의 Player Inventory List에 저장
        }
        /// <summary>
        /// 레시피 리셋 버튼
        /// </summary>
        void ResetIngredients()
        {
            selected = 0;
            // 레시피에 올려진 아이템들 전부 인벤토리로 복귀
        }
        /// <summary>
        /// 전투 중, 아이템을 클릭해서 사용.
        /// </summary>
        void UseCookedItem()
        {
            // 버튼 클릭 시, 클릭된 아이템 제거 + 고유한 효과 발동. (엑티브 스킬 추가 예정있음.)
        }
    }
}
