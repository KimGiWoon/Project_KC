public class BranchReward
{
    public enum RewardType
    {
        None,
        Coin,
        DebuffRelic,
        BuffRelic,
        Relic,
    }

    public RewardType Type;
    public int Amount;

    public BranchReward(RewardType type, int amount)
    {
        Type = type;
        Amount = amount;
    }
}