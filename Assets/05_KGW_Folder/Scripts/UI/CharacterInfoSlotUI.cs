using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoSlotUI : MonoBehaviour
{
    [Header("Character Info Slot Setting")]
    [SerializeField] Image _characterPortrait;  // 캐릭터 초상화
    [SerializeField] Slider _characterHp;       // 캐릭터 체력바 
    [SerializeField] Slider _characterMp;       // 캐릭터 마나바
    [SerializeField] TMP_Text _characterPower;     // 캐릭터 전투력

    CharacterController _characterController;

    private void Awake()
    {
        // 체력, 마나 게이지 최소, 최대값 초기화
        if (_characterHp) { _characterHp.minValue = 0; _characterHp.maxValue = 1; }
        if (_characterMp) { _characterMp.minValue = 0; _characterMp.maxValue = 1; }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        _characterController.OnHpChange -= HpChangeCheck;
        _characterController.OnMpChange -= MpChangeCheck;
    }

    // 캐릭터 데이터 가져오기
    public void GetCharacterData(CharacterDataSO data)
    {
        _characterPortrait.sprite = data._characterSprite;
        _characterPower.text = "2000";        
        _characterHp.value = 1f;
        _characterMp.value = 0f;
    }

    // 캐릭터 컨트롤러 가져오기
    public void GetCharacterController(CharacterController chaData)
    {
        // 기존에 구독되어 있으면 해제
        if (_characterController != null)
        {
            _characterController.OnHpChange -= HpChangeCheck;
            _characterController.OnMpChange -= MpChangeCheck;
        }

        _characterController = chaData;

        // 체력, 마나 변화 이벤트 구독
        _characterController.OnHpChange += HpChangeCheck;
        _characterController.OnMpChange += MpChangeCheck;
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
}
