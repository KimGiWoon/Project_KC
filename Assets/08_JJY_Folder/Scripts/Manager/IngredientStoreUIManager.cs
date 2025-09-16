using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

namespace JJY
{
    [Serializable]
    public class StoreSlotUIData
    {
        public Sprite iconSprite; // 재료 아이콘
        public string ingNameText; // 재료 이름
        public string priceText; // 가격 텍스트
        public bool isEnabled;
        public bool canInteractable;
    }

    public class IngredientStoreUIManager : MonoBehaviour
    {
        public static IngredientStoreUIManager Instance { get; private set; }

        [Header("Store Slot (3)")]
        [SerializeField]
        private List<StoreSlotUIData> slots = new List<StoreSlotUIData>(); // 구매 가능한 목록 (버튼의 데이터는 모든 재료 가운데 랜덤으로 정해진다.)

        [Header("Config")]
        [SerializeField]
        private int refreshCost = 5;
        [SerializeField] private IngredientDatabase ingredientDatabase;
        private Dictionary<Ingredient, IngredientData> ingredientMap;

        private CoinManager _coin;

        private class SlotData
        {
            public Ingredient ingredient;
            public int price;
            public bool sold;
        }

        private List<SlotData> slotDatas = new List<SlotData>();

        public int selectedSlotIndex = -1;
        private Ingredient[] allIngredients;
        private bool logAction = true;
        // On/Off하며 테스트
        [SerializeField] private bool hasDiscountRelic;
        [SerializeField] private ShoppingUI _shoppingUI;

        private void Awake()
        {
            // 싱글톤 초기화: 이미 인스턴스가 있으면 자신을 파괴
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            StartCoroutine(DelayedInit());
            _coin = GameManager.Instance.Coin;
        }

        private IEnumerator DelayedInit()
        {
            yield return new WaitForSeconds(1f);

            // #if UNITY_EDITOR
            //             TestAddYeopjeon();
            // #endif
            InitIngredientIndexMap();
            InitIngredients();
            InitStoreSlots();
        }

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
        private void InitIngredientIndexMap()
        {
            var values = Enum.GetValues(typeof(Ingredient));
            var tmp = new List<Ingredient>();
            foreach (Ingredient ing in values)
            {
                if (ing == Ingredient.None) continue;
                tmp.Add(ing);
            }
            allIngredients = tmp.ToArray();
        }

        // #if UNITY_EDITOR
        //         private void TestAddYeopjeon()
        //         {
        //             _coin.AddYeopjeon(500);
        //         }
        // #endif

        private void HasDiscountRelic()
        {
            hasDiscountRelic = false;
            hasDiscountRelic = GameManager.Instance.InGameItem.HasRelic(RelicTarget.Shop);
        }
        private void InitStoreSlots()
        {
            slotDatas.Clear();
            for (int i = 0; i < slots.Count; i++)
            {
                slotDatas.Add(new SlotData { ingredient = Ingredient.None, price = 0, sold = false });
            }

            HasDiscountRelic();
            for (int i = 0; i < slotDatas.Count; i++)
            {
                // 랜덤 선택(중복 허용)
                var chosen = allIngredients[UnityEngine.Random.Range(0, allIngredients.Length)];

                IngredientData data = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(chosen, out data);

                slotDatas[i].ingredient = data.ingredient;
                // 할인율 변경 시 이곳에서도 변경해야함.
                if (hasDiscountRelic) slotDatas[i].price = Mathf.RoundToInt(data.cost * 0.7f);
                else slotDatas[i].price = data.cost;
                slotDatas[i].price = data.cost;
                slotDatas[i].sold = false;
            }

            selectedSlotIndex = -1;
            UpdateSlotUI();
        }

        /// <summary>
        /// 상점을 엽전을 사용해 새로고침한다.
        /// </summary>
        public void RefreshStore()
        {
            selectedSlotIndex = -1;

            if (_coin.yeopjeon < 5)
            {
                _shoppingUI.ActiveFailedPanel("어짜피 다른 거 가져와도 못 사실 텐데요?");
                return; // 새로고침 시 5엽전 소모.
            }
            _coin.SubtractYeopjeon(5);

            HasDiscountRelic();
            for (int i = 0; i < slotDatas.Count; i++)
            {
                // 랜덤 선택(중복 허용)
                var chosen = allIngredients[UnityEngine.Random.Range(0, allIngredients.Length)];

                IngredientData data = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(chosen, out data);

                slotDatas[i].ingredient = data.ingredient;
                // 할인율 변경 시 이곳에서도 변경해야함.
                if (hasDiscountRelic) slotDatas[i].price = Mathf.RoundToInt(data.cost * 0.7f);
                else slotDatas[i].price = data.cost;
                slotDatas[i].price = data.cost;
                slotDatas[i].sold = false;
            }

            UpdateSlotUI();
            if (logAction) Debug.Log($"상점 새로고침:엽전 {_coin.yeopjeon}개 보유중");
        }
        /// <summary>
        /// 슬릇의 정보를 동기화한다.
        /// </summary>
        public void UpdateSlotUI()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slotUI = slots[i];
                var data = slotDatas[i];

                // name
                if (slotUI.ingNameText != null)
                {
                    slotUI.ingNameText = data.ingredient != Ingredient.None ? data.ingredient.ToString() : "";
                }

                IngredientData ingdata = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(data.ingredient, out ingdata);
                if (ingdata != null && ingdata.icon != null)
                {
                    slotUI.iconSprite = ingdata.icon;
                    slotUI.isEnabled = true;
                }

                // price
                if (slotUI.priceText != null)
                {
                    slotUI.priceText = $"{data.price}";
                }

                // sold이면 비활성
                slotUI.canInteractable = !data.sold;
            }

            _shoppingUI.UpdateSlotUI(slots);
        }

        public void OnSlotClicked(int index)
        {
            if (index < 0 || index >= slotDatas.Count) return;

            var data = slotDatas[index];
            if (data.sold) return;

            // 선택 표시
            selectedSlotIndex = index;
            UpdateSlotUI();
        }

        /// <summary>
        /// 아이템을 구매한다.
        /// </summary>
        public void TryBuyIngredient()
        {
            if (selectedSlotIndex < 0 || selectedSlotIndex >= slotDatas.Count)
            {
                _shoppingUI.ActiveFailedPanel("아이쇼핑만 하실 거예요~ 셰프님? ^^");
                return;
            }
            var data = slotDatas[selectedSlotIndex];

            if (_coin.yeopjeon < data.price)
            {
                selectedSlotIndex = -1;
                _shoppingUI.ResetIngredientColor();
                _shoppingUI.ActiveFailedPanel("셰프님…? 한도 초과라고 하네요…?");
                return;
            }

            _coin.SubtractYeopjeon(data.price);
            if (logAction) Debug.Log($"아이템 {data.ingredient} 구매: 엽전 {_coin.yeopjeon}개 보유중");
            if (!CookManager.Instance.playerIngredientInventory.ContainsKey(data.ingredient))
                CookManager.Instance.playerIngredientInventory[data.ingredient] = 1;
            else
                CookManager.Instance.playerIngredientInventory[data.ingredient]++;
            slotDatas[selectedSlotIndex].sold = true;
            selectedSlotIndex = -1;
            UpdateSlotUI();
        }
    }
}