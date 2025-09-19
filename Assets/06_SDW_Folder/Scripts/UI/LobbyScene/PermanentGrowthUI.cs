using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using KSH;

namespace SDW
{
    public class PermanentGrowthUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _pointText;
        [SerializeField] private Button _initializeButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private GameObject _contents;
        private List<GrowthNodeUI> _growthNodes = new List<GrowthNodeUI>();
        private List<Button> _growthNodeButtons = new List<Button>();

        public Action<UIName, GrowthNodeUI> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        private void OnEnable()
        {
            _initializeButton.onClick.AddListener(InitializeButtonClicked);
            _backButton.onClick.AddListener(BackButtonClicked);
            GameManager.Instance.Coin.OnPointChanged += SetTotalPoint;
        }

        private void OnDisable()
        {
            _initializeButton.onClick.RemoveListener(InitializeButtonClicked);
            _backButton.onClick.RemoveListener(BackButtonClicked);
            GameManager.Instance.Coin.OnPointChanged -= SetTotalPoint;

            for (int i = 0; i < _growthNodes.Count; i++)
            {
                _growthNodeButtons[i].onClick.RemoveAllListeners();
            }
            _growthNodeButtons.Clear();
            _growthNodes.Clear();
        }

        public override void Open()
        {
            SetTotalPoint();
            base.Open();
        }

        private void SetTotalPoint()
        {
            _pointText.text = "x" + GameManager.Instance.Coin.point;
        }

        public void AddNode(GrowthNodeUI growthNode)
        {
            _growthNodes.Add(growthNode);
            var button = growthNode.GetComponent<Button>();
            _growthNodeButtons.Add(button);

            button.onClick.AddListener(() =>
                OpenNodeDescription(growthNode)
            );
        }

        private void InitializeButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.NodeInitializeUI, null);
        }

        private void BackButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.MainLobbyUI, null);
            OnUICloseRequested?.Invoke(UIName.PermanentGrowthUI);
        }

        private void OpenNodeDescription(GrowthNodeUI growthNode)
        {
            OnUIOpenRequested?.Invoke(UIName.NodeDescriptionUI, growthNode);
        }
    }
}