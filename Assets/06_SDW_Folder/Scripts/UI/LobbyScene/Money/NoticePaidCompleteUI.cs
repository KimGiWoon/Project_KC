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
        [SerializeField] private TweenAlpha_Image _backgroundPanel;
        private bool _isProgress;

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

        public override void Open()
        {
            _okButton.interactable = true;
            _isProgress = false;
            _backgroundPanel.gameObject.SetActive(true);
            base.Open();
        }

        public override void Close()
        {
            _backgroundPanel.FadeOut();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_backgroundPanel.TweenTime);
            base.Close();
        }

        private void OkButtonClicked()
        {
            _okButton.interactable = false;
            _isProgress = true;
            OnUICloseRequested?.Invoke(UIName.NoticePaidCompleteUI);
        }

        public void SetItemInfo(int sugarStar, int price)
        {
            _currencyValueText.text = $"{sugarStar}개";
        }
    }
}