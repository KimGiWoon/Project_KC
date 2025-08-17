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

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _signOutButton.onClick.AddListener(SignOutButtonClicked);
            _deleteButton.onClick.AddListener(DeleteButtonClicked);
        }

        private void SignOutButtonClicked()
        {
            OnSignOutButtonClicked?.Invoke();
        }

        private void DeleteButtonClicked()
        {
            OnDeleteButtonClicked?.Invoke();
        }
    }
}