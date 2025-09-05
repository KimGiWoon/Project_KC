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
    public string _monEffectValue;
    public SkillCC _monSkillCC;
    public string _monSkillDescription;

    // 보스의 스킬 사용 함수
    public virtual void UseSkill(Transform caster, MonoBehaviour target)
    {
        // 보스의 공격 스킬 사용하려면 해당 함수 사용
    }

    // 보스의 몬스터 소환 스킬 사용 함수
    public virtual void UseSkill(Transform caster, MonoBehaviour target, Transform[] point)
    {
        // 보스의 몬스터 소환 스킬 사용하려면 해당 함수 사용
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
}
