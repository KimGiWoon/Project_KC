using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterSelectManager : SingletonManager<CharacterSelectManager> 
{
    [Header("Character Select List")]
    [SerializeField] public List<CharacterDataSO> _characterSelectList = new();    // 선택한 플레이어 리스트

    [Header("Character Select UI")]
    [SerializeField] Transform _characterSelectSlot;    // 선택한 캐릭터 프리팹 생성 위치
    [SerializeField] GameObject _selectSlotPrefab;      // 선택한 캐릭터 프리팹

    [Header("Character Select Setting")]
    [SerializeField] int _partNumber = 3;   // 전투 참가 인원

    public bool _canBettle = false;

    protected override void Awake()
    {
        base.Awake();
    }

    // 다시 생성될 선택한 캐릭터 슬롯 위치 설정
    public void SelectSlotReset(Transform slotPos)
    {
        _characterSelectSlot = slotPos;
        RebuildSelectUI();
    }

    // 선택한 캐릭터 재생성
    private void RebuildSelectUI()
    {
        // 중복 생성 방지로 이미 있던 슬롯 삭제 후 재생성
        for (int i = _characterSelectSlot.childCount - 1; i >= 0; i--)
        {
            Destroy(_characterSelectSlot.GetChild(i));
        }

        foreach(var selectSlot in _characterSelectList.Take(_partNumber))
        {
            CreateSelectUI(selectSlot);
        }
    }

    // 캐릭터 선택
    public void CharacterSelect(CharacterDataSO characterData)
    {
        // 전투 참가 인원에 충족하거나 이미 캐릭터 선택을 했으면 넘어감
        if (_characterSelectList.Contains(characterData)) return;

        if (_characterSelectList.Count >= _partNumber)
        {
            Debug.Log("전투 참가 가능 인원입니다. 더이상 추가할 수 없습니다.");
            return;
        }  

        // 선택한 플레이어 리스트에 추가
        _characterSelectList.Add(characterData);

        // 선택한 캐릭터 프리팹 생성
        CreateSelectUI(characterData);

        // 전투 가능 체크
        BettleStartCheck();
    }

    // 캐릭터 선택 해제
    public void CharacterDeselect(CharacterDataSO characterData)
    {
        // 선택한 캐릭터의 리스트에 취소하려고 한 캐릭터가 있는지 찾기
        if (_characterSelectList.Contains(characterData))
        {
            // 캐릭터가 있으면 리스트에서 삭제
            _characterSelectList.Remove(characterData);

            // 파티 인원이 충족이 안됬으므로 전투 불가능
            _canBettle = false;
            Debug.Log("전투 불가능");
        }
    }

    // 선택한 캐릭터 프리팹 생성
    public void CreateSelectUI(CharacterDataSO characterData)
    {
        // 선택한 캐릭터 프리팹을 슬롯에 생성
        GameObject characterSlot = Instantiate(_selectSlotPrefab, _characterSelectSlot);
        
        // 선택한 캐릭터 이름 입력
        characterSlot.GetComponentInChildren<TMP_Text>().text = characterData._characterName;

        // 캐릭터의 데이터 전달
        CharacterDeselect slotData = characterSlot.GetComponent<CharacterDeselect>();
        slotData.SetData(characterData);
    }

    // 전투 가능 체크
    public void BettleStartCheck()
    {
        // 파티 인원 충족하면 전투 가능
        if (_characterSelectList.Count == _partNumber)
        {
            _canBettle = true;
            Debug.Log("전투 가능");
        }
    }
}
