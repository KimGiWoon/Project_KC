using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SDW
{
    public class NodeInitializeUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _initializeButton;
        [SerializeField] private Button _cancelButton;

        private string _originalText;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _originalText = _descriptionText.text;
        }

        private void OnEnable()
        {
            _initializeButton.onClick.AddListener(InitializeButtonClicked);
            _cancelButton.onClick.AddListener(CancelButtonClicked);
        }

        private void OnDisable()
        {
            _initializeButton.onClick.RemoveListener(InitializeButtonClicked);
            _cancelButton.onClick.RemoveListener(CancelButtonClicked);
        }

        public override void Close()
        {
            _descriptionText.text = _originalText;
            base.Close();
        }

        public void SetDescriptionText(int pointValue)
        {
            _descriptionText.text = _descriptionText.text.Replace(
                "n", pointValue.ToString()
            );
        }

        private void InitializeButtonClicked()
        {
            //todo 초기화로 변경해야 함
            OnUICloseRequested?.Invoke(UIName.NodeInitializeUI);
        }

        private void CancelButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.NodeInitializeUI);
        }
    }
}