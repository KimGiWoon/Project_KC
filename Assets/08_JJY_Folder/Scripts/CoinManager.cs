using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJY
{
    public class CoinManager : MonoBehaviour
    {
        public static CoinManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);

            // TODO : Firebase와 연동
            items.Add(beek, 0);
            items.Add(fineDining, 0);
            items.Add(masterChef, 0);
        }


        public int inGameCoin { get; private set; } // 인게임 재화
        public int totalInGameCoin { get; private set; } // 인게임에서 얻은 총 재화량
        // 아웃게임 아이템
        private Dictionary<string, int> items = new Dictionary<string, int>();
        private string _beek = "Beek's Recipe Book";
        private string _fineDining = "Fine Dining Recipe Book";
        private string _masterChef = "Master Chef Recipe Book";
        public string beek { get { return _beek; } }
        public string fineDining { get { return _fineDining; } }
        public string masterChef { get { return _masterChef; } }
        public Action OnItemsChanged;

        /// <summary>
        /// 아이템의 수량을 받아오는 함수.
        /// </summary>
        public int GetRecipeItemCount(string itemName)
        {
            if (items.ContainsKey(itemName))
            {
                return items[itemName];
            }
            else
            {
                Debug.LogError($"{itemName} : 아이템 이름 오류");
                return -1;
            }
        }
        /// <summary>
        /// 아웃게임 아이템 증가
        /// </summary>
        public void AddRecipeItem(string itemName, int value)
        {
            if (items.ContainsKey(itemName))
            {
                items[itemName] += value;
                OnItemsChanged?.Invoke();
            }
            else Debug.LogError($"{itemName} : 아이템 이름 오류");
        }
        /// <summary>
        /// 아웃게임 아이템 소모
        /// </summary>
        public void SubtractRecipeItem(string itemName, int value)
        {
            if (items.ContainsKey(itemName))
            {
                if (items[itemName] >= value)
                {
                    items[itemName] -= value;
                    OnItemsChanged?.Invoke();
                }
                else
                {
                    value = GetRecipeItemCount(itemName);
                    items[itemName] -= value;
                    OnItemsChanged?.Invoke();
                    Debug.Log($"현재 선택된 아이템의 개수 : {items[itemName]}, 사용하려는 아이템의 개수{value}\t {items[itemName]}을 최대 개수만큼 사용합니다.");
                }
            }
            else Debug.LogError($"{itemName},{value} : 아이템 이름 또는 오류");
        }

        /// <summary>
        /// 인게임 재화 증가
        /// </summary>
        public void AddInGameCoin(int value)
        {
            inGameCoin += value;
            totalInGameCoin += value;
        }
        /// <summary>
        /// 인게임 재화 소모
        /// </summary>
        public void SubtractInGameCoin(int value)
        {
            if (inGameCoin < value) return;

            inGameCoin -= value;
            totalInGameCoin -= value;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 레시피 획득 테스트 전용 코드
        /// </summary>
        public void TestGetRecipe()
        {
            items[masterChef]++;
            items[fineDining]++;
            items[beek]++;
            OnItemsChanged?.Invoke();
        }
#endif
    }
}
