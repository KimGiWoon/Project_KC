using System.Collections.Generic;
using SDW;
using UnityEngine;

// 몬스터 데이터 저장
[CreateAssetMenu(fileName = "MonsterData", menuName = "Monsters/MonsterData")]

// 몬스터의 정보를 담는 몬스터 스크립터블 오브젝트
public class MonsterDataSO : ScriptableObject
{
    // 몬스터의 데이터
    [Header("Monster Data Setting")]
    public int MonId;
    public string MonName;
    public MonsterEnName MonEnName;
    public MonsterType MonType;
    public bool MonSummon;
    public int MonAtkRange;
    public float MonMoveSpeed;
    public List<int> MonSkill;
    public List<int> MonSummonID;
    public string MonTag;
    public List<string> MonDescription;

    // 몬스터의 스탯 데이터
    [Header("Monster Stat Setting")]
    public int MonLv;
    public bool MonBreak;
    public float BreakGage;
    public float MonHP;
    //# Increase는 % 증가(명중률은 +)
    public float MonHPIncrase;
    public float MonAtkSpeed;
    public float MonAttack;
    public float MonAttackIncrease;
    public float MonArmor;
    public float MonArmorIncrease;
    public float MonAccuracy;
    public float MonAvoid;
    public float MonAvoidIncrease;
    public float MonReg;

    // 몬스터 세팅
    [Header("Monster Setting")]
    public GameObject _prefab;  // 몬스터 프리팹

    [Header("Skills Setting")]
    public float _useSkillTime;     // 스킬 사용 시간

    [Header("Boss Attack Skill")]
    public MonsterSkillDataSO _bossSkill;   // 보스의 공격 스킬

    [Header("Boss Monster Recall Skill")]
    public SkillDataSO[] _recallSkills;           // 보스의 몬스터 소환 스킬
    public MonsterSkillDataSO _monterRecallSkill;   // 보스의 몬스터 소환 스킬

    // 파싱 데이터를 매핑
    public virtual void DataApply(MonsterDataFileData monsterData, MonsterStatFileData statData)
    {
        MonId = monsterData.MonId;
        MonName = monsterData.MonName;
        MonEnName = monsterData.MonEnName;
        MonType = monsterData.MonType;
        MonSummon = monsterData.MonSummon;
        MonAtkRange = monsterData.MonAtkRange;
        MonMoveSpeed = monsterData.MonMoveSpeed;
        MonSkill = monsterData.MonSkill;
        MonSummonID = monsterData.MonSummonID;
        MonTag = monsterData.MonTag;
        MonDescription = monsterData.MonDescription;

        MonLv = statData.MonLv;
        MonBreak = statData.MonBreak;
        MonHP = statData.MonHP;
        MonHPIncrase = statData.MonHPIncrase;
        MonAtkSpeed = statData.MonAtkSpeed;
        MonAttack = statData.MonAttack;
        MonAttackIncrease = statData.MonAttackIncrease;
        MonArmor = statData.MonArmor;
        MonArmorIncrease = statData.MonArmorIncrease;
        MonAccuracy = statData.MonAccuracy;
        MonAvoid = statData.MonAvoid;
        MonAvoidIncrease = statData.MonAvoidIncrease;
        MonReg = statData.MonReg;
    }
}
