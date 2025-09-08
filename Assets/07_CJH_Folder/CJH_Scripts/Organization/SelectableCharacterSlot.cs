using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectableCharacterSlot : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI levelText;

    private CharacterDataSO characterData;
    private TeamFormationManager manager;

    public void Setup(CharacterDataSO data, TeamFormationManager formationManager)
    {
        characterData = data;
        manager = formationManager;

        // UI 업데이트
        characterImage.sprite = characterData._characterSprite;
        levelText.text = "Lv." + characterData._chaLv;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        manager.SelectCharacter(characterData);
    }
}