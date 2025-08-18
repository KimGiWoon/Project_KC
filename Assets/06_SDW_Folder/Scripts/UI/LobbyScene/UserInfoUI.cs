using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class UserInfoUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _signOutButton;
        [SerializeField] private Button _deleteButton;

        public Action OnSignOutButtonClicked;
        public Action OnDeleteButtonClicked;

        /// <summary>
        /// UI 컴포넌트 활성화 설정 및 이벤트 리스너 할당을 수행
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
            _signOutButton.onClick.AddListener(SignOutButtonClicked);
            _deleteButton.onClick.AddListener(DeleteButtonClicked);
        }

        /// <summary>
        /// 호출된 경우 사용자 정보 UI에서로그아웃 기능을 실행하는 이벤트 핸들러 메소드
        /// </summary>
        private void SignOutButtonClicked()
        {
            OnSignOutButtonClicked?.Invoke();
        }

        /// <summary>
        /// Delete 버튼 클릭 이벤트 핸들러
        /// </summary>
        private void DeleteButtonClicked()
        {
            OnDeleteButtonClicked?.Invoke();
        }
    }
}