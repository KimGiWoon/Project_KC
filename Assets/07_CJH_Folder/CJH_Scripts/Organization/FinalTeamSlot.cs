using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SDW;

public class FinalTeamSlot : MonoBehaviour
{
    [SerializeField] private GameObject characterInfoGroup;
    [SerializeField] private Image _characterHp;
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
        _characterHp.GetComponent<Image>();

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

    private void OnEnable()
    {
        HpChangeCheck();
    }

    private void Start()
    {
        manager = TeamFormationManager.Instance;
        // manager를 Start에서 찾으면 패널이 꺼져있을 때 못찾을 수 있으니, DisplayCharacter에서 설정
        if (characterButton) characterButton.onClick.AddListener(OnClick);
    }

    public void DisplayCharacter(CharacterDataSO data, int index, bool canInteractable)
    {
        int curlevel = GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName];
        int curUpgradeLevel = GameManager.Instance.CharacterData.BeadsInventory[data._chaBaseData.ChaEnName];

        characterData = data;

        characterInfoGroup.SetActive(true);
        characterImage.sprite = data._characterSprite;
        levelText.text = curlevel.ToString();
        characterButton.interactable = canInteractable;
        classIconImage.sprite = data.roleIcon;
        classIconImage.gameObject.SetActive(true);

        // 캐릭터의 레벨과 업그레이드 레벨 전달
        GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName] = curlevel;
        GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName] = curUpgradeLevel;

        HpChangeCheck();
    }

    // 체력바 변화
    private void HpChangeCheck()
    {
        // 캐릭터의 데이터가 없거나 저장된 데이터가 없으면 실행하지 않음
        if (characterData == null ||
            !GameManager.Instance.CharacterBattleDataSave._chaHpSave.ContainsKey(characterData._chaBaseData.ChaEnName)) return;

        var levelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[characterData._modifiedCharacterState._chaLevel];
            var beadData =
                GameManager.Instance.CharacterData.ChaBeadsData[
                    GameManager.Instance.CharacterData.BeadsInventory[characterData._chaBaseData.ChaEnName]];
        float curHp = GameManager.Instance.CharacterBattleDataSave._chaHpSave[characterData._chaBaseData.ChaEnName];
        float maxHp = GameManager.Instance.CharacterBattleDataSave._chaMaxHpSave[characterData._chaBaseData.ChaEnName] /
                      levelData.ChaHPIncrease / beadData.ChaHP;

        _characterHp.fillAmount = curHp / maxHp;
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
            HpChangeCheck();
        }
    }

    public CharacterDataSO GetCharacterData() => characterData;
}