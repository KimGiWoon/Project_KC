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

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _userInfoButton.onClick.AddListener(UserInfoButtonClicked);
        }

        private void UserInfoButtonClicked()
        {
            OnButtonClicked?.Invoke(UIName.UserInfoUI);
        }
    }
}