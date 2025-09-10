using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeamFormationManager : MonoBehaviour
{
    [Header("--- UI 요소 연결 ---")]
    [SerializeField] private GameObject characterSlotPrefab;
    [SerializeField] private Transform ownedCharacterGrid;

    [Header("--- 최종 팀 슬롯 패널 (두 곳 모두 연결) ---")]
    [SerializeField] private List<FinalTeamSlot> finalTeamSlots_PanelA; // 첫 번째 패널의 슬롯 3개
    [SerializeField] private List<FinalTeamSlot> finalTeamSlots_PanelB; // 두 번째 패널의 슬롯 3개
    [SerializeField] private GameObject teamFormationPanel;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [Header("--- 팝업 UI 연결 ---")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Button popupConfirmButton;

    [Header("--- 데이터 (테스트용) ---")]
    [SerializeField] private List<CharacterDataSO> allOwnedCharacters;

    private List<CharacterDataSO> selectedTeam = new List<CharacterDataSO>();
    private List<SelectableCharacterSlot> selectableSlots = new List<SelectableCharacterSlot>();
    void Awake()
    {
        Initialize();
        if (confirmButton) confirmButton.onClick.AddListener(OnConfirm);
        if (closeButton) closeButton.onClick.AddListener(OnClose);
        if (popupConfirmButton) popupConfirmButton.onClick.AddListener(CloseWarningPopup);
    }

    private void Initialize()
    {
        if (warningPanel) warningPanel.SetActive(false);
        selectedTeam.Clear();
        PopulateOwnedCharacterGrid();
        UpdateAllVisuals();
    }
    private void PopulateOwnedCharacterGrid()
    {
        foreach (Transform child in ownedCharacterGrid) Destroy(child.gameObject);
        selectableSlots.Clear();
        foreach (var characterData in allOwnedCharacters)
        {
            GameObject slotGO = Instantiate(characterSlotPrefab, ownedCharacterGrid);
            var slotScript = slotGO.GetComponent<SelectableCharacterSlot>();
            slotScript.Setup(characterData, this);
            selectableSlots.Add(slotScript);
        }
    }

    private void UpdateAllVisuals()
    {
        // 상단 슬롯 오버레이 업데이트
        foreach (var slot in selectableSlots)
        {
            slot.UpdateSelectionVisual(selectedTeam.Contains(slot.GetCharacterData()));
        }

        UpdateFinalTeamPanel(finalTeamSlots_PanelA);
        UpdateFinalTeamPanel(finalTeamSlots_PanelB);
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
    public void SelectCharacter(CharacterDataSO character)
    {
        if (selectedTeam.Contains(character)) return;
        if (selectedTeam.Count >= 3) return;
        selectedTeam.Add(character);
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

    private void OnConfirm()
    {
        // 팀원이 3명 이하로 선택된 상태에서 편성하면 경고 팝업 표시
        if (selectedTeam.Count < 3)
        {
            ShowWarningPopup();
            return;
        }
        Debug.Log("팀 편성 완료");
        if (teamFormationPanel) teamFormationPanel.SetActive(false);
    }

    private void OnClose()
    {
        // 팀원이 3명 이하로 선택된 상태에서 닫으려고 하면 경고 팝업 표시
        if (selectedTeam.Count < 3)
        {
            ShowWarningPopup();
            return;
        }
        if (teamFormationPanel) teamFormationPanel.SetActive(false);
    }
    private void ShowWarningPopup() { if (warningPanel) warningPanel.SetActive(true); }
    private void CloseWarningPopup() { if (warningPanel) warningPanel.SetActive(false); }
}