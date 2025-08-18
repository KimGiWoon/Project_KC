using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 타입
public enum CharacterType
{
    // 근거리 딜러,       원거리 딜러,     탱커,   디버퍼,  버퍼
    ShortRangeDealer, LongDistanceDealer, Tanker, Debuffer, Buffer
}

// 캐릭터 등급
public enum characterRating
{
    //일반, 레어
    Common, Rare
}

// 캐릭터 데이터 저장
[CreateAssetMenu(fileName = "CharacterData", menuName = "Characters/CharacterData")]

// 캐릭터의 정보를 담는 플레이어 스크립터블 오브젝트
public class CharacterDataSO : ScriptableObject
{
    [Header("Character Data Setting")]
    public string _characterName;               // 캐릭터 이름
    public CharacterType _characterType;        // 캐릭터 타입
    public characterRating _characterRating;    // 캐릭터 등급
    public Sprite _characterSprite;             // 캐릭터 아이콘
    public GameObject _prefab;                  // 캐릭터 프리팹

    [Header("Character Stat Setting")]
    public float _moveSpeed;                    // 캐릭터 이동 속도 
    //public float _attackRange;                // 캐릭터 공격 사거리
    public float _attackSpeed;                  // 캐릭터 공격 속도
    public float _maxMp;                        // 캐릭터 마력
    public float _maxHp;                        // 캐릭터 체력
    public float _attackDamage;                 // 캐릭터 데미지
    public float _attackDefense;                // 캐릭터 방어력

    [Header("Skills Setting")]
    public SkillDataSO[] _skills;               // 캐릭터의 보유 스킬
}
