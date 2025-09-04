using System.Collections;
using System.Collections.Generic;
using KSH;
using UnityEngine;

public class BuffRelicManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private MyCharacterController myCharacterController;
    
    public void ApplyStatToCharacter(RelicEffect effect, int value) //기본 스탯 ++
    {
        //TODO: 아직 캐릭터가 생성이 안되어서 실제로 적용은 안되지만 함수는 돌고 있음
        Debug.Log($"효과 적용! effect: {effect}, value: {value}%, 캐릭터 수: {battleManager._characters.Count}");
        foreach (var p in battleManager._characters)
        {
            if ((effect & RelicEffect.chaAttack) != 0)
            {
                p._characterState._chaAttack += p._characterState._chaAttack * (value / 100f);
                Debug.Log($"공격력 적용 {value}%");
            }

            if ((effect & RelicEffect.chaArmor) != 0)
            {
                p._characterState._chaArmor += p._characterState._chaArmor * (value / 100f);
                Debug.Log($"방어력 적용 {value}%");
            }

            if ((effect & RelicEffect.chaAtkSpeed) != 0)
            {
                p._characterState._chaAtkSpeed += p._characterState._chaAtkSpeed * (value / 100f);
                Debug.Log($"공격 스피드 적용 {value}%");
            }

            if ((effect & RelicEffect.chaHP) != 0)
            {
                p._characterState._chaMaxHP += p._characterState._chaMaxHP * (value / 100f);
                Debug.Log($"최대 체력 적용 {value}%");
            }

            if ((effect & RelicEffect.chaMPRecovery) != 0)
            {
                p._characterState._chaMPRecovery += p._characterState._chaMPRecovery * (value / 100f);
                Debug.Log($"마나 최대 적용 {value}%");
            }

            if ((effect & RelicEffect.chaCritDmg) != 0)
            {
                p._characterState._chaCritDmg += p._characterState._chaCritDmg * (value / 100f);
                Debug.Log($"치명타 데미지 적용 {value}%");
            }    
        }
    }

    public void ApplyRelicEffect(Relic relic) //기본 스탯 적용
    {
        foreach (var effect in relic.relicEffectValues)
        {
            switch (relic.relicTarget)
            {
                case RelicTarget.Character:
                    //만약 유물 적용 대상이 Character, 유물 발동조건이 True, 유물 발동 타입이 None, 발동 가능 역할군이 None이면
                    if(relic.relicRole == RelicRole.None && relic.isPassive && relic.relicType == RelicType.None) 
                        ApplyStatToCharacter(effect.effect, effect.value);
                    else if(relic.relicRole == RelicRole.None && !relic.isPassive && relic.relicType == RelicType.ActiveSkill)
                       myCharacterController.OnRelicEffect += () => ApplyStatToCharacter(effect.effect, effect.value);
                        //RelicDropManager.Instance.OnRelicSkill += () => ApplyStatToCharacter(effect.effect, effect.value);
                    break;    
            }    
        }
            //만약 유물적용 대상이 Monster,유물 발동조건이 True, 유물 발동 타입이 None, 발동가능역할군이None이면
            //ApplyStatToMonster(effect.effect, effect.value);
    }
}
