using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    // TODO : GameManager에 연결하기.
    // 매 전투마다 초기화 되기 때문에, DontDestroyOnLoad일 필요가 없다.
    public class BuffManager : MonoBehaviour
    {
        public static BuffManager Instance { get; private set; }
        [SerializeField] BattleManager btManager;
        [SerializeField] Transform foodContent;
        [SerializeField] GameObject foodBtnPrefab;
        [SerializeField] List<RecipeData> testFoodInventory = new List<RecipeData>();

        Coroutine coroutine;
        List<MyCharacterController> downed;

        // 디버그 옵션
        [Header("Debug")]
        [SerializeField] bool logActions = true;


        void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            if (coroutine != null)
            {
                coroutine = null;
                StopCoroutine(coroutine);
            }
            InitFoodIcon();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        void InitFoodIcon()
        {
            var btn = foodBtnPrefab.GetComponent<Button>();
            if (btn == null) return;
            for (int i = 0; i < testFoodInventory.Count; i++)
            {
                Instantiate(foodBtnPrefab, foodContent);
                btn.onClick.AddListener(() => ApplyEffectEntry(testFoodInventory[i].effects[0]));
            }
        }

        #region 외부 호출

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
        }

        #endregion

        #region 즉시 적용 함수들 (Instant effects)

        void ApplyInstantHealAll(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] HP HEAL! : {e.value}");

            foreach (var p in btManager._characters)
            {
                if (!p._isAlive) return;

                p._currentHp += e.value;
                if (p._currentHp >= p._characterData._maxHp)
                {
                    p._currentHp = p._characterData._maxHp;
                }
            }
        }

        void ApplyRestoreManaPercentAll(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] MANA HEAL! : {e.value}");

            foreach (var p in btManager._characters)
            {
                if (!p._isAlive) return;

                p._currentMp += (int)(e.value * (1 / p._characterData._maxMp));
                if (p._currentMp >= p._characterData._maxMp)
                {
                    p._currentMp = p._characterData._maxMp;
                }
            }
        }

        void TryReviveRandomAlly(FoodEffectData e)
        {
            // downed.Clear();
            // foreach (var p in btManager._characters)
            // {
            //     if (p._isAlive) return;
            //     downed.Add(p);
            // }

            // if (downed == null || downed.Count == 0)
            // {
            //     // TODO : 죽은 인원이 없으면 그냥 사용됨.
            //     return;
            // }

            // var chosen = downed[UnityEngine.Random.Range(0, downed.Count)];
            var chosen = btManager._characters[UnityEngine.Random.Range(0, btManager._characters.Count)];
            if (!chosen._isAlive)
            {
                chosen._isAlive = true;
                chosen._currentHp += (int)(e.value * (1 / chosen._characterData._maxHp));
                if (logActions) Debug.Log($"[BuffManager] REVIVE! name : {chosen.name} HP : ({e.value}%), 스폰 포인트 지정해야함.");
            }
            else
            {
                if (logActions) Debug.Log($"{e.name} : 이미 부활한 캐릭터({chosen.name})에게 적용됨.");
            }
        }

        void ApplyAccumulateBossGroggy(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] ACCUMULATE BOSS GROGGY! : {e.value}");

            // boss의 그로기 게이지 적립
        }

        // Barrier 생성 (모든 아군)
        void ApplyCreateBarrierForAll(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] CREATE BARRIER! : {e.value}");

            // foreach (var p in _characters)
            // {
            //     p.barrierCount += e.value;
            // }
        }

        void ApplyGroggyBonus(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] GROGGY BONUS! : {e.value}");

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
        // 효과 발동 -> e.duration초 이전에 게임이 끝나면?
        // player의 스탯을 받아오는 스크립트에서 스탯을 변경하는 함수를 만들기?

        // 아군 버프 (공격/방어)
        IEnumerator ApplyAllyBuff(FoodEffectData e)
        {
            if (e.type == EffectType.AttackBuff)
            {
                if (logActions) Debug.Log($"[BuffManager] ATTACK BUFF! : {e.value}");

                foreach (var p in btManager._characters)
                {
                    p._characterData._attackDamage += e.value;
                    if (logActions) Debug.Log($"[BuffManager] {p.name} : {p._characterData._attackDamage}");

                    yield return new WaitForSeconds(e.duration);
                    p._characterData._attackDamage -= e.value;
                    if (logActions) Debug.Log($"[BuffManager] {p.name} : {p._characterData._attackDamage}");
                }
            }
            else if (e.type == EffectType.DefenseBuff)
            {
                if (logActions) Debug.Log($"[BuffManager] DEFENSE BUFF! : {e.value}");

                foreach (var p in btManager._characters)
                {
                    p._characterData._attackDefense += e.value;
                    if (logActions) Debug.Log($"[BuffManager] {p.name} : {p._characterData._attackDefense}");

                    yield return new WaitForSeconds(e.duration);
                    p._characterData._attackDefense -= e.value;
                    if (logActions) Debug.Log($"[BuffManager] {p.name} : {p._characterData._attackDefense}");
                }
            }
            else
            {
                // 기타 유형 처리
            }
            yield return null;
        }


        // 적 디버프 (전역형으로 적 전체에 적용)
        IEnumerator ApplyEnemyDebuff(FoodEffectData e)
        {
            if (e.type == EffectType.EnemyAttackDebuff)
            {
                if (logActions) Debug.Log($"[BuffManager] ENEMY ATTACK DEBUFF! : {e.value}");

                foreach (var m in btManager._monsters)
                {
                    m._monsterData._attackDamage -= e.value;
                    if (logActions) Debug.Log($"[BuffManager] {m.name} : {m._monsterData._attackDamage}");

                    yield return new WaitForSeconds(e.duration);
                    m._monsterData._attackDamage += e.value;
                    if (logActions) Debug.Log($"[BuffManager] {m.name} : {m._monsterData._attackDamage}");
                }
            }
            else if (e.type == EffectType.EnemyDefenseDebuff)
            {
                if (logActions) Debug.Log($"[BuffManager] ENEMY DEFENSE DEBUFF! : {e.value}");

                foreach (var m in btManager._monsters)
                {
                    m._monsterData._attackDefense -= e.value;
                    if (logActions) Debug.Log($"[BuffManager] {m.name} : {m._monsterData._attackDefense}");

                    yield return new WaitForSeconds(e.duration);
                    m._monsterData._attackDefense += e.value;
                    if (logActions) Debug.Log($"[BuffManager] {m.name} : {m._monsterData._attackDefense}");
                }
            }
            else
            {
                // 기타 유형 처리
            }
        }
    }

    #endregion
}
