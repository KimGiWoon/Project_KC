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
        Transform classIconTransform = transform.Find("ClaasIconCircle/ClaasIcon");
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

        if (classIconImage != null && classIconSprites != null)
        {
            // 실제로 어떤 직업 번호가 들어오는지 콘솔에 출력해봅니다.
            int roleIndex = (int)data._chaBaseData.ChaRole;
            Debug.Log($"캐릭터: {data._chaBaseData.ChaName}, 직업 번호(roleIndex): {roleIndex}");

            // 아이콘 배열에 이미지가 있는지, 배열 길이는 몇인지 확인합니다.
            if (classIconSprites.Length <= roleIndex)
            {
                Debug.LogError($"[에러!] 직업 번호({roleIndex})에 맞는 아이콘이 없습니다. Job Icon Sprites 배열의 크기({classIconSprites.Length})를 확인해주세요!");
            }
            else if (classIconSprites[roleIndex] == null)
            {
                Debug.LogError($"[에러!] 직업 번호({roleIndex})에 해당하는 아이콘이 비어있습니다(null). 프리팹 인스펙터에서 아이콘을 할당해주세요!");
            }
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