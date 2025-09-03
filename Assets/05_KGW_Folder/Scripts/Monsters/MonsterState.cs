using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterState
{
    [Header("Monster Stat Data")]
    public int _monID;
    public string _monName;
    public int _monLevel;
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
}
