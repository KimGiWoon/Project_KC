using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataManager : SingletonManager<SkillDataManager>
{
    [Header("Character Skills List")]
    [SerializeField] private CharacterSkillDataSO[] _characterAllSkills;

    [Header("Monster Skills List")]
    [SerializeField] private MonsterSkillDataSO[] _monsterAllSkills;

    private Dictionary<int, CharacterSkillDataSO> _chaSkillDic = new Dictionary<int, CharacterSkillDataSO>();
    private Dictionary<int, MonsterSkillDataSO> _monSkillDic = new Dictionary<int, MonsterSkillDataSO>();

    protected override void Awake()
    {
        base.Awake();

        // 딕셔너리에 스킬 저장
        foreach (var skill in _characterAllSkills)
        {
            _chaSkillDic[skill._chaSkillID] = skill;
        }
        foreach (var skill in _monsterAllSkills)
        {
            _monSkillDic[skill._monSkillID] = skill;
        }
    }

    // 캐릭터 스킬 가져오기
    public CharacterSkillDataSO GetCharacterSkill(int skillID)
    {
        return _chaSkillDic.TryGetValue(skillID, out var skill) ? skill : null;
    }

    // 몬스터 스킬 가져오기
    public MonsterSkillDataSO GetMonsterSkill(int skillID)
    {
        return _monSkillDic.TryGetValue(skillID, out var skill) ? skill : null;
    }
}
