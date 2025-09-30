using System.Collections.Generic;
using UnityEngine;
using SDW;
using System.Collections;
using System.Linq;
using JJY;

public class BuffRelicManager : MonoBehaviour
{
    public static BuffRelicManager Instance;
    [SerializeField] private BattleManager battleManager;
    private List<MyCharacterController> myCharacterController;
    private List<MonsterController> monsterController;

    private Dictionary<MyCharacterController, CharacterState> baseStates =
        new Dictionary<MyCharacterController, CharacterState>();

    private RelicDatas currentRelic;
    private int attackCount = 0;
    private int aliveCount = 0;
    private List<RelicDatas> addRewardList = new List<RelicDatas>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CacheBaseState();
    }

    private void Start()
    {
        battleManager.OnCharacterSpawned += CharacterSpawned;
        RoguelikeManager.Instance.OnBattleEnd += BattleEnd;
        GameManager.Instance.Coin.OnRelicChanged = null;
    }

    private void OnDisable()
    {
        battleManager.OnCharacterSpawned -= CharacterSpawned;
        RoguelikeManager.Instance.OnBattleEnd -= BattleEnd;
    }

    // private void Awake()
    // {
    //     base.Awake();
    //     CacheBaseState();
    // }

    public void CharacterSpawned()
    {
        CacheBaseState();
        ResetAll();
        addRewardList.Clear();
        ApplyAll();
    }

    public void BattleEnd()
    {
        ResetAll();
    }

    private void CacheBaseState() //캐릭터 기본 스탯 캐싱
    {
        baseStates.Clear();
        foreach (var p in battleManager._characters)
        {
            var state = new CharacterState
            {
                _chaAtkSpeed = p._characterState._chaAtkSpeed,
                _chaAttack = p._characterState._chaAttack,
                _chaAvoid = p._characterState._chaAvoid,
                _chaCritDmg = p._characterState._chaCritDmg,
                _chaAccuracy = p._characterState._chaAccuracy,
                _chaArmor = p._characterState._chaArmor,
                _chaMaxHP = p._characterState._chaMaxHP,
                _chaMPRecovery = p._characterState._chaMPRecovery
            };

            baseStates[p] = state;
        }
    }

    private void ResetState(MyCharacterController p)
    {
        if (baseStates.TryGetValue(p, out var state))
        {
            p._characterState._chaAtkSpeed = state._chaAtkSpeed;
            p._characterState._chaAttack = state._chaAttack;
            p._characterState._chaAvoid = state._chaAvoid;
            p._characterState._chaCritDmg = state._chaCritDmg;
            p._characterState._chaAccuracy = state._chaAccuracy;
            p._characterState._chaArmor = state._chaArmor;
            p._characterState._chaMaxHP = state._chaMaxHP;
            p._characterState._chaMPRecovery = state._chaMPRecovery;
        }
    }

    private void ResetAll()
    {
        foreach (var p in battleManager._characters)
        {
            ResetState(p);
        }
    }

    private void ApplyAll()
    {
        foreach (var r in GameManager.Instance.InGameItem.relicInventory)
        {
            ApplyRelicEffect(r.relic);
        }
    }

    private float AddStat(float stat, float percent) => stat * (percent / 100f);

    public void ApplyStatToMonster(MonsterController m, RelicDatas relic)
    {
        if (relic.monArmor != 0)
        {
            m._monsterState._monArmor += AddStat(m._monsterState._monArmor, relic.monArmor);
            // Debug.Log(
            //     $"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 방어력 +{relic.monArmor}% → 최종 {m._monsterState._monArmor}");
        }

        if (relic.monHP != 0)
        {
            m._monsterState._monMaxHP += AddStat(m._monsterState._monMaxHP, relic.monHP);
            // Debug.Log(
            //     $"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 최대체력 +{relic.monHP}% → 최종 {m._monsterState._monMaxHP}");
        }

        if (relic.monAttack != 0)
        {
            m._monsterState._monAttack += AddStat(m._monsterState._monAttack, relic.monAttack);
            // Debug.Log(
            //     $"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 공격력 +{relic.monAttack}% → 최종 {m._monsterState._monAttack}");
        }

        if (relic.monAtkSpeed != 0)
        {
            m._monsterState._monAtkSpeed += AddStat(m._monsterState._monAtkSpeed, relic.monAtkSpeed);
            // Debug.Log(
            //     $"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 공격속도 +{relic.monAtkSpeed}% → 최종 {m._monsterState._monAtkSpeed}");
        }
    }

    public void ApplySingleToCharacter(MyCharacterController p, RelicDatas relic)
    {
        if (relic.chaAttack != 0) //공격력
        {
            p._characterState._chaAttack += AddStat(p._characterState._chaAttack, relic.chaAttack);
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격력 +{relic.chaAttack}% → 최종 {p._characterState._chaAttack}");
        }

        if (relic.chaArmor != 0) //방어력
        {
            p._characterState._chaArmor += relic.chaArmor;
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 방어력 +{relic.chaArmor}% → 최종 {p._characterState._chaArmor}");
        }

        if (relic.chaAtkSpeed != 0) //공격 속도
        {
            p._characterState._chaAtkSpeed -= AddStat(p._characterState._chaAtkSpeed, relic.chaAtkSpeed);
            Debug.Log(
                $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격속도 +{relic.chaAtkSpeed}% → 최종 {p._characterState._chaAtkSpeed}");
        }

        if (relic.chaHP != 0) //최대 체력
        {
            p._characterState._chaMaxHP += AddStat(p._characterState._chaMaxHP, relic.chaHP);
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 최대 체력 +{relic.chaHP}% → 최종 {p._characterState._chaMaxHP}");
        }

        if (relic.chaMPRecovery != 0) //마나 회복량
        {
            p._characterState._chaMPRecovery *= 1f + relic.chaMPRecovery / 100f;
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 마나 회복량 +{relic.chaMPRecovery}% → 최종 {p._characterState._chaMPRecovery}");
        }

        if (relic.chaCritDmg != 0) //치명타 데미지
        {
            p._characterState._chaCritDmg += AddStat(p._characterState._chaCritDmg, relic.chaCritDmg);
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 치명타 데미지 +{relic.chaCritDmg}% → 최종 {p._characterState._chaCritDmg}");
        }

        if (relic.chaAccuracy != 0) //명중률
        {
            p._characterState._chaAccuracy += AddStat(p._characterState._chaAccuracy, relic.chaAccuracy);
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 명중률 +{relic.chaAccuracy}% → 최종 {p._characterState._chaAccuracy}");
        }

        if (relic.chaAvoid != 0) //회피율
        {
            p._characterState._chaAvoid += AddStat(p._characterState._chaAvoid, relic.chaAvoid);
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.chaAvoid}: 회피율 +{relic.chaAvoid}% → 최종 {p._characterState._chaAvoid}");
        }

        if (relic.chaAvoid != 0) //회피율
        {
            p._characterState._chaAvoid += AddStat(p._characterState._chaAvoid, relic.chaAvoid);
            // Debug.Log(
            //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.chaAvoid}: 회피율 +{relic.chaAvoid}% → 최종 {p._characterState._chaAvoid}");
        }
    }

    public void ApplyStatToCharacter(RelicDatas relic) //기본 스탯 ++
    {
        foreach (var p in battleManager._characters)
        {
            ApplySingleToCharacter(p, relic);
        }
    }

    public void ApplyRelicEffect(RelicDatas relic) //기본 스탯 적용
    {
        currentRelic = relic;

        switch (relic.relicTarget)
        {
            case RelicTarget.Character: //유물 적용 대상이 Character
                //기본 스탯만 올라가는 유물들
                if (relic.relicRole == RelicRole.None && relic.relicIsPassive)
                {
                    if (relic.relicType == RelicType.None)
                    {
                        if (relic.chaDrain != 0)
                        {
                            foreach (var p in battleManager._characters)
                            {
                                p.OnRelicAttack -= OnRelicAttackHeal;
                                p.OnRelicAttack += OnRelicAttackHeal;
                            }
                        }

                        ApplyStatToCharacter(relic);
                    }
                    else if (relic.relicType == RelicType.RelicNumber)
                    {
                        RelicCountCheck(RelicKind.Buf);
                        RelicCountCheck(RelicKind.Debuff);
                        Debug.Log("OnRelicNumberHandler");
                    }
                }
                else if (relic.relicRole == RelicRole.None && !relic.relicIsPassive)
                {
                    if (relic.relicType == RelicType.ActiveSkill)
                    {
                        foreach (var p in battleManager._characters)
                        {
                            p.OnRelicEffect -= OnRelicEffectStat;
                            p.OnRelicEffect += OnRelicEffectStat;
                        }
                    }
                    else if (relic.relicType == RelicType.Attack)
                    {
                        foreach (var p in battleManager._characters)
                        {
                            p.OnRelicAttack -= OnRelicAttackStack;
                            p.OnRelicAttack += OnRelicAttackStack;
                        }
                    }
                    else if (relic.relicType == RelicType.Time)
                    {
                        //초당 적용
                        StartCoroutine(AttackTime(relic));
                    }
                    else if (relic.relicType == RelicType.MonKill)
                    {
                        foreach (var m in battleManager._monsters)
                        {
                            m.OnRelicMonsterDeath -= MonsterDieBuff;
                            m.OnRelicMonsterDeath += MonsterDieBuff;
                        }
                    }
                    else if (relic.relicType == RelicType.ChaNumber)
                    {
                        BattleCharacterCheck(relic);
                        battleManager.OnCharacterDeath -= BattleCharacterCheck;
                        battleManager.OnCharacterDeath += BattleCharacterCheck;
                        battleManager.OnCharacterDeath -= CharacterNumberCheck;
                        battleManager.OnCharacterDeath += CharacterNumberCheck;
                        Debug.Log("이벤트 구독");
                    }
                    else if (relic.relicType == RelicType.None)
                    {
                        foreach (var p in battleManager._characters)
                        {
                            p._characterState._isBarrier = Random.Range(0, 100) < 2; //2% 확률로 True
                            Debug.Log($"회피 {p._characterState._isBarrier}");
                        }
                    }
                }
                else if (relic.relicType == RelicType.ChaRole && relic.relicIsPassive)
                {
                    if (relic.relicRole == RelicRole.Melee)
                    {
                        foreach (var p in battleManager._characters)
                        {
                            if (p._characterState._chaRole == CharacterRole.Melee)
                            {
                                ApplySingleToCharacter(p, relic);
                            }
                        }
                    }
                    else if (relic.relicRole == RelicRole.Ranged)
                    {
                        foreach (var p in battleManager._characters)
                        {
                            if (p._characterState._chaRole == CharacterRole.Ranged)
                            {
                                ApplySingleToCharacter(p, relic);
                            }
                        }
                    }
                }
                break;

            case RelicTarget.Monster: //유물 적용 대상이 몬스터
                if (relic.relicRole == RelicRole.None && relic.relicIsPassive && relic.relicType == RelicType.None)
                {
                    foreach (var m in battleManager._monsters)
                    {
                        ApplyStatToMonster(m, relic);
                    }
                }
                break;

            case RelicTarget.Currency: //유물 적용 대상이 엽전
                if (relic.relicRole == RelicRole.None && relic.relicType == RelicType.Clear)
                {
                    GameManager.Instance.Coin.OnRelicChanged -= OnYeopjeonBonus;
                    GameManager.Instance.Coin.OnRelicChanged += OnYeopjeonBonus;
                    addRewardList.Add(currentRelic);
                }
                else if (relic.relicType == RelicType.ChaNumber)
                {
                    GameManager.Instance.Coin.OnRelicChanged -= CharacterCheckYeopjeon;
                    GameManager.Instance.Coin.OnRelicChanged += CharacterCheckYeopjeon;
                    addRewardList.Add(currentRelic);
                }
                break;
        }
    }

    public void OnYeopjeonBonus() => GameManager.Instance.Coin.BonusYeopjeon(addRewardList);
    public void OnRelicAttackHeal() => CharacterHeal(currentRelic);
    public void OnRelicEffectStat() => ApplyStatToCharacter(currentRelic);
    public void OnRelicAttackStack() => AttackSpeedStack(currentRelic);
    public void MonsterDieBuff() => ApplyStatToCharacter(currentRelic);
    public void BattleCharacterCheck() => BattleCharacterCheck(currentRelic);

    private void Heal(MyCharacterController p, RelicDatas relic) //회복 기능
    {
        float heal = p._characterState._chaAttack * (relic.chaDrain / 100f);
        p._characterState._chaCurrentHP = Mathf.Clamp(
            p._characterState._chaCurrentHP + heal, 0, p._characterState._chaMaxHP);
        // Debug.Log(
        //     $"캐릭터 이름 {p._characterState._chaEnName}: 피해량에 따른 회복 +{relic.chaDrain}% → 최종 {p._characterState._chaCurrentHP}");
    }

    private void CharacterHeal(RelicDatas relic)
    {
        foreach (var p in battleManager._characters)
        {
            Heal(p, relic);
        }
    }

    private void AttackSpeedStack(RelicDatas relic)
    {
        if (attackCount < 10)
        {
            attackCount++;
            foreach (var p in battleManager._characters)
            {
                if (relic.chaAtkSpeed != 0) //공격 속도
                {
                    p._characterState._chaAtkSpeed -= AddStat(p._characterState._chaAtkSpeed, relic.chaAtkSpeed);
                    Debug.Log(
                        $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격속도 +{relic.chaAtkSpeed}% → 최종 {p._characterState._chaAtkSpeed}");
                }
            }
        }
    }

    private IEnumerator AttackTime(RelicDatas relic)
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            foreach (var p in battleManager._characters)
            {
                if (relic.chaAttack != 0) //공격력
                {
                    p._characterState._chaAttack += AddStat(p._characterState._chaAttack, relic.chaAttack);
                    Debug.Log(
                        $"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격력 +{relic.chaAttack}% → 최종 {p._characterState._chaAttack}");
                }
            }
        }
    }

    private void CharacterNumberCheck()
    {
        if (battleManager._characterCount == 1 && currentRelic.chaAttack != 0)
        {
            foreach (var p in battleManager._characters)
            {
                if (p._isAlive)
                {
                    ApplySingleToCharacter(p, currentRelic);
                    Heal(p, currentRelic);
                }
            }
        }
    }

    private void CharacterCheckYeopjeon()
    {
        aliveCount = 0;

        if (addRewardList.Count == 0) return;

        //캐릭터 수 체크해서
        foreach (var p in battleManager._characters)
        {
            if (p._isAlive)
            {
                aliveCount++;
            }
        }
        //캐릭터 수 당 10% 증가
        int baseBonus = GameManager.Instance.Coin._yeopjeonBonus;

        RelicDatas relic = null;
        foreach (var addReward in addRewardList)
        {
            if (addReward.relicEnName != RelicEnName.LuckyReceipt) continue;

            relic = addReward;
            break;
        }

        if (relic == null) return;

        int aliveBonus = relic.addReward * aliveCount;
        int lastBonus = baseBonus + aliveBonus;
        GameManager.Instance.Coin.BonusYeopjeon(lastBonus);
    }

    private void BattleCharacterCheck(RelicDatas relic)
    {
        if (relic.chaAvoid == 0) return;
        // Debug.Log("BattleCharacterCheck");
        int aliveCount = battleManager._characterCount; //살아있는 캐릭터 수

        foreach (var p in battleManager._characters)
        {
            if (!baseStates.TryGetValue(p, out var baseState))
                continue;

            if (p._isAlive)
            {
                float addAvoid = baseStates[p]._chaAvoid * (aliveCount * (relic.chaAvoid * 0.01f));
                float totalAvoid = baseStates[p]._chaAvoid + addAvoid;
                p._characterState._chaAvoid = totalAvoid;

                // Debug.Log(
                //     $"캐릭터 이름 {p._characterState._chaEnName},{relic.chaAvoid}: 회피율 +{aliveCount * relic.chaAvoid}% → 최종 {p._characterState._chaAvoid}");
            }
        }
    }

    public void RelicCountCheck(RelicKind kind)
    {
        // Debug.Log($"[RelicCountCheck] 호출됨 | kind={kind}");
        // Debug.Log($"[RelicCountCheck] 현재 relicInventory 개수: {GameManager.Instance.InGameItem.relicInventory.Count}");

        var buffRelicCount = GameManager.Instance.InGameItem.relicInventory
            .Where(r => r.relic.relicKind == kind)
            .Select(r => r.relic)
            .ToList();
        // Debug.Log($"[RelicCountCheck] {kind} 필터 후 개수: {buffRelicCount.Count}");
        foreach (var p in battleManager._characters)
        {
            foreach (var buffRelic in buffRelicCount)
            {
                if (buffRelic.chaAtkSpeed != 0)
                {
                    p._characterState._chaAtkSpeed -=
                        AddRelicCountStat(baseStates[p]._chaAtkSpeed, buffRelic.chaAtkSpeed, buffRelicCount.Count);
                    Debug.Log(
                        $"캐릭터 이름 {p._characterState._chaEnName}, 최종 {p._characterState._chaAtkSpeed}");
                }

                if (buffRelic.chaAttack != 0)
                {
                    p._characterState._chaAttack +=
                        AddRelicCountStat(baseStates[p]._chaAttack, buffRelic.chaAttack, buffRelicCount.Count);
                    // Debug.Log(
                    //     $"[DEBUG] 캐릭터 {p._characterState._chaEnName} | {buffRelic.relicName}: 공격력 +{buffRelic.chaAttack * buffRelicCount.Count}% → 최종 {p._characterState._chaAttack}");
                }

                if (buffRelic.chaAvoid != 0)
                    p._characterState._chaAvoid +=
                        AddRelicCountStat(baseStates[p]._chaAvoid, buffRelic.chaAvoid, buffRelicCount.Count);

                if (buffRelic.chaCritDmg != 0)
                    p._characterState._chaCritDmg +=
                        AddRelicCountStat(baseStates[p]._chaCritDmg, buffRelic.chaCritDmg, buffRelicCount.Count);

                if (buffRelic.chaAccuracy != 0)
                    p._characterState._chaAccuracy +=
                        AddRelicCountStat(baseStates[p]._chaAccuracy, buffRelic.chaAccuracy, buffRelicCount.Count);

                if (buffRelic.chaArmor != 0)
                    p._characterState._chaArmor += buffRelic.chaArmor;

                if (buffRelic.chaHP != 0)
                    p._characterState._chaMaxHP +=
                        AddRelicCountStat(baseStates[p]._chaMaxHP, buffRelic.chaHP, buffRelicCount.Count);

                if (buffRelic.chaMPRecovery != 0)
                    p._characterState._chaMPRecovery *= 1f + buffRelicCount.Count * (buffRelic.chaMPRecovery / 100f);
            }
        }
    }

    private float AddRelicCountStat(float stat, float percent, int relicCount) => stat * (relicCount * (percent / 100f));
}