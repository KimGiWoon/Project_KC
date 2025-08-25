using SDW;
using UnityEngine;

// 스킬의 정보를 담는 스킬 스크립터블 오브젝트
public abstract class Test_SkillDataSO : ScriptableObject
{
    [Header("Skill Data Setting")]
    public int _chaSkillID;
    public string _chaSkillName;
    public string _chaSkillNameEn;
    public SkillType _chaSkillType;
    public SkillTargetType _chaSkillTargetType;
    public SkillCC _chaSkillCC;
    public SkillEffectType _chaSkillChance;
    public AttackRange _chaSkillRange;
    public float _chaSkillDuration;
    public float _chaSkillTick;
    public float _charSkillHit;
    public float _chaSkillValue;
    public int _chaSkillAnim;
    public string _chaSkillImg;
    public string _chaSkillEffect;

    // 스킬 사용 함수
    public abstract void UseSkill(Transform caster, MonoBehaviour target);

    // 파싱 데이터를 매핑
    public virtual void DataApply(CharacterSkillFileData skillFileData)
    {
        _chaSkillID = skillFileData.ChaSkillID;
        _chaSkillName = skillFileData.ChaSkillName;
        _chaSkillNameEn = skillFileData.ChaSkillNameEn;
        _chaSkillType = skillFileData.ChaSkillType;
        _chaSkillTargetType = skillFileData.ChaSkillTargetType;
        _chaSkillCC = skillFileData.ChaSkillCC;
        _chaSkillChance = skillFileData.ChaSkillChance;
        _chaSkillRange = skillFileData.ChaSkillRange;
        _chaSkillDuration = skillFileData.ChaSkillDuration;
        _chaSkillTick = skillFileData.ChaSkillTick;
        _charSkillHit = skillFileData.CharSkillHit;      
        _chaSkillValue = skillFileData.ChaSkillValue;
        _chaSkillAnim = skillFileData.ChaSkillAnim;
        _chaSkillImg = skillFileData.ChaSkillImg;
        _chaSkillEffect = skillFileData.ChaSkillEffect;
    }
}
