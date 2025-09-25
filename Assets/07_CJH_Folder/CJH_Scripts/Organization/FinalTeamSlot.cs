using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SDW;

public class FinalTeamSlot : MonoBehaviour
{
    [SerializeField] private GameObject characterInfoGroup;
    public Image characterImage;
    public TextMeshProUGUI levelText;
    public Button characterButton;

    public CharacterDataSO characterData;
    private TeamFormationManager manager;

    private void Start()
    {
        manager = TeamFormationManager.Instance;
        // manager를 Start에서 찾으면 패널이 꺼져있을 때 못찾을 수 있으니, DisplayCharacter에서 설정
        if (characterButton) characterButton.onClick.AddListener(OnClick);
    }

    public void DisplayCharacter(CharacterDataSO data)
    {
        characterData = data;

        characterInfoGroup.SetActive(true);
        characterImage.sprite = data._characterSprite;
        levelText.text = GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName].ToString();
        characterButton.interactable = true;
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
            manager.SelectCharacter(characterData);
        }
    }
}