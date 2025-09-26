using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net.Http;
using UnityEngine.UI;
using SDW;
using KSH;

public class DailyQuestManager : MonoBehaviour
{
    [SerializeField] private List<DailyQuest> dailyQuests = new List<DailyQuest>();
    private List<QuestUI> questUIList;
    private bool reward = false;
    public int currentQuestGoal = 3;
    private bool _canReward = false;
    private bool _isDownloaded;
    private GameManager _gameManager;
    private Dictionary<QuestType, int> _roguelikeUpdate = new Dictionary<QuestType, int>();
    private Dictionary<QuestType, int> _lobbylikeUpdate = new Dictionary<QuestType, int>();

    public event Action<int> OnStarCandyChange;

    private void Start()
    {
        _gameManager = GameManager.Instance;

        if (_gameManager.Time != null)
        {
            _gameManager.Time.OnDailyReset += InitQuest;
        }
    }

    private void OnDisable()
    {
        if (_gameManager == null) return;
        if (_gameManager.Time != null)
        {
            _gameManager.Time.OnDailyReset -= InitQuest;
        }
    }

    // private void Update() //테스트용
    // {
    //     if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
    //         !_gameManager.Firebase.IsLoaded || _isDownloaded) return;
    // }

    public void AddQuestUI(DailyQuestUI dailyQuestUI)
    {
        //퀘스트UI 이름에서 숫자만 뽑아서 순서대로 나열하여 저장
        questUIList = dailyQuestUI.GetComponentsInChildren<QuestUI>(true).OrderBy(q => ExtractNumber(q.gameObject.name)).ToList();

        for (int i = 0; i < dailyQuests.Count; i++) // dailyQuest의 크기만큼 반복
        {
            if (dailyQuests[i] == null) break;
            questUIList[i].dailyQuest = dailyQuests[i]; //리스트 i번째 UI에 i번째 퀘스트 데이터 연결
            questUIList[i].InitUI(); //연결한 걸 기반으로 초기화
            questUIList[i].UpdateCountText(dailyQuests[i]);
        }

        InitQuestFromDatabase();

        CompleteQuest(QuestType.GameLogin, 1);
    }

    private void ClearQuestUI() => questUIList?.Clear();

    public void InitQuest() //퀘스트 초기화
    {
        foreach (var quest in dailyQuests)
        {
            quest.isComplete = false;
            quest.currentProgress = 0;
        }

        _gameManager.Firebase.InitQuest();
        ClearQuestUI();
        _canReward = false;
        reward = false;
        Debug.Log("리셋");
    }

    //todo quest 저장 시 db에 저장하도록 수정해야 함
    private void InitQuestFromDatabase()
    {
        int completedQuestCount = 0;
        var dbQuestDictionary = _gameManager.Firebase.DailyQuest as Dictionary<string, object>;
        var dbQuestProgressDictionary = _gameManager.Firebase.DailyQuestProgress as Dictionary<string, object>;
        for (int i = 0; i < dailyQuests.Count; i++)
        {
            if (Convert.ToBoolean(dbQuestDictionary[dailyQuests[i].questType.ToString()]))
            {
                dailyQuests[i].isComplete = true;
                completedQuestCount++;
                dailyQuests[i].currentProgress = dailyQuests[i].questGoal;
                questUIList[i].InitUI(); //연결한 걸 기반으로 초기화
                questUIList[i].dailyQuest = dailyQuests[i]; //리스트 i번째 UI에 i번째 퀘스트 데이터 연결
                questUIList[i].UpdateCountText(dailyQuests[i]);
                questUIList[i].CheckUI();
            }
            else
            {
                dailyQuests[i].isComplete = false;
                dailyQuests[i].currentProgress = Convert.ToInt32(dbQuestProgressDictionary[dailyQuests[i].questType.ToString()]);
                questUIList[i].InitUI(); //연결한 걸 기반으로 초기화
                questUIList[i].dailyQuest = dailyQuests[i]; //리스트 i번째 UI에 i번째 퀘스트 데이터 연결
                questUIList[i].UpdateCountText(dailyQuests[i]);
            }
        }

        if (Convert.ToBoolean(dbQuestDictionary["GetReward"]))
        {
            _canReward = false;
            reward = true;
        }
        else if (completedQuestCount >= 3)
        {
            _canReward = true;
            reward = false;
        }
        else
        {
            _canReward = false;
            reward = false;
        }

        CheckQuests();
        _isDownloaded = true;
    }

    public int ExtractNumber(string name) //이름에서 숫자만 뽑기
    {
        string number = new string(name.Where(char.IsDigit).ToArray()); //문자열안에서 숫자만 뽑은 후 문자 배열로 변환하여 문자열로 합침
        return int.TryParse(number, out int result) ? result : 0; //정수로 변환
    }

    public void CompleteQuest(QuestType questType, int amount) //퀘스트가 완료되었는지 확인
    {
        var dbQuestDictionary = _gameManager.Firebase.DailyQuest as Dictionary<string, object>;
        var dbQuestProgressDictionary = _gameManager.Firebase.DailyQuestProgress as Dictionary<string, object>;

        for (int i = 0; i < dailyQuests.Count; i++)
        {
            var quest = dailyQuests[i];
            if (quest.questType != questType || quest.isComplete) // 이미 완료된 퀘스트는 건너뛰기
                continue;

            quest.currentProgress += amount; //해당 퀘스트의 진행도 추가

            // if(questUIList != null)
            questUIList[i].UpdateCountText(quest); //업데이트 UI

            if (quest.currentProgress >= quest.questGoal) //퀘스트 목표가 같거나 높으면
            {
                Debug.Log("퀘스트완료");
                quest.isComplete = true; //완료
                questUIList[i].CheckUI();
                CheckQuests();
            }

            if (Convert.ToBoolean(dbQuestDictionary[dailyQuests[i].questType.ToString()]) == quest.isComplete) continue;
            if (Convert.ToInt32(dbQuestProgressDictionary[dailyQuests[i].questType.ToString()]) == quest.currentProgress)
                continue;

            _gameManager.Firebase.SetQuestState(quest.questType, quest.isComplete, quest.currentProgress);
        }
    }

    public void CompleteQuestInRoguelikeScene(QuestType questType, int amount)
    {
        _roguelikeUpdate[questType] = amount;
    }

    public void CompleteQuestInLobbyScene(QuestType questType, int amount)
    {
        _lobbylikeUpdate[questType] = amount;
    }

    public void UpdateFromRoguelikeScene()
    {
        foreach (var completedQuest in _roguelikeUpdate)
        {
            CompleteQuest(completedQuest.Key, completedQuest.Value);
        }
        _roguelikeUpdate.Clear();
    }

    public void UpdateFromLobbyScene()
    {
        foreach (var completedQuest in _lobbylikeUpdate)
        {
            CompleteQuest(completedQuest.Key, completedQuest.Value);
        }
        _lobbylikeUpdate.Clear();
    }

    public void CheckQuests() //퀘스트 3회 이상 완료되었는지 확인
    {
        int completedQuests = 0;

        foreach (var quest in dailyQuests)
        {
            if (quest.isComplete) //퀘스트가 완료이면
                completedQuests++;
        }

        if (!reward && completedQuests >= currentQuestGoal) //완료된 퀘스트가 3회 이상이거나 같으면
        {
            _canReward = true;
        }
        else
        {
            _canReward = false;
        }
    }

    public void Reward(int rewardAmount)
    {
        if (!reward)
        {
            GameManager.Instance.Coin.SetStarCandy(GameManager.Instance.Coin.starCandy + rewardAmount);
            OnStarCandyChange?.Invoke(GameManager.Instance.Coin.starCandy);
            reward = true;
            _canReward = false;
            _gameManager.Firebase.SetQuestReward(reward);
        }
    }

    public bool CanReward() => _canReward;
}