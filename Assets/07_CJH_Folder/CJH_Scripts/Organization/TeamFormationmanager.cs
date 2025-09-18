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

    private List<CharacterDataSO> _sortedCharacterData = new List<CharacterDataSO>();

    private List<SelectableCharacterSlot> selectableSlots = new List<SelectableCharacterSlot>();
    private CharacterDataSO _lastSelectedCharacter;
    private CharacterDataManager _charData;
    private List<CharacterDataSO> _selectedTeam = new List<CharacterDataSO>();
    private GameManager _gameManager;
    private bool _isDownloaded;

    private void Awake()
    {
        // 싱글톤 초기화: 이미 인스턴스가 있으면 자신을 파괴
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _charData = _gameManager.CharacterData;
    }

    private void Update()
    {
        if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
            _isDownloaded) return;

        InitPartUI();
        _isDownloaded = true;
    }

    public void InitPartUI()
    {
        for (int i = 0; i < _charData.SelectedTeam.Count; i++)
        {
            finalTeamSlots[i].characterImage.sprite = _charData.SelectedTeam[i]._characterSprite;
            finalTeamSlots[i].characterButton.interactable = false;
            finalTeamSlots[i].levelText.text = "Lv." + _charData.SelectedTeam[i]._chaLv.ToString();
            finalTeamSlots[i].characterData = _charData.SelectedTeam[i];
            finalTeamSlots[i].gameObject.SetActive(true);
            _selectedTeam.Add(_charData.SelectedTeam[i]);
        }
    }

    public void Initialize()
    {
        _charData = GameManager.Instance.CharacterData;

        for (int i = 0; i < finalTeamSlots.Count; i++)
        {
            _prevFinalTeamSlots.Add(new TeamCharacterInfo
            {
                Sprite = finalTeamSlots[i].characterImage.sprite,
                LevelText = finalTeamSlots[i].levelText.text,
                Data = finalTeamSlots[i].characterData
            });
        }

        //# 1. Level - 내림차순, 2. 이름 - 오름차순
        //# 레벨 >> 등급 (레어가 먼저) >> 역할군 (탱커, 근거리, 디버퍼, 힐러, 원거리) >> 이름
        _sortedCharacterData = _charData.AllOwnedCharacters
            .OrderByDescending(data => data._chaLv)
            .OrderByDescending(data => data._chaBaseData.ChaGrade)
            .ThenBy(data => data._chaBaseData.ChaRole)
            .ThenBy(data => data._chaBaseData.ChaName)
            .ToList();

        _charData.ClearSelectedTeam();
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
        foreach (var characterData in _sortedCharacterData)
        {
            var slotGO = Instantiate(characterSlotPrefab, ownedCharacterGrid);
            var slotScript = slotGO.GetComponent<SelectableCharacterSlot>();
            slotScript.Setup(characterData, this);
            selectableSlots.Add(slotScript);
        }
    }

    private void UpdateSelectedTeam()
    {
        var selectedTeam = new List<CharacterDataSO>();
        foreach (var finalTeam in finalTeamSlots)
        {
            finalTeam.characterButton.interactable = true;
            selectedTeam.Add(finalTeam.characterData);
        }
        _charData.SetSelectedTeam(selectedTeam, false);
    }

    private void UpdateAllVisuals()
    {
        foreach (var slot in selectableSlots)
        {
            slot.UpdateSelectionVisual(_charData.SelectedTeam.Contains(slot.GetCharacterData()));
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
            if (i < _charData.SelectedTeam.Count)
            {
                slots[i].DisplayCharacter(_charData.SelectedTeam[i]);
            }
            else
            {
                slots[i].DisplayEmpty();
            }
        }
    }


    public void SelectCharacter(CharacterDataSO character)
    {
        //# 선택/비선택 구분
        _lastSelectedCharacter = character;
        if (_charData.SelectedTeam.Contains(character))
        {
            _charData.RemoveSelectedTeamMember(character);
            _selectedTeam.Remove(character);
        }
        else if (_charData.SelectedTeam.Count >= 3) return;
        else
        {
            _charData.AddSelectedTeamMember(character);
            _selectedTeam.Add(character);
        }
        UpdateAllVisuals();
    }

    public bool OnConfirm()
    {
        // 팀원이 3명 이하로 선택된 상태에서 편성하면 경고 팝업 표시
        if (_charData.SelectedTeam.Count < 3)
        {
            ShowWarningPopup();
            return false;
        }

        for (int i = 0; i < _prevFinalTeamSlots.Count; i++)
        {
            finalTeamSlots[i].characterButton.interactable = false;
        }

        _charData.SetSelectedTeam(_selectedTeam, true);
        _selectedTeam.Clear();
        _lastSelectedCharacter = null;
        return true;
    }

    public void OnClose()
    {
        _charData.ClearSelectedTeam();

        var selectedTeam = new List<CharacterDataSO>();

        for (int i = 0; i < _prevFinalTeamSlots.Count; i++)
        {
            finalTeamSlots[i].characterImage.sprite = _prevFinalTeamSlots[i].Sprite;
            finalTeamSlots[i].characterButton.interactable = false;
            finalTeamSlots[i].levelText.text = "Lv. " + _prevFinalTeamSlots[i].LevelText;
            finalTeamSlots[i].characterData = _prevFinalTeamSlots[i].Data;
            finalTeamSlots[i].gameObject.SetActive(true);

            selectedTeam.Add(_prevFinalTeamSlots[i].Data);

            finalTeamSlots[i].characterData = _prevFinalTeamSlots[i].Data;
            finalTeamSlots[i].gameObject.SetActive(true);
        }

        _charData.SetSelectedTeam(selectedTeam, false);
        _lastSelectedCharacter = null;
    }
    private void ShowWarningPopup() => _partyCharListUI.ShowWarningPopup();

    public void ClearPrevFinalTeamSlots() => _prevFinalTeamSlots.Clear();

    public CharacterDataSO GetLastSelectedCharacterInfo() => _lastSelectedCharacter;
}