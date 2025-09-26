using UnityEngine;

namespace JJY
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RecipeData/FoodEffectData")]
    public class FoodEffectData : ScriptableObject
    {
        public EffectType type;
        public float value = 0f;
        public bool applyBarrier = false;
        public float duration = 0f;      // 0 => 즉시 적용, >0 => 지속(초)
        public string sourceId;         // 출처 식별자 (예: recipeName 등, 갱신 정책용)

        public FoodEffectData() { }

        // 복사 생성자 (안전하게 복제해서 보관)
        public FoodEffectData(FoodEffectData other)
        {
            if (other == null) return;
            type = other.type;
            value = other.value;
            duration = other.duration;
            sourceId = other.sourceId;
        }
    }

    public enum EffectType
    {
        // 아군 관련
        AttackBuff,                  // 공격력 대폭 증가
        DefenseBuff,                 // 방어력 대폭 증가
        InstantHealAll,              // 전투 중인 모든 캐릭터 체력 즉시 회복
        RestoreManaPercentAll,       // 전투중인 캐릭터들의 마나를 (n)% 회복

        // 적에게 적용되는 디버프
        EnemyAttackDebuff,           // 적 공격력 디버프
        EnemyDefenseDebuff,          // 적 방어력 디버프

        // 특수 작용
        ReviveRandomAllyPercentHP,   // 전투 불가 상태의 캐릭터 한명을 랜덤으로 (최대 체력%) 부활
        BonusDamageToGroggyMonsters, // 그로기 상태의 몬스터에게 추가 피해
        CreateBarrierForAll,         // 전투 중인 캐릭터들에게 몬스터의 공격을 1회 방어할 수 있는 베리어 생성
        BrokenBarrierForAll,
        AccumulateBossGroggyPercent, // 보스 몬스터 그로기 게이지 즉시 (n)% 누적
    }
    // public enum Target
    // {
    //     CharacterAll,
    //     Boss
    // }
}
