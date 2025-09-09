using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectableCharacterSlot : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject selectionOverlay;
    [SerializeField] private Button characterButton;

    private CharacterDataSO characterData;
    private TeamFormationManager manager;

    public void Setup(CharacterDataSO data, TeamFormationManager formationManager)
    {
        characterData = data;
        manager = formationManager;

        characterImage.sprite = data._characterSprite;
        levelText.text = "Lv." + data._chaLv;

        if (characterButton) characterButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        manager.ToggleCharacterSelection(characterData);
    }

    public void UpdateSelectionVisual(bool isSelected)
    {
        if (selectionOverlay) selectionOverlay.SetActive(isSelected);
    }

    public CharacterDataSO GetCharacterData()
    {
        return characterData;
    }
}