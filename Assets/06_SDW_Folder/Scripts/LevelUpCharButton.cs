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
        [SerializeField] private Image _glowImage; //# 레어면 활성화 // 이거 뭐임 몰라
        // [SerializeField] private GameObject _darkImageObj; //# 없으면 dark?
        [SerializeField] private TextMeshProUGUI _chaUpgradeText;
        [SerializeField] private Image _circleImage; //# 선택 시 활성화? 이것이 레어나 노말일 때 색 바뀜.
        [SerializeField] private Color normalColor;
        [SerializeField] private Color rareColor;

        [HideInInspector] public bool IsSelected;
        [HideInInspector] public CharacterEnName ChaEnName;

        // private void Awake()
        // {
        //     _glowImage.gameObject.SetActive(false);
        //     // _darkImageObj.SetActive(false);
        //     _circleImage.gameObject.SetActive(false);
        //     _chaUpgradeText.text = "";
        // }

        public void SetLevelUpChar(CharacterDataSO data)
        {
            _chaIconImage.sprite = data._characterCircleSprite;
            _glowImage.gameObject.SetActive(true);
            _circleImage.gameObject.SetActive(true);

            ChaEnName = data._chaBaseData.ChaEnName;
            bool isRare = data._chaBaseData.ChaGrade == CharacterGrade.Rare ? true : false;
            // _glowImageObj.SetActive(isRare);
            if (isRare)
            {
                _circleImage.color = rareColor;
            }
            else
            {
                _circleImage.color = normalColor;
            }

            bool isOwned = GameManager.Instance.CharacterData.OwnedCharacters.ContainsKey(ChaEnName);
            // _darkImageObj.SetActive(!isOwned);
            Debug.Log($"{ChaEnName} : {isOwned}");
            if (!isOwned)
            {
                _chaUpgradeText.text = "";
                return;
            }

            Debug.Log($"{ChaEnName} : {GameManager.Instance.CharacterData.BeadsInventory[ChaEnName]}");
            _chaUpgradeText.text = GameManager.Instance.CharacterData.BeadsInventory[ChaEnName] > 0 ? "+" + GameManager.Instance.CharacterData.BeadsInventory[ChaEnName] : "";

            // _chaUpgradeText.text = data.Beads > 0 ? "+" + data.Beads : "";

            // if (IsSelected) _circleImageObj.SetActive(true);
            // else _circleImageObj.SetActive(false);
        }

        // public void SetSelected(bool value)
        // {
        //     IsSelected = value;
        //     if (IsSelected) _circleImageObj.SetActive(true);
        //     else _circleImageObj.SetActive(false);
        // }
    }
}