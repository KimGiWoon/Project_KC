using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    public class CookManager : MonoBehaviour
    {
        public static CookManager Instance { get; private set; }

        // --- 데이터 구조 ---
        Dictionary<Ingredient, RecipeData> recipes = new Dictionary<Ingredient, RecipeData>(); // 레시피 사전(조합마스크 -> 데이터)
        Ingredient selected = Ingredient.None; // 현재 선택된 재료들의 비트마스크
        int selectedCount = 0;
        Dictionary<Ingredient, int> playerIngredientInventory = new Dictionary<Ingredient, int>(); // 플레이어 재료 실제 보유량
        Dictionary<Ingredient, int> reservedIngredients = new Dictionary<Ingredient, int>(); // 플레이어 재료  보유량 표시 UI

        // --- Inspector에서 연결할 것들 ---
        [Header("Prefabs & Parents")]
        [SerializeField] GameObject ingredientButtonPrefab; // 인벤토리 버튼 프리팹 (Button + Image + TMP Text)
        [SerializeField] Transform inventoryContent;        // 동적 버튼이 붙을 부모(ScrollView Content 등)

        [Header("Recipe / Slots")]
        [SerializeField] List<Image> recipeSlots; // 상단에 고정으로 배치될 레시피 슬롯
        [SerializeField] Button resultButton;      // 중앙 완성(요리) 버튼 (이미지+텍스트 포함)
        [SerializeField] List<RecipeData> recipeSO;

        [Header("Icons")]
        [SerializeField] List<Sprite> ingredientSprites; // 인덱스 기반 재료 아이콘(ingredientIndexMap 순서와 동일)

        [Header("Reset / Cook Button")]
        [SerializeField] Button resetBtn;
        [SerializeField] Button cookBtn;

        [Header("Description")]
        [SerializeField] Image foodImage;
        [SerializeField] TextMeshProUGUI foodName;
        [SerializeField] TextMeshProUGUI foodDescription;
        [SerializeField] TextMeshProUGUI foodDescription2;

        // ingredientIndexMap: UI 인덱스 ↔ Ingredient enum 매핑(고정 순서)
        Ingredient[] ingredientIndexMap =
        {
            Ingredient.마늘, Ingredient.기름, Ingredient.물, Ingredient.빵, Ingredient.두부,
            Ingredient.면, Ingredient.허브, Ingredient.고기, Ingredient.양파, Ingredient.감자,
            Ingredient.우유, Ingredient.고추, Ingredient.밥, Ingredient.채소, Ingredient.버터
        };

        // --- 풀링 관련 컬렉션 ---
        Queue<GameObject> pool = new Queue<GameObject>();               // 비활성화된(재사용 가능한) 버튼 풀
        List<GameObject> activeButtons = new List<GameObject>();        // 현재 활성화된 버튼들 추적
        Dictionary<Ingredient, GameObject> buttonByIngredient = new Dictionary<Ingredient, GameObject>(); // 재료 -> 버튼 매핑

        // ---------------------
        void Awake()
        {
            // 싱글톤 초기화: 이미 인스턴스가 있으면 자신을 파괴
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            InitRecipes();         // 레시피 데이터 초기화
            InitDummyInventory();  // (테스트) 플레이어 인벤토리 더미 채우기
            PrewarmPool(6);        // 버튼 풀을 미리 만들어둠 (초기화 성능을 위해)

            // result/cook/reset 버튼 기본 바인딩 설정 (Inspector에서 연결되어야 함)
            if (resultButton != null)
            {
                resultButton.onClick.RemoveAllListeners();             // 기존 리스너 제거
                // resultButton.onClick.AddListener(() => SuccessCook()); // 누르면 요리 시도
                resultButton.onClick.AddListener(() => InitDescription()); // 요리 상세정보 표시
                resultButton.gameObject.SetActive(false);              // 초기에는 비활성
            }

            // 초기 UI 갱신
            RefreshInventoryUI();    // 하단 인벤토리 UI 생성/갱신
            UpdateRecipeSlotsUI();   // 상단 슬롯 갱신(선택된 항목 반영)
            UpdateResultButton();    // result 버튼 활성화 여부 반영

            resetBtn.interactable = false;
            cookBtn.interactable = false;
        }

        // ---------------------
        // 레시피/데이터 초기화
        void InitRecipes()
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

        void InitDummyInventory()
        {
            playerIngredientInventory.Clear(); // 기존 데이터 제거
            // 테스트용으로 각 재료를 3개씩 채움 (실무: PlayerData에서 불러오는 부분)
            foreach (Ingredient ing in Enum.GetValues(typeof(Ingredient)))
            {
                if (ing == Ingredient.None) continue; // None 항목은 건너뜀
                playerIngredientInventory[ing] = 3;    // 모든 재료 3개로 설정 (테스트용)
            }
        }

        // ---------------------
        // 풀링: 미리 버튼을 만들어두는 초기화
        void PrewarmPool(int count)
        {
            // prefab 또는 parent가 없으면 동작 안 함
            if (ingredientButtonPrefab == null || inventoryContent == null) return;
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(ingredientButtonPrefab, inventoryContent); // 프리팹 인스턴스화
                go.SetActive(false);                                            // 비활성화 상태로 보관
                pool.Enqueue(go);                                                // 풀에 추가
            }
        }

        // 풀에서 버튼을 꺼내 활성화(없으면 새로 생성)
        GameObject GetButtonFromPool()
        {
            GameObject go = pool.Count > 0 ? pool.Dequeue() : Instantiate(ingredientButtonPrefab, inventoryContent); // 풀에서 꺼내거나 생성
            go.transform.SetParent(inventoryContent, false);
            go.transform.SetAsLastSibling();

            go.SetActive(false);                    // 깜빡임 현상 (수정중)
            activeButtons.Add(go);                  // 활성 리스트에 추가
            return go;                              // 반환
        }

        // 모든 활성 버튼을 풀로 되돌림
        void ReturnAllButtonsToPool()
        {
            for (int i = activeButtons.Count - 1; i >= 0; i--) // 뒤에서부터 순회
            {
                var go = activeButtons[i];                     // 활성 버튼 가져오기
                if (go == null) continue;                      // null 체크
                go.SetActive(false);
                var btn = go.GetComponent<Button>();           // 버튼 컴포넌트 참조
                if (btn != null) btn.onClick.RemoveAllListeners(); // 리스너 제거
                pool.Enqueue(go);                              // 풀에 반환
            }
            activeButtons.Clear();                              // 활성 리스트 비움
            buttonByIngredient.Clear();                         // 매핑 초기화
        }

        // ---------------------
        // 인벤토리 UI 생성/갱신: 보유한 재료만 표시
        int GetDisplayCount(Ingredient ing)
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
            if (ingredientButtonPrefab == null || inventoryContent == null) return; // 필요 요소 체크

            ReturnAllButtonsToPool(); // 기존 버튼 모두 반환(재사용 준비)

            // ingredientIndexMap 순서대로 보유 수량이 있는 것만 버튼 생성
            for (int i = 0; i < ingredientIndexMap.Length; i++)
            {
                Ingredient ing = ingredientIndexMap[i];                  // 해당 인덱스의 재료
                playerIngredientInventory.TryGetValue(ing, out int count); // 보유 수량 조회
                if (count <= 0 && !reservedIngredients.ContainsKey(ing)) continue; // 0 이면 표시하지 않음

                GameObject go = GetButtonFromPool();                      // 풀에서 버튼 획득
                go.SetActive(true);
                var btn = go.GetComponent<Button>();                      // 버튼 컴포넌트
                var img = go.GetComponent<Image>();                       // 아이콘용 이미지
                var txt = go.GetComponentInChildren<TextMeshProUGUI>();  // 카운트 텍스트(TMP)

                // 아이콘 세팅: ingredientSprites가 할당되어 있다면 매핑된 스프라이트 사용
                if (img != null && ingredientSprites != null && i < ingredientSprites.Count) img.sprite = ingredientSprites[i];

                // 카운트 텍스트 세팅: 0이면 "0", 1 이상이면 숫자 표기
                int displayCount = GetDisplayCount(ing);
                if (txt != null) txt.text = displayCount > 1 ? displayCount.ToString() : (displayCount == 1 ? "1" : "0");

                // 클릭 이벤트 바인딩: 로컬 변수 캡처로 안전하게 처리
                Ingredient ingLocal = ing;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnInventoryButtonClicked(ingLocal));

                // 버튼 활성 여부: 이미 상단에 올려져(selected에 포함) 있다면 비활성화
                bool isReserved = reservedIngredients.ContainsKey(ing) && reservedIngredients[ing] > 0;
                btn.interactable = !isReserved;     // 깜빡임 현상 (수정중)
                // btn.interactable = (selected & ing) == 0;

                // 재료 -> 버튼 매핑 저장 (상태 변경시 빠르게 찾아 쓸 용도)
                buttonByIngredient[ing] = go;
            }
        }

        // ---------------------
        // 인벤토리 버튼 클릭 처리 (토글: 예약 추가/해제)
        void OnInventoryButtonClicked(Ingredient ing)
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
        void ReserveIngredient(Ingredient ing, int count = 1)
        {
            if (count <= 0 || selectedCount >= recipeSlots.Count) return;

            // reservedIngredients 갱신
            if (!reservedIngredients.ContainsKey(ing)) reservedIngredients[ing] = 0;
            reservedIngredients[ing] += count;

            // selected 비트 추가 (중복 예약 방지 정책에 따라 1개만 허용하면 이 부분을 수정)
            selected |= ing;
            selectedCount++;

            // 예약 시 버튼 비활성화(중복 선택 방지)
            if (buttonByIngredient.TryGetValue(ing, out GameObject go))
            {
                var b = go.GetComponent<Button>();
                if (b != null) b.interactable = false;
            }

            // UI 갱신 (한 번만 호출되게)
            UpdateRecipeSlotsUI();
            UpdateResultButton();
            RefreshInventoryUI();
            resetBtn.interactable = true;
        }

        // 예약 해제(복구): UI에 표시된 숫자가 다시 올라옴 (실제 재고는 안건드림)
        // void ReleaseReservation(Ingredient ing, int count = 1)
        // {
        //     if (count <= 0) return;
        //     if (!reservedIngredients.ContainsKey(ing) || reservedIngredients[ing] <= 0) return;

        //     reservedIngredients[ing] -= count;
        //     if (reservedIngredients[ing] <= 0) reservedIngredients.Remove(ing);

        //     // 선택 비트 제거(만약 중복 예약을 허용하면 로직 수정 필요)
        //     selected &= ~ing;
        //     selectedCount = Math.Max(0, selectedCount - 1);

        //     // 버튼 재활성화
        //     if (buttonByIngredient.TryGetValue(ing, out GameObject go))
        //     {
        //         var b = go.GetComponent<Button>();
        //         if (b != null) b.interactable = true;
        //     }

        //     // UI 갱신
        //     UpdateRecipeSlotsUI();
        //     UpdateResultButton();
        //     RefreshInventoryUI();

        //     if (selectedCount == 0) resetBtn.interactable = false;
        // }

        // ---------------------
        // 상단 레시피 슬롯 UI 갱신: selected 비트에 따라 왼쪽부터 채움
        void UpdateRecipeSlotsUI()
        {
            // 모든 슬롯 초기화(비활성화 + 이미지 제거)
            foreach (var slot in recipeSlots)
            {
                slot.gameObject.SetActive(false);
                var im = slot.GetComponent<Image>();
                if (im != null) im.sprite = null;
            }

            // 선택된 재료를 배열로 얻어서 슬롯에 채움
            Ingredient[] selectedList = GetIngredientsFromMask(selected);
            for (int i = 0; i < selectedList.Length && i < recipeSlots.Count; i++)
            {
                var slot = recipeSlots[i];                     // 슬롯 버튼
                slot.gameObject.SetActive(true);               // 슬롯 활성화
                var im = slot.GetComponent<Image>();           // 슬롯 이미지
                int idx = GetIndexByIngredient(selectedList[i]); // 재료 인덱스 찾기
                if (im != null && ingredientSprites != null && idx >= 0 && idx < ingredientSprites.Count)
                    im.sprite = ingredientSprites[idx];       // 슬롯에 재료 아이콘 세팅
            }
        }

        // ---------------------
        // 선택된 조합이 레시피와 정확히 일치하면 Cook 버튼을 활성/세팅
        void UpdateResultButton()
        {
            if (recipes.TryGetValue(selected, out RecipeData dish))        // 레시피 일치 확인
            {
                cookBtn.interactable = true;
                resultButton.gameObject.SetActive(true);                   // 버튼 보이기
                var img = resultButton.GetComponent<Image>();              // 버튼 이미지
                if (img != null) img.sprite = dish.image;                  // TODO : Addressable
            }
            else
            {
                resultButton.gameObject.SetActive(false); // 일치하지 않으면 숨김
                cookBtn.interactable = false;
            }
        }

        // 레시피에 재료 추가(비트마스크에 OR)
        // public void AddIngredientToRecipe(Ingredient ing)
        // {
        //     if ((selected & ing) != 0 || selectedCount >= recipeSlots.Count) return; // 이미 있으면 무시, 3개까지만 재료 추가 버튼 가능
        //     selected |= ing;                   // 비트 추가
        //     selectedCount++;

        //     UpdateRecipeSlotsUI();             // UI 갱신
        //     UpdateResultButton();              // 결과 버튼 갱신
        //     resetBtn.interactable = true;
        // }

        // // 레시피에서 재료 제거(비트마스크에서 비트 제거)
        // public void RemoveIngredientFromRecipe(Ingredient ing)
        // {
        //     if ((selected & ing) == 0) return; // 없으면 무시
        //     selected &= ~ing;                  // 비트 제거

        //     // 관련 인벤토리 버튼이 있으면 재활성화
        //     // if (buttonByIngredient.TryGetValue(ing, out GameObject go))
        //     // {
        //     //     var b = go.GetComponent<Button>();
        //     //     if (b != null) b.interactable = true;
        //     // }

        //     UpdateRecipeSlotsUI();             // UI 갱신
        //     UpdateResultButton();              // 결과 버튼 갱신
        // }

        // ---------------------
        // 요리 시도: 레시피가 일치하면 재료 소모 및 UI 갱신
        public void SuccessCook()
        {
            if (!recipes.TryGetValue(selected, out RecipeData dish))
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
                Ingredient ing = kv.Key;
                int reserveCount = kv.Value;
                // 실제 차감
                if (playerIngredientInventory.ContainsKey(ing))
                {
                    playerIngredientInventory[ing] -= reserveCount;
                    if (playerIngredientInventory[ing] < 0) playerIngredientInventory[ing] = 0;
                }
            }

            Debug.Log($"요리 완성: {dish.name}, 완성품을 플레이어 인벤토리에 추가해야 함");

            reservedIngredients.Clear();
            selectedCount = 0;
            resetBtn.interactable = false;
            selected = Ingredient.None;     // 선택 초기화
            RefreshInventoryUI();           // 인벤토리 UI 갱신(사라진 아이템 반영)
            UpdateRecipeSlotsUI();          // 슬롯 비우기
            UpdateResultButton();           // 결과 버튼 숨기기
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

            resetBtn.interactable = false;

            UpdateRecipeSlotsUI();
            UpdateResultButton();
            RefreshInventoryUI();
        }

        // 상세설명 Image, Text 변경
        public void InitDescription()
        {
            foodImage.sprite = null;
            foodName.text = "";
            foodDescription.text = "";
            foodDescription2.text = "";

            if (recipes.TryGetValue(selected, out RecipeData dish))
            {
                foodImage.sprite = dish.image;                  // TODO : Addressable
                foodName.text = dish.recipeName;
                foodDescription.text = dish.description;        // TODO : CSV
                foodDescription2.text = dish.description2;
            }
        }

        // ---------------------
        // 인벤토리 수량 변경: 획득/소모 시 호출
        public void AddIngredient(Ingredient ing, int count = 1)
        {
            if (!playerIngredientInventory.ContainsKey(ing)) playerIngredientInventory[ing] = 0; // 없으면 0으로 초기화
            playerIngredientInventory[ing] += count; // 수량 증가
            RefreshInventoryUI(); // UI 갱신
        }

        public void SubtractIngredient(Ingredient ing, int count = 1)
        {
            if (!playerIngredientInventory.ContainsKey(ing)) return; // 없으면 무시
            playerIngredientInventory[ing] -= count;                 // 수량 차감
            if (playerIngredientInventory[ing] < 0) playerIngredientInventory[ing] = 0; // 음수 방지
            RefreshInventoryUI(); // UI 갱신
        }

        // ---------------------
        // 헬퍼: 마스크에서 포함된 재료 리스트 반환
        Ingredient[] GetIngredientsFromMask(Ingredient mask)
        {
            var list = new List<Ingredient>(); // 결과 리스트 생성
            foreach (Ingredient ing in Enum.GetValues(typeof(Ingredient)))
            {
                if (ing == Ingredient.None) continue; // None 패스
                if ((mask & ing) != 0) list.Add(ing); // 해당 비트가 켜져 있으면 추가
            }
            return list.ToArray(); // 배열로 반환
        }

        // 헬퍼: ingredientIndexMap에서 재료의 인덱스 찾기
        int GetIndexByIngredient(Ingredient ing)
        {
            for (int i = 0; i < ingredientIndexMap.Length; i++)
                if (ingredientIndexMap[i] == ing) return i;
            return -1; // 못찾으면 -1 반환
        }
    }
}
