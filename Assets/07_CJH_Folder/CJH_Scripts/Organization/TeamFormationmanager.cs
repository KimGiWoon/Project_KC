using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TeamFormationManager : MonoBehaviour
{
    [Header("--- UI 요소 연결 ---")]
    [SerializeField] private GameObject teamFormationPanel;
    [SerializeField] private GameObject characterSlotPrefab;
    [SerializeField] private Transform ownedCharacterGrid;
    [SerializeField] private List<FinalTeamSlot> finalTeamSlots_InPanel; // 편성창 내부의 최종 슬롯
    [SerializeField] private List<FinalTeamSlot> finalTeamSlots_OutPanel; // 바깥의 최종 슬롯
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [Header("--- 팝업 UI 연결 ---")]
    [SerializeField] private GameObject warningPopup;
    [SerializeField] private Button popupConfirmButton;

    [Header("--- 데이터 (테스트용) ---")]
    [SerializeField] private List<CharacterDataSO> allOwnedCharacters;

    private List<CharacterDataSO> pendingTeam = new List<CharacterDataSO>();   // 임시 선택 명단
    private List<CharacterDataSO> confirmedTeam = new List<CharacterDataSO>(); // 확정된 최종 팀 명단
    private List<SelectableCharacterSlot> selectableSlots = new List<SelectableCharacterSlot>();

    void Awake()
    {
        // 버튼 기능 연결
        if (confirmButton) confirmButton.onClick.AddListener(OnConfirm);
        if (closeButton) closeButton.onClick.AddListener(OnClose);
        if (popupConfirmButton) popupConfirmButton.onClick.AddListener(CloseWarningPopup);
    }

    void Start()
    {
        // 시작할 때 패널과 팝업은 비활성화
        if (teamFormationPanel) teamFormationPanel.SetActive(false);
        if (warningPopup) warningPopup.SetActive(false);
        // 시작할 때 외부 패널을 초기화
        UpdateFinalTeamPanel(finalTeamSlots_OutPanel, confirmedTeam);
    }

    // [외부용] 편성 창을 여는 함수
    public void OpenFormationPanel()
    {
        if (teamFormationPanel) teamFormationPanel.SetActive(true);
        Initialize();
    }

    // 창이 열릴 때마다 호출
    private void Initialize()
    {
        if (warningPopup) warningPopup.SetActive(false);
        // 이전에 확정했던 팀을 임시 선택팀으로 복사해서 시작
        pendingTeam = new List<CharacterDataSO>(confirmedTeam);

        PopulateOwnedCharacterGrid();
        UpdateAllVisuals();
    }

    // 상단 보유 캐릭터 목록 UI 생성
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

    // 편성창이 열려있을 때 모든 UI를 현재 선택 상태(pendingTeam)에 맞춰 새로고침
    private void UpdateAllVisuals()
    {
        // 1. 상단 슬롯들의 오버레이(체크 표시) 업데이트
        foreach (var slot in selectableSlots)
        {
            slot.UpdateSelectionVisual(pendingTeam.Contains(slot.GetCharacterData()));
        }

        // 2. 편성창 내부의 하단 슬롯 UI를 업데이트
        UpdateFinalTeamPanel(finalTeamSlots_InPanel, pendingTeam);
    }

    // 최종 팀 슬롯 패널 한 개를 업데이트하는 전용 함수
    private void UpdateFinalTeamPanel(List<FinalTeamSlot> slots, List<CharacterDataSO> teamData)
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < teamData.Count)
            {
                slots[i].DisplayCharacter(teamData[i]);
            }
            else
            {
                slots[i].DisplayEmpty();
            }
        }
    }

    // [상단/하단 슬롯 공용] 캐릭터 선택/해제 처리
    public void ToggleCharacterSelection(CharacterDataSO character)
    {
        if (pendingTeam.Contains(character))
        {
            pendingTeam.Remove(character); // 이미 있으면 제거 (선택 해제)
        }
        else
        {
            if (pendingTeam.Count < 3)
            {
                pendingTeam.Add(character); // 없으면 추가 (선택)
            }
        }
        UpdateAllVisuals(); // 모든 UI 즉시 새로고침
    }

    // '편성' 버튼 기능 (저장)
    private void OnConfirm()
    {
        if (pendingTeam.Count < 3)
        {
            ShowWarningPopup();
            return;
        }
        Debug.Log("팀 편성 확정! 변경사항을 저장합니다.");
        // 임시 명단을 확정 명단으로 복사(저장)
        confirmedTeam = new List<CharacterDataSO>(pendingTeam);
        // 바깥 패널도 최신 정보로 업데이트
        UpdateFinalTeamPanel(finalTeamSlots_OutPanel, confirmedTeam);
        if (teamFormationPanel) teamFormationPanel.SetActive(false);
    }

    // '닫기' 버튼 기능 (저장 안함)
    private void OnClose()
    {
        Debug.Log("편성 취소. 변경사항을 저장하지 않습니다.");
        if (teamFormationPanel) teamFormationPanel.SetActive(false);
    }

    private void ShowWarningPopup() { if (warningPopup) warningPopup.SetActive(true); }
    private void CloseWarningPopup() { if (warningPopup) warningPopup.SetActive(false); }
}