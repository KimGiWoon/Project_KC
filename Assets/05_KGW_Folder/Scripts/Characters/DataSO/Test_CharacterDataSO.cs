using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

// 캐릭터 데이터 저장
[CreateAssetMenu(fileName = "CharacterData", menuName = "Characters/Test_CharacterData")]

// 캐릭터의 정보를 담는 플레이어 스크립터블 오브젝트
public class Test_CharacterDataSO : ScriptableObject
{
    [Header("Character Data Setting")]
    public int _chaID;
    public string _chaName;
    public string _chaNameEn;
    public CharacterGrade _chaGrade;
    public CharacterRole _chaRole;
    public AttackRange _chaAtkRange;
    public float _chaMoveSpeed;
    public int _charSKill1;
    public int _charSkill2;
    public int _charSkill3;
    public bool _chaUpgrade1;
    public bool _chaUpgrade2;
    public bool _chaUpgrade3;
    public bool _chaUpgrade4;
    public bool _chaUpgrade5;
    public bool _chaUpgrade6;
    public int _chaAniIdle;
    public int _chaAniAttack;
    public int _chaAnimDeath;

    //// 파싱 데이터를 매핑
    //public virtual void DataApply(CharacterDataManager characterFiledata)
    //{
    //    _chaID = characterFiledata.ChaID;
    //    _chaName = characterFiledata.ChaName;
    //    _chaNameEn = characterFiledata.ChaNameEn;
    //    _chaGrade = characterFiledata.ChaGrade;
    //    _chaRole = characterFiledata.ChaRole;
    //    _chaAtkRange = characterFiledata.ChaAtkRange;
    //    _chaMoveSpeed = characterFiledata.ChaMoveSpeed;
    //    _charSKill1 = characterFiledata.CharSKill1;
    //    _charSkill2 = characterFiledata.CharSkill2;
    //    _charSkill3 = characterFiledata.CharSkill3;
    //    _chaUpgrade1 = characterFiledata.ChaUpgrade1;
    //    _chaUpgrade2 = characterFiledata.ChaUpgrade2;
    //    _chaUpgrade3 = characterFiledata.ChaUpgrade3;
    //    _chaUpgrade4 = characterFiledata.ChaUpgrade4;
    //    _chaUpgrade5 = characterFiledata.ChaUpgrade5;
    //    _chaUpgrade6 = characterFiledata.ChaUpgrade6;
    //    _chaAniIdle = characterFiledata.ChaAniIdle;
    //    _chaAniAttack = characterFiledata.ChaAniAttack;
    //    _chaAnimDeath = characterFiledata.ChaAnimDeath;
    //}
}
