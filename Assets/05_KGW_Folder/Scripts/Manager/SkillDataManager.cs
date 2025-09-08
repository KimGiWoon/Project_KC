using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDW;

// public class SkillDataManager : SingletonManager<SkillDataManager>
public class SkillDataManager : MonoBehaviour
{
    [Header("Character Skills List")]
    [SerializeField] private CharacterSkillDataSO[] _characterAllSkills;

    [Header("Monster Skills List")]
    [SerializeField] private MonsterSkillDataSO[] _monsterAllSkills;

    private Dictionary<int, CharacterSkillDataSO> _chaSkillDic = new Dictionary<int, CharacterSkillDataSO>();
    private Dictionary<int, MonsterSkillDataSO> _monSkillDic = new Dictionary<int, MonsterSkillDataSO>();
    private GameManager _gameManager;
    private bool _isDownloaded;

    // protected override void Awake()
    // {
    //     base.Awake();

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
            _isDownloaded) return;

        // 딕셔너리에 스킬 저장
        foreach (var skill in _characterAllSkills)
        {
            _chaSkillDic[skill._chaSkillID] = skill;
        }
        foreach (var skill in _monsterAllSkills)
        {
            _monSkillDic[skill._monSkillID] = skill;
        }
        _isDownloaded = true;
    }

    // 캐릭터 스킬 가져오기
    public CharacterSkillDataSO GetCharacterSkill(int skillID) => _chaSkillDic.TryGetValue(skillID, out var skill) ? skill : null;

    // 몬스터 스킬 가져오기
    public MonsterSkillDataSO GetMonsterSkill(int skillID) => _monSkillDic.TryGetValue(skillID, out var skill) ? skill : null;
}