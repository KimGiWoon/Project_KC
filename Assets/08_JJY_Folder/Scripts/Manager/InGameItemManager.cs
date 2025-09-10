using System.Collections.Generic;
using JJY;
using UnityEngine;

public class InGameItemManager : MonoBehaviour
{
    // 여기에는 노드 - 전투 씬에 사용 될 아이템들을 보관한다.
    private List<InventoryItem> _foodInventory = new List<InventoryItem>();
    public List<InventoryItem> foodInventory
    {
        get => _foodInventory;
        private set => foodInventory = value;
    }
    private List<InventoryItem> _relicInventory = new List<InventoryItem>();
    public List<InventoryItem> relicInventory
    {
        get => _relicInventory;
        private set => relicInventory = value;
    }

    public void AddItem(RecipeData dish)
    {
        var item = new InventoryItem(dish);
        foodInventory.Add(item);
    }
    public void AddItem(RelicDatas relic)
    {
        var item = new InventoryItem(relic);
        relicInventory.Add(item);
    }

    public void SubtractFood(InventoryItem food)
    {
        foodInventory.Remove(food);
    }
}
