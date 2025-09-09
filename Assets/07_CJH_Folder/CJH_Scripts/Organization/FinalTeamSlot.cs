using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalTeamSlot : MonoBehaviour
{
    [SerializeField] private GameObject characterInfoGroup;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button characterButton;

    private CharacterDataSO characterData;
    private TeamFormationManager manager;

    void Start()
    {
        // manager를 Start에서 찾으면 패널이 꺼져있을 때 못찾을 수 있으니, DisplayCharacter에서 설정
        if (characterButton) characterButton.onClick.AddListener(OnClick);
    }

    public void DisplayCharacter(CharacterDataSO data)
    {
        characterData = data;
        // manager를 처음 데이터를 받을 때 한 번만 찾아옵니다.
        if (manager == null) manager = FindObjectOfType<TeamFormationManager>();

        characterInfoGroup.SetActive(true);
        characterImage.sprite = data._characterSprite;
        levelText.text = "Lv." + data._chaLv;
    }

    public void DisplayEmpty()
    {
        characterData = null;
        characterInfoGroup.SetActive(false);
    }

    private void OnClick()
    {
        if (characterData != null && manager != null)
        {
            manager.ToggleCharacterSelection(characterData);
        }
    }
}