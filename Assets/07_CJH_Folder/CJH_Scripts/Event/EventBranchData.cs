using SDW;
using System.Collections.Generic;

public class EventBranchData
{
    public string Text;
    public List<string> Choices;
    public List<string> Results;
    public List<List<BranchReward>> Rewards; // 선택지별 보상
    public EncounterType? Type { get; set; }

    public EventBranchData(string text, List<string> choices, List<string> results, List<List<BranchReward>> rewards = null, EncounterType? type = null)
    {
        Text = text;
        Choices = choices;
        Results = results;
        Rewards = rewards ?? new List<List<BranchReward>>();
        Type = type;
    }
}
