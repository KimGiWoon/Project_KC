using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class RoguelikeClosingUI : BaseUI
    {
        [Header("Background")]
        [SerializeField] private PanelFadeOut _sharpBackgroundFadeOut;

        [Header("Popup Panel")]
        [SerializeField] private Image _resultPopupPanel;
        [SerializeField] private TextMeshProUGUI _clearText;
        [SerializeField] private Image _cutLineImage;
        [SerializeField] private GameObject _battleResultObj;
        [SerializeField] private GameObject _relicResultObj;
        [SerializeField] private GameObject _cookcingResultObj;
        [SerializeField] private GameObject _yeopjeonResultObj;

        [Header("Interval Time")]
        [SerializeField] private float _resultInterval;
        [SerializeField] private float _totalResultInterval;

        [Header("Numbers")]
        [SerializeField] private TextMeshProUGUI _numberOfBattleText;
        [SerializeField] private TextMeshProUGUI _numberOfRelicText;
        [SerializeField] private TextMeshProUGUI _numberOfCookText;
        [SerializeField] private TextMeshProUGUI _numberOfYeopjeonText;

        [Header("Scores")]
        [SerializeField] private TextMeshProUGUI _battleScoreText;
        [SerializeField] private TextMeshProUGUI _relicScoreText;
        [SerializeField] private TextMeshProUGUI _cookScoreText;
        [SerializeField] private TextMeshProUGUI _yeopjeonScoreText;
        [SerializeField] private TextMeshProUGUI _totalScoreText;

        [Header("Recipe and Point")]
        [SerializeField] private Button _scorePanelButton;
        [SerializeField] private TextMeshProUGUI _baekRecipeBookText;
        [SerializeField] private TextMeshProUGUI _fineDiningRecipeBookText;
        [SerializeField] private TextMeshProUGUI _masterChefRecipeBookText;
        [SerializeField] private TextMeshProUGUI _pointText;

        private float _resultPopupPanelAlpha;
        private float _clearTextAlpha;
        private float _cutLineAlpha;
        private int _totalResultScore;
        private GameManager _gameManager;
        private BattleManager _battleManager;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _scorePanelButton.gameObject.SetActive(false);
            _gameManager = GameManager.Instance;
            _resultPopupPanelAlpha = _resultPopupPanel.color.a;
            _clearTextAlpha = _clearText.color.a;
            _cutLineAlpha = _cutLineImage.color.a;
            ClearAlpha();
            InactiveUI();
        }

        protected override void Start()
        {
            base.Start();
            _battleManager = FindObjectOfType<BattleManager>();
        }

        private void OnEnable()
        {
            _scorePanelButton.onClick.AddListener(ScorePanelButtonClicked);
        }

        private void OnDisable()
        {
            _scorePanelButton.onClick.RemoveListener(ScorePanelButtonClicked);
        }

        public override void Open()
        {
            StartCoroutine(FadeIn());
            base.Open();
        }

        private IEnumerator FadeIn()
        {
            yield return new WaitForSeconds(_sharpBackgroundFadeOut.fadeDuration);
            float elapsedTime = 0f;

            while (elapsedTime < _resultInterval)
            {
                elapsedTime += Time.deltaTime;

                float textAlpha = Mathf.Lerp(0f, _clearTextAlpha, elapsedTime / _resultInterval);
                var textColor = _clearText.color;
                textColor.a = textAlpha;
                _clearText.color = textColor;

                float resultPopupPanelAlpha = Mathf.Lerp(0f, _resultPopupPanelAlpha, elapsedTime / _resultInterval);
                _resultPopupPanel.color = new Color(
                    _resultPopupPanel.color.r,
                    _resultPopupPanel.color.g,
                    _resultPopupPanel.color.b,
                    resultPopupPanelAlpha
                );

                float cutLineAlpha = Mathf.Lerp(0f, _cutLineAlpha, elapsedTime / _resultInterval);
                _cutLineImage.color = new Color(
                    _cutLineImage.color.r,
                    _cutLineImage.color.g,
                    _cutLineImage.color.b,
                    cutLineAlpha
                );

                yield return null;
            }

            var resultIntervalTime = new WaitForSeconds(_resultInterval);
            yield return resultIntervalTime;
            SetBattleScore();
            yield return resultIntervalTime;
            SetRelicScore();
            yield return resultIntervalTime;
            SetCookScore();
            yield return resultIntervalTime;
            SetYeopjeonScore();
            yield return new WaitForSeconds(_totalResultInterval);
            SetTotalScore();
            SetRecipeAndPoint();
            _scorePanelButton.gameObject.SetActive(true);
        }

        private void ClearAlpha()
        {
            var textColor = _clearText.color;

            textColor.a = 0f;
            _clearText.color = textColor;

            _resultPopupPanel.color = new Color(
                _resultPopupPanel.color.r,
                _resultPopupPanel.color.g,
                _resultPopupPanel.color.b,
                0f
            );

            _cutLineImage.color = new Color(
                _cutLineImage.color.r,
                _cutLineImage.color.g,
                _cutLineImage.color.b,
                0f
            );
        }

        private void InactiveUI()
        {
            _battleResultObj.SetActive(false);
            _relicResultObj.SetActive(false);
            _cookcingResultObj.SetActive(false);
            _yeopjeonResultObj.SetActive(false);
        }

        //todo 각 항목별 수를 반영해야 함
        private void SetBattleScore()
        {
            _numberOfBattleText.text = _gameManager.ClearCount.ToString();
            _battleScoreText.text = _gameManager.Score.ToString();
            _totalResultScore += _gameManager.Score;
            _battleResultObj.SetActive(true);
        }

        private void SetRelicScore()
        {
            int relicCount = _gameManager.InGameItem.RelicCount;
            _numberOfRelicText.text = relicCount.ToString();

            var relicGradeCount = _gameManager.InGameItem.RelicGradeCount;
            int relicScore = 0;

            foreach (var gradeCount in relicGradeCount)
            {
                switch (gradeCount.Key)
                {
                    case RelicGrade.Normal:
                        relicScore += gradeCount.Value * 70;
                        break;
                    case RelicGrade.Rare:
                        relicScore += gradeCount.Value * 120;
                        break;
                    case RelicGrade.Debuff:
                        relicScore += gradeCount.Value * 30;
                        break;
                }
            }

            _relicScoreText.text = relicScore.ToString();
            _totalResultScore += relicScore;
            _relicResultObj.SetActive(true);
        }

        private void SetCookScore()
        {
            int cookCount = _gameManager.InGameItem.CookCount;
            _numberOfCookText.text = cookCount.ToString();

            int cookScore = cookCount * 30;

            _cookScoreText.text = cookScore.ToString();
            _totalResultScore += cookScore;
            _cookcingResultObj.SetActive(true);
        }

        private void SetYeopjeonScore()
        {
            int yeopjeonScore = _gameManager.Coin.totalYeopjeon;

            //# 총 획득한 엽전
            _numberOfYeopjeonText.text = yeopjeonScore.ToString();

            yeopjeonScore = _gameManager.Coin.totalYeopjeon / 100;
            yeopjeonScore *= 10;

            //# 엽전에 의한 점수
            _yeopjeonScoreText.text = yeopjeonScore.ToString();
            _totalResultScore += yeopjeonScore;
            _yeopjeonResultObj.SetActive(true);
        }

        private void SetTotalScore()
        {
            _totalScoreText.text = _totalResultScore.ToString();
        }

        private void SetRecipeAndPoint()
        {
            _scorePanelButton.interactable = false;
            int totalScore = _totalResultScore * 10 % 20000;

            int masterChef = _totalResultScore * 10 / 20000;
            int fineDining = totalScore / 5000;
            totalScore %= 5000;
            int baek = totalScore / 1000;
            int point = 0;

            if (_totalResultScore >= 200)
                point = (_totalResultScore - 200) / 250 + 1;

            _baekRecipeBookText.text = baek.ToString();
            _fineDiningRecipeBookText.text = fineDining.ToString();
            _masterChefRecipeBookText.text = masterChef.ToString();
            _pointText.text = point + " pts";

            if (baek != 0) _gameManager.Coin.AddRecipeItem(_gameManager.Coin.beek, baek);
            if (fineDining != 0) _gameManager.Coin.AddRecipeItem(_gameManager.Coin.fineDining, fineDining);
            if (masterChef != 0) _gameManager.Coin.AddRecipeItem(_gameManager.Coin.masterChef, masterChef);
            _gameManager.Coin.AddPoint(point);
            _gameManager.InGameItem.ClearItemCounts();
            _gameManager.ClearScore();
            _gameManager.ClearStageCount();
            _battleManager.ClearCharacterHp();
            //todo QA가 아닌 버전에서는 엽전도 Clear 해야 함
            _gameManager.Coin.ClearYeopjeon();
            _gameManager.ClearStage();

            _scorePanelButton.interactable = true;
        }

        private void ScorePanelButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.RoguelikeClosingUI);
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene);
        }
    }
}