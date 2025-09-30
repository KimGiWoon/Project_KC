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
        [SerializeField] private TextMeshProUGUI _mainTitleText;

        [Header("Animation")]
        [SerializeField] private TweenAnimation _bottomTweenAnimation;
        [SerializeField] private CharInfoStatsUI _charInfoStatsUI;
        private TweenAnimation _tweenAnimation;

        public Action<UIName, bool> OnUIOpenRequested;
        public Action<UIName, bool> OnUICloseRequested;

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
            OnUIOpenRequested?.Invoke(UIName.CharInfoStatsUI, true);
            StartCoroutine(DelayedOpen());
            _tweenAnimation.moveAway();
        }

        private IEnumerator DelayedOpen()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            OnUIOpenRequested?.Invoke(UIName.CharInfoBottomUI, true);
        }

        public override void Close()
        {
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            //todo CharInfo가 펴져있으면 접고 실행해야 함
            if (_charInfoStatsUI.IsExpanded)
            {
                _charInfoStatsUI.SetShrink();
                yield return new WaitForSeconds(0.4f);
            }

            OnUICloseRequested?.Invoke(UIName.CharInfoBottomUI, true);
            yield return new WaitForSeconds(_bottomTweenAnimation.tweenTime);

            _tweenAnimation.moveBack();
            OnUIOpenRequested?.Invoke(UIName.MainLobbyUI, false);
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);

            OnUICloseRequested?.Invoke(UIName.CharInfoStatsUI, true);
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
            if (_mainTitleText.text.Equals("메인 로비")) return;
            OnUICloseRequested?.Invoke(UIName.CharLevelUpMainUI, false);
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