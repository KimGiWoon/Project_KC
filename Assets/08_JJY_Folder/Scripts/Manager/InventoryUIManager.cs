using System.Collections.Generic;
using System.Globalization;
using KSH;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    public class InventoryUIManager : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] Button foodTabBtn;
        [SerializeField] Button relicTabBtn;

        [Header("Description Pop Up")]
        [SerializeField] GameObject descriptionPanel;
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI des_1;
        [SerializeField] TextMeshProUGUI des_2;

        [Header("Prefab")]
        [SerializeField] Transform inventoryContent;
        [SerializeField] GameObject itemPrefab;

        // --- 풀링 관련 컬렉션 ---
        Queue<GameObject> pool = new Queue<GameObject>();               // 비활성화된(재사용 가능한) 버튼 풀
        List<GameObject> activeButtons = new List<GameObject>();        // 현재 활성화된 버튼들 추적

        void Start()
        {
            PrewarmPool(8);
            // InitFoodInventory();

            foodTabBtn.onClick.AddListener(InitFoodInventory);
            relicTabBtn.onClick.AddListener(InitRelicInventory);
        }
        void OnEnable()
        {
            InitFoodInventory();
        }
        void OnDisable()
        {
            SeenNewItems();
        }

        #region 오브젝트 풀
        void PrewarmPool(int count)
        {
            // prefab 또는 parent가 없으면 동작 안 함
            if (itemPrefab == null || inventoryContent == null) return;
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(itemPrefab, inventoryContent); // 프리팹 인스턴스화
                go.SetActive(false);                                // 비활성화 상태로 보관
                pool.Enqueue(go);                                   // 풀에 추가
            }
        }

        // 풀에서 버튼을 꺼내 활성화(없으면 새로 생성)
        GameObject GetButtonFromPool()
        {
            GameObject go;
            if (pool.Count > 0) go = pool.Dequeue();
            else go = Instantiate(itemPrefab, inventoryContent, false);

            go.transform.SetParent(inventoryContent, false);
            go.transform.SetAsLastSibling();

            go.SetActive(false);                    // 깜빡임 현상
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
            activeButtons.Clear();                             // 활성 리스트 비움
        }
        #endregion

        /// <summary>
        /// 보유중인 음식 리스트를 역순으로 배치한다.
        /// </summary>
        void InitFoodInventory()
        {
            if (itemPrefab == null || inventoryContent == null) return;

            ReturnAllButtonsToPool();

            if (CookManager.Instance == null) return;
            var list = CookManager.Instance.playerFoodInventory;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                InventoryItem food = list[i];
                if (food == null) continue;

                GameObject go = GetButtonFromPool();
                var btn = go.GetComponent<Button>();
                var img = go.GetComponent<Image>();
                var newIcon = go.transform.Find("NewIcon")?.gameObject;

                // 이미지 설정
                if (img != null)
                {
                    if (food.recipe.image != null)
                    {
                        img.sprite = food.recipe.image;
                        img.enabled = true;
                    }
                    else
                    {
                        img.sprite = null;
                        img.enabled = false;
                    }
                }

                // new item 표시
                if (newIcon != null)
                {
                    newIcon.SetActive(food.isNew);
                }

                // 안전한 캡처
                RecipeData recipeLocal = food.recipe;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ShowFoodDescription(recipeLocal));

                // 활성 리스트에 추가하고 활성화 (한 번에)
                activeButtons.Add(go);
                go.SetActive(true);
            }
        }

        /// <summary>
        /// 보유중인 유물 리스트를 역순으로 배치한다.
        /// </summary>
        void InitRelicInventory()
        {
            if (itemPrefab == null || inventoryContent == null) return;

            ReturnAllButtonsToPool();

            if (RelicDropManager.Instance == null) return;
            var list = RelicDropManager.Instance.acquiredRelicLists;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                InventoryItem relicItem = list[i];
                if (relicItem == null) continue;

                GameObject go = GetButtonFromPool();
                var btn = go.GetComponent<Button>();
                var img = go.GetComponent<Image>();
                var newIcon = go.transform.Find("NewIcon")?.gameObject;

                // 이미지 설정
                if (img != null)
                {
                    if (relicItem.relic.relicImage != null)
                    {
                        img.sprite = relicItem.relic.relicImage;
                        img.enabled = true;
                    }
                    else
                    {
                        img.sprite = null;
                        img.enabled = false;
                    }
                }

                // new item 표시
                if (newIcon != null)
                {
                    // newIcon.SetActive(item.isNew);
                }

                // 안전한 캡처
                RelicDatas relicLocal = relicItem.relic;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ShowRelicDescription(relicLocal));

                // 활성 리스트에 추가하고 활성화 (한 번에)
                activeButtons.Add(go);
                go.SetActive(true);
            }
        }
        /// <summary>
        /// 음식 설명란의 정보를 동기화한다.
        /// </summary>
        void ShowFoodDescription(RecipeData recipe)
        {
            if (recipe == null) return;
            descriptionPanel.SetActive(true);
            if (itemIcon != null) { itemIcon.sprite = recipe.image; itemIcon.enabled = recipe.image != null; }
            if (itemName != null) itemName.text = recipe.recipeName ?? "";
            if (des_1 != null) des_1.text = recipe.description ?? "";
            if (des_2 != null) des_2.text = recipe.description2 ?? "";
        }

        /// <summary>
        /// 음식 설명란의 정보를 동기화한다.
        /// </summary>
        void ShowRelicDescription(RelicDatas relic)
        {
            if (relic == null) return;
            descriptionPanel.SetActive(true);
            if (itemIcon != null) { itemIcon.sprite = relic.relicImage; itemIcon.enabled = relic.relicImage != null; }
            if (itemName != null) itemName.text = relic.relicName ?? "";
            if (des_1 != null) des_1.text = (relic.relicDescription.Count > 0) ? relic.relicDescription[0] : "";
            if (des_2 != null) des_2.text = (relic.relicDescription.Count > 1) ? relic.relicDescription[1] : "";
        }

        void SeenNewItems()
        {
            foreach (var i in CookManager.Instance.playerFoodInventory)
            {
                i.isNew = false;
            }
        }
    }
}
