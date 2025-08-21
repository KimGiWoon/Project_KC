using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    // TODO : CoinManager => GameManager 연결 시 코드 재정리.
    public class CharacterLevelUpSceneManager : MonoBehaviour
    {
        [Header("User Info")]
        [SerializeField] TextMeshProUGUI userLevelText; // 현재 레벨 수치
        private int curLevel;                           // TODO : 유저의 레벨
        [SerializeField] TextMeshProUGUI curExpText;    // 현재 경험치 수치 (현재 경험치 / 최대 경험치)
        private int curExp;                             // TODO : 유저의 현재 경험치
        private int maxExp;                             // TODO : 유저의 현재 레벨의 최대 경험치 (CSV)
        [SerializeField] TextMeshProUGUI addedExpText;  // 아이템을 사용해서 얻는 경험치 수치
        [SerializeField] TextMeshProUGUI addedLevelText;// 아이템을 사용해서 얻는 레벨 수치
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
        [SerializeField] Image dialogPanelColor;
        [SerializeField] TextMeshProUGUI dialogText;    // dialog 표시

        [Header("Button")]
        [SerializeField] Button backBtn;            // TODO : 돌아가기 버튼
        [SerializeField] Button useItemBtn;         // 아이템 사용 버튼
        [SerializeField] Button beekBtn;            // beek 버튼
        [SerializeField] Button fineDinigBtn;       // fineDining 버튼
        [SerializeField] Button masterChefBtn;      // masterChef 버튼

        private Dictionary<string, int> itemExpTable = new Dictionary<string, int>();
        private Coroutine dialogCoroutine;

        void Start()
        {
            CoinManager.Instance.OnItemsChanged += InitItemCountText;
            InitEXPTable();
            InitItemCountText();
            InitUserInfo();
            InitButtonFunctions();
        }

        // TODO : CSV 연결
        /// <summary>
        /// 아이템의 경험치 수치 초기화.
        /// </summary>
        void InitEXPTable()
        {
            itemExpTable[CoinManager.Instance.beek] = 300;
            itemExpTable[CoinManager.Instance.fineDining] = 500;
            itemExpTable[CoinManager.Instance.masterChef] = 1000;
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
                if (addedExpText.gameObject.activeSelf) addedExpText.gameObject.SetActive(false);
                if (addedLevelText.gameObject.activeSelf) addedLevelText.gameObject.SetActive(false);
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

            if (!addedExpText.gameObject.activeSelf) addedExpText.gameObject.SetActive(true);
            int gainedExp = itemExpTable[selectedItem] * selectedItemUseCount;
            addedExpText.text = $"+{gainedExp}";

            int tempExp = curExp;
            int tempLevel = curLevel;
            int tempMaxExp = maxExp;
            int levelUpCount = 0;

            int remainingExp = gainedExp;
            while (remainingExp > 0)
            {
                if (tempExp + remainingExp >= tempMaxExp)
                {
                    remainingExp -= tempMaxExp - tempExp;
                    tempLevel++;
                    levelUpCount++;
                    tempExp = 0;
                    tempMaxExp = GetMaxExpByLevel(tempLevel);
                }
                else
                {
                    tempExp += remainingExp;
                    remainingExp = 0;
                }
            }

            if (!addedLevelText.gameObject.activeSelf) addedLevelText.gameObject.SetActive(true);
            addedLevelText.text = levelUpCount > 0 ? $"+{levelUpCount}" : "";

        }

        private void UseItem()
        {
            if (selectedItem == null || selectedItemUseCount <= 0) return;

            if (backBtn.interactable) backBtn.interactable = false;

            CoinManager.Instance.SubtractRecipeItem(selectedItem, selectedItemUseCount);

            int gainedExp;
            if (selectedItem == CoinManager.Instance.beek) gainedExp = itemExpTable[CoinManager.Instance.beek] * selectedItemUseCount;
            else if (selectedItem == CoinManager.Instance.fineDining) gainedExp = itemExpTable[CoinManager.Instance.fineDining] * selectedItemUseCount;
            else if (selectedItem == CoinManager.Instance.masterChef) gainedExp = itemExpTable[CoinManager.Instance.masterChef] * selectedItemUseCount;
            else gainedExp = 0;

            if (gainedExp <= 0) return;
            StartCoroutine(AddExpRoutine(gainedExp));

            useItemPanel.SetActive(false);
            addedExpText.gameObject.SetActive(false);
            addedLevelText.gameObject.SetActive(false);

            if (dialogCoroutine != null)
            {
                StopCoroutine(dialogCoroutine);
                dialogCoroutine = null;
            }
            dialogCoroutine = StartCoroutine(DialogPanelFadeOut());
        }
        /// <summary>
        /// 경험치 증가 + 레벨업 처리
        /// </summary>
        private IEnumerator AddExpRoutine(int gainedExp)
        {
            beekBtn.interactable = false;
            fineDinigBtn.interactable = false;
            masterChefBtn.interactable = false;
            while (gainedExp > 0)
            {
                curExp += 10;
                gainedExp -= 10;

                if (curExp >= maxExp)
                {
                    curExp -= maxExp;
                    curLevel++;
                    maxExp = GetMaxExpByLevel(curLevel);
                    userLevelText.text = $"{curLevel}";
                }

                expBar.fillAmount = (float)curExp / maxExp;
                curExpText.text = $"{curExp}/{maxExp}";


                yield return new WaitForSeconds(0.01f);
            }
            beekBtn.interactable = true;
            fineDinigBtn.interactable = true;
            masterChefBtn.interactable = true;
            if (!backBtn.interactable) backBtn.interactable = true;
        }

        // TODO : CSV와 연결
        /// <summary>
        /// 테스트 레벨 경험치
        /// </summary>
        private int GetMaxExpByLevel(int level)
        {
            return 1000 + (level - 1) * 200;
        }

        IEnumerator DialogPanelFadeOut()
        {
            dialogPanelColor.color = new Color(dialogPanelColor.color.r, dialogPanelColor.color.g, dialogPanelColor.color.b, 200f / 255f);
            dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, 1f);
            yield return new WaitForSeconds(10f);

            float duration = 0.5f;
            float elapsed = 0f;

            Color startPanelColor = dialogPanelColor.color;
            Color startDialogColor = dialogText.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                Color newPanelColor = startPanelColor;
                newPanelColor.a = Mathf.Lerp(startPanelColor.a, 0f, t);

                Color newDialogColor = startDialogColor;
                newDialogColor.a = Mathf.Lerp(startDialogColor.a, 0f, t);

                dialogPanelColor.color = newPanelColor;
                dialogText.color = newDialogColor;
                yield return null;
            }

            dialogPanelColor.color = new Color(dialogPanelColor.color.r, dialogPanelColor.color.g, dialogPanelColor.color.b, 0f);
            dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, 0f);
        }
    }
}
