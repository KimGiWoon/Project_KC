using UnityEngine;

// 스킬의 정보를 담는 스킬 스크립터블 오브젝트
public abstract class SkillDataSO : ScriptableObject
{
    [Header("Skill Data Setting")]
    public string _skillName;       // 스킬의 이름
    public float _skillRange;       // 스킬의 사거리
    public float _skillDamage;      // 스킬의 데미지
    public GameObject _skillPrefab; // 스킬 프리팹

    public abstract void UseSkill(Transform caster, MonoBehaviour target);
}
