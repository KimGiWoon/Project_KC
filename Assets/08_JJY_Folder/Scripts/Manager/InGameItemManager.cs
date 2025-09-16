using System.Collections.Generic;
using JJY;
using UnityEngine;
using System;
using SDW;

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

    /// <summary>
    /// 인벤토리에서 무작위 유물 하나를 제거합니다.
    /// </summary>
    /// <returns>성공적으로 제거된 유물 데이터를 반환합니다. 잃을 유물이 없으면 null을 반환합니다.</returns>
    public RelicDatas RemoveRandomRelic()
    {
        // 인벤토리에 유물이 있는지 먼저 확인합니다.
        if (_relicInventory.Count == 0)
        {
            Debug.Log("제거할 유물이 인벤토리에 없습니다.");
            return null; // 잃을 유물이 없으면 null을 반환합니다.
        }

        // 0부터 현재 유물 개수 -1 사이의 무작위 숫자를 선택합니다.
        int randomIndex = UnityEngine.Random.Range(0, _relicInventory.Count);
        InventoryItem itemToRemove = _relicInventory[randomIndex];

        // 무작위로 선택된 유물을 인벤토리에서 제거합니다.
        _relicInventory.RemoveAt(randomIndex);
        Debug.Log($"[InGameItemManager] 유물 '{itemToRemove.relic.relicName}'을(를) 잃었습니다.");

        // 아이템이 변경되었음을 알려 UI 등을 업데이트합니다.
        OnItemChanged?.Invoke();

        //. 어떤 유물을 잃었는지 알려주기 위해 해당 유물 데이터를 반환합니다.
        return itemToRemove.relic;
    }

    public bool HasRelic(RelicTarget target)
    {
        if (_relicInventory == null) return false;
        foreach (var inv in _relicInventory)
        {
            if (inv?.relic == null) continue;
            if (inv.relic.relicTarget == target) return true;
        }
        return false;
    }

    public void SubtractFood(InventoryItem food)
    {
        foodInventory.Remove(food);
    }
}
