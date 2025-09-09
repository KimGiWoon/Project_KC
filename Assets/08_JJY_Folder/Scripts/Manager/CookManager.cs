using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

namespace JJY
{
    // TODO : GameManager 연결
    public class CookManager : MonoBehaviour
    {
        public static CookManager Instance { get; private set; }

        // --- 데이터 구조 ---
        private Dictionary<Ingredient, RecipeData> recipes = new Dictionary<Ingredient, RecipeData>(); // 레시피 사전(조합마스크 -> 데이터)
        private Ingredient selected = Ingredient.None; // 현재 선택된 재료들의 비트마스크
        private int selectedCount = 0;
        private List<InventoryItem> _playerFoodInventory = new List<InventoryItem>(); // 플레이어 음식 인벤토리

        public List<InventoryItem> playerFoodInventory
        {
            get => _playerFoodInventory;
            private set => _playerFoodInventory = value;
        }

        private Dictionary<Ingredient, int> _playerIngredientInventory = new Dictionary<Ingredient, int>(); // 플레이어 재료 실제 보유량

        public Dictionary<Ingredient, int> playerIngredientInventory
        {
            get => _playerIngredientInventory;
            private set => _playerIngredientInventory = value;
        }

        private Dictionary<Ingredient, int> reservedIngredients = new Dictionary<Ingredient, int>(); // 플레이어 재료 보유량 표시 UI

        // --- Inspector에서 연결할 것들 ---
        [Header("Prefabs & Parents")]
        [SerializeField]
        private GameObject ingredientButtonPrefab; // 인벤토리 버튼 프리팹 (Button + Image + TMP Text)

        [Header("Recipe / Slots")]
        [SerializeField] private List<RecipeData> recipeSO;

        // [Header("Icons")]
        // [SerializeField] List<Sprite> ingredientSprites; // 인덱스 기반 재료 아이콘(ingredientIndexMap 순서와 동일)

        [Header("Ingredient Data base")]
        [SerializeField]
        private IngredientDatabase ingredientDatabase;
        private Dictionary<Ingredient, IngredientData> ingredientMap;

        // ingredientIndexMap: UI 인덱스 ↔ Ingredient enum 매핑(고정 순서)
        private Ingredient[] ingredientIndexMap =
        {
            Ingredient.마늘, Ingredient.기름, Ingredient.물, Ingredient.빵, Ingredient.두부,
            Ingredient.면, Ingredient.허브, Ingredient.고기, Ingredient.양파, Ingredient.감자,
            Ingredient.우유, Ingredient.고추, Ingredient.밥, Ingredient.채소, Ingredient.버터
        };

        // --- 풀링 관련 컬렉션 ---
        private Queue<GameObject> pool = new Queue<GameObject>(); // 비활성화된(재사용 가능한) 버튼 풀
        private List<GameObject> activeButtons = new List<GameObject>(); // 현재 활성화된 버튼들 추적
        private Dictionary<Ingredient, GameObject> buttonByIngredient = new Dictionary<Ingredient, GameObject>(); // 재료 -> 버튼 매핑

        [SerializeField] private CookingUI _cookingUI;

        // ---------------------
        private void Awake()
        {
            // 싱글톤 초기화: 이미 인스턴스가 있으면 자신을 파괴
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            StartCoroutine(DelayedInit());
        }

        private IEnumerator DelayedInit()
        {
            yield return new WaitForSeconds(1f);

            InitIngredients(); // 재료 초기화
            InitRecipes(); // 레시피 데이터 초기화
#if UNITY_EDITOR
            InitDummyInventory(); // (테스트) 플레이어 인벤토리 더미 채우기
#endif
            PrewarmPool(8); // 버튼 풀을 미리 만들어둠 (초기화 성능을 위해)
        }

        private void OnEnable()
        {
            // 초기 UI 갱신
            // RefreshInventoryUI(); // 하단 인벤토리 UI 생성/갱신
            UpdateRecipeSlotsUI(); // 상단 슬롯 갱신(선택된 항목 반영)
            UpdateResultButton(); // result 버튼 활성화 여부 반영
        }

        // ---------------------
        // 재료 매핑
        private void InitIngredients()
        {
            ingredientMap = new Dictionary<Ingredient, IngredientData>();
            if (ingredientDatabase == null)
            {
                Debug.LogWarning("[CookManager] ingredientDatabase 미설정");
                return;
            }
            foreach (var d in ingredientDatabase.ingredients)
            {
                if (d == null) continue;
                if (!ingredientMap.ContainsKey(d.ingredient)) ingredientMap[d.ingredient] = d;
                else Debug.LogWarning($"중복 IngredientData: {d.ingredient}");
            }
        }
        // 레시피/데이터 초기화
        private void InitRecipes()
        {
            recipes.Clear();
            // recipes.Add(Ingredient.마늘 | Ingredient.기름, new RecipeData("구운 마늘 조각"));
            // recipes.Add(Ingredient.마늘 | Ingredient.물, new RecipeData("마늘 즙"));
            // recipes.Add(Ingredient.마늘 | Ingredient.빵, new RecipeData("마늘 빵"));
            // recipes.Add(Ingredient.마늘 | Ingredient.면 | Ingredient.허브, new RecipeData("알리오 올리오"));
            // recipes.Add(Ingredient.마늘 | Ingredient.고기 | Ingredient.양파, new RecipeData("마늘 돼지 볶음"));
            // recipes.Add(Ingredient.마늘 | Ingredient.감자 | Ingredient.우유, new RecipeData("마늘 스프"));
            // recipes.Add(Ingredient.마늘 | Ingredient.고추 | Ingredient.밥, new RecipeData("마늘 볶음밥"));
            // recipes.Add(Ingredient.고기 | Ingredient.버터 | Ingredient.허브, new RecipeData("고기 스테이크"));
            // recipes.Add(Ingredient.면 | Ingredient.우유 | Ingredient.버터, new RecipeData("크림 파스타"));
            // recipes.Add(Ingredient.두부 | Ingredient.채소, new RecipeData("두부 채소 볶음"));
            if (recipeSO == null) return;
            foreach (var so in recipeSO)
            {
                if (so == null) continue;
                // so.OnValidate() 로 만든 so.mask 사용
                recipes[so.mask] = so;
            }
        }

#if UNITY_EDITOR
        private void InitDummyInventory()
        {
            playerIngredientInventory.Clear(); // 기존 데이터 제거
            // 테스트용으로 각 재료를 3개씩 채움 (실무: PlayerData에서 불러오는 부분)
            foreach (Ingredient ing in Enum.GetValues(typeof(Ingredient)))
            {
                if (ing == Ingredient.None) continue; // None 항목은 건너뜀
                AddIngredient(ing, 3);
            }
        }
#endif

        #region 오브젝트 풀

        // 풀링: 미리 버튼을 만들어두는 초기화
        private void PrewarmPool(int count)
        {
            // prefab 또는 parent가 없으면 동작 안 함
            if (ingredientButtonPrefab == null) return;
            for (int i = 0; i < count; i++)
            {
                // var go = Instantiate(ingredientButtonPrefab, inventoryContent); // 프리팹 인스턴스화
                var go = Instantiate(ingredientButtonPrefab); // 프리팹 인스턴스화
                go.SetActive(false); // 비활성화 상태로 보관
                pool.Enqueue(go); // 풀에 추가
            }
        }

        // 풀에서 버튼을 꺼내 활성화(없으면 새로 생성)
        private GameObject GetButtonFromPool()
        {
            var go = pool.Count > 0 ? pool.Dequeue() : Instantiate(ingredientButtonPrefab); // 풀에서 꺼내거나 생성
            // go.transform.SetParent(inventoryContent, false);
            go.transform.SetAsLastSibling();

            go.SetActive(false); // 깜빡임 현상
            activeButtons.Add(go); // 활성 리스트에 추가
            return go; // 반환
        }

        // 모든 활성 버튼을 풀로 되돌림
        private void ReturnAllButtonsToPool()
        {
            for (int i = activeButtons.Count - 1; i >= 0; i--) // 뒤에서부터 순회
            {
                var go = activeButtons[i]; // 활성 버튼 가져오기
                if (go == null) continue; // null 체크
                go.SetActive(false);
                var btn = go.GetComponent<Button>(); // 버튼 컴포넌트 참조
                if (btn != null) btn.onClick.RemoveAllListeners(); // 리스너 제거
                pool.Enqueue(go); // 풀에 반환
            }
            activeButtons.Clear(); // 활성 리스트 비움
            buttonByIngredient.Clear(); // 매핑 초기화
        }

        #endregion

        #region 요리 인벤토리

        // ---------------------
        // 인벤토리 UI 생성/갱신: 보유한 재료만 표시
        private int GetDisplayCount(Ingredient ing)
        {
            playerIngredientInventory.TryGetValue(ing, out int actual);
            reservedIngredients.TryGetValue(ing, out int reserved);
            int display = actual - reserved;
            if (display < 0) display = 0;
            return display;
        }
        // 인벤토리 UI 생성/갱신 (보유한 재료만 표시)
        public void RefreshInventoryUI()
        {
            if (ingredientButtonPrefab == null) return; // 필요 요소 체크

            ReturnAllButtonsToPool(); // 기존 버튼 모두 반환(재사용 준비)
            StartCoroutine(SetContentsInit());
        }

        private IEnumerator SetContentsInit()
        {
            yield return null;
            var ingredientsList = new List<GameObject>();

            // ingredientIndexMap 순서대로 보유 수량이 있는 것만 버튼 생성
            for (int i = 0; i < ingredientIndexMap.Length; i++)
            {
                var ing = ingredientIndexMap[i]; // 해당 인덱스의 재료
                playerIngredientInventory.TryGetValue(ing, out int count); // 보유 수량 조회
                if (count <= 0 && !reservedIngredients.ContainsKey(ing)) continue; // 0 이면 표시하지 않음

                var go = GetButtonFromPool(); // 풀에서 버튼 획득
                var btn = go.GetComponent<Button>(); // 버튼 컴포넌트
                var img = go.GetComponent<Image>(); // 아이콘용 이미지
                var txt = go.GetComponentInChildren<TextMeshProUGUI>(); // 카운트 텍스트(TMP)

                IngredientData data = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(ing, out data);
                // 아이콘 세팅: ingredientSprites가 할당되어 있다면 매핑된 스프라이트 사용
                if (data != null && data.icon != null)
                {
                    img.sprite = data.icon;
                }

                // 카운트 텍스트 세팅: 0이면 "0", 1 이상이면 숫자 표기
                int displayCount = GetDisplayCount(ing);
                if (txt != null) txt.text = displayCount > 1 ? displayCount.ToString() : displayCount == 1 ? "1" : "0";

                // 클릭 이벤트 바인딩: 로컬 변수 캡처로 안전하게 처리
                var ingLocal = ing;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnInventoryButtonClicked(ingLocal));

                // 버튼 활성 여부: 이미 상단에 올려져(selected에 포함) 있다면 비활성화
                bool isReserved = reservedIngredients.ContainsKey(ing) && reservedIngredients[ing] > 0;
                btn.interactable = !isReserved;
                // btn.interactable = (selected & ing) == 0;

                // 재료 -> 버튼 매핑 저장 (상태 변경시 빠르게 찾아 쓸 용도)
                buttonByIngredient[ing] = go;

                ingredientsList.Add(go);
                // go.SetActive(true); // 여기서 보이게 설정해줘야 깜빡임 없어짐.
            }

            _cookingUI.SetContentsInit(ingredientsList);
        }

        // InventoryUI 쪽
        public void UpdateSingleIngredientUI(Ingredient ing)
        {
            // 버튼이 이미 존재하는지 확인
            buttonByIngredient.TryGetValue(ing, out var go);

            // 버튼 갱신
            var btn = go.GetComponent<Button>();
            var img = go.GetComponent<Image>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();

            // 아이콘 세팅
            if (ingredientMap.TryGetValue(ing, out var data) && data.icon != null)
                img.sprite = data.icon;

            // 카운트 세팅
            int displayCount = GetDisplayCount(ing);
            txt.text = displayCount.ToString();

            // 예약 여부
            bool isReserved = reservedIngredients.ContainsKey(ing) && reservedIngredients[ing] > 0;
            btn.interactable = !isReserved;

            go.SetActive(true);
        }

        // ---------------------
        // 인벤토리 버튼 클릭 처리 (토글: 예약 추가/해제)
        private void OnInventoryButtonClicked(Ingredient ing)
        {
            // 이미 예약되어 있다면 예약 해제(복구)
            // if (reservedIngredients.ContainsKey(ing) && reservedIngredients[ing] > 0)
            // {
            //     ReleaseReservation(ing, 1); // 내부에서 UI 갱신
            // }
            // else
            // {
            // 예약 시 실제 재고의 '가용 수량' 확인
            int available = GetDisplayCount(ing); // actual - reserved
            if (available <= 0) return;

            // 예약 추가 (1개)
            ReserveIngredient(ing, 1); // 내부에서 UI 갱신 및 selected 처리
            // }
        }

        // 예약 추가: UI 텍스트이 바로 차감되는 효과 (실제 playerIngredientInventory는 아직 줄지 않음)
        private void ReserveIngredient(Ingredient ing, int count = 1)
        {
            if (count <= 0 || selectedCount >= _cookingUI.GetRecipeSlotsCout()) return;

            // reservedIngredients 갱신
            if (!reservedIngredients.ContainsKey(ing)) reservedIngredients[ing] = 0;
            reservedIngredients[ing] += count;

            // selected 비트 추가 (중복 예약 방지 정책에 따라 1개만 허용하면 이 부분을 수정)
            selected |= ing;
            selectedCount++;

            // 예약 시 버튼 비활성화(중복 선택 방지)
            if (buttonByIngredient.TryGetValue(ing, out var go))
            {
                var b = go.GetComponent<Button>();
                if (b != null) b.interactable = false;
            }

            // UI 갱신 (한 번만 호출되게)
            UpdateRecipeSlotsUI();
            UpdateResultButton();
            // RefreshInventoryUI();
            UpdateSingleIngredientUI(ing);
        }

        // ---------------------
        // 상단 레시피 슬롯 UI 갱신: selected 비트에 따라 왼쪽부터 채움
        private void UpdateRecipeSlotsUI()
        {
            // 모든 슬롯 초기화(비활성화 + 이미지 제거)
            _cookingUI.InitRecipeSlots();

            // 선택된 재료를 배열로 얻어서 슬롯에 채움
            var selectedList = GetIngredientsFromMask(selected);
            var selectedSpriteList = new List<Sprite>();

            for (int i = 0; i < selectedList.Length && i < _cookingUI.GetRecipeSlotsCout(); i++)
            {
                IngredientData data = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(selectedList[i], out data);
                if (data != null && data.icon != null)
                {
                    selectedSpriteList.Add(data.icon);
                }

                // int idx = GetIndexByIngredient(selectedList[i]); // 재료 인덱스 찾기
                // if (img != null && ingredientSprites != null && idx >= 0 && idx < ingredientSprites.Count)
                //     img.sprite = ingredientSprites[idx];       // 슬롯에 재료 아이콘 세팅
            }

            _cookingUI.SetRecipeSlots(selectedSpriteList);
        }

        // ---------------------
        // 선택된 조합이 레시피와 정확히 일치하면 Cook 버튼을 활성/세팅
        public void UpdateResultButton()
        {
            if (recipes.TryGetValue(selected, out var dish)) // 레시피 일치 확인
                _cookingUI.SetFoodInfoButton(dish.image, true);
            else
                _cookingUI.SetFoodInfoButton(null, false);
        }

        #endregion

        #region 버튼 연결 함수

        // 요리 시도: 레시피가 일치하면 재료 소모 및 UI 갱신
        public void SuccessCook()
        {
            if (!recipes.TryGetValue(selected, out var dish))
            {
                Debug.Log("요리 불가: 레시피 불일치"); // 일치하지 않으면 종료
                return;
            }

            // selected에 포함된 재료들을 하나씩 소모
            // foreach (Ingredient ing in GetIngredientsFromMask(selected))
            // {
            //     SubtractIngredient(ing, 1); // 수량 차감
            // }

            // 예약된 재료들을 실제 재고에서 차감
            // (한 번에 적용 -> RefreshInventoryUI 한 번만 호출)
            foreach (var kv in new Dictionary<Ingredient, int>(reservedIngredients))
            {
                var ing = kv.Key;
                int reserveCount = kv.Value;
                // 실제 차감
                if (playerIngredientInventory.ContainsKey(ing))
                {
                    SubtractIngredient(ing, reserveCount);
                }
            }

            AddFood(dish);
            Debug.Log($"{dish.recipeName} 완성! : {playerFoodInventory.Count}개 음식 보유중");

            reservedIngredients.Clear();
            selectedCount = 0;
            selected = Ingredient.None; // 선택 초기화
            RefreshInventoryUI(); // 인벤토리 UI 갱신(사라진 아이템 반영)
            UpdateRecipeSlotsUI(); // 슬롯 비우기
            UpdateResultButton(); // 결과 버튼 숨기기

            // TODO : 완성 이펙트 연출
        }

        // 리셋: 예약 상태 복구 (예약된 수량은 실제에서 차감되지 않았으니 단순 초기화)
        public void ResetIngredients()
        {
            // 예약 목록을 초기화하면 UI가 복구됨
            reservedIngredients.Clear();
            selected = Ingredient.None;
            selectedCount = 0;

            // 모든 버튼 활성화
            foreach (var kv in buttonByIngredient)
            {
                var b = kv.Value.GetComponent<Button>();
                if (b != null) b.interactable = true;
            }

            UpdateRecipeSlotsUI();
            UpdateResultButton();
            RefreshInventoryUI();
        }

        // 상세설명 Image, Text 변경
        public PopupDescription InitDescription()
        {
            Sprite sprite = null;
            string foodName = "";
            string foodEffect = "";
            string foodDescription = "";

            if (recipes.TryGetValue(selected, out var dish))
            {
                sprite = dish.image; // TODO : Addressable
                foodName = dish.recipeName;
                foodEffect = dish.description; // TODO : CSV
                foodDescription = dish.effect;
            }

            return new PopupDescription
            {
                Sprite = sprite,
                Name = foodName,
                Effect = foodEffect,
                Description = foodDescription
            };
        }

        //! 의사코드
        //# CookManager -> 보유하고 있는 재료를 Contents에 추가
        //# CookingUI에서 Contents 내 재료 선택 -> CookManager
        //@ 버튼 클릭 시 - 어느 버튼이 눌려졌는지 파악해야 함
        //# CookManager -> 상단에 추가(자리가 있을 때)

        #endregion

        #region 인벤토리 수량 변화 함수

        public void AddIngredient(Ingredient ing, int count = 1)
        {
            if (!playerIngredientInventory.ContainsKey(ing)) playerIngredientInventory[ing] = 0; // 없으면 0으로 초기화
            playerIngredientInventory[ing] += count; // 수량 증가
            // RefreshInventoryUI(); // UI 갱신
        }
        public void SubtractIngredient(Ingredient ing, int count = 1)
        {
            if (!playerIngredientInventory.ContainsKey(ing)) return; // 없으면 무시
            playerIngredientInventory[ing] -= count; // 수량 차감
            if (playerIngredientInventory[ing] < 0) playerIngredientInventory[ing] = 0; // 음수 방지
            // RefreshInventoryUI(); // UI 갱신
        }

        public void AddFood(RecipeData dish)
        {
            var item = new InventoryItem(dish);
            _playerFoodInventory.Add(item);
        }
        public void SubtractFood(InventoryItem item)
        {
            _playerFoodInventory.Remove(item);
        }

        #endregion

        #region 헬퍼

        // 마스크에서 포함된 재료 리스트 반환
        private Ingredient[] GetIngredientsFromMask(Ingredient mask)
        {
            var list = new List<Ingredient>(); // 결과 리스트 생성
            foreach (Ingredient ing in Enum.GetValues(typeof(Ingredient)))
            {
                if (ing == Ingredient.None) continue; // None 패스
                if ((mask & ing) != 0) list.Add(ing); // 해당 비트가 켜져 있으면 추가
            }
            return list.ToArray(); // 배열로 반환
        }
        // // ingredientIndexMap에서 재료의 인덱스 찾기
        // int GetIndexByIngredient(Ingredient ing)
        // {
        //     for (int i = 0; i < ingredientIndexMap.Length; i++)
        //         if (ingredientIndexMap[i] == ing) return i;
        //     return -1; // 못찾으면 -1 반환
        // }

        #endregion
    }
}