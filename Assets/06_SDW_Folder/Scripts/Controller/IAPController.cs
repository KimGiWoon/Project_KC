using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

namespace SDW
{
    public class IAPController : MonoBehaviour, IDetailedStoreListener
    {
        [Header("UI Components")]
        [SerializeField] private PaidStoreUI _paidStoreUI;
        [SerializeField] private NoticePaidConfirmUI _paidConfirmUI;
        [SerializeField] private List<TextMeshProUGUI> _priceTextList;

        [Header("Paid Store Components")]
        [SerializeField] private List<ProductButton> _productButtonList;
        [SerializeField] private List<int> _getStarList;
        [SerializeField] private List<double> _paidPriceList;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        private bool _purchaseProcessing = false;

        private void Awake()
        {
            foreach (var productButton in _productButtonList)
            {
                productButton.Button.interactable = false;
            }

            if (_storeController != null)
            {
                return;
            }

            // ConfigurationBuilder 생성
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // 상품 추가
            builder.AddProduct("shiningstar2000", ProductType.Consumable);
            builder.AddProduct("shiningstar4500", ProductType.Consumable);
            builder.AddProduct("shiningstar10000", ProductType.Consumable);
            builder.AddProduct("shiningstar17000", ProductType.Consumable);

            // 초기화 시작 - 여기서 this를 전달!
            UnityPurchasing.Initialize(this, builder);
        }

        private void OnEnable()
        {
            _paidConfirmUI.OnPayButtonClicked += PayedButtonClicked;
        }

        private void OnDisable()
        {
            foreach (var productButton in _productButtonList)
            {
                productButton.Button.onClick.AddListener(() => ItemButtonClicked(productButton.PublicId));
            }
            _paidConfirmUI.OnPayButtonClicked -= PayedButtonClicked;
        }

        private void ItemButtonClicked(string productId)
        {
            if (_storeController != null)
            {
                foreach (var productButton in _productButtonList)
                {
                    if (productId == productButton.PublicId)
                    {
                        int index = -1;
                        switch (productButton.PublicId)
                        {
                            case "shiningstar2000":
                                index = 0;
                                break;
                            case "shiningstar4500":
                                index = 1;
                                break;
                            case "shiningstar10000":
                                index = 2;
                                break;
                            case "shiningstar17000":
                                index = 3;
                                break;
                        }
                        if (index != -1)
                            _paidStoreUI.ItemSelected(_getStarList[index], _paidPriceList[index], productId);
                    }
                }
            }
        }

        public void PayedButtonClicked(string productId)
        {
            _purchaseProcessing = false;
            _storeController.InitiatePurchase(productId);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            _paidPriceList.Clear();

            for (int i = 0; i < _productButtonList.Count; i++)
            {
                var product = _storeController.products.WithID(_productButtonList[i].PublicId);

                string localizedPrice = product.metadata.localizedPriceString;
                _priceTextList[i].text = localizedPrice + " 냥";

                string numericString = new string(localizedPrice.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
                numericString = numericString.Replace(",", "");
                if (double.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                {
                    _paidPriceList.Add(price);
                }

                if (product != null && product.availableToPurchase)
                {
                    var buttonId = _productButtonList[i].Button.GetComponent<ButtonId>();
                    _productButtonList[i].Button.interactable = true;
                    _productButtonList[i].Button.onClick.RemoveAllListeners();
                    _productButtonList[i].Button.onClick
                        .AddListener(() => ItemButtonClicked(_productButtonList[buttonId.Id].PublicId)
                        );
                }
                else
                    _productButtonList[i].Button.interactable = false;
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            foreach (var productButton in _productButtonList)
            {
                productButton.Button.interactable = false;
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message = null)
        {
            foreach (var productButton in _productButtonList)
            {
                productButton.Button.interactable = false;
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if (_purchaseProcessing)
                return PurchaseProcessingResult.Complete;

            _purchaseProcessing = true;
            _paidConfirmUI.Success();

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _paidConfirmUI.Failed();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            _paidConfirmUI.Failed();
        }
    }
}