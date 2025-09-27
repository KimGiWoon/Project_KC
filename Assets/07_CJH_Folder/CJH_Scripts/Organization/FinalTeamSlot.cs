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
    private TextMeshProUGUI orderNumberText;
    private Image classIconImage;
    [SerializeField] private Sprite[] classIconSprites;

    private void Awake()
    {
        // OrderNumberText 라는 이름의 자식 오브젝트에서 TextMeshProUGUI 컴포넌트를 찾습니다.
        Transform orderTextTransform = transform.Find("OrderNumberText");
        if (orderTextTransform != null)
        {
            orderNumberText = orderTextTransform.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("'OrderNumberText' 이름의 자식 오브젝트를 찾을 수 없습니다. 프리팹에서 이름을 확인해주세요.", gameObject);
        }

        // ClaasIcon 이라는 이름의 자식 오브젝트에서 Image 컴포넌트를 찾습니다.
        Transform classIconTransform = transform.Find("ClaasIcon");
        if (classIconTransform != null)
        {
            classIconImage = classIconTransform.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("'ClaasIcon' 이름의 자식 오브젝트를 찾을 수 없습니다. 프리팹에서 이름을 확인해주세요.", gameObject);
        }
    }

    private void Start()
    {
        manager = TeamFormationManager.Instance;
        // manager를 Start에서 찾으면 패널이 꺼져있을 때 못찾을 수 있으니, DisplayCharacter에서 설정
        if (characterButton) characterButton.onClick.AddListener(OnClick);
    }

    public void DisplayCharacter(CharacterDataSO data, int index)
    {
        characterData = data;

        characterInfoGroup.SetActive(true);
        characterImage.sprite = data._characterSprite;
        levelText.text = GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName].ToString();
        characterButton.interactable = true;

        // 편성 순서 표시 (1, 2, 3)
        if (orderNumberText != null)
        {
            orderNumberText.text = (index + 1).ToString();
            orderNumberText.gameObject.SetActive(true);
        }

        // 직업 아이콘 표시
        if (classIconImage != null && classIconSprites != null && classIconSprites.Length > 0)
        {
            // CharacterDataSO의 _chaBaseData.ChaRole을 인덱스로 사용합니다.
            int roleIndex = (int)data._chaBaseData.ChaRole;
            if (roleIndex >= 0 && roleIndex < classIconSprites.Length)
            {
                classIconImage.sprite = classIconSprites[roleIndex];
                classIconImage.gameObject.SetActive(true);
            }
        }
    }

    public void DisplayEmpty()
    {
        characterData = null;
        characterInfoGroup.SetActive(false);

        if (orderNumberText != null) orderNumberText.gameObject.SetActive(false);
        if (classIconImage != null) classIconImage.gameObject.SetActive(false);
    }

    private void OnClick()
    {
        if (characterData != null && manager != null)
        {
            manager.SelectCharacter(characterData);
        }
    }
}