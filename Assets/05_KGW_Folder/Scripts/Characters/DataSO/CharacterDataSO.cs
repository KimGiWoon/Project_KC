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

    [HideInInspector] public CharacterState _modifiedCharacterState = new CharacterState();
    [HideInInspector] public CharacterState _originalCharacterState = new CharacterState();

    // 파싱 데이터를 매핑
    public virtual void DataApply(CharacterBaseDataFileData characterData, CharacterTypeFileData typeData)
    {
        _chaBaseData = characterData;
        _chaTypeData = typeData;
        _chaLv = 1;
        _chaUpgradeLevel = 0;
    }

    public CharacterState GetOriginalCharacterState()
    {
        _originalCharacterState._chaLevel = _chaLv;
        _originalCharacterState._chaUpgrade = _chaUpgradeLevel;
        _originalCharacterState._chaID = _chaBaseData.ChaID;

        _originalCharacterState._chaName = _chaBaseData.ChaName;
        _originalCharacterState._chaEnName = _chaBaseData.ChaEnName;
        _originalCharacterState._chaGrade = _chaBaseData.ChaGrade;
        _originalCharacterState._chaRole = _chaBaseData.ChaRole;
        _originalCharacterState._chaCurrentHP = _chaBaseData.ChaHP;
        _originalCharacterState._chaMaxHP = _chaBaseData.ChaHP;
        _originalCharacterState._chaCurrentMP = 0f;
        _originalCharacterState._chaMaxMP = _chaBaseData.ChaMP;
        _originalCharacterState._chaMPRecovery = _chaBaseData.ChaMPRecovery;
        _originalCharacterState._chaAtkSpeed = _chaBaseData.ChaAtkSpeed;
        _originalCharacterState._chaAttack = _chaBaseData.ChaAttack;
        _originalCharacterState._chaArmor = _chaBaseData.ChaArmor;
        _originalCharacterState._chaAtkIsMelee = _chaTypeData.ChaAtkIsMelee;
        _originalCharacterState._chaAccuracy = _chaTypeData.ChaAccuracy;
        _originalCharacterState._chaAvoid = _chaTypeData.ChaAvoid;
        _originalCharacterState._chaCrit = _chaTypeData.ChaCrit;
        _originalCharacterState._chaCritDmg = _chaTypeData.ChaCritDmg;
        _originalCharacterState._chaReg = _chaTypeData.ChaReg;
        _originalCharacterState._chaMoveSpeed = _chaTypeData.ChaMoveSpeed;
        _originalCharacterState._reductionUpValue = 0f;
        _originalCharacterState._reductionDownValue = 0f;

        _originalCharacterState._isBarrier = false;
        _originalCharacterState._groggyDamage = 0f;
        _originalCharacterState._chaPassiveSkill = _chaPassiveSkill;
        _originalCharacterState._isResurrection = false;

        return _originalCharacterState;
    }

    public CharacterState GetModifiedCharacterState()
    {
        _modifiedCharacterState._chaLevel = _chaLv;
        _modifiedCharacterState._chaUpgrade = _chaUpgradeLevel;
        _modifiedCharacterState._chaID = _chaBaseData.ChaID;

        _modifiedCharacterState._chaName = _chaBaseData.ChaName;
        _modifiedCharacterState._chaEnName = _chaBaseData.ChaEnName;
        _modifiedCharacterState._chaGrade = _chaBaseData.ChaGrade;
        _modifiedCharacterState._chaRole = _chaBaseData.ChaRole;
        _modifiedCharacterState._chaCurrentHP = _chaBaseData.ChaHP;
        _modifiedCharacterState._chaMaxHP = _chaBaseData.ChaHP;
        _modifiedCharacterState._chaCurrentMP = 0f;
        _modifiedCharacterState._chaMaxMP = _chaBaseData.ChaMP;
        _modifiedCharacterState._chaMPRecovery = _chaBaseData.ChaMPRecovery;
        _modifiedCharacterState._chaAtkSpeed = _chaBaseData.ChaAtkSpeed;
        _modifiedCharacterState._chaAttack = _chaBaseData.ChaAttack;
        _modifiedCharacterState._chaArmor = _chaBaseData.ChaArmor;
        _modifiedCharacterState._chaAtkIsMelee = _chaTypeData.ChaAtkIsMelee;
        _modifiedCharacterState._chaAccuracy = _chaTypeData.ChaAccuracy;
        _modifiedCharacterState._chaAvoid = _chaTypeData.ChaAvoid;
        _modifiedCharacterState._chaCrit = _chaTypeData.ChaCrit;
        _modifiedCharacterState._chaCritDmg = _chaTypeData.ChaCritDmg;
        _modifiedCharacterState._chaReg = _chaTypeData.ChaReg;
        _modifiedCharacterState._chaMoveSpeed = _chaTypeData.ChaMoveSpeed;
        _modifiedCharacterState._reductionUpValue = 0f;
        _modifiedCharacterState._reductionDownValue = 0f;

        _modifiedCharacterState._isBarrier = false;
        _modifiedCharacterState._groggyDamage = 0f;
        _modifiedCharacterState._chaPassiveSkill = _chaPassiveSkill;
        _modifiedCharacterState._isResurrection = false;

        return _modifiedCharacterState;
    }
}