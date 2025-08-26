using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class ChangeIconUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _confirmButton;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);

            _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        }

        private void ConfirmButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.ChangeIconUI);
        }
    }
}