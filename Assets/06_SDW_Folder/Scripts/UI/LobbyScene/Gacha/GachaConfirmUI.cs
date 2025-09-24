using System;
using KSH;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class GachaConfirmUI : BaseUI
    {
        [Header("CharacterGacha")]
        [SerializeField] private CharacterGacha gacha;

        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _acceptButton;
        private RectTransform _rectTransform;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        private bool _isSingleGacha;
        private int _sugarStar;
        private string _originalDescription;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _rectTransform = GetComponent<RectTransform>();
            _originalDescription = _descriptionText.text;
        }

        protected override void Start()
        {
            base.Start();
            gacha = GameManager.Instance.Gacha;
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(BackButtonClicked);
            _acceptButton.onClick.AddListener(AcceptButtonClicked);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(BackButtonClicked);
            _acceptButton.onClick.RemoveListener(AcceptButtonClicked);
        }

        /// <summary>
        /// UI 외부 터치 시 UI를 Close
        /// </summary>
        public void Update()
        {
            if (!_panelContainer.activeSelf) return;

            //# 안드로이드 터치 감지
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (!RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, touchPos))
                {
                    OnUICloseRequested?.Invoke(UIName.GachaConfirmUI);
                }
            }
        }

        public override void Close()
        {
            base.Close();
            _descriptionText.text = _originalDescription;
        }

        public void SetDescriptionText(int sugarStar, int count)
        {
            _isSingleGacha = count == 1;
            _sugarStar = sugarStar;

            _descriptionText.text = _descriptionText.text.Replace(
                "{sugarStar}", sugarStar.ToString()
            ).Replace(
                "{count}", count.ToString()
            );
        }

        private void BackButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.GachaConfirmUI);
        }

        private void AcceptButtonClicked()
        {
            GameManager.Instance.Reward.AddStarCandy(-_sugarStar);
            gacha.SetGachaType(_isSingleGacha);
            //todo 결과창을 띄워야 함
        }
    }
}