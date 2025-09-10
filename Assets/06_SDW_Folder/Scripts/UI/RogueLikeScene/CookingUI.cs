using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJY;
using UnityEngine.UI;

namespace SDW
{
    public class CookingUI : BaseUI
    {
        [Header("Recipe Ingredients")]
        [SerializeField] private List<Image> _recipeImages = new List<Image>();
        [SerializeField] private Image _foodImage;

        [Header("Buttons")]
        [SerializeField] private Button _foodInfoButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _cookButton;

        [Header("ETC")]
        [SerializeField] private GameObject _contents;
        [SerializeField] private RectTransform _cookPanelRect;
        [SerializeField] private RectTransform[] _buttonsRect;

        private CookManager _cookManager;
        private TweenAnimation _tweenAnimation;
        private WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
        private bool _canInteract;

        public Action<UIName, PopupDescription> OnUIOpenRequested;
        public Action<UIName, bool> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _tweenAnimation = GetComponent<TweenAnimation>();
        }

        protected override void Start()
        {
            base.Start();
            _cookManager = CookManager.Instance;
        }

        private void OnEnable()
        {
            _foodInfoButton.onClick.AddListener(FoodInfoButtonClicked);
            _resetButton.onClick.AddListener(ResetButtonClicked);
            _cookButton.onClick.AddListener(CookButtonClicked);
        }

        private void OnDisable()
        {
            _foodInfoButton.onClick.RemoveListener(FoodInfoButtonClicked);
            _resetButton.onClick.RemoveListener(ResetButtonClicked);
            _cookButton.onClick.RemoveListener(CookButtonClicked);
        }

        public override void Open()
        {
            StartCoroutine(InteractDelay());
            base.Open();
            _tweenAnimation.moveAway();
            _cookManager.RefreshInventoryUI();
            _cookManager.UpdateResultButton(); // result 버튼 활성화 여부 반영
        }

        public override void Close()
        {
            _tweenAnimation.moveBack();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator InteractDelay()
        {
            yield return _waitForSeconds;
            _canInteract = true;
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_tweenAnimation.tweenTime);
            _cookManager.ResetIngredients();
            base.Close();
        }

        private void Update()
        {
            if (!_panelContainer.activeSelf || !_canInteract) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (RectTransformUtility.RectangleContainsScreenPoint(_cookPanelRect, touchPos)) return;

                //# 버튼을 클릭했는지 확인
                foreach (var buttonRect in _buttonsRect)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, touchPos))
                    {
                        if (buttonRect.CompareTag("InventoryButton")) return;
                        StartCoroutine(InteractDelay());
                        return;
                    }
                }

                StartCoroutine(DelayedCloseCall());
            }
        }

        private IEnumerator DelayedCloseCall(bool uiOnly = false)
        {
            _canInteract = false;
            StartCoroutine(InteractDelay());
            yield return new WaitForSeconds(0.1f);

            OnUICloseRequested?.Invoke(UIName.CookingUI, uiOnly);
        }

        public void SetContentsInit(List<GameObject> ingredientList)
        {
            if (!_panelContainer.activeSelf) return;


            foreach (var ingredient in ingredientList)
            {
                ingredient.transform.SetParent(_contents.transform);
                ingredient.SetActive(true);
            }
        }

        public void SetFoodInfoButton(Sprite sprite, bool isActive)
        {
            _foodImage.gameObject.SetActive(isActive);
            // _foodInfoButton.interactable = isActive;

            if (!isActive) return;

            _foodImage.sprite = sprite;
        }

        public void InitRecipeSlots()
        {
            foreach (var recipeImage in _recipeImages)
            {
                recipeImage.gameObject.SetActive(false);
                var img = recipeImage.GetComponent<Image>();
                img.sprite = null;
            }
        }

        public void SetRecipeSlots(List<Sprite> sprites)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                _recipeImages[i].gameObject.SetActive(true);
                _recipeImages[i].sprite = sprites[i];
            }
        }

        public int GetRecipeSlotsCout() => _recipeImages.Count;

        #region Button Methods

        private void FoodInfoButtonClicked()
        {
            var description = _cookManager.InitDescription();
            OnUIOpenRequested?.Invoke(UIName.FoodDescriptionUI, description);
        }

        private void ResetButtonClicked() => _cookManager.ResetIngredients();

        private void CookButtonClicked() => _cookManager.SuccessCook();

        #endregion
    }
}