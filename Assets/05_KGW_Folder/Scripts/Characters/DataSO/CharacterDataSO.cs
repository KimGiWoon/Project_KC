using SDW;
using UnityEngine;

// 캐릭터 데이터 저장
[CreateAssetMenu(fileName = "CharacterData", menuName = "Characters/CharacterData")]

// 캐릭터의 정보를 담는 플레이어 스크립터블 오브젝트
public class CharacterDataSO : ScriptableObject
{
    // 캐릭터의 베이스 데이터
    [Header("Character Base Data")]
    public CharacterBaseDataFileData _chaBaseData;

    // 캐릭터의 타입 데이터
    [Header("Character Role State Data")]
    public CharacterTypeFileData _chaTypeData;

    // 캐릭터 세팅
    [Header("Character Setting")]
    public Sprite _characterSprite; // 캐릭터 아이콘
    public Sprite _characterCircleSprite; // 원형 아이콘
    public GameObject _prefab; // 캐릭터 프리팹
    public int _chaLv; // 캐릭터 레벨
    public int _chaUpgradeLevel; // 돌파 레벨

    [Header("맵 캐릭터 비주얼")]
    public Sprite frontViewSprite; // 맵에서 멈춰있을 때 보일 앞모습
    public Sprite backViewSprite; // 맵에서 이동할 때 보일 뒷모습

    [Header("Character Skill")]
    public CharacterSkillDataSO _chaPassiveSkill; // 캐릭터 패시브 스킬
    public CharacterSkillDataSO _chaActiveSkill; // 캐릭터 액티브 스킬
    public Sprite _passiveSkillSprite; // 캐릭터 패시브 스킬 이미지
    public Sprite _activeSkillSprite; // 캐릭터 액티브 스킬 이미지

    [Header("Gacha")]
    public int Beads;
    public Sprite GachaBackground;
    public Sprite GachaBackgroundTen;

    [Header("Character Role Type Icon")]
    public Sprite roleIcon;

    [Header("Character Large Deformation Image")]
    public Sprite largeDeformationSprite;

    // 파싱 데이터를 매핑
    public virtual void DataApply(CharacterBaseDataFileData characterData, CharacterTypeFileData typeData)
    {
        _chaBaseData = characterData;
        _chaTypeData = typeData;
        _chaLv = 1;
        _chaUpgradeLevel = 0;
    }
}