using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

namespace KSH
{
    public class DailyQuestManager : SingletonManager<DailyQuestManager>
{
    [SerializeField] private List<DailyQuest> dailyQuests = new List<DailyQuest>();
    [SerializeField] private Button rewardButton;
    private QuestUI[] questUIList;
    private bool reward = false;
    public int currentQuestGoal = 3;

    public event Action OnQuestComplete;

    protected override void Awake()
    {
        base.Awake();
        InitQuest();
        AddQuestUI();
    }

    private void Start()
    {
        rewardButton.onClick.AddListener(Reward);
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDailyReset += InitQuest;
        }
    }
    
    protected override void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDailyReset -= InitQuest;
        }
        
        base.OnDestroy();
    }

    private void Update() //테스트용
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CompleteQuest(QuestType.ChallengeDungeon, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CompleteQuest(QuestType.UseFood, 3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CompleteQuest(QuestType.GetArtifact, 5);
        }
    }
    
    public void AddQuestUI()
    {
        //퀘스트UI 이름에서 숫자만 뽑아서 순서대로 나열하여 저장
        questUIList = FindObjectsOfType<QuestUI>().OrderBy((q => ExtractNumber(q.gameObject.name))).ToArray();
        
        for (int i = 0; i < dailyQuests.Count; i++) // dailyQuest의 크기만큼 반복
        {
            questUIList[i].dailyQuest = dailyQuests[i]; //리스트 i번째 UI에 i번째 퀘스트 데이터 연결
            questUIList[i].InitUI(); //연결한 걸 기반으로 초기화
        }
    }

    public void InitQuest() //퀘스트 초기화
    {
        foreach (var quest in dailyQuests)
        {
            quest.isComplete = false;
            quest.currentProgress = 0;
        }
        
        rewardButton.interactable = false;
        reward = false;
        Debug.Log("리셋");
    }

    public int ExtractNumber(string name) //이름에서 숫자만 뽑기
    {
        string number = new string(name.Where(char.IsDigit).ToArray()); //문자열안에서 숫자만 뽑은 후 문자 배열로 변환하여 문자열로 합침
        return int.TryParse(number, out int result) ? result : 0; //정수로 변환
    }
    
    public void CompleteQuest(QuestType questType, int amount) //퀘스트가 완료되었는지 확인
    {
        foreach (var quest in dailyQuests)
        {
            if(quest.questType != questType || quest.isComplete) // 이미 완료된 퀘스트는 건너뛰기
                continue;
            
            quest.currentProgress += amount; //해당 퀘스트의 진행도 추가

            if (quest.currentProgress >= quest.questGoal) //퀘스트 목표가 같거나 높으면
            {
                Debug.Log("퀘스트완료");
                quest.isComplete = true; //완료
                OnQuestComplete?.Invoke();
                CheckQuests();
            }
        }
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
            rewardButton.interactable = true; //버튼 활성화
        }
        else
        {
            rewardButton.interactable = false;
        }
    }
    
    public void Reward()
    {
        if (!reward)
        {
            //보상지급적어야함
            Debug.Log("보상이 지급되었습니다.");
            reward = true;
            rewardButton.interactable = false;
        }
    }
}
    
}