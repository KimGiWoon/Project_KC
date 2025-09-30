using System.Collections.Generic;
using JJY;
using UnityEngine;
using System;
using SDW;
using System.Linq;
using KSH;

public class InGameItemManager : MonoBehaviour
{
    // 여기에는 노드 - 전투 씬에 사용 될 아이템들을 보관한다.
    private List<InventoryItem> _foodInventory = new List<InventoryItem>();
    public List<InventoryItem> foodInventory { get => _foodInventory; private set => _foodInventory = value; }
    private List<InventoryItem> _relicInventory = new List<InventoryItem>();
    public List<InventoryItem> relicInventory { get => _relicInventory; private set => _relicInventory = value; }

    private int _cookCount = 0;
    public int CookCount => _cookCount;

    private int _relicCount = 0;
    public int RelicCount => _relicCount;

    private int _usedCookCount = 0;
    public int UsedCookCount => _usedCookCount;

    private int _getRelicCount = 0;
    public int GetRelicCount => _getRelicCount;

    private Dictionary<RelicGrade, int> _relicGradeCount = new Dictionary<RelicGrade, int>();
    public Dictionary<RelicGrade, int> RelicGradeCount => _relicGradeCount;

    public event Action OnItemChanged;

    private GameManager _gameManager;
    private FirebaseManager _firebase;
    private bool _isLoaded;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField]
    private List<InventoryItem> testRelic = new List<InventoryItem>();
#endif

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _firebase = _gameManager.Firebase;
        _firebase.OnUserInfoUpdated += ClearIsLoaded;
    }

    private void Update()
    {
        if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
            !_gameManager.Firebase.IsLoaded || _isLoaded) return;

        LoadItemData(_gameManager.Firebase.EtcData);
        LoadQuestCount(_gameManager.Firebase.DailyQuestProgress);
        _isLoaded = true;
    }

#if UNITY_EDITOR
    public void TestRelic()
    {
        for (int i = 0; i < testRelic.Count; i++)
        {
            if (testRelic[i].relic != null) RelicDropManager.Instance.GetRelic(testRelic[i].relic);
        }
    }
#endif

    private void LoadItemData(IReadOnlyDictionary<string, object> etcData)
    {
        _cookCount = Convert.ToInt32(etcData["cookCount"]);
        _relicCount = Convert.ToInt32(etcData["relicCount"]);

        foreach (string grade in Enum.GetNames(typeof(RelicGrade)))
        {
            var relicGrade = (RelicGrade)Enum.Parse(typeof(RelicGrade), grade);

            if (relicGrade == RelicGrade.None) continue;

            _relicGradeCount[relicGrade] = Convert.ToInt32(etcData["relic" + grade]);
        }
    }

    private void LoadQuestCount(IReadOnlyDictionary<string, object> dailyQuestProgress)
    {
        var dbQuestProgressDictionary = dailyQuestProgress as Dictionary<string, object>;

        SetUsedCookCount(Convert.ToInt32(dbQuestProgressDictionary[QuestType.UseFood.ToString()]));
        SetGetRelicCount(Convert.ToInt32(dbQuestProgressDictionary[QuestType.GetArtifact.ToString()]));
    }

    public void AddItem(RecipeData dish)
    {
        var item = new InventoryItem(dish);
        _foodInventory.Add(item);
    }

    public void IncreaseCookCount()
    {
        _cookCount++;
        _usedCookCount++;
        _firebase.SetCookCount(_cookCount);
        _firebase.SetQuestState(QuestType.UseFood, _usedCookCount >= 3, _usedCookCount);
    }

    public void SetUsedCookCount(int value)
    {
        _usedCookCount = value;
        _firebase.SetQuestState(QuestType.UseFood, _usedCookCount >= 3, _usedCookCount);
    }

    public void SetGetRelicCount(int value)
    {
        _getRelicCount = value;
        _firebase.SetQuestState(QuestType.GetArtifact, _getRelicCount >= 5, _getRelicCount);
    }

    public void AddItem(RelicDatas relic)
    {
        var item = new InventoryItem(relic);
        _relicInventory.Add(item);
        OnItemChanged?.Invoke();
        _relicCount++;
        _getRelicCount++;

        _relicGradeCount[relic.relicGrade]++;
        _firebase.SetRelicCount(_relicCount, _relicGradeCount);
        _firebase.SetQuestState(QuestType.GetArtifact, _getRelicCount >= 5, _getRelicCount);
        GameManager.Instance.DailyQuest.CompleteQuestInRoguelikeScene(QuestType.GetArtifact, _getRelicCount); //음식 먹을 때 퀘스트 클리어
    }

    public void ClearItemCounts()
    {
        _cookCount = 0;
        _relicCount = 0;

        foreach (var key in _relicGradeCount.Keys.ToList())
        {
            _relicGradeCount[key] = 0;
        }

        _firebase.SetCookCount(_cookCount);
        _firebase.SetRelicCount(_relicCount, _relicGradeCount);
    }

    public void IncreaseUsedCookCount()
    {
        _usedCookCount++;
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
        var itemToRemove = _relicInventory[randomIndex];

        // 무작위로 선택된 유물을 인벤토리에서 제거합니다.
        _relicInventory.RemoveAt(randomIndex);
        _firebase.SetRelicCount(_relicCount, _relicGradeCount);
        Debug.Log($"[InGameItemManager] 유물 '{itemToRemove.relic.relicName}'을(를) 잃었습니다.");

        // 아이템이 변경되었음을 알려 UI 등을 업데이트합니다.
        OnItemChanged?.Invoke();

        //. 어떤 유물을 잃었는지 알려주기 위해 해당 유물 데이터를 반환합니다.
        return itemToRemove.relic;
    }

    // CJH 코드 추가

    /// <summary>
    /// 제공된 전체 유물 목록 중에서, 현재 가지고 있지 않은 유물 하나를 무작위로 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="allRelicsDatabase">게임에 존재하는 모든 유물의 목록입니다.</param>
    /// <returns>성공적으로 추가된 유물 데이터를 반환합니다. 추가할 유물이 없으면 null을 반환합니다.</returns>
    public RelicDatas AddRandomRelic(List<RelicDatas> allRelicsDatabase)
    {
        // 획득 가능한 유물 목록을 찾습니다.
        var acquirableRelics = allRelicsDatabase.Where(target => !HasRelic(target)).ToList();

        // 획득 가능한 유물이 더 이상 없는 경우
        if (acquirableRelics.Count == 0)
        {
            Debug.Log("획득할 수 있는 새로운 유물이 없습니다.");
            return null;
        }

        // 획득 가능한 유물 목록 내에서 무작위 인덱스를 선택합니다.
        int randomIndex = UnityEngine.Random.Range(0, acquirableRelics.Count);
        var relicToAdd = acquirableRelics[randomIndex];

        // 기존의 AddItem 함수를 사용해 인벤토리에 추가합니다.
        AddItem(relicToAdd);

        // Debug.Log($"[InGameItemManager] 유물 '{relicToAdd.relicName}'을(를) 획득했습니다!");

        // 어떤 유물을 얻었는지 알려주기 위해 해당 유물 데이터를 반환합니다.
        return relicToAdd;
    }

    // CJH 코드 추가

    /// <summary>
    /// 특정 '유물 데이터'를 인벤토리에서 이미 소유하고 있는지 확인합니다.
    /// </summary>
    public bool HasRelic(RelicDatas relicData)
    {
        if (relicData == null) return false;
        // _relicInventory 안에 relicData와 동일한 유물이 있는지 확인
        return _relicInventory.Any(item => item.relic == relicData);
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

    private void ClearIsLoaded() => _isLoaded = false;
}