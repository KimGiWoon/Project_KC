using System;
using System.Collections.Generic;
using UnityEngine;

namespace JJY
{
    // TODO : GameManager에 연결하기.
    public class BuffManager : MonoBehaviour
    {
        public static BuffManager Instance { get; private set; }

        #region 내부 타입

        // 런타임으로 관리되는 활성화된 효과 인스턴스, 이거 왜 필요한지 공부
        class EffectInstance
        {
            public FoodEffectData entry;    // 원본 데이터(참조)
            public float remaining;         // 남은 시간 (초)
            public int stacks;              // 현재 스택 수
            public string id;               // 디버깅용 식별자

            // 어떤 대상을 위해 등록했는지를 보관할 수 있음.
            // null == 전역(All targets 등) 혹은 special 처리
            public GameObject target;       // (선택): 특정 캐릭터/몬스터 대상
        }

        #endregion

        // 활성 효과 목록
        List<EffectInstance> activeEffects = new List<EffectInstance>();

        // UI / 다른 시스템이 구독해서 업데이트를 반영하도록 이벤트 제공
        public event Action OnBuffsChanged;

        // 디버그 옵션
        [Header("Debug")]
        [SerializeField] bool logActions = false;

        void Awake()
        {
            // 모든 캐릭터의 스탯들은 스테이지 시작 시 저장하고, 음식의 value는 저장된 값을 바꾸는 형식
            // 예시 character.cs)
            // int hp = 캐릭터 정보(SCV로 받아온 값);
            // characterHP = hp;
            // 회복 음식 섭취
            // characterHP += value;

            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        #region 외부 호출 API

        /// <summary>
        /// 개별 효과 바로 적용(테스트용 또는 내부 호출)
        /// </summary>
        public void ApplyEffectEntry(FoodEffectData e)
        {
            if (e == null) return;
            switch (e.type)
            {
                // --- 즉시 효과(Instant) ---
                case EffectType.InstantHealAll:
                    ApplyInstantHealAll(e);
                    break;
                case EffectType.RestoreManaPercentAll:
                    ApplyRestoreManaPercentAll(e);
                    break;
                case EffectType.ReviveRandomAllyPercentHP:
                    TryReviveRandomAlly(e);
                    break;
                case EffectType.AccumulateBossGroggyPercent:
                    ApplyAccumulateBossGroggy(e);
                    break;
                case EffectType.BonusDamageToGroggyMonsters:
                    ApplyGroggyBonus(e);
                    break;

                // --- 지속형: 아군 버프 ---
                case EffectType.AttackBuff:
                case EffectType.DefenseBuff:
                    ApplyAllyBuff(e);
                    break;

                // --- 지속형: 적 디버프 ---
                case EffectType.EnemyAttackDebuff:
                case EffectType.EnemyDefenseDebuff:
                    ApplyEnemyDebuff(e);
                    break;

                case EffectType.CreateBarrierForAll:
                    ApplyCreateBarrierForAll(e);
                    break;

                default:
                    Debug.LogWarning($"[BuffManager] : {e.type} 타입을 확인해주세요.");
                    break;
            }
            OnBuffsChanged?.Invoke();
        }

        #endregion

        #region 즉시 적용 함수들 (Instant effects)

        void ApplyInstantHealAll(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] Heal! : {e.value}");

            // foreach (var p in 모든 캐릭터())
            // {
            //      p.현재 체력 += e.value;
            //      if (p.현재 체력 >= p.최대체력)
            //      {
            //          p.현재 체력 = p.최대체력
            //      }
            // }
        }

        void ApplyRestoreManaPercentAll(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] Mana Heal! : {e.value}%");
            // foreach (var p in 모든 캐릭터())
            // {
            //      p.현재 마나 += e.value;
            //      if (p.현재 마나 >= p.최대 마나)
            //      {
            //          p.현재 마나 = p.최대 마나
            //      }
            // }
        }

        void TryReviveRandomAlly(FoodEffectData e)
        {
            // var downed = 파티원들중 죽은 플레이어 리스트
            // if (downed == null || downed.Count == 0)
            // {
            //     // TODO : 죽은 인원이 없으면 어떻게 할 것인가.
            //     return;
            // }

            // var chosen = downed[UnityEngine.Random.Range(0, downed.Count)];
            // chosen.현재 체력 += e.value * {1 / 최대 체력};
            // chosen.isDead = false;
            // if (logActions) Debug.Log($"[BuffManager] Revive! name : {chosen.name} HP : ({e.value}%)");
        }

        void ApplyAccumulateBossGroggy(FoodEffectData e)
        {
            // boss의 그로기 게이지 적립
        }

        // Barrier 생성 (모든 아군)
        void ApplyCreateBarrierForAll(FoodEffectData e)
        {
            // foreach (캐릭터 p in 모든 캐릭터)
            // {
            //     p.베리어카운트 += e.value;
            // }
        }

        void ApplyGroggyBonus(FoodEffectData e)
        {
            // boss의 TakeDamage 계산식의 그로기 추가피해 += e.value;
            // 예시) 
            // 보스가 그로기 상태일 때는 최종 데미지 20% 추가 피해를 입음.
            // e.value = 15 가정.
            // 그로기 상태일때는 35%의 추가 피해를 입음.
            //
            // foreach (보스 몬스터 m in 모든 보스 몬스터)
            // {
            //     m.그로기 상태의 추가 데미지 상수 += e.value 하는 함수(e.value, e.duration);
            //     e.duration 이후에 원상 복귀.
            // }
        }

        #endregion

        #region 지속형 / 전역형 효과 처리 (등록 / 갱신 / 만료)

        // 아군 버프 (공격/방어)
        void ApplyAllyBuff(FoodEffectData e)
        {
            // if (e.type == EffectType.AttackBuff)
            // {
            //     foreach (var p in 모든 캐릭터)
            //     {
            //         p.공격력 += e.value;
            //         e.duration 이후에 스탯 복귀
            //         p.공격력 -= e.value;
            //     }
            // }
            // else if (e.type == EffectType.DefenseBuff)
            // {
            //     foreach (var p in 모든 캐릭터)
            //     {
            //         p.방어력 += e.value;
            //         e.duration 이후에 스탯 복귀
            //         p.방어력 -= e.value;
            //     }
            // }
            // else
            // {
            //     // 기타 유형 처리
            // }
        }


        // 적 디버프 (전역형으로 적 전체에 적용)
        void ApplyEnemyDebuff(FoodEffectData e)
        {
            //     if (e.type == EffectType.AttackDeBuff)
            //     {
            //          foreach (var m in 모든 몬스터)
            //          {
            //              m.공격력 -= e.value;
            //          }
            //     }
            //     else if (e.type == EffectType.DefenseDeBuff)
            //     {
            //          foreach (var p in 모든 몬스터)
            //          {
            //              m.방어력 -= e.value;
            //          }
            //     }
            //     else
            //     {
            //         // 기타 유형 처리
            //     }
            // }
        }

        #endregion
    }
}
