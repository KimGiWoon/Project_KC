using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class SignInUI : BaseUI
    {
        [SerializeField] private GameObject _signInPanel;
        [SerializeField] private Button _signInButton;

        [Header("Button Sprites")]
        [SerializeField] private Sprite _signUpSprite;
        [SerializeField] private Sprite _continueWithGoogleSprite;
        private Image _signInImage;

        public Action OnSignInButtonClicked;

        /// <summary>
        /// UI 요소 연결 및 이벤트 리스너 설정을 수행
        /// </summary>
        private void Awake()
        {
            _signInPanel.SetActive(false);
            _signInImage = _signInButton.GetComponent<Image>();
            _signInButton.onClick.AddListener(SignInButtonClicked);
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 초기 설정 및 이벤트 연결을 수행
        /// </summary>
        private void OnEnable() => GameManager.Instance.UI.AddPanel(this);

        /// <summary>
        /// UI 요소 해제 시 필요한 리소스 해제 및 참조 제거 수행
        /// </summary>
        private void OnDisable() => GameManager.Instance.UI.RemovePanel(this);

        /// <summary>
        /// SignIn 버튼 클릭 이벤트 핸들러 호출 메서드
        /// </summary>
        private void SignInButtonClicked() => OnSignInButtonClicked?.Invoke();

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
                case ButtonType.ContinueButton:
                    _signInImage.sprite = _continueWithGoogleSprite;
                    break;
            }

            _signInPanel.SetActive(true);
        }
    }
}