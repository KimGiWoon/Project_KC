using System.Collections.Generic;
using UnityEngine;
using SDW;
public class BuffRelicManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    List<MyCharacterController> myCharacterController;
    
    public void ApplyStatToCharacter(RelicDatas relic) //기본 스탯 ++
    {
        foreach (var p in battleManager._characters)
        {
            if (relic.chaAttack != 0) //공격력
            {
                p._characterState._chaAttack += p._characterState._chaAttack * (relic.chaAttack / 100f);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격력 +{relic.chaAttack}% → 최종 {p._characterState._chaAttack}");    
            }

            if (relic.chaArmor != 0) //방어력
            {
                p._characterState._chaArmor += p._characterState._chaArmor * (relic.chaArmor / 100f);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 방어력 +{relic.chaArmor}% → 최종 {p._characterState._chaArmor}");       
            }

            if (relic.chaAtkSpeed != 0) //공격 속도
            {
                p._characterState._chaAtkSpeed += p._characterState._chaAtkSpeed * (relic.chaAtkSpeed / 100f);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 공격속도 +{relic.chaAtkSpeed}% → 최종 {p._characterState._chaAtkSpeed}");      
            }

            if (relic.chaHP != 0) //최대 체력
            {
                p._characterState._chaMaxHP += p._characterState._chaMaxHP * (relic.chaHP / 100f);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 최대 체력 +{relic.chaHP}% → 최종 {p._characterState._chaMaxHP}");          
            }

            if (relic.chaMPRecovery != 0) //마나 회복량
            {
                p._characterState._chaMPRecovery *= (1f + (relic.chaMPRecovery / 100f));
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 마나 회복량 +{relic.chaMPRecovery}% → 최종 {p._characterState._chaMPRecovery}");      
            }

            if (relic.chaCritDmg != 0) //치명타 데미지
            {
                p._characterState._chaCritDmg += p._characterState._chaCritDmg * (relic.chaCritDmg / 100f);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 치명타 데미지 +{relic.chaCritDmg}% → 최종 {p._characterState._chaCritDmg}");        
            }

            if (relic.chaAccuracy != 0) //명중률
            {
                p._characterState._chaAccuracy += p._characterState._chaAccuracy * (relic.chaAccuracy / 100f);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.relicName}: 명중률 +{relic.chaAccuracy}% → 최종 {p._characterState._chaAccuracy}");     
            }

            if (relic.chaAvoid != 0) //회피율
            {
                p._characterState._chaAvoid += p._characterState._chaAvoid * (relic.chaAvoid / 100f);
                Debug.Log($"캐릭터 이름 {p._characterState._chaEnName},{relic.chaAvoid}: 회피율 +{relic.chaAvoid}% → 최종 {p._characterState._chaAvoid}");     
            }
        }
    }

    public void ApplyRelicEffect(RelicDatas relic) //기본 스탯 적용
    {
        switch (relic.relicTarget)
        {
            case RelicTarget.Character: //유물 적용 대상이 Character
                    //발동 가능 역할군이 None, 유물 발동조건이 True, 유물 발동 타입이 None
                    if (relic.relicRole == RelicRole.None && relic.relicIsPassive && relic.relicType == RelicType.None)
                    {
                        if (relic.chaDrain != 0)
                        {
                            foreach (var p in battleManager._characters)
                            {
                              // p.OnAttack -= () => Heal(p, relic);
                              // p.OnAttack += () => Heal(p, relic);
                            }
                        }
                        ApplyStatToCharacter(relic);   
                    }
                    else if (relic.relicRole == RelicRole.None && !relic.relicIsPassive && relic.relicType == RelicType.ActiveSkill)
                    {
                        foreach (var p in battleManager._characters)
                        {
                            p.OnRelicEffect -= () => ApplyStatToCharacter(relic);
                            p.OnRelicEffect += () => ApplyStatToCharacter(relic);    
                            //RelicDropManager.Instance.OnRelicSkill -= () => ApplyStatToCharacter(relic);
                            //RelicDropManager.Instance.OnRelicSkill += () => ApplyStatToCharacter(relic);
                        }
                    }
                        
                    break;    
        }    
    }

    public void Heal(MyCharacterController p, RelicDatas relic) //회복 기능
    {
        float heal = p._characterState._chaAttack * (relic.chaDrain / 100f);
        p._characterState._chaCurrentHP = Mathf.Clamp(
            p._characterState._chaCurrentHP + heal, 0, p._characterState._chaMaxHP);
        Debug.Log(
            $"캐릭터 이름 {p._characterState._chaEnName},{relic.chaDrain}: 피해량에 따른 회복 +{relic.chaDrain}% → 최종 {p._characterState._chaCurrentHP}");
    }
}
