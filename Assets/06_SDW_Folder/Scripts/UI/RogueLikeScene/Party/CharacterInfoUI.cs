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
            // 키가 존재하지 않으면 추가 (초기 세팅)
            if (!GameManager.Instance.CharacterBattleDataSave._chaLevel.ContainsKey(data._chaBaseData.ChaEnName))
            {
                GameManager.Instance.CharacterBattleDataSave._chaLevel.Add(data._chaBaseData.ChaEnName, data._chaLv);
            }
            if (!GameManager.Instance.CharacterBattleDataSave._chaUpgrade.ContainsKey(data._chaBaseData.ChaEnName))
            {
                GameManager.Instance.CharacterBattleDataSave._chaUpgrade.Add(data._chaBaseData.ChaEnName, data._chaUpgradeLevel);
            }

            var levelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName]];
            var upgradeData = GameManager.Instance.CharacterData.ChaBeadsData[GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName]];

            _characterImage.sprite = data._characterSprite;
            _characterNameText.text = data._chaBaseData.ChaName;
            _startValueText.text = GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName].ToString();
            _classNameText.text = data._chaBaseData.ChaRole.ToString();
            _classLevelText.text = "Lv. " + GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName];
            _hpText.text = (data._chaBaseData.ChaHP * levelData.ChaHPIncrease * upgradeData.ChaHP).ToString("F0");
            _attackText.text = (data._chaBaseData.ChaAttack * levelData.ChaAttackIncrease * upgradeData.ChaAttack).ToString("F0");
            _deffenceText.text = (data._chaBaseData.ChaArmor * levelData.ChaArmorIncrease * upgradeData.ChaArmor).ToString("F0");

            var characterBaseData = data._chaBaseData;

            var passiveSkill = data._chaPassiveSkill;
            _passiveSkillImage.sprite = data._passiveSkillSprite;
            _passiveSkillNameText.text = passiveSkill._chaSkillName;
            _passiveSkillDescriptionText.text = GetDescription(passiveSkill, characterBaseData);

            var activeSkill = data._chaActiveSkill;
            if (activeSkill == null) return;

            if (activeSkill._chaSkillID == -1) return;

            _activeSkillImage.sprite = data._activeSkillSprite;
            _activeSkillNameText.text = activeSkill._chaSkillName;
            _activeSkillDescriptionText.text = GetDescription(passiveSkill, characterBaseData);
        }

        private string GetDescription(CharacterSkillDataSO skillData, CharacterBaseDataFileData characterBaseData)
        {
            string description = skillData._chaSkillDescription.Replace(
                "{chaSkillChance}", skillData._chaSkillChance.ToString()
            );
            description = description.Replace(
                "{chaAttack*chaSkillValue}", (skillData._chaSkillValue * characterBaseData.ChaAttack).ToString()
            );
            description = description.Replace(
                "{chaEffectValue}", skillData._chaEffectValue.ToString()
            );
            description = description.Replace(
                "{chaSkillHit}", skillData._chaSkillHit.ToString()
            );

            return description;
        }

        private void CloseButtonClicked() => OnUICloseRequested?.Invoke(UIName.CharInfoUI);
    }
}