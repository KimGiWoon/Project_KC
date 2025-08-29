using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class ChangeIconUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private GameObject _contents;
        private List<Button> _iconChangeButtons = new List<Button>();

        public Action<UIName> OnUICloseRequested;
        public Action OnApplyIconClicked;
        public Action<Sprite> OnIconSelected;

        private ScrollRect _scrollRect;

        private void Awake()
        {
            _panelContainer.SetActive(false);

            _scrollRect = GetComponentInChildren<ScrollRect>(true);

            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 1f;

            _iconChangeButtons = _contents.GetComponentsInChildren<Button>(true).ToList();
        }

        private void OnEnable()
        {
            if (_scrollRect.content != null)
            {
                var pos = _scrollRect.content.anchoredPosition;
                pos.y = 0f; // 최상단 위치
                _scrollRect.content.anchoredPosition = pos;
            }

            _confirmButton.onClick.AddListener(ConfirmButtonClicked);

            foreach (var icon in _iconChangeButtons)
            {
                icon.onClick.AddListener(() =>
                {
                    var sprite = icon.GetComponent<Image>().sprite;
                    IconSelected(sprite);
                });
            }
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);

            foreach (var icon in _iconChangeButtons)
            {
                icon.onClick.RemoveListener(() =>
                {
                    var sprite = icon.GetComponent<Image>().sprite;
                    IconSelected(sprite);
                });
            }
        }

        public void IconSelected(Sprite sprite) => OnIconSelected?.Invoke(sprite);

        public void ConfirmButtonClicked()
        {
            OnApplyIconClicked?.Invoke();
            OnUICloseRequested?.Invoke(UIName.ChangeIconUI);
        }
    }
}