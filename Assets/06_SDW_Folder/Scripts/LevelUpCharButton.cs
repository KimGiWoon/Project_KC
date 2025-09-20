using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class LevelUpCharButton : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image _chaIconImage;
        [SerializeField] private GameObject _glowImageObj; //# 레어면 활성화
        [SerializeField] private GameObject _darkImageObj; //# 없으면 dark?
        [SerializeField] private TextMeshProUGUI _chaUpgradeText;
        [SerializeField] private GameObject _circleImageObj; //# 선택 시 활성화?

        [HideInInspector] public bool IsSelected;
        [HideInInspector] public CharacterEnName ChaEnName;

        private CharacterDataManager _characterDataManager;

        private void Awake()
        {
            _glowImageObj.SetActive(false);
            _darkImageObj.SetActive(false);
            _circleImageObj.SetActive(false);
            _chaUpgradeText.text = "";
        }

        public void SetLevelUpChar(CharacterDataSO data)
        {
            _chaIconImage.sprite = data._characterCircleSprite;

            ChaEnName = data._chaBaseData.ChaEnName;
            bool isRare = data._chaBaseData.ChaGrade == CharacterGrade.Rare ? true : false;
            _glowImageObj.SetActive(isRare);

            bool isOwned = _characterDataManager.OwnedCharacters.ContainsKey(ChaEnName);
            _darkImageObj.SetActive(!isOwned);

            _chaUpgradeText.text = "+" + _characterDataManager.BeadsInventory[ChaEnName];

            if (IsSelected) _circleImageObj.SetActive(true);
            else _circleImageObj.SetActive(false);
        }

        public void SetSelected(bool value)
        {
            IsSelected = value;
            if (IsSelected) _circleImageObj.SetActive(true);
            else _circleImageObj.SetActive(false);
        }
    }
}