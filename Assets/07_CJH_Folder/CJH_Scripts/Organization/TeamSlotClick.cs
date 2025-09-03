using UnityEngine;
using UnityEngine.UI;
using CJH;

[RequireComponent(typeof(Button))]
public class TeamSlotClick : MonoBehaviour
{
    [Tooltip("이 슬롯의 번호 (0, 1, 2)")]
    public int slotIndex;

    private Button button;
    private TeamManager teamManager;

    void Start()
    {
        button = GetComponent<Button>();
        teamManager = FindObjectOfType<TeamManager>();

        if (teamManager == null) return;

        // 버튼 클릭 시 OnRemoveCharacter 함수를 호출하도록 연결
        button.onClick.AddListener(OnRemoveCharacter);

        // 처음엔 아이콘 비활성화
        //iconImage.enabled = false;
    }

    /// <summary>
    /// 팀 슬롯이 클릭되었을 때 호출되는 함수
    /// </summary>
    private void OnRemoveCharacter()
    {
        // TeamManager에게 이 슬롯의 캐릭터를 제거해달라고 요청
        teamManager.RemoveCharacterFromTeam(slotIndex);
    }

    /// <summary>
    /// TeamManager가 호출하여 슬롯의 UI를 업데이트하는 함수
    /// </summary>
    public void UpdateSlot(CharacterData data)
    {
        if (data == null)
        {
            // 데이터가 없으면 아이콘 숨기기
            //iconImage.enabled = false;
        }
        else
        {
            // 데이터가 있으면 아이콘 표시
            //iconImage.sprite = data.characterIcon;
            //iconImage.enabled = true;
        }
    }
}