using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoSlotUI : MonoBehaviour
{
    [Header("Character Info Slot Setting")]
    [SerializeField] Image _characterPortrait;  // 캐릭터 초상화
    [SerializeField] Image _skillCanUseImage;  // 스킬 사용 가능 이미지
    [SerializeField] Slider _characterHp;       // 캐릭터 체력바 
    [SerializeField] Slider _characterMp;       // 캐릭터 마나바
    [SerializeField] Button _characterSkillButton;    // 캐릭터 스킬 사용 버튼

    [Header("Skill Ready Visual")]
    [SerializeField] RectTransform _CharacterSlot; // 움직일 캐릭터 슬롯
    [SerializeField] float _moveDistance = 40f;     // 움직일 거리

    MyCharacterController _characterController;
    Vector2 _basicPosition;
    bool _ready;

    private void Awake()
    {
        // 체력, 마나 게이지 최소, 최대값 초기화
        if (_characterHp) { _characterHp.minValue = 0; _characterHp.maxValue = 1; }
        if (_characterMp) { _characterMp.minValue = 0; _characterMp.maxValue = 1; }
        _basicPosition = _CharacterSlot.anchoredPosition;

        // 스킬 사용 버튼 지정
        _characterSkillButton.onClick.AddListener(OnUseSkillClick);
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        _characterController.OnHpChange -= HpChangeCheck;
        _characterController.OnMpChange -= MpChangeCheck;
        _characterController.OnSkillModeChange -= CanUseSkillMode;

        // 스킬 사용 버튼 해제
        _characterSkillButton.onClick.RemoveListener(OnUseSkillClick);
    }

    // 캐릭터 데이터 가져오기
    public void GetCharacterData(CharacterDataSO data)
    {
        _characterPortrait.sprite = data._characterSprite;
        _characterHp.value = 1f;
        _characterMp.value = 0f;
    }

    // 캐릭터 컨트롤러 가져오기
    public void GetCharacterController(MyCharacterController chaData)
    {
        // 기존에 구독되어 있으면 해제
        if (_characterController != null)
        {
            _characterController.OnHpChange -= HpChangeCheck;
            _characterController.OnMpChange -= MpChangeCheck;
            _characterController.OnSkillModeChange -= CanUseSkillMode;
        }

        _characterController = chaData;

        // 체력, 마나 변화 이벤트 구독
        _characterController.OnHpChange += HpChangeCheck;
        _characterController.OnMpChange += MpChangeCheck;
        _characterController.OnSkillModeChange += CanUseSkillMode;
    }

    // 받은 데미지로 체력바 변화
    private void HpChangeCheck(float _curHp)
    {
        if (_characterHp)
        {
            _characterHp.value = _curHp;
        }
    }

    // 충전되는 마나바 변화
    private void MpChangeCheck(float _curMp)
    {
        if (_characterMp)
        {
            _characterMp.value = _curMp;
        }
    }

    // 스킬 사용 버튼 클릭
    private void OnUseSkillClick()
    {
        // 스킬 사용 준비가 되면
        if (_ready)
        {
            // 캐릭터 스킬 사용
            _characterController.UseSkill();
        }
    }

    // 스킬 사용 모드
    private void CanUseSkillMode(bool canUseSkill)
    {
        // 스킬 사용이 가능하면
        if (canUseSkill)
        {
            // 사용 가능
            ReadySkill();
        }
        else
        {
            // 사용 불가능
            UnReadySkill();
        }
    }

    // 스킬 사용 가능
    private void ReadySkill()
    {
        if (_ready) return;

        _CharacterSlot.anchoredPosition = _basicPosition + Vector2.up * _moveDistance;
        _ready = true;
    }

    // 스킬 사용 불가능
    private void UnReadySkill()
    {
        if (!_ready) return;

        _CharacterSlot.anchoredPosition = _basicPosition;
        _ready = false;
    }
}
