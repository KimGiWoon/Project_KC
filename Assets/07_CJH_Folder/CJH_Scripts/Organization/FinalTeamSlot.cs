using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalTeamSlot : MonoBehaviour
{
    [SerializeField] private GameObject characterInfoGroup; // 이미지, 텍스트를 포함하는 그룹
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI levelText;

    private CharacterDataSO characterData;
    private TeamFormationManager manager;

    void Start()
    {
        manager = GetComponentInParent<TeamFormationManager>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // 캐릭터 정보 표시
    public void DisplayCharacter(CharacterDataSO data)
    {
        characterData = data;
        characterInfoGroup.SetActive(true);
        characterImage.sprite = characterData._characterSprite;
        levelText.text = "Lv." + characterData._chaLv;
    }

    // 빈 슬롯으로 표시
    public void DisplayEmpty()
    {
        characterData = null;
        characterInfoGroup.SetActive(false);
    }

    private void OnClick()
    {
        Debug.Log(gameObject.name + " 버튼 클릭됨! 현재 캐릭터: " + (characterData != null ? characterData._chaBaseData.ChaName : "없음"));
        // 캐릭터가 할당된 상태에서만 (빈 슬롯이 아닐 때만)
        if (characterData != null)
        {
            // 매니저에게 내 캐릭터를 팀에서 빼달라고 요청합니다.
            manager.DeselectCharacter(characterData);
        }
    }
}