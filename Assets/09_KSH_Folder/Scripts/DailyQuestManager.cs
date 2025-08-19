using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestManager : MonoBehaviour
{
    [SerializeField] private List<DailyQuest> dailyQuests = new List<DailyQuest>();
    public int currentQuestGoal = 3;
    
    public void CompleteQuest(QuestType questType, int amount) //퀘스트가 완료되었는지 확인
    {
        foreach (var quest in dailyQuests)
        {
            if(quest.questType != questType || quest.isComplete) // 이미 완료된 퀘스트는 건너뛰기
                continue;
            
            quest.currentProgress += amount;

            if (quest.currentProgress >= quest.questGoal) //퀘스트 목표가 같거나 높으면
            {
                quest.isComplete = true; //완료
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

        if (completedQuests >= currentQuestGoal) //완료된 퀘스트가 3회 이상이거나 같으면
        {
            Reward();
        }
    }
    
    public void Reward()
    {
        //보상지급
        Debug.Log("보상이 지급되었습니다.");
    }
}
