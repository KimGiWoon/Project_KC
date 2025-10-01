using System;
using System.Collections;
using System.Collections.Generic;
using KSH;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

namespace JJY
{
    public class InventoryUIManager : MonoBehaviour
    {
        public static InventoryUIManager Instance { get; private set; }

        [Header("Prefab")]
        [SerializeField] private GameObject itemPrefab;

        [Header("Debug")]
        [SerializeField]
        private List<InventoryItem> testInventory = new List<InventoryItem>();

        // --- 풀링 관련 컬렉션 ---
        private Queue<GameObject> pool = new Queue<GameObject>(); // 비활성화된(재사용 가능한) 버튼 풀
        private List<GameObject> activeButtons = new List<GameObject>(); // 현재 활성화된 버튼들 추적
        [SerializeField] private InventoryUI _inventoryUI;

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

#if UNITY_EDITOR
            TestInventory();
#endif
        }

        private void Start()
        {
            PrewarmPool(8);
            // InitFoodInventory();
        }
#if UNITY_EDITOR
        private void TestInventory()
        {
            for (int i = 0; i < testInventory.Count; i++)
            {
                if (testInventory[i].relic == null && testInventory[i].recipe == null) continue;
                // 유물은 GetRelic 호출 시 유뮬의 효과를 바로 적용하기 때문에 테스트 부적합. 테스트 하려면 RelicDropManager에서 주석처리 해야함.
                if (testInventory[i].relic != null) RelicDropManager.Instance.GetRelic(testInventory[i].relic);
                if (testInventory[i].recipe != null) GameManager.Instance.InGameItem.AddItem(testInventory[i].recipe);
            }
            InitFoodInventory();
        }
#endif
        private void OnEnable()
        {
#if UNITY_EDITOR
            GameManager.Instance.InGameItem.TestRelic();
#endif
            InitFoodInventory();
        }

        private void OnDestroy()
        {
            GameManager.Instance.InGameItem.foodInventory.Clear();
            GameManager.Instance.InGameItem.relicInventory.Clear();
            //todo 엽전도 초기화되어야 하나 테스트를 위해서 보류
            // GameManager.Instance.Coin.ClearYeopjeon();
        }

        // private void OnDisable()
        // {
        //     SeenNewItems();
        // }

        #region 오브젝트 풀

        private void PrewarmPool(int count)
        {
            // prefab 또는 parent가 없으면 동작 안 함
            if (itemPrefab == null) return;
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(itemPrefab); // 프리팹 인스턴스화
                go.SetActive(false); // 비활성화 상태로 보관
                pool.Enqueue(go); // 풀에 추가
            }
        }

        // 풀에서 버튼을 꺼내 활성화(없으면 새로 생성)
        private GameObject GetButtonFromPool()
        {
            GameObject go;
            if (pool.Count > 0) go = pool.Dequeue();
            else go = Instantiate(itemPrefab);

            go.transform.SetAsLastSibling();

            go.SetActive(false); // 깜빡임 현상
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
        }

        #endregion

        /// <summary>
        /// 보유중인 음식 리스트를 역순으로 배치한다.
        /// </summary>
        public void InitFoodInventory()
        {
            if (itemPrefab == null) return;

            ReturnAllButtonsToPool();

            if (CookManager.Instance == null) return;

            StartCoroutine(SetFoodContentsInit());
        }

        private IEnumerator SetFoodContentsInit()
        {
            yield return null;
            var foodList = new List<Button>();

            var list = GameManager.Instance.InGameItem.foodInventory;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var food = list[i];
                if (food == null) continue;

                var go = GetButtonFromPool();
                var btn = go.GetComponent<Button>();
                var imgs = go.GetComponentsInChildren<Image>(); // 아이콘용 이미지
                var img = imgs[1];
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
                var recipeLocal = food.recipe;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ShowFoodDescription(recipeLocal));

                // 활성 리스트에 추가하고 활성화 (한 번에)
                activeButtons.Add(go);
                foodList.Add(btn);
            }

            _inventoryUI.SetContent(foodList);
        }

        /// <summary>
        /// 보유중인 유물 리스트를 역순으로 배치한다.
        /// </summary>
        public void InitRelicInventory()
        {
            if (itemPrefab == null) return;

            ReturnAllButtonsToPool();

            if (RelicDropManager.Instance == null) return;

            StartCoroutine(SetRelicContentsInit());
        }

        private IEnumerator SetRelicContentsInit()
        {
            yield return null;
            var relicList = new List<Button>();

            var list = GameManager.Instance.InGameItem.relicInventory;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var relicItem = list[i];
                if (relicItem == null) continue;

                var go = GetButtonFromPool();
                var btn = go.GetComponent<Button>();
                var imgs = go.GetComponentsInChildren<Image>(); // 아이콘용 이미지
                var img = imgs[1];
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
                    newIcon.SetActive(relicItem.isNew);
                }

                // 안전한 캡처
                var relicLocal = relicItem.relic;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ShowRelicDescription(relicLocal));

                // 활성 리스트에 추가하고 활성화 (한 번에)
                activeButtons.Add(go);
                relicList.Add(btn);
            }
            _inventoryUI.SetContent(relicList);
        }

        /// <summary>
        /// 음식 설명란의 정보를 동기화한다.
        /// </summary>
        private void ShowFoodDescription(RecipeData recipe)
        {
            if (recipe == null) return;

            _inventoryUI.PopupDescription(new PopupDescription
            {
                Sprite = recipe.image ?? null,
                Name = recipe.recipeName ?? "",
                Description = recipe.description ?? "",
                Effect = recipe.effect ?? ""
            });
        }

        /// <summary>
        /// 음식 설명란의 정보를 동기화한다.
        /// </summary>
        private void ShowRelicDescription(RelicDatas relic)
        {
            if (relic == null) return;

            _inventoryUI.PopupDescription(new PopupDescription
            {
                Sprite = relic.relicImage ?? null,
                Name = relic.relicName ?? "",
                Description = relic.relicDescription[0] ?? "",
                Effect = relic.relicDescription[1] ?? ""
            });
        }

        public void SeenNewItems()
        {
            if (GameManager.Instance.InGameItem.foodInventory != null &&
                GameManager.Instance.InGameItem.foodInventory.Count > 0)
            {
                foreach (var i in GameManager.Instance.InGameItem.foodInventory)
                {
                    if (i.isNew) i.isNew = !i.isNew;
                }
            }

            if (GameManager.Instance.InGameItem.relicInventory != null &&
                GameManager.Instance.InGameItem.relicInventory.Count > 0)
            {
                foreach (var i in GameManager.Instance.InGameItem.relicInventory)
                {
                    if (i.isNew) i.isNew = !i.isNew;
                }
            }
        }
    }
}