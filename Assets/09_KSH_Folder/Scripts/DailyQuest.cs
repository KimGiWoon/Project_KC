using UnityEngine;

[CreateAssetMenu(fileName = "DailyQuest", menuName = "DailyQuest", order = 1)]
public class DailyQuest : ScriptableObject
{
    [Header("퀘스트 이름")]
    public string questName;
    
    [Header("퀘스트 목표")]
    public QuestType questType;
    public int questGoal;
    
    [Header("완료 테스트")]
    public int currentProgress = 0;
    [HideInInspector] public bool isComplete = false;
}

public enum QuestType
{
    ChallengeDungeon,
    UseFood,
    GetArtifact,
    UseStemina,
    LevelUp,
    SkillLevelUp,
    ChargeStemina
}
