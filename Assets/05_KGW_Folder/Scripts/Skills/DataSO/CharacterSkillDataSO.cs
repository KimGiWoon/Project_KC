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

    // 액티브 스킬 사용 함수
    public virtual void UseSkill(MyCharacterController caster, CharacterSkillDataSO skill, MonsterController target)
    {
        switch (skill._chaSkillEnName)
        {
            case CharacterSkillEnName.Cheer: // 응원
                skill.UseCheer(caster);
                break;
            case CharacterSkillEnName.ProtectiveGrenade: // 보호 수류탄
                skill.UseProtectiveGrenade(caster, target);
                break;
            case CharacterSkillEnName.PainfulScent: // 괴로운 향기
                skill.UsePainfulScent(caster, target);
                break;
            case CharacterSkillEnName.WolfSlash: // 늑대 베기
                skill.UseWolfSlash(caster, target);
                break;
            case CharacterSkillEnName.WrathOfTheGuardian: // 수호령의 분노
                skill.UseWrathOfTheGuardian(caster, target);
                break;
            default:
                Debug.Log("스킬이 없음");
                break;
        }
    }

    // 패시브 스킬 사용 함수
    public virtual float UsePassiveSkill(MyCharacterController caster, CharacterSkillDataSO skill, float value)
    {
        switch (skill._chaSkillEnName)
        {
            case CharacterSkillEnName.RegenerativeStrike: // 재생의 일격
                return UseRegenerativeStrike(caster, value);
            case CharacterSkillEnName.Vanguard: // 선봉장
                return UseVanguard(caster, value);
            case CharacterSkillEnName.MoraleDecline: // 사기 저하
                return UseMoraleDecline(caster, value);
            case CharacterSkillEnName.WaveOfSteel: // 강철의 파동
                return UseWaveOfSteel(caster, value);
            case CharacterSkillEnName.AimForTheWound: // 상처 조준
                return UseAimForTheWound(caster, value);
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

    #region 캐릭터의 액티브 스킬

    // 응원 액티브 스킬
    public void UseCheer(MyCharacterController caster)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 응원 액티브 발동");
        var cheer = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = cheer.GetComponent<CheerController>();

        skillCon.Init(caster, _chaEffectValue, _chaSkillDuration);
    }

    // 보호 수류탄 액티브 스킬
    public void UseProtectiveGrenade(MyCharacterController caster, MonsterController target)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 보호 수류탄 액티브 발동");
        var protectiveGrenade = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = protectiveGrenade.GetComponent<ProtectiveGrenadeController>();

        skillCon.Init(caster, target, _chaSkillValue);
    }

    // 괴로운 향기 액티브 스킬
    public void UsePainfulScent(MyCharacterController caster, MonsterController target)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 괴로운 향기 액티브 발동");
        var painfulScent = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = painfulScent.GetComponent<PainfulScentController>();

        skillCon.Init(caster, target, _chaSkillHit, _chaSkillValue, _chaEffectValue, _chaSkillDuration, _chaSkillTick);
    }

    // 늑대 베기 액티브 스킬
    public void UseWolfSlash(MyCharacterController caster, MonsterController target)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 늑대 베기 액티브 발동");
        var wolfSlash = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = wolfSlash.GetComponent<WolfSlashController>();

        skillCon.Init(caster, target, _chaSkillHit, _chaSkillValue, _chaEffectValue, _chaSkillTick);
    }

    // 수호령의 분노 액티브 스킬
    public void UseWrathOfTheGuardian(MyCharacterController caster, MonsterController target)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 수호령의 분노 액티브 발동");
        var wrathOfTheGuardian = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = wrathOfTheGuardian.GetComponent<WrathOfTheGuardianController>();

        skillCon.Init(caster, target, _chaSkillHit, _chaSkillValue, _chaSkillTick);
    }

    #endregion

    #region 캐릭터의 패시브 스킬

    // 재생의 일격 패시브 스킬
    public float UseRegenerativeStrike(MyCharacterController caster, float value)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 재생의 일격 패시브 발동");
        var regenerativeStrike = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = regenerativeStrike.GetComponent<RegenerativeStrikeContoller>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }

    // 선봉장 패시브 스킬
    public float UseVanguard(MyCharacterController caster, float value)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 선봉장 패시브 발동");
        var vanguard = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = vanguard.GetComponent<VanguardController>();

        return skillCon.Init(_chaEffectValue);
    }

    // 사기 저하 패시브 스킬
    public float UseMoraleDecline(MyCharacterController caster, float value)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 사기 저하 패시브 발동");
        var moraleDecline = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = moraleDecline.GetComponent<MoraleDeclineController>();

        return skillCon.Init(_chaSkillChance, _chaEffectValue);
    }

    // 강철의 파동 패시브 스킬
    public float UseWaveOfSteel(MyCharacterController caster, float value)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 강철의 파동 패시브 발동");
        var waveOfSteel = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = waveOfSteel.GetComponent<WaveOfSteelController>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }
    // 상처 조준 패시브 스킬
    public float UseAimForTheWound(MyCharacterController caster, float value)
    {
        // Debug.Log($"{caster._characterState._chaEnName} : 상처 조준 패시브 발동");
        var aimForTheWound = Instantiate(_chaSkillPrefab, caster.transform.position, caster.transform.rotation);
        var skillCon = aimForTheWound.GetComponent<AimForTheWoundController>();

        return skillCon.Init(_chaSkillChance, _chaSkillValue, caster._characterState._chaAttack);
    }

    #endregion
}