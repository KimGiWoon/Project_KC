using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터 등급
public enum MonsterRating
{
    //일반, 정예,    지역,      최종
    Common, Elite, LocalBoss, FinalBoss
}

// 몬스터 데이터 저장
[CreateAssetMenu(fileName = "MonsterData", menuName = "Monsters/MonsterData")]

// 몬스터의 정보를 담는 몬스터 스크립터블 오브젝트
public class MonsterDataSO : ScriptableObject
{
    [Header("Monster Data Setting")]
    public string _monsterName;             // 몬스터 이름
    public MonsterRating _monsterRating;    // 몬스터 등급
    public GameObject _prefab;              // 몬스터 프리팹

    [Header("Monster Stat Setting")]
    public float _moveSpeed;                // 몬스터 이동 속도
    public float _attackSpeed;              // 몬스터 공격 속도
    public float _attackRange;              // 몬스터 공격 사거리
    public float _maxHp;                    // 몬스터 체력
    public float _attackDamage;             // 몬스터 데미지
    public float _attackDefense;            // 몬스터 방어력

    [Header("Skills Setting")]
    public SkillDataSO[] _skills;           // 몬스터의 보유 스킬
}
