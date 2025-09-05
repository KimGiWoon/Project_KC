using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    // 활성화된 버프 데이터 (내부 관리용)
    public class ActiveBuff
    {
        public Guid id; // 고유 ID 식별자
        public FoodEffectData effect;
        public float remaining;
        public string sourceId;

        public ActiveBuff(FoodEffectData e)
        {
            id = Guid.NewGuid();
            effect = new FoodEffectData(e);
            remaining = e.duration;
            sourceId = e.sourceId;
        }
    }
    // TODO : GameManager에 연결하기.
    // 전투에만 포함되는 매니저. 전투끝나고 노드씬으로 이동 시 사라짐.
    public class BuffManager : MonoBehaviour
    {
        public static BuffManager Instance { get; private set; }
        [SerializeField] BattleManager btManager;

        List<MyCharacterController> downed = new List<MyCharacterController>();

        [Header("UI")]
        [SerializeField] Transform foodContent;
        [SerializeField] GameObject foodBtnPrefab;

        [Header("Debug")]
        [SerializeField] bool logActions = true;
        [SerializeField] List<InventoryItem> testFoodInventory = new List<InventoryItem>(); // 테스트 인벤토리, CookManager의 인벤토리와 연결해야함.
        // 활성 버프 리스트
        List<ActiveBuff> activeBuffs = new List<ActiveBuff>();


        void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            InitFoodIcon();
        }
#if UNITY_EDITOR
        void Start()
        {
            for (int i = 0; i < testFoodInventory.Count; i++)
            {
                CookManager.Instance.AddFood(testFoodInventory[i].recipe);
                if (logActions) Debug.Log($"{testFoodInventory[i].recipe.recipeName}추가됨");
            }
            InitFoodIcon();
        }
#endif
        void Update()
        {
            if (activeBuffs.Count == 0) return;

            float dt = Time.deltaTime;
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                activeBuffs[i].remaining -= dt;
                if (activeBuffs[i].remaining <= 0f)
                {
                    // 만료 시 원상복구
                    if (logActions) Debug.Log($"[BuffManager] 스탯 복구됨 : {activeBuffs[i].effect.type}");
                    RemoveBuffEffect(activeBuffs[i]);
                    activeBuffs.RemoveAt(i);
                }
            }
        }

        void InitFoodIcon()
        {
            for (int i = foodContent.childCount - 1; i >= 0; i--)
            {
                Destroy(foodContent.GetChild(i).gameObject);
            }

            var list = CookManager.Instance.playerFoodInventory;
            // var list = testFoodInventory;
            for (int i = 0; i < list.Count; i++)
            {
                InventoryItem food = list[i];
                if (food == null) continue;

                GameObject go = Instantiate(foodBtnPrefab, foodContent);
                go.name = $"FoodBtn_{i}_{food.recipe.recipeName}";

                Button btn = go.GetComponent<Button>();
                Image img = go.GetComponent<Image>();

                if (img != null && food.recipe.image != null)
                {
                    img.sprite = food.recipe.image;
                    img.enabled = true;
                }
                // 안전한 캡처: 로컬 변수에 담아서 리스너 바인딩
                InventoryItem itemLocal = food;
                GameObject instanceLocal = go;

                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnFoodButtonClicked(itemLocal, instanceLocal));
                }
            }
        }
        // 버튼 클릭 시 호출: recipeLocal을 testFoodInventory에서 제거하고 효과 적용
        void OnFoodButtonClicked(InventoryItem itemLocal, GameObject instanceLocal)
        {
            if (itemLocal == null)
            {
                Debug.LogWarning($"[BuffManager] 클릭된 RecipeData가 null입니다 : {itemLocal}");
                return;
            }

            if (itemLocal.recipe.effects != null)
            {
                foreach (var effect in itemLocal.recipe.effects)
                {
                    ApplyEffectEntry(effect);
                }
            }

            CookManager.Instance.SubtractFood(itemLocal);
            // var testItem = new InventoryItem(recipeLocal);
            // testFoodInventory.Remove(testItem);

            if (logActions) Debug.Log($"[BuffManager] 사용한 음식 삭제: {itemLocal.recipe.recipeName}");

            InitFoodIcon();
        }

        #region 외부 호출

        /// <summary>
        /// 개별 효과 바로 적용(테스트용 또는 내부 호출)
        /// </summary>
        public void ApplyEffectEntry(FoodEffectData e)
        {
            if (e == null) return;
            if (e.duration > 0f) ApplyOrRefreshBuff(e);
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
                case EffectType.CreateBarrierForAll:
                    ApplyCreateBarrierForAll(e);
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

                if (logActions) Debug.Log($"[BuffManager] {p.name} HP : {p._characterState._chaCurrentHP}");

                // p._characterState._chaCurrentHP += p._characterState._chaMaxHP * e.value;
                // if (p._characterState._chaCurrentHP >= p._characterState._chaMaxHP)
                // {
                //     p._characterState._chaCurrentHP = p._characterState._chaMaxHP;
                // }
                p.HPHeal(p._characterState._chaMaxHP * e.value);

                if (logActions) Debug.Log($"[BuffManager] {p.name} HP : {p._characterState._chaCurrentHP}");
            }
            if (logActions) Debug.Log("HP HEAL! TODO : UI 이벤트 함수 연결.");
        }

        void ApplyRestoreManaPercentAll(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] MANA HEAL! : {e.value}");

            foreach (var p in btManager._characters)
            {
                if (!p._isAlive) return;

                if (logActions) Debug.Log($"{p.name} MP : {p._characterState._chaCurrentMP}");

                // p._characterState._chaCurrentMP += p._characterState._chaMaxMP * e.value;
                // if (p._characterState._chaCurrentMP >= p._characterState._chaMaxMP)
                // {
                //     p._characterState._chaCurrentMP = p._characterState._chaMaxMP;
                // }
                p.MPHeal(p._characterState._chaMaxMP * e.value);

                if (logActions) Debug.Log($"{p.name} MP : {p._characterState._chaCurrentMP}");
            }

            if (logActions) Debug.Log("MANA HEAL! TODO : UI 이벤트 함수 연결.");
        }

        void TryReviveRandomAlly(FoodEffectData e)
        {
            downed.Clear();
            foreach (var p in btManager._characters)
            {
                if (p._isAlive) continue;
                downed.Add(p);
            }

            if (downed == null || downed.Count == 0)
            {
                // TODO : 죽은 인원이 없으면 그냥 사용됨.
                if (logActions) Debug.Log($"[BuffManager] REVIVE FAILED! Dead Character Count : {downed.Count}");
                return;
            }

            var chosen = downed[UnityEngine.Random.Range(0, downed.Count)];
            // var chosen = btManager._characters[UnityEngine.Random.Range(0, btManager._characters.Count)];
            if (!chosen._isAlive)
            {
                // chosen.SetActive(true);
                // chosen._isAlive = true;
                // chosen._characterState._chaCurrentHP = chosen._characterState._chaMaxHP * e.value;
                chosen.Revive(chosen._characterState._chaMaxHP * e.value);

                if (logActions) Debug.Log($"[BuffManager] REVIVE! name : {chosen.name}의 ({e.value * 100}%)만큼 HP 재설정 : {chosen._characterState._chaCurrentHP}, 스폰 포인트 지정해야함.");
                if (logActions) Debug.Log("REVIVE! TODO : UI 이벤트 함수 연결, 리스폰 기능 협의 필요");
            }
            else
            {
                if (logActions) Debug.Log($"{e.name} : 이미 부활한 캐릭터({chosen.name})에게 적용됨.");
            }
        }

        void ApplyAccumulateBossGroggy(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] ACCUMULATE BOSS GROGGY! : {e.value}");

            // TODO : MonsterController에 함수 만들 것.
            // foreach (var b in btManager._boosMonsters)
            // {
            //     b.BreakGage += e.value;
            //     if (b.BreakGage >= 1)
            //     {
            //         b.BreakGage = 1;
            //     }
            //     if (logActions) Debug.Log("TODO : UI 이벤트 연결");
            // }
        }

        // Barrier 생성 (모든 아군)
        void ApplyCreateBarrierForAll(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] CREATE BARRIER! : {e.applyBarrier}");
            foreach (var p in btManager._characters)
            {
                p.CreateBarrier(e.applyBarrier);
            }
        }

        void ApplyGroggyBonus(FoodEffectData e)
        {
            if (logActions) Debug.Log($"[BuffManager] GROGGY BONUS! : {e.value}");

            // boss의 TakeDamage 계산식의 그로기 추가피해 += e.value;
            // 예시) 
            // 보스가 그로기 상태일 때는 최종 데미지 n% 추가 피해를 입음.
            // e.value = 0.5 가정.
            // 그로기 상태일때는 70%의 추가 피해를 입음.

            foreach (var p in btManager._characters)
            {
                p.ApplyGroggyBonus(p._characterState._groggyDamage * e.value);
                // 현재 상태 : p.그로기 추가 피해량 = p.공격력 * 1;
            }
        }

        #endregion

        #region 지속형 / 전역형 효과 처리 (등록 / 갱신 / 만료)
        void ApplyOrRefreshBuff(FoodEffectData e)
        {
            // 갱신 정책: 동일 타입 + 동일 sourceId 가 있으면 remaining 갱신 (refresh)
            ActiveBuff existing = activeBuffs.Find(b => b.effect.type == e.type && b.sourceId == e.sourceId);

            if (existing != null && existing.effect != null)
            {
                existing.remaining = e.duration;
                existing.effect.value = e.value;

                int idx = activeBuffs.FindIndex(b => b.id == existing.id);
                if (idx >= 0) activeBuffs[idx] = existing;
                if (logActions) Debug.Log($"[BuffManager] 버프 갱신됨 : {e.type}");
                return;
            }

            // 신규 등록
            ActiveBuff buff = new ActiveBuff(e);
            buff.sourceId = e.sourceId;
            ApplyBuffEffect(buff);
            activeBuffs.Add(buff);
            if (logActions) Debug.Log($"[BuffManager] 버프 추가됨: {e.type}");
        }

        void ApplyBuffEffect(ActiveBuff buff)
        {
            var e = buff.effect;
            if (logActions) Debug.Log($"[BuffManager] 버프 시작! {e.type}");

            switch (e.type)
            {
                case EffectType.AttackBuff:
                    foreach (var p in btManager._characters)
                        if (p._isAlive) p._characterState._chaAttack += e.value;
                    break;
                case EffectType.DefenseBuff:
                    foreach (var p in btManager._characters)
                        if (p._isAlive) p._characterState._chaArmor += e.value;
                    break;
                case EffectType.EnemyAttackDebuff:
                    foreach (var m in btManager._monsters)
                        if (m._isAlive) m._monsterState._monAttack -= e.value;
                    break;
                case EffectType.EnemyDefenseDebuff:
                    foreach (var m in btManager._monsters)
                        if (m._isAlive) m._monsterState._monArmor -= e.value;
                    break;
            }
        }

        void RemoveBuffEffect(ActiveBuff buff)
        {
            var e = buff.effect;
            if (logActions) Debug.Log($"[BuffManager] 버프 삭제됨 : {e.type} ");

            switch (e.type)
            {
                case EffectType.AttackBuff:
                    foreach (var p in btManager._characters)
                        if (p._isAlive) p._characterState._chaAttack -= e.value;
                    break;
                case EffectType.DefenseBuff:
                    foreach (var p in btManager._characters)
                        if (p._isAlive) p._characterState._chaArmor -= e.value;
                    break;
                case EffectType.EnemyAttackDebuff:
                    foreach (var m in btManager._monsters)
                        if (m._isAlive) m._monsterState._monAttack += e.value;
                    break;
                case EffectType.EnemyDefenseDebuff:
                    foreach (var m in btManager._monsters)
                        if (m._isAlive) m._monsterState._monArmor += e.value;
                    break;
            }
        }
    }
    #endregion
}
