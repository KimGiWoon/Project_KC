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
        [SerializeField] private float _pointPercentage = 0.05f;

        private float _resultPopupPanelAlpha;
        private float _clearTextAlpha;
        private float _cutLineAlpha;
        private int _totalResultScore;
        private GameManager _gameManager;

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

            //todo 각각의 성과를 표시해야 함
            SetBattleScore();
            yield return new WaitForSeconds(_resultInterval);
            SetRelicScore();
            yield return new WaitForSeconds(_resultInterval);
            SetCookScore();
            yield return new WaitForSeconds(_resultInterval);
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

        //todo 각 항목별 수를 반영해야 함
        private void SetBattleScore()
        {
            _battleScoreText.text = _gameManager.Score.ToString();
            _totalResultScore += _gameManager.Score;
            // _numberOfBattleText.text = ;
        }

        private void SetRelicScore()
        {
            _relicScoreText.text = _gameManager.Score.ToString();
            _totalResultScore += _gameManager.Score;
            // _numberOfRelicText.text = ;
        }

        private void SetCookScore()
        {
            _cookScoreText.text = _gameManager.Score.ToString();
            _totalResultScore += _gameManager.Score;
            // _numberOfCookText.text = ;
        }

        private void SetYeopjeonScore()
        {
            _yeopjeonScoreText.text = _gameManager.Score.ToString();
            _totalResultScore += _gameManager.Score;
            // _numberOfYeopjeonText.text = ;
        }

        private void SetTotalScore()
        {
            _totalScoreText.text = _gameManager.Score.ToString();
        }

        private void SetRecipeAndPoint()
        {
            _scorePanelButton.interactable = false;
            int totalScore = _totalResultScore % 20000;

            int masterChef = _totalResultScore / 20000;
            int fineDining = totalScore / 5000;
            totalScore = totalScore % 5000;
            int baek = totalScore % 1000 / 1000;
            int point = (int)(_totalResultScore / _pointPercentage);

            _baekRecipeBookText.text = baek.ToString();
            _fineDiningRecipeBookText.text = fineDining.ToString();
            _masterChefRecipeBookText.text = masterChef.ToString();
            _pointText.text = point.ToString() + " pts";

            _gameManager.Coin.AddRecipeItem(_gameManager.Coin.Baek, baek);
            _gameManager.Coin.AddRecipeItem(_gameManager.Coin.fineDining, fineDining);
            _gameManager.Coin.AddRecipeItem(_gameManager.Coin.masterChef, masterChef);
            _gameManager.Coin.AddPoint(point);

            GameManager.Instance.ClearScore();
            _scorePanelButton.interactable = true;
        }

        private void ScorePanelButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.RoguelikeClosingUI);
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene);
        }
    }
}