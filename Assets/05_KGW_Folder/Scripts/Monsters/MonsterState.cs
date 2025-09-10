using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

[Serializable]
public class MonsterState
{
    [Header("Monster Stat Data")]
    public int _monID;
    public string _monName;
    public MonsterEnName _monEnName;
    public MonsterType _monType;
    public int _monLevel;
    public bool _monBreak;
    public float _breakGage;
    public float _monCurrentHP;
    public float _monMaxHP;
    public int _monAtkRange;
    public float _monAttack;
    public float _monAtkSpeed;
    public float _monMoveSpeed;
    public float _monArmor;
    public float _monAccuracy;
    public float _monAvoid;
    public float _monReg;

    [Header("Monster Stat Increase")]
    public float _monHPIncrase;
    public float _monAttackIncrease;
    public float _monArmorIncrease;
    public float _monAvoidIncrease;

    [Header("Monster Skill Data")]
    public MonsterSkillDataSO _monActiveSkill_1;
    public MonsterSkillDataSO _monActiveSkill_2;
}
