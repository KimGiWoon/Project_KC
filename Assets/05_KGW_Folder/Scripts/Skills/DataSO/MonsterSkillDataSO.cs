using SDW;
using UnityEngine;

// 몬스터 스킬 데이터 저장
[CreateAssetMenu(fileName = "MonsterSkillData", menuName = "Monsters/MonsterSkillData")]

public class MonsterSkillDataSO : ScriptableObject
{
    [Header("Monster Skill Data Setting")]
    public int _monSkillID;
    public string _monSkillName;
    public MonsterSkillEnName _monSkillEnName;
    public SkillType _monSkillType;
    public SkillTargetType _monSkillTargetType;
    public SkillEffectType _monSkillEffectType;
    public float _monSkillCd;
    public int _monSkillRange;
    public int _monSkillConHP;
    public float _monSkillChance;
    public float _monSkillDuration;
    public float _monSkillTick;
    public int _monSkillHit;
    public float _monSkillValue;
    public float _monEffectValue;
    public SkillCC _monSkillCC;
    public string _monSkillDescription;

    public GameObject _monSkillPrefab; // 스킬 프리팹

    // 보스의 스킬 사용 함수
    public virtual void UseSkill(MonsterController caster, MonsterSkillDataSO skill, MyCharacterController target)
    {
        switch (skill._monSkillEnName)
        {
            case MonsterSkillEnName.EerieDarkness:  // 기괴한 어둠
                skill.UseEerieDarkness(caster);
                break;
            case MonsterSkillEnName.DangerousPollen:    // 위험한 꽃가루
                skill.UseDangerousPollen(caster);
                break;
            case MonsterSkillEnName.RockFist:    // 바위 주먹
                skill.UseSavageSlash(caster,target);
                break;
            case MonsterSkillEnName.Stomp:  // 발구르기
                skill.UseStomp(caster,target); 
                break;
            case MonsterSkillEnName.SavageRush: // 낙폭한 돌진
                skill.UseSavageRush(caster,target);
                break;
            case MonsterSkillEnName.Swing:  // 휘두르기
                skill.UseSwing(caster,target);
                break;
            case MonsterSkillEnName.Roar:   // 포효
                skill.UseRoar(caster,target);
                break;
            case MonsterSkillEnName.BrutalSlash: // 난폭한 베기
                skill.UseBrutalSlash(caster,target);
                break;
            case MonsterSkillEnName.MonkeyBlade1:   // 원숭이 검술 1식
                skill.UseMonkeyBlade1(caster,target);
                break;
            default:
                Debug.Log("스킬이 없습니다.");
                break;
        }
    }

    // 파싱 데이터를 매핑
    public virtual void DataApply(MonsterSkillFileData skillFileData)
    {
        _monSkillID = skillFileData.MonSkillID;
        _monSkillName = skillFileData.MonSkillName;
        _monSkillEnName = skillFileData.MonSkillEnName;
        _monSkillType = skillFileData.MonSkillType;
        _monSkillTargetType = skillFileData.MonSkillTargetType;
        _monSkillEffectType = skillFileData.MonSkillEffectType;
        _monSkillCd = skillFileData.MonSkillCd;
        _monSkillRange = skillFileData.MonSkillRange;
        _monSkillConHP = skillFileData.MonSkillConHP;
        _monSkillChance = skillFileData.MonSkillChance;
        _monSkillDuration = skillFileData.MonSkillDuration;
        _monSkillTick = skillFileData.MonSkillTick;
        _monSkillHit = skillFileData.MonSkillHit;
        _monSkillValue = skillFileData.MonSkillValue;
        _monEffectValue = skillFileData.MonEffectValue;
        _monSkillCC = skillFileData.MonSkillCC;
        _monSkillDescription = skillFileData.SkillDescription;
    }

    #region 몬스터의 액티브 스킬
    // 기괴한 어둠 액티브 스킬
    public void UseEerieDarkness(MonsterController caster)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 기괴한 어둠 액티브 발동");
        GameObject eerieDarkness = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        EerieDarknessController skillCon = eerieDarkness.GetComponent<EerieDarknessController>();

        skillCon.Init(caster, _monEffectValue, _monSkillDuration);
    }

    // 위험한 꽃가루 액티브 스킬
    public void UseDangerousPollen(MonsterController caster)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 위험한 꽃가루 액티브 발동");
        GameObject dangerousPollen = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        DangerousPollenController skillCon = dangerousPollen.GetComponent<DangerousPollenController>();

        skillCon.Init(caster, _monSkillValue, _monSkillDuration);
    }

    // 난도질 액티브 스킬
    public void UseSavageSlash(MonsterController caster, MyCharacterController target)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 난도질 액티브 발동");
        GameObject savageSlash = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        RockFistController skillCon = savageSlash.GetComponent<RockFistController>();

        skillCon.Init(caster, target, _monSkillHit, _monSkillValue, _monSkillTick);
    }

    // 발구르기 액티브 스킬
    public void UseStomp(MonsterController caster, MyCharacterController target)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 발구르기 액티브 발동");
        GameObject stomp = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        StompController skillCon = stomp.GetComponent<StompController>();

        skillCon.Init(caster, target, _monSkillValue);
    }

    // 난폭한 돌진 액티브 스킬
    public void UseSavageRush(MonsterController caster, MyCharacterController target)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 난폭한 돌진 액티브 발동");
        GameObject savageRush = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        SavageRushController skillCon = savageRush.GetComponent<SavageRushController>();

        skillCon.Init(caster, target, _monSkillValue);
    }

    // 휘두르기 액티브 스킬
    public void UseSwing(MonsterController caster, MyCharacterController target)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 휘두르기 액티브 발동");
        GameObject swing = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        SwingController skillCon = swing.GetComponent<SwingController>();

        skillCon.Init(caster, target, _monSkillValue);
    }

    // 포효 액티브 스킬
    public void UseRoar(MonsterController caster, MyCharacterController target)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 포효 액티브 발동");
        GameObject roar = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        RoarController skillCon = roar.GetComponent<RoarController>();

        skillCon.Init(caster, target, _monSkillDuration, _monSkillValue, _monEffectValue);
    }

    // 난폭한 베기 액티브 스킬
    public void UseBrutalSlash(MonsterController caster, MyCharacterController target)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 난폭한 베기 액티브 발동");
        GameObject brutalSlash = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        BrutalSlashController skillCon = brutalSlash.GetComponent<BrutalSlashController>();

        skillCon.Init(caster, target, _monSkillHit, _monSkillValue, _monSkillTick);
    }

    // 원숭이 검술 1식 액티브 스킬
    public void UseMonkeyBlade1(MonsterController caster, MyCharacterController target)
    {
        Debug.Log($"{caster._monsterState._monEnName} : 원숭이 검술 1식 액티브 발동");
        GameObject MonkeyBlade1 = Instantiate(_monSkillPrefab, caster.transform.position, caster.transform.rotation);
        MonkeyBlade1Controller skillCon = MonkeyBlade1.GetComponent<MonkeyBlade1Controller>();

        skillCon.Init(caster, target, _monSkillValue);
    }
    #endregion
}
