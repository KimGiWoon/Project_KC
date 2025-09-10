using System;
using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

[Serializable]
public class CharacterState
{
    [Header("Character Base Data")]
    public int _chaID;
    public string _chaName;
    public CharacterEnName _chaEnName;
    public CharacterGrade _chaGrade;
    public CharacterRole _chaRole;
    public int _chaLevel;
    public float _chaCurrentHP;
    public float _chaMaxHP;
    public float _chaCurrentMP;
    public float _chaMaxMP;
    public float _chaMPRecovery;
    public float _chaAtkSpeed;
    public float _chaAttack;
    public float _chaArmor;
    public bool _isBarrier;
    public float _groggyDamage;
    public float _reductionUpValue;
    public float _reductionDownValue;

    [Header("Character Role State Data")]
    public int _chaAtkIsMelee;
    public float _chaAccuracy;
    public float _chaAvoid;
    public float _chaCrit;
    public float _chaCritDmg;
    public float _chaReg;
    public float _chaMoveSpeed;

    [Header("Character Skill Data")]
    public CharacterSkillDataSO _chaPassiveSkill;
    public CharacterSkillDataSO _chaActiveSkill;

    public bool _isManaFull;    // 마나 풀 여부
}
