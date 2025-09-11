using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using SDW;

public class TeamFormationManager : MonoBehaviour
{
    public static TeamFormationManager Instance { get; private set; }

    [Header("--- UI 요소 연결 ---")]
    [SerializeField] private GameObject characterSlotPrefab;
    [SerializeField] private Transform ownedCharacterGrid;
    [SerializeField] private PartyCharListUI _partyCharListUI;

    [Header("--- 최종 팀 슬롯 패널 (두 곳 모두 연결) ---")]
    //# PartyContainer의 slot
    [SerializeField] private List<FinalTeamSlot> finalTeamSlots; // 첫 번째 패널의 슬롯 3개
    private List<TeamCharacterInfo> _prevFinalTeamSlots = new List<TeamCharacterInfo>();

    [Header("--- 데이터 (테스트용) ---")]
    [SerializeField] private List<CharacterDataSO> allOwnedCharacters;

    private List<CharacterDataSO> selectedTeam = new List<CharacterDataSO>();
    private List<SelectableCharacterSlot> selectableSlots = new List<SelectableCharacterSlot>();
    private CharacterDataSO _lastSelectedCharacter;

    private void Awake()
    {
        // 싱글톤 초기화: 이미 인스턴스가 있으면 자신을 파괴
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < finalTeamSlots.Count; i++)
        {
            _prevFinalTeamSlots.Add(new TeamCharacterInfo
            {
                Sprite = finalTeamSlots[i].characterImage.sprite,
                LevelText = finalTeamSlots[i].levelText.text,
                Data = finalTeamSlots[i].characterData
            });
        }

        //# 1. Level - 내ㅊ림        
        //# 1. Level - 내림차순, 2. 이름 - 오름차순
        //# 1. Level - 내림차순, 2. 이름 - 오름차순
        var sorted = allOwnedCharacters
            .OrderByDescending(name => name._chaLv)
            .ThenBy(name => name._chaBaseData.ChaName);

        allOwnedCharacters = sorted.ToList();

        selectedTeam.Clear();
        PopulateOwnedCharacterGrid();
        UpdateSelectedTeam();
        UpdateAllVisuals();
    }

    private void PopulateOwnedCharacterGrid()
    {
        foreach (Transform child in ownedCharacterGrid)
        {
            Destroy(child.gameObject);
        }
        selectableSlots.Clear();
        foreach (var characterData in allOwnedCharacters)
        {
            var slotGO = Instantiate(characterSlotPrefab, ownedCharacterGrid);
            var slotScript = slotGO.GetComponent<SelectableCharacterSlot>();
            slotScript.Setup(characterData, this);
            selectableSlots.Add(slotScript);
        }
    }

    private void UpdateSelectedTeam()
    {
        foreach (var finalTeam in finalTeamSlots)
        {
            selectedTeam.Add(finalTeam.characterData);
        }
    }

    private void UpdateAllVisuals()
    {
        //todo 선택 취소 기능
        // 상단 슬롯 오버레이 업데이트
        foreach (var slot in selectableSlots)
        {
            slot.UpdateSelectionVisual(selectedTeam.Contains(slot.GetCharacterData()));
        }

        UpdateFinalTeamPanel(finalTeamSlots);
    }

    // 하단 패널 한 개를 업데이트하는 전용 함수
    private void UpdateFinalTeamPanel(List<FinalTeamSlot> slots)
    {
        // 리스트가 비어있으면 아무것도 하지 않음
        if (slots == null || slots.Count == 0) return;
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < selectedTeam.Count)
            {
                slots[i].DisplayCharacter(selectedTeam[i]);
            }
            else
            {
                slots[i].DisplayEmpty();
            }
        }
    }
    private void ClearFinalTeamPanel(List<FinalTeamSlot> slots)
    {
        // 리스트가 비어있으면 아무것도 하지 않음
        if (slots == null || slots.Count == 0) return;

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].DisplayEmpty();
        }
    }

    public void SelectCharacter(CharacterDataSO character)
    {
        //# 선택/비선택 구분
        _lastSelectedCharacter = character;
        if (selectedTeam.Contains(character))
        {
            selectedTeam.Remove(character);
        }
        else if (selectedTeam.Count >= 3) return;
        else
        {
            selectedTeam.Add(character);
        }
        UpdateAllVisuals();
    }
    public void DeselectCharacter(CharacterDataSO character)
    {
        if (selectedTeam.Contains(character))
        {
            selectedTeam.Remove(character);
            UpdateAllVisuals();
        }
    }

    public bool OnConfirm()
    {
        // 팀원이 3명 이하로 선택된 상태에서 편성하면 경고 팝업 표시
        if (selectedTeam.Count < 3)
        {
            ShowWarningPopup();
            return false;
        }

        for (int i = 0; i < _prevFinalTeamSlots.Count; i++)
        {
            finalTeamSlots[i].characterButton.interactable = false;
        }

        _lastSelectedCharacter = null;
        return true;
    }

    public void OnClose()
    {
        for (int i = 0; i < _prevFinalTeamSlots.Count; i++)
        {
            finalTeamSlots[i].characterImage.sprite = _prevFinalTeamSlots[i].Sprite;
            finalTeamSlots[i].characterButton.interactable = false;
            finalTeamSlots[i].levelText.text = _prevFinalTeamSlots[i].LevelText;
            finalTeamSlots[i].characterData = _prevFinalTeamSlots[i].Data;
            finalTeamSlots[i].gameObject.SetActive(true);
        }

        _lastSelectedCharacter = null;
    }
    private void ShowWarningPopup() => _partyCharListUI.ShowWarningPopup();

    public void ClearPrevFinalTeamSlots() => _prevFinalTeamSlots.Clear();

    public CharacterDataSO GetLastSelectedCharacterInfo() => _lastSelectedCharacter;
}