using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJY
{
    public class CoinManager : MonoBehaviour
    {
        public int inGameCoin { get; private set; } // 인게임 재화
        public int totalInGameCoin { get; private set; } // 인게임에서 얻은 총 재화량
        // 아웃게임 아이템
        private Dictionary<string, int> items = new Dictionary<string, int>();

        // TODO : Firebase에서 저장된 값을 가져와야함. 없다면 추가.
        void Start()
        {
            items.Add("Beek's Recipe Book", 0);
            items.Add("Fine Dining Recipe Book", 0);
            items.Add("Master Chef Recipe Book", 0);
        }

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
            }
            else Debug.LogError($"{itemName} : 아이템 이름 오류");
        }
        /// <summary>
        /// 아웃게임 아이템 소모
        /// </summary>
        public void UseRecipeItem(string itemName, int value)
        {
            if (items.ContainsKey(itemName))
            {
                if (items[itemName] > value)
                {
                    items[itemName] -= value;
                }
                else
                {
                    value = GetRecipeItemCount(itemName);
                    items[itemName] -= value;
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
    }
}
