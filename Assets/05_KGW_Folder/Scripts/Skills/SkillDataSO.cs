using UnityEngine;

// 스킬의 정보를 담는 스킬 스크립터블 오브젝트
public abstract class SkillDataSO : ScriptableObject
{
    [Header("Skill Data Setting")]
    public string _skillName;       // 스킬의 이름
    public float _skillRange;       // 스킬의 사거리
    public float _skillDamage;      // 스킬의 데미지
    public GameObject _skillPrefab; // 스킬 프리팹

    // 보스전용 스킬사용 메서드
    public virtual void UseSkill(Transform caster, MonoBehaviour target, Transform[] point)
    {
        // 보스의 스킬 사용으로 몬스터 소환위치 필요 시 해당 메서드 사용
    }

    // 캐릭터 전용 스킬사용 메서드
    public virtual void UseSkill(Transform caster, MonoBehaviour target)
    {
        // 캐릭터 스킬 사용으로 필요 시 해당 메서드 사용
    }

}
