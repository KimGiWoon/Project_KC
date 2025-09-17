using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class SignInUI : BaseUI
    {
        // private Button _signInButton;

        [Header("Button Sprites")]
        // [SerializeField] private Sprite _signUpSprite;
        // [SerializeField] private Sprite _signInSprite;
        // [SerializeField] private Sprite _continueWithGoogleSprite;
        [SerializeField] private Button _signUpButton;
        [SerializeField] private Button _signInButton;
        [SerializeField] private Button _continueWithGoogleButton;
        private Image _signInImage;

        public Action OnSignInButtonClicked;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            // _signInButton = _panelContainer.GetComponentInChildren<Button>(true);
            _signInImage = _signInButton.GetComponent<Image>();
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            _signUpButton.onClick.AddListener(SignInButtonClicked);
            _signInButton.onClick.AddListener(SignInButtonClicked);
            _continueWithGoogleButton.onClick.AddListener(SignInButtonClicked);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _signUpButton.onClick.RemoveListener(SignInButtonClicked);
            _signInButton.onClick.RemoveListener(SignInButtonClicked);
            _continueWithGoogleButton.onClick.RemoveListener(SignInButtonClicked);
        }

        public override void Open()
        {
            base.Open();
            SetButtonImage(GameManager.Instance.Firebase.ButtonType);
        }

        /// <summary>
        /// SignIn 버튼 클릭 이벤트 핸들러 호출 메서드
        /// </summary>
        private void SignInButtonClicked()
        {
            OnSignInButtonClicked?.Invoke();
            _signUpButton.interactable = false;
            _signInButton.interactable = false;
            _continueWithGoogleButton.interactable = false;
        }

        /// <summary>
        /// 지정된 버튼 타입에 따라 버튼 이미지를 설정
        /// </summary>
        /// <param name="buttonType">설정할 버튼의 타입</param>
        public void SetButtonImage(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.SignUpButton:
                    // _signInImage.sprite = _signUpSprite;
                    _signUpButton.gameObject.SetActive(true);
                    _signInButton.gameObject.SetActive(false);
                    _continueWithGoogleButton.gameObject.SetActive(false);
                    break;
                case ButtonType.SignInButton:
                    // _signInImage.sprite = _signInSprite;
                    _signInButton.gameObject.SetActive(true);
                    _signUpButton.gameObject.SetActive(false);
                    _continueWithGoogleButton.gameObject.SetActive(false);
                    break;
                case ButtonType.ContinueButton:
                    // _signInImage.sprite = _continueWithGoogleSprite;
                    _continueWithGoogleButton.gameObject.SetActive(true);
                    _signUpButton.gameObject.SetActive(false);
                    _signInButton.gameObject.SetActive(false);
                    break;
            }
            Canvas.ForceUpdateCanvases();
        }
    }
}