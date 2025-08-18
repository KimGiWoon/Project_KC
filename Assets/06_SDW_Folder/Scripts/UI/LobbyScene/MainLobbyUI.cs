using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class MainLobbyUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _userInfoButton;

        public Action<UIName> OnButtonClicked;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _userInfoButton.onClick.AddListener(UserInfoButtonClicked);
        }

        /// <summary>
        /// 사용자 정보 버튼 클릭 이벤트 핸들러 메서드 호출
        /// </summary>
        private void UserInfoButtonClicked()
        {
            OnButtonClicked?.Invoke(UIName.UserInfoUI);
        }
    }
}