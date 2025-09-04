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

    // 스킬 사용 함수
    public virtual void UseSkill(Transform caster, MonoBehaviour target)
    {
        // 캐릭터의 공격 스킬 사용하려면 해당 함수 사용
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
}