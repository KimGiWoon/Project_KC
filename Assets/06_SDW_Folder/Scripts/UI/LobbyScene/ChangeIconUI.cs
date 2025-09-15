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
        private Dictionary<int, Sprite> _spriteIndex = new Dictionary<int, Sprite>();
        private ScrollRect _scrollRect;
        private int _selectedIconIndex;

        public Action<UIName> OnUICloseRequested;
        public Action OnApplyIconClicked;
        public Action<Sprite> OnIconSelected;
        public Action<int> OnIconSelectedIndex;

        /// <summary>
        /// UI 요소가 활성화 준비를 마치고 초기화 작업을 수행하는 메서드
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);

            _scrollRect = GetComponentInChildren<ScrollRect>(true);

            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 1f;

            _iconChangeButtons = _contents.GetComponentsInChildren<Button>(true).ToList();

            foreach (var icon in _iconChangeButtons)
            {
                var buttonId = icon.GetComponent<ButtonId>();
                var sprite = icon.GetComponent<Image>().sprite;
                _spriteIndex[buttonId.Id] = sprite;
            }
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 초기 설정 및 이벤트 연결 수행
        /// </summary>
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
                    var buttonId = icon.GetComponent<ButtonId>();
                    IconSelected(sprite, buttonId.Id);
                });
            }
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거 및 리소스 해제를 수행
        /// </summary>
        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);

            foreach (var icon in _iconChangeButtons)
            {
                icon.onClick.RemoveListener(() =>
                {
                    var sprite = icon.GetComponent<Image>().sprite;
                    var buttonId = icon.GetComponent<ButtonId>();
                    IconSelected(sprite, buttonId.Id);
                });
            }
        }

        /// <summary>
        /// IconChangeButtonClicked 핸들러 메서드 호출로 사용자가 선택한 Icon의 Sprite를 전달
        /// </summary>
        /// <param name="sprite">전달할 sprite</param>
        private void IconSelected(Sprite sprite, int buttonId)
        {
            OnIconSelected?.Invoke(sprite);
            _selectedIconIndex = buttonId;
        }

        /// <summary>
        /// ConfirmButtonClicked 핸들러 메서드 호출로 사용자가 확인 버튼을 클릭했을 때 현재 선택된 Icon으로 Apply
        /// </summary>
        private void ConfirmButtonClicked()
        {
            OnApplyIconClicked?.Invoke();
            OnIconSelectedIndex?.Invoke(_selectedIconIndex);
            OnUICloseRequested?.Invoke(UIName.ChangeIconUI);
        }

        public Sprite GetIcon(int iconNumber) => _spriteIndex[iconNumber];
    }
}