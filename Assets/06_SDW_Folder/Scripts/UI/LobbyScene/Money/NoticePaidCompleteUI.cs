using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class NoticePaidCompleteUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _currencyValueText;
        [SerializeField] private Button _okButton;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        private void OnEnable()
        {
            _okButton.onClick.AddListener(OkButtonClicked);
        }

        private void OnDisable()
        {
            _okButton.onClick.AddListener(OkButtonClicked);
        }

        private void OkButtonClicked()
        {
            OnUICloseRequested?.Invoke(UIName.NoticePaidCompleteUI);
        }

        public void SetItemInfo(int sugarStar, int price)
        {
            _currencyValueText.text = $"{sugarStar}개";
        }
    }
}