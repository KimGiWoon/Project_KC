using System;
using System.Collections.Generic;
using UnityEngine;

namespace JJY
{
    public class CoinManager : MonoBehaviour
    {
        public int yeopjeon { get; private set; } // 전투에서 획득, 소모하는 재화. 서버에 저장할 필요 없음.
        public int totalYeopjeon { get; private set; } // 이번 전투에서 얻은 총 재화량
        public int starCandy { get; private set; } // 인게임 재화, GameManager의 변수명 변경해야함. (Fire base)

        public int shiningStarCandy { get; private set; } // 인게임 유료 재화, 변수명 변경해야함. (Fire base)
        public int point { get; private set; }

        // public static CoinManager Instance { get; private set; }
        private void Awake()
        {
            // TODO : Firebase와 연동
            items.Add(beek, 0);
            items.Add(fineDining, 0);
            items.Add(masterChef, 0);
            yeopjeon = 999999;
            starCandy = 999999;
        }

        // 아웃게임 아이템
        private Dictionary<string, int> items = new Dictionary<string, int>();
        private string _beek = "Beek's Recipe Book";
        private string _fineDining = "Fine Dining Recipe Book";
        private string _masterChef = "Master Chef Recipe Book";
        public string beek => _beek;
        public string fineDining => _fineDining;
        public string masterChef => _masterChef;
        public Action OnItemsChanged;

        /// <summary>
        /// 경험치 재화의 수량을 받아오는 함수.
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
        /// 경험치 재화의 획득
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
        /// 경험치 재화 소모
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
        /// yeopjeon 재화 증가
        /// </summary>
        public void AddYeopjeon(int value)
        {
            yeopjeon += value;
            totalYeopjeon += value;
        }
        /// <summary>
        /// yeopjeon 재화 소모
        /// </summary>
        public void SubtractYeopjeon(int value)
        {
            if (yeopjeon < value) return;

            yeopjeon -= value;
        }

        public void ClearYeopjeon()
        {
            yeopjeon = 0;
            totalYeopjeon = 0;
        }

        /// <summary>
        /// StarCandy 재화 증가
        /// </summary>
        public void AddStarCandy(int value)
        {
            starCandy += value;
        }
        /// <summary>
        /// StarCandy 재화 소모
        /// </summary>
        public void SubtractStarCandy(int value)
        {
            if (starCandy < value) return;

            starCandy -= value;
        }
        /// <summary>
        /// ShiningStarCandy 재화 증가
        /// </summary>
        public void AddShiningStarCandy(int value)
        {
            shiningStarCandy += value;
        }
        /// <summary>
        /// ShiningStarCandy 재화 소모
        /// </summary>
        public void SubtractShiningStarCandy(int value)
        {
            if (shiningStarCandy < value) return;

            shiningStarCandy -= value;
        }

        public void AddPoint(int value)
        {
            point += value;
        }

        public bool SubtractPoint(int value)
        {
            if (point < value) return false;

            point += value;
            return true;
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