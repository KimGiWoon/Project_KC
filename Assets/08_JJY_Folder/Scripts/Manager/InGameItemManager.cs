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

    /// <summary>
    /// 제공된 전체 유물 목록 중에서, 현재 가지고 있지 않은 유물 하나를 무작위로 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="allRelicsDatabase">게임에 존재하는 모든 유물의 목록입니다.</param>
    /// <returns>성공적으로 추가된 유물 데이터를 반환합니다. 추가할 유물이 없으면 null을 반환합니다.</returns>
    public RelicDatas AddRandomRelic(List<RelicDatas> allRelicsDatabase)
    {
        // 획득 가능한 유물 목록을 찾습니다.
        var acquirableRelics = allRelicsDatabase.Where(relic => !HasRelic(relic)).ToList();

        // 획득 가능한 유물이 더 이상 없는 경우
        if (acquirableRelics.Count == 0)
        {
            Debug.Log("획득할 수 있는 새로운 유물이 없습니다.");
            return null;
        }

        // 획득 가능한 유물 목록 내에서 무작위 인덱스를 선택합니다.
        int randomIndex = UnityEngine.Random.Range(0, acquirableRelics.Count);
        RelicDatas relicToAdd = acquirableRelics[randomIndex];

        // 기존의 AddItem 함수를 사용해 인벤토리에 추가합니다.
        AddItem(relicToAdd);

        Debug.Log($"[InGameItemManager] 유물 '{relicToAdd.relicName}'을(를) 획득했습니다!");

        // 어떤 유물을 얻었는지 알려주기 위해 해당 유물 데이터를 반환합니다.
        return relicToAdd;
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
