using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class SetNicknameUI : BaseUI
    {
        private TMP_InputField _nicknameInputField;
        private TextMeshProUGUI _nicknameErrorText;
        private Button _nicknameButton;

        public Action<string> OnNicknameChange;

        /// <summary>
        /// 컴포넌트 할당 및 이벤트 리스너 설정
        /// UI 요소들을 찾아 할당하고, 닉네임 적용 버튼 클릭 이벤트를 연결
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _nicknameInputField = _panelContainer.GetComponentInChildren<TMP_InputField>(true);
            _nicknameErrorText = _panelContainer.GetComponentInChildren<TextMeshProUGUI>(true);
            _nicknameButton = _panelContainer.GetComponentInChildren<Button>(true);
            _nicknameButton.onClick.AddListener(ApplyButtonClicked);
        }

        /// <summary>
        /// 호출된 시점에 사용자가 입력한 닉네임을 검증하고 처리하는 메서드
        /// 빈 닉네임 입력 시에는 경고 메시지를 표시하며, 유효한 닉네임이 입력되면 관련 콜백을 실행
        /// </summary>
        private void ApplyButtonClicked()
        {
            if (string.IsNullOrEmpty(_nicknameInputField.text))
            {
                _nicknameErrorText.text = "닉네임을 입력해주세요.";
                return;
            }

            _nicknameErrorText.text = "";
            OnNicknameChange?.Invoke(_nicknameInputField.text);
        }
    }
}