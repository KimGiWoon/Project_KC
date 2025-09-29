using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class CharacterInfoUI : BaseUI
    {
        [Header("Top Components")]
        [SerializeField] private Image _characterImage;
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
        [SerializeField] private GameObject _activeContainerObject;
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

        public void SetCharacterInfo(CharacterDataSO data)
        {
            int curlevel = GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName];
            int curUpgradeLevel = GameManager.Instance.CharacterData.BeadsInventory[data._chaBaseData.ChaEnName];

            //JJY 수정
            CharacterState currentCharacterStatData = data.GetModifiedCharacterState();

            GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName] = curlevel;
            GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName] = curUpgradeLevel;

            var levelData =
                GameManager.Instance.CharacterData.ChaLevelUpStatData[
                    GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName]];
            var upgradeData =
                GameManager.Instance.CharacterData.ChaBeadsData[
                    GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName]];

            _characterImage.sprite = data._characterSprite;
            _characterNameText.text = data._chaBaseData.ChaName;
            _startValueText.text =
                GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName].ToString();
            _classNameText.text = data._chaBaseData.ChaRole.ToString();
            _classLevelText.text = "Lv. " + GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName];
            _hpText.text = (data._chaBaseData.ChaHP * levelData.ChaHPIncrease * upgradeData.ChaHP).ToString("F0");
            _attackText.text = (currentCharacterStatData._chaAttack * levelData.ChaAttackIncrease * upgradeData.ChaAttack).ToString("F0");
            _deffenceText.text = (currentCharacterStatData._chaArmor * levelData.ChaArmorIncrease * upgradeData.ChaArmor).ToString("F0");

            // var characterBaseData = data._chaBaseData;

            var passiveSkill = data._chaPassiveSkill;
            _passiveSkillImage.sprite = data._passiveSkillSprite;
            _passiveSkillNameText.text = passiveSkill._chaSkillName;
            _passiveSkillDescriptionText.text = GetDescription(passiveSkill, currentCharacterStatData);

            var activeSkill = data._chaActiveSkill;
            if (activeSkill == null)
            {
                _activeContainerObject.SetActive(false);
                return;
            }

            if (activeSkill._chaSkillID == -1)
            {
                _activeContainerObject.SetActive(false);
                return;
            }

            _activeSkillImage.sprite = data._activeSkillSprite;
            _activeSkillNameText.text = activeSkill._chaSkillName;
            _activeSkillDescriptionText.text = GetDescription(activeSkill, currentCharacterStatData);
            _activeContainerObject.SetActive(true);
        }

        private string GetDescription(CharacterSkillDataSO skillData, CharacterState characterState)
        {
            string description = skillData._chaSkillDescription.Replace(
                "<chaSkillChance>", skillData._chaSkillChance.ToString("F0")
            );
            description = description.Replace(
                "<chaAttack>*<chaSkillValue>", (skillData._chaSkillValue * characterState._chaAttack).ToString("F0")
            );
            description = description.Replace(
                "<chaEffectValue*100>", (skillData._chaEffectValue * 100).ToString("F0")
            );
            description = description.Replace(
                "<chaSkillHit>", skillData._chaSkillHit.ToString("F0")
            );

            return description;
        }

        private void CloseButtonClicked() => OnUICloseRequested?.Invoke(UIName.CharInfoUI);
    }
}