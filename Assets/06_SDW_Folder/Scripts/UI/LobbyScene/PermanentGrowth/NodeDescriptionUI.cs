using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class NodeDescriptionUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private RectTransform _panelRect;
        [SerializeField] private TextMeshProUGUI _nodeTypeText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _pointText;
        [SerializeField] private Button _activateButton;
        [SerializeField] private TextMeshProUGUI _activateInfoText;
        [SerializeField] private GameObject _backgroundPanelObject;

        [SerializeField] private string _inactivate = "선행 흔적 활성화 필요";
        [SerializeField] private string _activateComplete = "활성 완료";

        [SerializeField] private GameObject _warningPopup;
        [SerializeField] private GrowthManager _growthManager;

        private int _needPoint;
        private int _nodeId;

        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _warningPopup.SetActive(false);
            _backgroundPanelObject.SetActive(false);
        }

        private void OnEnable()
        {
            _activateButton.onClick.AddListener(ActivateButtonClicked);
        }

        private void OnDisable()
        {
            _activateButton.onClick.RemoveListener(ActivateButtonClicked);
        }

        public override void Open()
        {
            _backgroundPanelObject.SetActive(true);
            base.Open();
        }

        public override void Close()
        {
            _backgroundPanelObject.SetActive(false);
            base.Close();
        }

        //# 안드로이드 터치 감지
        private void Update()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var touchPos = Input.GetTouch(0).position;

                //# 패널 안에 터치가 있는지 확인
                if (!RectTransformUtility.RectangleContainsScreenPoint(_panelRect, touchPos))
                {
                    OnUICloseRequested?.Invoke(UIName.NodeDescriptionUI);
                }
            }
        }

        public void SetDescription(GrowthNodeUI growthNode)
        {
            if (growthNode.Unlocked && growthNode.CanActivate)
            {
                //# 활성 가능 상태
                _activateInfoText.gameObject.SetActive(false);
                _activateButton.gameObject.SetActive(true);
            }
            else if (growthNode.Unlocked && !growthNode.CanActivate)
            {
                //# 활성 상태
                _activateButton.gameObject.SetActive(false);
                _activateInfoText.text = _activateComplete;
                _activateInfoText.gameObject.SetActive(true);
            }
            else
            {
                //# 해금 X, Activate X
                //# 활성 불가능 상태

                _activateButton.gameObject.SetActive(false);
                _activateInfoText.text = _inactivate;
                _activateInfoText.gameObject.SetActive(true);
            }

            _descriptionText.text = growthNode.GetDescription();
            _pointText.text = growthNode.GetCurrency().ToString();
            _needPoint = growthNode.GetCurrency();
            _nodeId = growthNode.GetNodeId();

            switch (growthNode.Grade)
            {
                case NodeGrade.Contents:
                    _nodeTypeText.text = "특수";
                    break;
                case NodeGrade.Main:
                    _nodeTypeText.text = "핵심";
                    break;
                case NodeGrade.Sub:
                    _nodeTypeText.text = "일반";
                    break;
            }
        }

        private void ActivateButtonClicked()
        {
            if (_needPoint > GameManager.Instance.Coin.point)
                _warningPopup.SetActive(true);
            else
            {
                GameManager.Instance.Coin.SubtractPoint(_needPoint);
                GameManager.Instance.AddGrowthCompleteNode(_nodeId);
                _growthManager.UpdateAllNode();

                GrowthDatas growthDatas;
                if (_growthManager.GrowthDataDic.TryGetValue(_nodeId, out growthDatas))
                    _growthManager.AllApplyGrowth(growthDatas); //모두 적용
                
                OnUICloseRequested?.Invoke(UIName.NodeDescriptionUI);
            }
        }
    }
}