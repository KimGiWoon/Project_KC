using System.Collections.Generic;
using JJY;
using UnityEngine;
using System;
using System.Linq;
public class InGameItemManager : MonoBehaviour
{
    // 여기에는 노드 - 전투 씬에 사용 될 아이템들을 보관한다.
    private List<InventoryItem> _foodInventory = new List<InventoryItem>();
    public List<InventoryItem> foodInventory
    {
        get => _foodInventory;
        private set => _foodInventory = value;
    }
    private List<InventoryItem> _relicInventory = new List<InventoryItem>();
    public List<InventoryItem> relicInventory
    {
        get => _relicInventory;
        private set => _relicInventory = value;
    }

    public event Action OnItemChanged;

    public void AddItem(RecipeData dish)
    {
        var item = new InventoryItem(dish);
        _foodInventory.Add(item);
    }
    public void AddItem(RelicDatas relic)
    {
        var item = new InventoryItem(relic);
        _relicInventory.Add(item);
        OnItemChanged?.Invoke();
    }

    // CJH 코드 추가
    public void RemoveItem(RelicDatas relicToRemove)
    {
        // 인벤토리에서 제거할 유물을 찾습니다.
        InventoryItem itemToRemove = relicInventory.FirstOrDefault(r => r.relic == relicToRemove);

        // 유물이 인벤토리에 있으면 제거합니다.
        if (itemToRemove != null)
        {
            relicInventory.Remove(itemToRemove);
            OnItemChanged?.Invoke(); // 아이템 변경 이벤트를 호출하여 UI를 업데이트합니다.
        }
    }

    public bool HasRelic(RelicDatas relic)
    {
        if (relic == null) return false;
        return _relicInventory.Any(item => item.relic == relic);
    }

    public void SubtractFood(InventoryItem food)
    {
        foodInventory.Remove(food);
    }
}
