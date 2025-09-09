using System.Collections.Generic;
using UnityEngine;
using SDW;
using System.Collections;

public class BuffRelicManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    List<MyCharacterController> myCharacterController;
    List<MonsterController> monsterController;
    
    private RelicDatas currentRelic;
    private int attackCount = 0;

    private float AddStat(float stat, float percent)
    {
        return stat * (percent / 100f);
    }

    public void ApplyStatToMonster(MonsterController m, RelicDatas relic)
    {
        if (relic.monArmor != 0)
        {
            m._monsterState._monArmor += AddStat(m._monsterState._monArmor, relic.monArmor);
            Debug.Log($"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 방어력 +{relic.monArmor}% → 최종 {m._monsterState._monArmor}");
        }
        
        if (relic.monHP != 0)
        {
            m._monsterState._monMaxHP += AddStat(m._monsterState._monMaxHP, relic.monHP);
            Debug.Log($"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 최대체력 +{relic.monHP}% → 최종 {m._monsterState._monMaxHP}");
        }
        
        if (relic.monAttack != 0)
        {
            m._monsterState._monAttack += AddStat(m._monsterState._monAttack, relic.monAttack);
            Debug.Log($"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 공격력 +{relic.monAttack}% → 최종 {m._monsterState._monAttack}");
        }
        
        if (relic.monAtkSpeed != 0)
        {
            m._monsterState._monAtkSpeed += AddStat(m._monsterState._monAtkSpeed, relic.monAtkSpeed);
            Debug.Log($"몬스터 이름 {m._monsterState._monEnName},{relic.relicName}: 공격속도 +{relic.monAtkSpeed}% → 최종 {m._monsterState._monAtkSpeed}");
        }
    }

    public void ApplySingleToCharacter(MyCharacterController p , RelicDatas relic)
    {
        if (relic.chaAttack != 0) //공격력
            {
                p._characterState._chaAttack += AddStat(p._characterState._chaAttack, relic.chaAttack);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격력 +{relic.chaAttack}% → 최종 {p._characterState._chaAttack}");
            }

            if (relic.chaArmor != 0) //방어력
            {
                p._characterState._chaArmor += AddStat(p._characterState._chaArmor, relic.chaArmor);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 방어력 +{relic.chaArmor}% → 최종 {p._characterState._chaArmor}");
            }

            if (relic.chaAtkSpeed != 0) //공격 속도
            {
                p._characterState._chaAtkSpeed += AddStat(p._characterState._chaAtkSpeed, relic.chaAtkSpeed);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격속도 +{relic.chaAtkSpeed}% → 최종 {p._characterState._chaAtkSpeed}");
            }

            if (relic.chaHP != 0) //최대 체력
            {
                p._characterState._chaMaxHP += AddStat(p._characterState._chaMaxHP, relic.chaHP);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 최대 체력 +{relic.chaHP}% → 최종 {p._characterState._chaMaxHP}");
            }

            if (relic.chaMPRecovery != 0) //마나 회복량
            {
                p._characterState._chaMPRecovery *= (1f + (relic.chaMPRecovery / 100f));
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 마나 회복량 +{relic.chaMPRecovery}% → 최종 {p._characterState._chaMPRecovery}");
            }

            if (relic.chaCritDmg != 0) //치명타 데미지
            {
                p._characterState._chaCritDmg += AddStat(p._characterState._chaCritDmg, relic.chaCritDmg);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 치명타 데미지 +{relic.chaCritDmg}% → 최종 {p._characterState._chaCritDmg}");
            }

            if (relic.chaAccuracy != 0) //명중률
            {
                p._characterState._chaAccuracy += AddStat(p._characterState._chaAccuracy, relic.chaAccuracy);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 명중률 +{relic.chaAccuracy}% → 최종 {p._characterState._chaAccuracy}");
            }

            if (relic.chaAvoid != 0) //회피율
            {
                p._characterState._chaAvoid += AddStat(p._characterState._chaAvoid, relic.chaAvoid);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.chaAvoid}: 회피율 +{relic.chaAvoid}% → 최종 {p._characterState._chaAvoid}");
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
                if (relic.relicRole == RelicRole.None && relic.relicIsPassive && relic.relicType == RelicType.None)
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
                //스킬을 쓸 때 스탯이 올라가는 유물
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
                        battleManager.OnCharacterDeath -= OnChaNumberHandler;
                        battleManager.OnCharacterDeath += OnChaNumberHandler;
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
        }
    }

    private void OnChaNumberHandler()
    {
        CharacterNumberCheck();
        BattleChaCountCheck();
    }
    
    private void OnRelicAttackHeal() => CharacterHeal(currentRelic);
    private void OnRelicEffectStat() => ApplyStatToCharacter(currentRelic);
    private void OnRelicAttackStack() => AttackSpeedStack(currentRelic);
    private void MonsterDieBuff() => ApplyStatToCharacter(currentRelic);
    private void BattleChaCountCheck()=> BattleCharacterCheck(currentRelic);

    private void Heal(MyCharacterController p, RelicDatas relic) //회복 기능
    {
        float heal = p._characterState._chaAttack * (relic.chaDrain / 100f);
        p._characterState._chaCurrentHP = Mathf.Clamp(
            p._characterState._chaCurrentHP + heal, 0, p._characterState._chaMaxHP);
        Debug.Log(
            $"캐릭터 이름 {p._characterState._chaEnName}: 피해량에 따른 회복 +{relic.chaDrain}% → 최종 {p._characterState._chaCurrentHP}");
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
                    p._characterState._chaAtkSpeed = AddStat(p._characterState._chaAtkSpeed, relic.chaAtkSpeed);
                    Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격속도 +{relic.chaAtkSpeed}% → 최종 {p._characterState._chaAtkSpeed}");
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
                    p._characterState._chaAttack = AddStat(p._characterState._chaAttack, relic.chaAttack);
                    Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격력 +{relic.chaAttack}% → 최종 {p._characterState._chaAttack}");
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

    private void BattleCharacterCheck(RelicDatas relic) 
    {
        foreach (var p in battleManager._characters)
        {
            if (p._isAlive && relic.chaAvoid != 0)
            {
                float avoid = p._characterState._chaAvoid;
                p._characterState._chaAvoid = avoid * (1f + battleManager._characterCount * (relic.chaAvoid / 100f));
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.chaAvoid}: 회피율 +{relic.chaAvoid}% → 최종 {p._characterState._chaAvoid}");
            }
        }
    }
}
