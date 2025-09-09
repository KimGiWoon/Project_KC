using System;
using UnityEngine;
using UnityEngine.UI;
using SDW;
using TMPro;

public class CharacterInfoUI : BaseUI
{
    [Header("Top Components")]
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private TextMeshProUGUI _startValueText;
    [SerializeField] private TextMeshProUGUI _classNameText;
    [SerializeField] private TextMeshProUGUI _classLevelText;

    [Header("HP Components")]
    [SerializeField] private TextMeshProUGUI _hpText;

    [Header("Attack Components")]
    [SerializeField] private TextMeshProUGUI _attackText;

    [Header("Defence Components")]
    [SerializeField] private TextMeshProUGUI _deffenceText;

    [Header("Passive Skill Components")]
    [SerializeField] private Image _passiveSkillImage;
    [SerializeField] private TextMeshProUGUI _passiveSkillNameText;
    [SerializeField] private TextMeshProUGUI _passiveSkillDescriptionText;

    [Header("Active Skill Components")]
    [SerializeField] private Image _activeSkillImage;
    [SerializeField] private TextMeshProUGUI _activeSkillNameText;
    [SerializeField] private TextMeshProUGUI _activeSkillDescriptionText;

    [SerializeField] private Button _closeButton;

    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(CloseButtonClicked);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(CloseButtonClicked);
    }

    //todo UIManager에서 SetCharacterInfo를 호출하면서 data 전달이 필요함
    //todo CharacterDataSO는 임시로 넣은거라 변경하시면 됩니다.
    public void SetCharacterInfo(CharacterDataSO data)
    {
        _characterNameText.text = data.name;
        _startValueText.text = data._chaUpgradeLevel.ToString();
        _classNameText.text = data._chaBaseData.ChaRole.ToString();
        _classLevelText.text = "Lv. " + data._chaLv;
        //todo Level에 따른 HP로 변경되어야 함
        _hpText.text = data._chaBaseData.ChaHP.ToString();
        //todo Level에 따른 공격력으로 변경되어야 함
        _attackText.text = data._chaBaseData.ChaAttack.ToString();
        //todo Level에 따른 방어력으로 변경되어야 함
        _deffenceText.text = data._chaBaseData.ChaArmor.ToString();

        var passiveSkill = GameManager.Instance.CharacterData.ChaIdSkillData[data._chaBaseData.ChaPassiveSkill];
        _passiveSkillImage.sprite = data._passiveSkillSprite;
        _passiveSkillNameText.text = passiveSkill.ChaSkillName;
        _passiveSkillDescriptionText.text = passiveSkill.ChaSkillDescription;

        var activeSkill = GameManager.Instance.CharacterData.ChaIdSkillData[data._chaBaseData.ChaActiveSkill];

        if (activeSkill.ChaSkillID == -1) return;

        _activeSkillImage.sprite = data._activeSkillSprite;
        _activeSkillNameText.text = activeSkill.ChaSkillName;
        _activeSkillDescriptionText.text = activeSkill.ChaSkillDescription;
    }

    private void CloseButtonClicked() => OnUICloseRequested?.Invoke(UIName.CharInfoUI);
}