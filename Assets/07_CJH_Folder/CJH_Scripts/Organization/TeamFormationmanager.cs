using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TeamFormationManager : MonoBehaviour
{
    [Header("--- UI 연결 ---")]
    [SerializeField] private GameObject characterSlotPrefab; // 보유 캐릭터 슬롯의 프리팹
    [SerializeField] private Transform ownedCharacterGrid;   // 보유 캐릭터들이 생성될 Grid 영역
    [SerializeField] private List<FinalTeamSlot> finalTeamSlots; // 최종 팀 슬롯 3개의 스크립트
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [Header("--- 데이터 ---")]
    [SerializeField] private List<CharacterDataSO> allOwnedCharacters; // 테스트용 보유 캐릭터 목록

    private List<CharacterDataSO> selectedTeam = new List<CharacterDataSO>(); // 현재 선택된 팀원 목록

    void Start()
    {
        Initialize();
    }

    // 이 패널 오브젝트가 활성화될 때마다 호출됩니다.
    void OnEnable()
    {
        Initialize();
    }

    // 초기화 함수
    public void Initialize()
    {
        // 테스트용 보유 캐릭터 강제 추가
        // allOwnedCharacters.Add(...);

        // 선택된 팀 목록 초기화
        selectedTeam.Clear();


        // UI 업데이트
        PopulateOwnedCharacterGrid();
        UpdateFinalTeamUI();
    }

    // 보유 캐릭터 목록 UI를 채우는 함수
    private void PopulateOwnedCharacterGrid()
    {
        // 기존에 있던 슬롯들 모두 삭제
        foreach (Transform child in ownedCharacterGrid)
        {
            Destroy(child.gameObject);
        }

        // 보유한 캐릭터 수만큼 슬롯 생성
        foreach (var characterData in allOwnedCharacters)
        {
            GameObject slotGO = Instantiate(characterSlotPrefab, ownedCharacterGrid);
            var slotScript = slotGO.GetComponent<SelectableCharacterSlot>();
            slotScript.Setup(characterData, this);
        }
    }

    // 하단 최종 팀 슬롯 3개의 UI를 업데이트하는 함수
    private void UpdateFinalTeamUI()
    {
        for (int i = 0; i < finalTeamSlots.Count; i++)
        {
            if (i < selectedTeam.Count)
            {
                finalTeamSlots[i].DisplayCharacter(selectedTeam[i]);
            }
            else
            {
                finalTeamSlots[i].DisplayEmpty(); // 빈 슬롯으로 표시
            }
        }

        // 팀원이 3명일 때만 버튼 활성화
        bool isTeamFull = selectedTeam.Count == 3;
        confirmButton.interactable = isTeamFull;
        closeButton.interactable = isTeamFull;
    }

    // (SelectableCharacterSlot에서 호출) 캐릭터를 팀에 추가 시도
    public void SelectCharacter(CharacterDataSO character)
    {
        // 중복 확인
        if (selectedTeam.Contains(character))
        {
            Debug.Log(character._chaBaseData.ChaName + "은(는) 이미 팀에 포함되어 있습니다.");
            return;
        }

        // 팀이 꽉 찼는지 확인
        if (selectedTeam.Count >= 3)
        {
            Debug.Log("팀이 가득 찼습니다.");
            return;
        }

        // 팀에 추가
        selectedTeam.Add(character);
        UpdateFinalTeamUI();
    }

    /// <summary>
    /// (FinalTeamSlot에서 호출) 캐릭터를 팀에서 제거합니다.
    /// </summary>
    public void DeselectCharacter(CharacterDataSO character)
    {
        // 명단에 빼려는 캐릭터가 있는지 확인
        if (selectedTeam.Contains(character))
        {
            // 명단에서 제거
            selectedTeam.Remove(character);

            // UI를 최신 상태로 업데이트하여 빈 칸으로 보이게 함
            UpdateFinalTeamUI();
        }
    }

    // 외부에서 호출할 함수들 (버튼에 연결)
    public void OnConfirm()
    {
        Debug.Log("팀 편성 완료!");
        // TODO: 팀 정보를 저장하고 창을 닫는 로직
        gameObject.SetActive(false);
    }

    public void OnClose()
    {
        Debug.Log("팀 편성 취소");
        gameObject.SetActive(false);
    }
}