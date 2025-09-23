using System;
using System.Collections;
using JJY;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class LevelUpUI : BaseUI
    {
        [Header("UI Elements")]
        [SerializeField] private TweenAnimation _centerTweenAnimation;
        [SerializeField] private TweenAnimation _bottomTweenAnimation;

        [Header("Center Container")]
        [SerializeField] private Button _beekRecipeBook;
        [SerializeField] private Button _fineDiningRecipeBook;
        [SerializeField] private Button _masterChefRecipeBook;
        [SerializeField] private TextMeshProUGUI _beekValueText;
        [SerializeField] private TextMeshProUGUI _fineDiningValueText;
        [SerializeField] private TextMeshProUGUI _masterChefValueText;
        [SerializeField] private Slider _gaugeSlider;
        [SerializeField] private TextMeshProUGUI _selectedBookCountText;

        [SerializeField] private Sprite _beekRecipeSelectedSprite;
        [SerializeField] private Sprite _fineDiningRecipeSelectedSprite;
        [SerializeField] private Sprite _masterChefRecipeSelectedSprite;

        private Image _beekRecipeImage;
        private Image _fineDiningRecipeImage;
        private Image _masterChefRecipeImage;
        private Sprite _beekRecipeUnselectedSprite;
        private Sprite _fineDiningRecipeUnselectedSprite;
        private Sprite _masterChefRecipeUnselectedSprite;

        [Header("Bottom Container")]
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private Button _backButton;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        public Action<string, int> OnRecipeButtonClicked;

        private CoinManager _coin;
        private int _maxCount;
        private int _selectedRecipeValue;
        private string _selectedRecipe;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _gaugeSlider.gameObject.SetActive(false);
            _levelUpButton.gameObject.SetActive(false);

            _beekRecipeImage = _beekRecipeBook.GetComponent<Image>();
            _beekRecipeUnselectedSprite = _beekRecipeImage.sprite;

            _fineDiningRecipeImage = _fineDiningRecipeBook.GetComponent<Image>();
            _fineDiningRecipeUnselectedSprite = _fineDiningRecipeImage.sprite;

            _masterChefRecipeImage = _masterChefRecipeBook.GetComponent<Image>();
            _masterChefRecipeUnselectedSprite = _masterChefRecipeImage.sprite;
        }

        protected override void Start()
        {
            base.Start();
            _coin = GameManager.Instance.Coin;
        }

        private void OnEnable()
        {
            _beekRecipeBook.onClick.AddListener(() => { RecipeButtonClicked(_coin.beek); });
            _fineDiningRecipeBook.onClick.AddListener(() => { RecipeButtonClicked(_coin.fineDining); });
            _masterChefRecipeBook.onClick.AddListener(() => { RecipeButtonClicked(_coin.masterChef); });
            // _levelUpButton.onClick.AddListener(LevelUpButtonClicked);
            _backButton.onClick.AddListener(BackButtonClicked);
            _gaugeSlider.onValueChanged.AddListener(SliderValueChanged);
        }

        private void OnDisable()
        {
            _beekRecipeBook.onClick.RemoveListener(() => { RecipeButtonClicked(_coin.beek); });
            _fineDiningRecipeBook.onClick.RemoveListener(() => { RecipeButtonClicked(_coin.fineDining); });
            _masterChefRecipeBook.onClick.RemoveListener(() => { RecipeButtonClicked(_coin.masterChef); });
            // _levelUpButton.onClick.RemoveListener(LevelUpButtonClicked);
            _backButton.onClick.RemoveListener(BackButtonClicked);
            _gaugeSlider.onValueChanged.RemoveListener(SliderValueChanged);
        }

        public override void Open()
        {
            _selectedRecipeValue = 0;
            _selectedRecipe = "";
            _maxCount = 0;
            UpdateCoin();
            base.Open();
            StartCoroutine(DelayedOpen());
        }

        private IEnumerator DelayedOpen()
        {
            yield return null;
            yield return null;
            _centerTweenAnimation.moveAway();
            _bottomTweenAnimation.moveAway();
        }

        public override void Close()
        {
            _centerTweenAnimation.moveBack();
            _bottomTweenAnimation.moveBack();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_centerTweenAnimation.tweenTime);
            base.Close();
            _gaugeSlider.gameObject.SetActive(false);
            _levelUpButton.gameObject.SetActive(false);
        }

        private void UpdateCoin()
        {
            _beekValueText.text = _coin.Items[_coin.beek].ToString();
            _fineDiningValueText.text = _coin.Items[_coin.fineDining].ToString();
            _masterChefValueText.text = _coin.Items[_coin.masterChef].ToString();


            if (!string.IsNullOrEmpty(_selectedRecipe))
                _maxCount = _coin.Items[_selectedRecipe];
        }

        private void RecipeButtonClicked(string recipe)
        {
            _maxCount = 0;
            _selectedRecipe = recipe;

            if (recipe.Equals(_coin.beek))
            {
                _beekRecipeImage.sprite = _beekRecipeSelectedSprite;
                _fineDiningRecipeImage.sprite = _fineDiningRecipeUnselectedSprite;
                _masterChefRecipeImage.sprite = _masterChefRecipeUnselectedSprite;
                _maxCount = _coin.Items[_coin.beek];
            }
            else if (recipe.Equals(_coin.fineDining))
            {
                _beekRecipeImage.sprite = _beekRecipeUnselectedSprite;
                _fineDiningRecipeImage.sprite = _fineDiningRecipeSelectedSprite;
                _masterChefRecipeImage.sprite = _masterChefRecipeUnselectedSprite;
                _maxCount = _coin.Items[_coin.fineDining];
            }
            else if (recipe.Equals(_coin.masterChef))
            {
                _beekRecipeImage.sprite = _beekRecipeUnselectedSprite;
                _fineDiningRecipeImage.sprite = _fineDiningRecipeUnselectedSprite;
                _masterChefRecipeImage.sprite = _masterChefRecipeSelectedSprite;
                _maxCount = _coin.Items[_coin.masterChef];
            }
            SetSliderMinMaxValue();
        }

        private void LevelUpButtonClicked()
        {
            _coin.SubtractRecipeItem(_selectedRecipe, _selectedRecipeValue);
            UpdateCoin();
            SetSliderMinMaxValue();
            OnRecipeButtonClicked?.Invoke(_selectedRecipe, _selectedRecipeValue);
        }

        private void BackButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.CharInfoStatsUI);
            OnUICloseRequested?.Invoke(UIName.LevelUpUI);
        }

        private void SliderValueChanged(float value)
        {
            _selectedRecipeValue = (int)value;
            _selectedBookCountText.text = $"{_selectedRecipeValue}/{_maxCount}";

            if (_selectedRecipeValue == 0) _levelUpButton.interactable = false;
            else _levelUpButton.interactable = true;
        }

        private void SetSliderMinMaxValue()
        {
            if (_maxCount > 0)
            {
                _gaugeSlider.minValue = 0;
                _gaugeSlider.value = 1;
                _gaugeSlider.maxValue = _maxCount;
                _selectedRecipeValue = 1;
                _selectedBookCountText.text = $"{_selectedRecipeValue}/{_maxCount}";
                _gaugeSlider.gameObject.SetActive(true);
                _levelUpButton.gameObject.SetActive(true);
            }
            else
            {
                _gaugeSlider.gameObject.SetActive(false);
                _levelUpButton.gameObject.SetActive(false);
            }
        }
    }
}