using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class SignInUI : BaseUI
    {
        private Button _signInButton;

        [Header("Button Sprites")]
        [SerializeField] private Sprite _signUpSprite;
        [SerializeField] private Sprite _signInSprite;
        [SerializeField] private Sprite _continueWithGoogleSprite;
        private Image _signInImage;

        public Action OnSignInButtonClicked;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _signInButton = _panelContainer.GetComponentInChildren<Button>(true);
            _signInImage = _signInButton.GetComponent<Image>();
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
            _signInButton.onClick.AddListener(SignInButtonClicked);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _signInButton.onClick.RemoveListener(SignInButtonClicked);
        }

        /// <summary>
        /// SignIn 버튼 클릭 이벤트 핸들러 호출 메서드
        /// </summary>
        private void SignInButtonClicked()
        {
            OnSignInButtonClicked?.Invoke();
            _signInButton.interactable = false;
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
                    _signInImage.sprite = _signUpSprite;
                    break;
                case ButtonType.SignInButton:
                    _signInImage.sprite = _signInSprite;
                    break;
                case ButtonType.ContinueButton:
                    _signInImage.sprite = _continueWithGoogleSprite;
                    break;
            }
        }
    }
}