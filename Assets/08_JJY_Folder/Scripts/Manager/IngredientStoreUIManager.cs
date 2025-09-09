using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    [Serializable]
    public class StoreSlotUI
    {
        public Button button;                // 슬롯 버튼
        public Image iconImage;              // 재료 아이콘
        public TextMeshProUGUI ingNameText;  // 재료 이름
        public TextMeshProUGUI priceText;    // 가격 텍스트
        public Ingredient ingredient;
    }

    public class IngredientStoreUIManager : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] Button outBtn;
        [SerializeField] Button buyBtn;
        [SerializeField] Button refreshBtn;

        [Header("PopUp UI")]
        [SerializeField] GameObject failedPanel;
        [SerializeField] TextMeshProUGUI failedText;
        [SerializeField] GameObject successedPanel;

        [Header("Store Slot (3)")]
        [SerializeField] List<StoreSlotUI> slots; // 구매 가능한 목록 (버튼의 데이터는 모든 재료 가운데 랜덤으로 정해진다.)

        [Header("Config")]
        [SerializeField] int refreshCost = 5;
        [SerializeField] IngredientDatabase ingredientDatabase;
        Dictionary<Ingredient, IngredientData> ingredientMap;

        class SlotData { public Ingredient ingredient; public int price; public bool sold; }
        List<SlotData> slotDatas = new List<SlotData>();

        int selectedSlotIndex = -1;
        Ingredient[] allIngredients;
        bool logAction = true;

        void Start()
        {
#if UNITY_EDITOR
            TestAddYeopjeon();
#endif
            InitIngredientIndexMap();
            InitIngredients();
            InitStoreSlots();

            for (int i = 0; i < slots.Count; i++)
            {
                int idx = i;
                if (slots[idx] != null && slots[idx].button != null)
                {
                    slots[idx].button.onClick.RemoveAllListeners();
                    slots[idx].button.onClick.AddListener(() => OnSlotClicked(idx));
                }
            }
            buyBtn.onClick.AddListener(TryBuyIngredient);
            refreshBtn.onClick.AddListener(RefreshStore);
        }
        void InitIngredients()
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
        void InitIngredientIndexMap()
        {
            var values = Enum.GetValues(typeof(Ingredient));
            List<Ingredient> tmp = new List<Ingredient>();
            foreach (Ingredient ing in values)
            {
                if (ing == Ingredient.None) continue;
                tmp.Add(ing);
            }
            allIngredients = tmp.ToArray();
        }
#if UNITY_EDITOR
        void TestAddYeopjeon()
        {
            CoinManager.Instance.AddYeopjeon(500);
        }
#endif
        void InitStoreSlots()
        {
            slotDatas.Clear();
            for (int i = 0; i < slots.Count; i++)
            {
                slotDatas.Add(new SlotData { ingredient = Ingredient.None, price = 0, sold = false });
            }

            for (int i = 0; i < slotDatas.Count; i++)
            {
                // 랜덤 선택(중복 허용)
                Ingredient chosen = allIngredients[UnityEngine.Random.Range(0, allIngredients.Length)];
                
                IngredientData data = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(chosen, out data);

                slotDatas[i].ingredient = data.ingredient;
                slotDatas[i].price = data.cost;
                slotDatas[i].sold = false;
            }

            selectedSlotIndex = -1;
            UpdateSlotUI();

            failedPanel.SetActive(false);
        }

        /// <summary>
        /// 상점을 엽전을 사용해 새로고침한다.
        /// </summary>
        public void RefreshStore()
        {
            if (CoinManager.Instance.yeopjeon < 5) return; // 새로고침 시 5엽전 소모.
            CoinManager.Instance.SubtractYeopjeon(5);

            for (int i = 0; i < slotDatas.Count; i++)
            {
                // 랜덤 선택(중복 허용)
                Ingredient chosen = allIngredients[UnityEngine.Random.Range(0, allIngredients.Length)];

                IngredientData data = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(chosen, out data);

                slotDatas[i].ingredient = data.ingredient;
                slotDatas[i].price = data.cost;
                slotDatas[i].sold = false;
            }

            selectedSlotIndex = -1;
            UpdateSlotUI();
            if (logAction) Debug.Log($"상점 새로고침:엽전 {CoinManager.Instance.yeopjeon}개 보유중");
        }
        /// <summary>
        /// 슬릇의 정보를 동기화한다.
        /// </summary>
        void UpdateSlotUI()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slotUI = slots[i];
                var data = slotDatas[i];

                // name
                if (slotUI.ingNameText != null)
                {
                    slotUI.ingNameText.text = data.ingredient != Ingredient.None ? data.ingredient.ToString() : "";
                }

                IngredientData ingdata = null;
                if (ingredientMap != null) ingredientMap.TryGetValue(data.ingredient, out ingdata);
                if (ingdata != null && ingdata.icon != null)
                {
                    slotUI.iconImage.sprite = ingdata.icon;
                    slotUI.iconImage.enabled = true;
                }

                // price
                if (slotUI.priceText != null)
                {
                    slotUI.priceText.text = $"{data.price}";
                }

                // Interactable
                if (slotUI.button != null)
                {
                    // sold이면 비활성
                    slotUI.button.interactable = !data.sold;
                }

                // 선택 표시
                var img = slotUI.button != null ? slotUI.button.GetComponent<Image>() : null;
                if (img != null)
                {
                    img.color = (selectedSlotIndex == i) ? Color.gray : Color.white;
                }
            }
            failedPanel.SetActive(false);
        }
        void OnSlotClicked(int index)
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
                if (failedPanel != null) failedPanel.SetActive(true);
                if (failedText != null) failedText.text = "아무런 아이템도 선택되지 않았습니다.";
                return;
            }
            var data = slotDatas[selectedSlotIndex];

            if (CoinManager.Instance.yeopjeon < data.price)
            {
                if (failedPanel != null) failedPanel.SetActive(true);
                if (failedText != null) failedText.text = "아이템을 구매하기 위한 재화가 부족합니다.";
                return;
            }

            CoinManager.Instance.SubtractYeopjeon(data.price);
            if (logAction) Debug.Log($"아이템 {data.ingredient} 구매: 엽전 {CoinManager.Instance.yeopjeon}개 보유중");
            CookManager.Instance.playerIngredientInventory[data.ingredient]++;
            slotDatas[selectedSlotIndex].sold = true;
            selectedSlotIndex = -1;
            UpdateSlotUI();
            successedPanel.SetActive(true);

        }
    }
}