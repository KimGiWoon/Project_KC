using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SDW;

public class SelectableCharacterSlot : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject selectionOverlay;
    [SerializeField] private Button characterButton;
    [SerializeField] private Image _classIcon;
    [SerializeField] private TextMeshProUGUI OrderNumberText;
    [SerializeField] private GameObject OrderNumberBackground;

    private CharacterDataSO characterData;
    private TeamFormationManager manager;

    public void Setup(CharacterDataSO data, TeamFormationManager formationManager)
    {
        characterData = data;
        manager = formationManager;

        characterImage.sprite = data._characterSprite;
        levelText.text = "Lv." + GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName];
        _classIcon.sprite = data.roleIcon;

        if (characterButton) characterButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        manager.SelectCharacter(characterData);
    }

    public void UpdateSelectionVisual(bool isSelected, int orderNumber)
    {
        if (selectionOverlay)
        {
            selectionOverlay.SetActive(isSelected);
            OrderNumberBackground.SetActive(isSelected);
            OrderNumberText.text = orderNumber.ToString();
        }
    }

    public CharacterDataSO GetCharacterData() => characterData;
}