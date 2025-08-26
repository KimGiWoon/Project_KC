using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class DeleteAccountUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _acceptButton;

        public Action<UIName> OnCloseButtonClicked;
        public Action OnDeleteAcceptButtonClicked;

        private void Awake()
        {
            _panelContainer.SetActive(false);

            _cancelButton.onClick.AddListener(DeleteCancelButtonClicked);
            _acceptButton.onClick.AddListener(DeleteAcceptButtonClicked);
        }

        private void DeleteCancelButtonClicked() => OnCloseButtonClicked?.Invoke(UIName.DeleteAccountUI);

        private void DeleteAcceptButtonClicked()
        {
            OnDeleteAcceptButtonClicked?.Invoke();
            OnCloseButtonClicked?.Invoke(UIName.DeleteAccountUI);
        }
    }
}