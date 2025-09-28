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
    public Image classIconImage;
    [SerializeField] private bool _isPartyUI;

    public CharacterDataSO characterData;
    private TeamFormationManager manager;
    private TextMeshProUGUI orderNumberText;

    private void Awake()
    {
        // OrderNumberText 라는 이름의 자식 오브젝트에서 TextMeshProUGUI 컴포넌트를 찾습니다.
        if (_isPartyUI) return;

        var orderTextTransform = transform.Find("OrderNumberTextCircle/OrderNumberText");
        if (orderTextTransform != null)
        {
            orderNumberText = orderTextTransform.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("'OrderNumberText' 이름의 자식 오브젝트를 찾을 수 없습니다. 프리팹에서 이름을 확인해주세요.", gameObject);
        }
    }

    private void Start()
    {
        manager = TeamFormationManager.Instance;
        // manager를 Start에서 찾으면 패널이 꺼져있을 때 못찾을 수 있으니, DisplayCharacter에서 설정
        if (characterButton) characterButton.onClick.AddListener(OnClick);
    }

    public void DisplayCharacter(CharacterDataSO data, int index, bool canInteractable)
    {
        characterData = data;

        characterInfoGroup.SetActive(true);
        characterImage.sprite = data._characterSprite;
        levelText.text = GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName].ToString();
        characterButton.interactable = canInteractable;
        classIconImage.sprite = data.roleIcon;
        classIconImage.gameObject.SetActive(true);
    }

    public void DisplayEmpty()
    {
        characterData = null;
        characterInfoGroup.SetActive(false);

        if (orderNumberText != null) orderNumberText.gameObject.SetActive(false);
    }

    private void OnClick()
    {
        if (characterData != null && manager != null)
        {
            manager.SelectCharacter(characterData);
        }
    }

    public CharacterDataSO GetCharacterData() => characterData;
}