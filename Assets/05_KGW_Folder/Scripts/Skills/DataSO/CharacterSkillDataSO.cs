using System.Collections;
using SDW;
using UnityEngine;

// 캐릭터 스킬 데이터 저장
[CreateAssetMenu(fileName = "CharacterSkillData", menuName = "Characters/CharacterSkillData")]

// 스킬의 정보를 담는 스킬 스크립터블 오브젝트
public class CharacterSkillDataSO : ScriptableObject
{
    [Header("Character Skill Data Setting")]
    public int _chaSkillID;
    public string _chaSkillName;
    public CharacterSkillEnName _chaSkillEnName;
    public SkillType _chaSkillType;
    public SkillTargetType _chaSkillTargetType;
    public SkillEffectType _chaSkillEffectType;
    public float _chaSkillChance;
    public float _chaSkillDuration;
    public float _chaSkillTick;
    public int _chaSkillHit;
    public float _chaSkillValue;
    public float _chaEffectValue;
    public string _chaSkillDescription;

    public GameObject _chaSkillPrefab; // 스킬 프리팹

    // 스킬 사용 함수
    public virtual void UseSkill(MyCharacterController caster, MonoBehaviour target)
    {
        // 캐릭터의 공격 스킬 사용하려면 해당 함수 사용
    }

    // 패시브 스킬 사용 함수
    public virtual float UsePassiveSkill(MyCharacterController caster, CharacterSkillDataSO skill, float value)
    {
        switch (skill._chaSkillEnName)
        {           
            case CharacterSkillEnName.RegenerativeStrike:   // 재생의 일격
                return UseRegenerativeStrike(caster, value);
            case CharacterSkillEnName.Vanguard:
                return UseVanguard(caster, value);
            case CharacterSkillEnName.MoraleDecline:
                return UseMoraleDecline(caster, value);
            case CharacterSkillEnName.WaveOfSteel:
                return UseWaveOfSteel(caster, value);
            case CharacterSkillEnName.AimForTheWound:
                return UseAimForTheWound(caster, value);    // 상처 조준
            default:
                Debug.Log("스킬이 없음");
                return value;
        }
    }

    // 파싱 데이터를 매핑
    public virtual void DataApply(CharacterSkillFileData skillFileData)
    {
        _chaSkillID = skillFileData.ChaSkillID;
        _chaSkillName = skillFileData.ChaSkillName;
        _chaSkillEnName = skillFileData.ChaSkillEnName;
        _chaSkillType = skillFileData.ChaSkillType;
        _chaSkillTargetType = skillFileData.ChaSkillTargetType;
        _chaSkillEffectType = skillFileData.ChaSkillEffectType;
        _chaSkillChance = skillFileData.ChaSkillChance;
        _chaSkillDuration = skillFileData.ChaSkillDuration;
        _chaSkillTick = skillFileData.ChaSkillTick;
        _chaSkillHit = skillFileData.ChaSkillHit;
        _chaSkillValue = skillFileData.ChaSkillValue;
        _chaEffectValue = skillFileData.ChaEffectValue;
        _chaSkillDescription = skillFileData.ChaSkillDescription;
    }

    // 재생의 일격 패시브 스킬
    public float UseRegenerativeStrike(MyCharacterController caster, float value)
    {
        Debug.Log($"{caster._characterState._chaEnName} : 재생의 일격 패시브 발동");
        GameObject regenerativeStrike = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        RegenerativeStrikeContoller skillCon = regenerativeStrike.GetComponent<RegenerativeStrikeContoller>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }

    // 선봉장 패시브 스킬
    public float UseVanguard(MyCharacterController caster, float value)
    {
        Debug.Log($"{caster._characterState._chaEnName} : 선봉장 패시브 발동");
        GameObject regenerativeStrike = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        RegenerativeStrikeContoller skillCon = regenerativeStrike.GetComponent<RegenerativeStrikeContoller>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }

    // 사기 저하 패시브 스킬
    public float UseMoraleDecline(MyCharacterController caster, float value)
    {
        Debug.Log($"{caster._characterState._chaEnName} : 사기 저하 패시브 발동");
        GameObject regenerativeStrike = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        RegenerativeStrikeContoller skillCon = regenerativeStrike.GetComponent<RegenerativeStrikeContoller>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }

    // 강철의 파동 패시브 스킬
    public float UseWaveOfSteel(MyCharacterController caster, float value)
    {
        Debug.Log($"{caster._characterState._chaEnName} : 강철의 파동 패시브 발동");
        GameObject regenerativeStrike = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        RegenerativeStrikeContoller skillCon = regenerativeStrike.GetComponent<RegenerativeStrikeContoller>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }
    // 상처 조준 패시브 스킬
    public float UseAimForTheWound(MyCharacterController caster, float value)
    {
        Debug.Log($"{caster._characterState._chaEnName} : 상처 조준 패시브 발동");
        GameObject aimForTheWound = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        AimForTheWoundController skillCon = aimForTheWound.GetComponent<AimForTheWoundController>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }
}