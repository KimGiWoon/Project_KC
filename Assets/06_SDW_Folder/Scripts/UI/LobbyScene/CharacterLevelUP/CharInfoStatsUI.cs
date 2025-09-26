using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class CharInfoStatsUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TweenAnimation _leftTweenAnimation;
        [SerializeField] private TweenAnimation _rightTweenAnimation;
        [SerializeField] private ExpandTweenAnimation _expandTweenAnimation;

        [Header("Left Components")]
        [SerializeField] private Image _charTypeImage;
        [SerializeField] private TextMeshProUGUI _charNameText;
        [SerializeField] private TextMeshProUGUI _charGradeText;
        [SerializeField] private Color _blankColor;
        [SerializeField] private Color _filledColor;
        [SerializeField] private List<Image> _charBeadImageLists;
        [SerializeField] private TextMeshProUGUI _charDescriptionText;
        [SerializeField] private Image _passiveSkillImage;
        [SerializeField] private TextMeshProUGUI _passiveSkillText;
        [SerializeField] private Image _activeSkillImage;
        [SerializeField] private TextMeshProUGUI _activeSkillText;
        [SerializeField] private Button _charInfoButton;
        private TextMeshProUGUI _charInfoButtonText;

        [Header("Right Components")]
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _defenceText;
        [SerializeField] private TextMeshProUGUI _manaText;
        [SerializeField] private TextMeshProUGUI _attackSpeed;
        [SerializeField] private TextMeshProUGUI _criticalPercentText;
        [SerializeField] private TextMeshProUGUI _criticalDamageText;
        [SerializeField] private Button _levelUpButton;

        [HideInInspector] public bool fromMain;
        private bool _isExpanded = false;
        public bool IsExpanded => _isExpanded;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _charInfoButtonText = _charInfoButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _charInfoButton.onClick.AddListener(CharInfoButtonClicked);
            _levelUpButton.onClick.AddListener(LevelUpButtonClicked);
        }

        private void OnDisable()
        {
            _charInfoButton.onClick.RemoveListener(CharInfoButtonClicked);
            _levelUpButton.onClick.RemoveListener(LevelUpButtonClicked);
        }

        public override void Open()
        {
            base.Open();
            if (fromMain)
            {
                fromMain = false;
                return;
            }

            _leftTweenAnimation.moveBack();
            _rightTweenAnimation.moveBack();
        }

        public override void Close()
        {
            if (fromMain)
            {
                base.Close();
                return;
            }

            _leftTweenAnimation.moveAway();
            _rightTweenAnimation.moveAway();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_leftTweenAnimation.tweenTime);
            base.Close();
        }

        // public void SetCharacterInfo(CharacterDataSO data)
        // {
        //     SetLeftComponents(data);
        //     SetRightComponents(data);
        // }

        private void SetLeftComponents(CharacterDataSO data)
        {
            //todo Character type image는 기획팀에 요청해야 함(패키지에 없음)
            // _charTypeImage.sprite = ;

            _charNameText.text = data._chaBaseData.ChaName;
            _charGradeText.text = data._chaBaseData.ChaGrade == CharacterGrade.Normal ? "노말" : "레어";

            int beadCount = GameManager.Instance.CharacterData.BeadsInventory[data._chaBaseData.ChaEnName];
            for (int i = 0; i < _charBeadImageLists.Count; i++)
            {
                if (i < beadCount) _charBeadImageLists[i].color = _filledColor;
                else _charBeadImageLists[i].color = _blankColor;
            }

            _charDescriptionText.text = data._chaBaseData.ChaIntroduction;
            _passiveSkillImage.sprite = data._passiveSkillSprite;
            _passiveSkillText.text = data._chaPassiveSkill._chaSkillDescription;

            if (data._chaActiveSkill == null) _activeSkillImage.gameObject.SetActive(false);
            else
            {
                _activeSkillImage.sprite = data._activeSkillSprite;
                _activeSkillText.text = data._chaActiveSkill._chaSkillDescription;
                _activeSkillImage.gameObject.SetActive(false);
            }
        }

        private void SetRightComponents(CharacterDataSO data)
        {
            //todo 추후 level 업, beads에 의해 증가된 능력치는 계산해서 넣어야 함
            _attackText.text = data._chaBaseData.ChaAttack.ToString();
            _healthText.text = data._chaBaseData.ChaHP.ToString();
            _defenceText.text = data._chaBaseData.ChaArmor.ToString();
            _manaText.text = data._chaBaseData.ChaMP.ToString();
            _attackSpeed.text = data._chaBaseData.ChaAtkSpeed.ToString();
            _criticalPercentText.text = data._chaTypeData.ChaCrit.ToString();
            _criticalDamageText.text = data._chaTypeData.ChaCritDmg.ToString();
        }

        private void CharInfoButtonClicked()
        {
            if (_isExpanded) SetShrink();
            else
            {
                _expandTweenAnimation.ToggleExpand();
                _isExpanded = true;
                _charInfoButtonText.text = "간단하게";
            }
        }

        public void SetShrink()
        {
            _expandTweenAnimation.ToggleShrink();
            _isExpanded = false;
            _charInfoButtonText.text = "자세히보기";
        }

        private void LevelUpButtonClicked()
        {
            SetShrink();
            OnUIOpenRequested?.Invoke(UIName.LevelUpUI);
            OnUICloseRequested?.Invoke(UIName.CharInfoStatsUI);
        }
    }
}