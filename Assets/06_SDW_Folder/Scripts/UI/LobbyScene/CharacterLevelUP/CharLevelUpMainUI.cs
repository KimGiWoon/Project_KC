using System;
using System.Collections;
using JJY;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class CharLevelUpMainUI : BaseUI
    {
        [Header("Character")]
        [SerializeField] private TweenAnimation _charTweenAnimation;
        [SerializeField] private TextMeshProUGUI _charCurrentLevelText;
        [SerializeField] private TextMeshProUGUI _charLevelUpText;
        [SerializeField] private TextMeshProUGUI _charPlusExpText;
        [SerializeField] private TextMeshProUGUI _charCurrentExpText;
        [SerializeField] private Image _charExpGaugeImage;

        [Header("Buttons")]
        [SerializeField] private Button _mainLobbyButton;

        private TweenAnimation _tweenAnimation;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        public Action<UIName, UIName> OnSubUIOpenRequested;
        public Action<UIName, UIName> OnSubUICloseRequested;

        private CharacterDataManager _charDataManager;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
            _charDataManager = GameManager.Instance.CharacterData;
        }

        private void OnEnable()
        {
            _mainLobbyButton.onClick.AddListener(MainLobbyButtonClicked);
        }

        private void OnDisable()
        {
            _mainLobbyButton.onClick.RemoveListener(MainLobbyButtonClicked);
        }

        public override void Open()
        {
            base.Open();
            OnSubUIOpenRequested?.Invoke(UIName.CharInfoStatsUI, UIName.CharInfoBottomUI);
            _tweenAnimation.moveAway();
        }

        public override void Close()
        {
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            // yield return new WaitForSeconds(1f);
            _tweenAnimation.moveBack();
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            OnSubUICloseRequested?.Invoke(UIName.CharInfoStatsUI, UIName.CharInfoBottomUI);
            CharacterMoveBack();
            base.Close();
        }

        #region Text Methods

        public void SetCharacterInfo(CharacterDataSO data, int increasedLevel, int increasedExp)
        {
            int level = _charDataManager.CharEnNameLevel[data._chaBaseData.ChaEnName];
            SetCurrentLevel(level);
            SetLevelUp(increasedLevel);

            int exp = _charDataManager.CharEnNameExp[data._chaBaseData.ChaEnName];
            int maxExp = _charDataManager.ChaLevelUpStatData[level].ChaLevelPoint;
            SetCurrentExp(exp, maxExp);
            SetPlusExp(increasedExp);
        }

        private void SetCurrentLevel(int level) => _charCurrentLevelText.text = level.ToString();

        private void SetLevelUp(int increasedLevel)
        {
            if (increasedLevel == 0) _charLevelUpText.text = "";
            _charLevelUpText.text = "+" + increasedLevel;
        }

        private void SetPlusExp(int increasedExp)
        {
            if (increasedExp == 0) _charPlusExpText.text = "";
            _charPlusExpText.text = "+" + increasedExp;
        }

        private void SetCurrentExp(int currentExp, int maxExp)
        {
            _charCurrentExpText.text = $"{currentExp}/{maxExp}";
            _charExpGaugeImage.fillAmount = (float)currentExp / maxExp;
        }

        #endregion

        private void MainLobbyButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.MainLobbyUI);
            OnUICloseRequested?.Invoke(UIName.CharLevelUpMainUI);
        }

        public void CharacterMoveAway()
        {
            _charTweenAnimation.moveAway();
        }

        public void CharacterMoveBack()
        {
            _charTweenAnimation.moveBack();
        }
    }
}