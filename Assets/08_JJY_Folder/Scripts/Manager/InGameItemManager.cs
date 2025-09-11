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
