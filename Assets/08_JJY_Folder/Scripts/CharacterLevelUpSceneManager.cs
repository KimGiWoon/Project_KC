using System.Collections;
using System.Collections.Generic;
using JJY;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    public class CharacterLevelUpSceneManager : MonoBehaviour
    {
        [Header("User Info")]
        [SerializeField] TextMeshProUGUI userLevelText; // 현재 레벨
        private int curLevel;                           // TODO : 유저의 레벨
        [SerializeField] TextMeshProUGUI curExpText;    // 현재 경험치 수치 (현재 경험치 / 최대 경험치)
        private int curExp;                             // TODO : 유저의 현재 경험치
        private int maxExp;                             // TODO : 유저의 현재 레벨의 최대 경험치
        [SerializeField] TextMeshProUGUI addedExpText;  // 아이템을 사용해서 얻는 경험치 수치
        [SerializeField] Image expBar;                  // 현재 경험치 바

        [Header("Item Count Text")]
        private string selectedItem;                            // 현재 선택된 아이템의 이름
        private int selectedItemUseCount;                       // 현재 선택된 아이템의 사용하려는 개수
        private int selectedItemMaxCount;                       // 현재 선택된 아이템의 최대 보유량
        private int beeksCount;                                 // TODO : 유저의 현재 beek's 보유량
        private int fineDinigCount;                             // TODO : 유저의 현재 fine Dining 보유량
        private int masterChefCount;                            // TODO : 유저의 현재 master Chef 보유량
        [SerializeField] TextMeshProUGUI beeksCountText;        // beek's의 수량 텍스트
        [SerializeField] TextMeshProUGUI finediningCountText;   // fine dining의 수량 텍스트
        [SerializeField] TextMeshProUGUI masterChefCountText;   // masterChef의 수량 텍스트
        [SerializeField] TextMeshProUGUI useItemCountText;      // 선택된 아이템의 사용량 (1 / 현재 보유량)
        [SerializeField] Image itemBar;                         // 아이템 사용 슬라이더 게이지
        [SerializeField] Slider itemBarSlider;                  // 아이템 사용 슬라이더
        [SerializeField] GameObject useItemPanel;               // 아이템 버튼 클릭 시, SetActive true가 될 Panel.

        [Header("Dialog Text")]
        [SerializeField] TextMeshProUGUI dialog;    // dialog 표시

        [Header("Button")]
        [SerializeField] Button backBtn;            // 돌아가기 버튼
        [SerializeField] Button useItemBtn;         // 아이템 사용 버튼
        [SerializeField] Button beekBtn;            // beek 버튼
        [SerializeField] Button fineDinigBtn;       // fineDining 버튼
        [SerializeField] Button masterChefBtn;      // masterChef 버튼


        void Start()
        {
            CoinManager.Instance.OnItemsChanged += InitItemCountText;
            InitItemCountText();
            InitUserInfo();
            InitButtonFunctions();
        }

        /// <summary>
        /// 현재 보유중인 아이템의 수량을 표기하기 위한 초기화 작업
        /// </summary>
        private void InitItemCountText()
        {
            beeksCount = CoinManager.Instance.GetRecipeItemCount(CoinManager.Instance.beek);
            fineDinigCount = CoinManager.Instance.GetRecipeItemCount(CoinManager.Instance.fineDining);
            masterChefCount = CoinManager.Instance.GetRecipeItemCount(CoinManager.Instance.masterChef);

            beeksCountText.text = $"{beeksCount}";
            finediningCountText.text = $"{fineDinigCount}";
            masterChefCountText.text = $"{masterChefCount}";
        }

        /// <summary>
        /// 유저 정보 초기화 작업
        /// </summary>
        private void InitUserInfo()
        {
#if UNITY_EDITOR
            curLevel = 1;
            curExp = 0;
            maxExp = 1000;
            userLevelText.text = $"{curLevel}";
            curExpText.text = $"{curExp}/{maxExp}";
            expBar.fillAmount = maxExp > 0 ? curExp / maxExp : 0f;
#endif
        }

        private void InitButtonFunctions()
        {
            itemBarSlider.onValueChanged.AddListener(ItemSlideUpdate);
            useItemBtn.onClick.AddListener(UseItem);
            beekBtn.onClick.AddListener(OnClickBeeks);
            fineDinigBtn.onClick.AddListener(OnClickFineDining);
            masterChefBtn.onClick.AddListener(OnClickMasterChef);
        }
        private void OnClickBeeks()
        {
            selectedItem = CoinManager.Instance.beek;
            selectedItemMaxCount = beeksCount;
            InitItemBar();
        }
        private void OnClickFineDining()
        {
            selectedItem = CoinManager.Instance.fineDining;
            selectedItemMaxCount = fineDinigCount;
            InitItemBar();
        }
        private void OnClickMasterChef()
        {
            selectedItem = CoinManager.Instance.masterChef;
            selectedItemMaxCount = masterChefCount;
            InitItemBar();
        }
        private void InitItemBar()
        {
            if (selectedItemMaxCount <= 0)
            {
                if (useItemPanel.activeSelf) useItemPanel.SetActive(false);
                return;
            }
            if (!useItemPanel.activeSelf) useItemPanel.SetActive(true);

            itemBarSlider.minValue = 1f / selectedItemMaxCount;
            itemBarSlider.maxValue = 1f;
            itemBarSlider.value = 1f / selectedItemMaxCount;

            selectedItemUseCount = 1;
            itemBar.fillAmount = itemBarSlider.value;
            useItemCountText.text = $"{selectedItemUseCount} / {selectedItemMaxCount}";
            InitItemCountText();
        }
        private void ItemSlideUpdate(float value)
        {
            if (selectedItemMaxCount <= 0) return;

            selectedItemUseCount = Mathf.Clamp(
                Mathf.RoundToInt(value * selectedItemMaxCount),
                1,
                selectedItemMaxCount
            );

            itemBar.fillAmount = (float)selectedItemUseCount / selectedItemMaxCount;
            useItemCountText.text = $"{selectedItemUseCount} / {selectedItemMaxCount}";
        }

        private void UseItem()
        {
            if (selectedItem == null || selectedItemUseCount <= 0) return;

            CoinManager.Instance.SubtractRecipeItem(selectedItem, selectedItemUseCount);

            int gainedExp;
            if (selectedItem == CoinManager.Instance.beek) gainedExp = 300 * selectedItemUseCount;
            else if (selectedItem == CoinManager.Instance.fineDining) gainedExp = 500 * selectedItemUseCount;
            else if (selectedItem == CoinManager.Instance.masterChef) gainedExp = 1000 * selectedItemUseCount;
            else gainedExp = 0;

            if (gainedExp <= 0) return;
            StartCoroutine(AddExpRoutine(gainedExp));

            useItemPanel.SetActive(false);
        }
        /// <summary>
        /// 경험치 증가 + 레벨업 처리
        /// </summary>
        private IEnumerator AddExpRoutine(int gainedExp)
        {
            while (gainedExp > 0)
            {
                curExp++;
                gainedExp--;

                expBar.fillAmount = (float)curExp / maxExp;
                curExpText.text = $"{curExp}/{maxExp}";

                if (curExp >= maxExp)
                {
                    curExp -= maxExp;
                    curLevel++;
                    maxExp = GetMaxExpByLevel(curLevel);
                    userLevelText.text = $"{curLevel}";
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        /// <summary>
        /// 테스트 레벨 경험치
        /// </summary>
        private int GetMaxExpByLevel(int level)
        {
            return 1000 + (level - 1) * 200;
        }
    }
}
