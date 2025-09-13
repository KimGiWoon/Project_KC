using System.Collections.Generic;

public static class EventBranches
{
    public static readonly Dictionary<(int id, int choiceIndex), EventBranchData> Map = new()
    {
        {
            (6001, 0),
            new EventBranchData(
                "로브를 뒤집어 쓰고 있던 어떤 수상해보이는 남성이 자신이 좋은 것을 가지고 있다며 사겠냐고 물어봅니다",
                new List<string> { "일단 사고보자(엽전 -500, 랜덤 유물 1개 휙득)", "안 산다" },
                new List<string> { "유물을 하나 얻었다", "그냥 지나간다" },
                new List<List<BranchReward>>
                {
                    // 선택지 0번: -500엽전, 랜덤 유물
                    new List<BranchReward>
                    {
                        new BranchReward(BranchReward.RewardType.Coin, -500),
                        new BranchReward(BranchReward.RewardType.Relic, 1)
                    },
                    // 선택지 1번: 보상 없음
                    new List<BranchReward>()
                }
            )
                },
        
        {
            (6005, 0),
            new EventBranchData(
                "당신은 유물을 꺼내 보입니다. 상대는 기뻐하며 가격을 제시합니다.",
                new List<string> { "바로 판다", "좀 더 비싸게 불러본다" },
                new List<string> { "엽전 +500", "엽전 +1000 (상대가 잠시 망설이더니 OK)" },
                    new List<List<BranchReward>>
        {
            // 선택지 0번: -500엽전, 랜덤 유물
            new List<BranchReward>
            {
                new BranchReward(BranchReward.RewardType.Coin, -500),
                new BranchReward(BranchReward.RewardType.BuffRelic, 1)
            },
            // 선택지 1번: 보상 없음
            new List<BranchReward>()
        }
            )
        },

        {
            (6008, 0),
            new EventBranchData(
                "당신은 전투를 시작합니다! 준비는 되었는가?",
                new List<string> { "주먹으로 싸운다", "무기를 꺼낸다" },
                new List<string> { "상대를 멋지게 제압했다! 엽전 +500", "너무 압도적이었다! 엽전 +800" }
            )
        },

        {
            (6011, 0),
            new EventBranchData(
                "그 도발을 참지 못한 당신은 도전을 받아들입니다. 전투 준비!",
                new List<string> { "주먹만으로 싸운다", "유물을 사용한다" },
                new List<string> { "당신은 맨주먹으로도 강했다!", "유물의 힘으로 승리했다!" }
            )
        },

        {
            (7001, 1),
            new EventBranchData(
                "무뢰배에게 맞설 준비를 한다. 어떻게 싸울까?",
                new List<string> { "협상을 시도한다", "먼저 기습한다" },
                new List<string> { "협상은 실패했지만 도망쳤다", "성공적인 기습! 적 제압 완료" }
            )
        }
    };
}