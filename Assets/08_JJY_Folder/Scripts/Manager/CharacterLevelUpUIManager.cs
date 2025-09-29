using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;
using System;
using KSH;

namespace JJY
{
    public class CharacterLevelUpUIManager : MonoBehaviour
    {
        [Header("Button Prefab")]
        [SerializeField] private Transform _contents;
        [SerializeField] private GameObject characterButtonPrefab;

        [Header("Item Count Text")]
        private string selectedItem; // 현재 선택된 아이템의 이름
        private int selectedItemUseCount; // 현재 선택된 아이템의 사용하려는 개수
        private int selectedItemMaxCount; // 현재 선택된 아이템의 최대 보유량
        private int beeksCount; // TODO : 유저의 현재 beek's 보유량
        private int fineDiningCount; // TODO : 유저의 현재 fine Dining 보유량
        private int masterChefCount; // TODO : 유저의 현재 master Chef 보유량
        [SerializeField] private TextMeshProUGUI beeksCountText; // beek's의 수량 텍스트
        [SerializeField] private TextMeshProUGUI finediningCountText; // fine dining의 수량 텍스트
        [SerializeField] private TextMeshProUGUI masterChefCountText; // masterChef의 수량 텍스트
        [SerializeField] private TextMeshProUGUI useItemCountText; // 선택된 아이템의 사용량 (1 / 현재 보유량)
        [SerializeField] private Image itemBar; // 아이템 사용 슬라이더 게이지
        [SerializeField] private Slider itemBarSlider; // 아이템 사용 슬라이더

        [Header("Button")]
        [SerializeField] private Button backButton; // 재화 사용 화면에서 뒤로가기 버튼
        [SerializeField] private Button useItemBtn; // 아이템 사용 버튼
        [SerializeField] private Button beekBtn; // beek 버튼
        [SerializeField] private Button fineDiningBtn; // fineDining 버튼
        [SerializeField] private Button masterChefBtn; // masterChef 버튼
        [SerializeField] private Button levelUpButton; // 레벨업 버튼
        [SerializeField] private Button beekSelectedBtn; // beek 버튼
        [SerializeField] private Button fineDiningSelectedBtn; // fineDining 버튼
        [SerializeField] private Button masterChefSelectedBtn; // masterChef 버튼
        [SerializeField] private Button characterStatInfoButton; // 자세히 보기 버튼

        [Header("Level Info")]
        [SerializeField] private TextMeshProUGUI addedExpText; // 아이템을 사용해서 얻는 경험치 수치
        [SerializeField] private TextMeshProUGUI addedLevelText; // 아이템을 사용해서 얻는 레벨 수치
        [SerializeField] private Image expBar; // 현재 경험치 바

        [Header("Selected Character Info")]
        [SerializeField] private GameObject _characterLevelInfoPanel;
        [SerializeField] private Image _characterImage;
        [SerializeField] private Image _characterTypeImage;
        [SerializeField] private Image _passiveSkillImage;
        [SerializeField] private Image _activeSkillImage;
        [SerializeField] private List<Image> _charBeadImageLists;
        [SerializeField] private Color _blankColor;
        [SerializeField] private Color _filledColor;
        [SerializeField] private TextMeshProUGUI _classLevelText;
        [SerializeField] private TextMeshProUGUI _currentEXP;
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private TextMeshProUGUI _classNameText;
        [SerializeField] private TextMeshProUGUI _characterDescription;
        [SerializeField] private TextMeshProUGUI _passiveSkillDescriptionText;
        [SerializeField] private TextMeshProUGUI _activeSkillDescriptionText;
        [SerializeField] private TextMeshProUGUI _mpText;
        [SerializeField] private TextMeshProUGUI _hpText;
        [SerializeField] private TextMeshProUGUI _attackSpeedText;
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _defenceText;
        [SerializeField] private TextMeshProUGUI _criticalChanceText;
        [SerializeField] private TextMeshProUGUI _criticalDamageText;

        private int previewExp;
        private int previewLevel;
        private int remainingExp;

        private Dictionary<string, int> itemExpTable = new Dictionary<string, int>();
        private CoinManager _coin;
        private GameManager _gameManager;
        private CharacterDataSO selectedCharacterData;
        private bool _isLoaded;
        public bool IsLoaded => _isLoaded;

// #if UNITY_EDITOR
//         private void TestAddRecipeBooks()
//         {
//             GameManager.Instance.Coin.AddRecipeItem("beeksRecipeBook", 5000);
//             GameManager.Instance.Coin.AddRecipeItem("fineDiningRecipeBook", 5000);
//             GameManager.Instance.Coin.AddRecipeItem("masterChefRecipeBook", 5000);
//         }
// #endif

        #region 초기화 작업

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _coin = GameManager.Instance.Coin;
        }

        private void Update()
        {
            if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                !_gameManager.Firebase.IsLoaded || !_gameManager.CharacterData.IsDownloaded || !_coin.IsDownloaded ||
                _isLoaded) return;

            _coin.OnItemsChanged += InitItemCountText;
            InitEXPTable();
            InitItemCountText();
            InitButtonFunctions();
            InitItemButtonImage();
            InitCharacterList();
            _characterLevelInfoPanel.gameObject.SetActive(false);

//             // Test
// #if UNITY_EDITOR
//             TestAddRecipeBooks();
// #endif

            _isLoaded = true;
        }

        private void InitEXPTable()
        {
            itemExpTable[_coin.beek] = 1000;
            itemExpTable[_coin.fineDining] = 5000;
            itemExpTable[_coin.masterChef] = 20000;
        }

        private void InitItemCountText()
        {
            beeksCount = _coin.Items[_coin.beek];
            fineDiningCount = _coin.Items[_coin.fineDining];
            masterChefCount = _coin.Items[_coin.masterChef];

            beeksCountText.text = $"{beeksCount}";
            finediningCountText.text = $"{fineDiningCount}";
            masterChefCountText.text = $"{masterChefCount}";
        }

        public void InitCharacterList()
        {
            if (characterButtonPrefab == null) return;

            StartCoroutine(SetCharacterContentsInit());
        }

        private IEnumerator SetCharacterContentsInit()
        {
            yield return null;

            for (int i = _contents.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_contents.transform.GetChild(i).gameObject);
            }

            var characterList = new List<GameObject>();
            var list = GameManager.Instance.CharacterData.CharacterLists;

            for (int i = 0; i < list.Count; i++)
            {
                var character = list[i];
                if (character == null) continue;
                var characterLocal = character;

                var go = Instantiate(characterButtonPrefab);
                var button = go.GetComponent<Button>();
                var init = go.GetComponent<LevelUpCharButton>();

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => InitCharacterInfo(characterLocal));

                bool hasCharacter;
                GameManager.Instance.CharacterData.OwnedCharacters.TryGetValue(characterLocal._chaBaseData.ChaEnName,
                    out hasCharacter);
                if (!hasCharacter)
                {
                    button.interactable = false;
                }
                characterList.Add(go);

                init.SetLevelUpChar(characterLocal);

                go.name = $"Character_Button_{characterLocal._chaBaseData.ChaEnName}";
            }
            SetContent(characterList);
        }

        private void SetContent(List<GameObject> characterList)
        {
            foreach (var character in characterList)
            {
                character.transform.SetParent(_contents.transform, false);
                character.gameObject.SetActive(true);
            }
        }

        private void InitCharacterInfo(CharacterDataSO data)
        {
            selectedCharacterData = data;
            CharacterState currentCharacterStatData = data.GetModifiedCharacterState();

            var levelData =
                GameManager.Instance.CharacterData.ChaLevelUpStatData[
                    GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName]];
            var beadData =
                GameManager.Instance.CharacterData.ChaBeadsData[
                    GameManager.Instance.CharacterData.BeadsInventory[data._chaBaseData.ChaEnName]];

            int curExp = GameManager.Instance.CharacterData.CharEnNameExp[data._chaBaseData.ChaEnName];
            int curlevel = GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName];
            // int curUpgradeLevel = GameManager.Instance.CharacterData.BeadsInventory[data._chaBaseData.ChaEnName];
            int curMaxExp = GameManager.Instance.CharacterData.ChaLevelUpStatData[curlevel].ChaLevelPoint;

            // GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName] = curlevel;
            // GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName] = curUpgradeLevel;

            expBar.fillAmount = (float)curExp / curMaxExp;

            _classLevelText.text = curlevel.ToString();

            _currentEXP.text = GameManager.Instance.CharacterData.CharEnNameExp[data._chaBaseData.ChaEnName] + " / " +
                               levelData.ChaLevelPoint.ToString();

            if (!_characterImage.gameObject.activeSelf) _characterImage.gameObject.SetActive(true);
            _characterImage.sprite = data.largeDeformationSprite;

            _characterNameText.text = data._chaBaseData.ChaName;

            if (!_characterTypeImage.gameObject.activeSelf) _characterTypeImage.gameObject.SetActive(true);
            _characterTypeImage.sprite = data.roleIcon;

            int beadCount = GameManager.Instance.CharacterData.BeadsInventory[data._chaBaseData.ChaEnName];
            for (int i = 0; i < _charBeadImageLists.Count; i++)
            {
                if (i < beadCount) _charBeadImageLists[i].color = _filledColor;
                else _charBeadImageLists[i].color = _blankColor;
            }
            _classNameText.text = data._chaBaseData.ChaRole.ToString();
            _characterDescription.text = data._chaBaseData.ChaIntroduction;

            _passiveSkillImage.sprite = data._passiveSkillSprite;
            _passiveSkillDescriptionText.text = GetDescription(data._chaPassiveSkill, currentCharacterStatData);
            if (data._chaActiveSkill == null) _activeSkillImage.gameObject.SetActive(false);
            else
            {
                _activeSkillImage.sprite = data._activeSkillSprite;
                _activeSkillDescriptionText.text = GetDescription(data._chaActiveSkill, currentCharacterStatData);
                _activeSkillImage.gameObject.SetActive(true);
            }

            _mpText.text = currentCharacterStatData._chaMaxMP.ToString();
            _hpText.text = (data._chaBaseData.ChaHP * levelData.ChaHPIncrease * beadData.ChaHP).ToString("F0");
            _attackSpeedText.text = currentCharacterStatData._chaAtkSpeed.ToString("n2");
            _attackText.text = (currentCharacterStatData._chaAttack * levelData.ChaAttackIncrease * beadData.ChaAttack).ToString("F0");
            _defenceText.text = (currentCharacterStatData._chaArmor * levelData.ChaArmorIncrease * beadData.ChaArmor).ToString("F0");
            _criticalChanceText.text = currentCharacterStatData._chaCrit.ToString();
            _criticalDamageText.text = currentCharacterStatData._chaCritDmg.ToString();

            _characterLevelInfoPanel.gameObject.SetActive(true);
            characterStatInfoButton.interactable = true;

            if (GameManager.Instance.CharacterData.CharEnNameLevel[data._chaBaseData.ChaEnName] > 30)
            {
                levelUpButton.interactable = false;
                return;
            }
            levelUpButton.interactable = true;
        }

        private string GetDescription(CharacterSkillDataSO skillData, CharacterState characterState)
        {
            string description = skillData._chaSkillDescription.Replace(
                "<chaSkillChance>", skillData._chaSkillChance.ToString("F0")
            );
            description = description.Replace(
                "<chaAttack>*<chaSkillValue>", (skillData._chaSkillValue * characterState._chaAttack).ToString("F0")
            );
            description = description.Replace(
                "<chaEffectValue*100>", (skillData._chaEffectValue * 100).ToString("F0")
            );
            description = description.Replace(
                "<chaSkillHit>", skillData._chaSkillHit.ToString("F0")
            );

            return description;
        }

        private void InitButtonFunctions()
        {
            levelUpButton.interactable = false;
            characterStatInfoButton.interactable = false;
            itemBarSlider.onValueChanged.AddListener(ItemSlideUpdate);
            useItemBtn.onClick.AddListener(UseItem);
            beekBtn.onClick.AddListener(OnClickBeeks);
            fineDiningBtn.onClick.AddListener(OnClickFineDining);
            masterChefBtn.onClick.AddListener(OnClickMasterChef);
            backButton.onClick.AddListener(BackButtonClicked);
            beekSelectedBtn.onClick.AddListener(OnClickBeeks);
            fineDiningSelectedBtn.onClick.AddListener(OnClickFineDining);
            masterChefSelectedBtn.onClick.AddListener(OnClickMasterChef);
        }

        private void OnDisable()
        {
            itemBarSlider.onValueChanged.RemoveListener(ItemSlideUpdate);
            useItemBtn.onClick.RemoveListener(UseItem);
            beekBtn.onClick.RemoveListener(OnClickBeeks);
            fineDiningBtn.onClick.RemoveListener(OnClickFineDining);
            masterChefBtn.onClick.RemoveListener(OnClickMasterChef);
            backButton.onClick.RemoveListener(BackButtonClicked);
            beekSelectedBtn.onClick.RemoveListener(OnClickBeeks);
            fineDiningSelectedBtn.onClick.RemoveListener(OnClickFineDining);
            masterChefSelectedBtn.onClick.RemoveListener(OnClickMasterChef);
        }

        private void OnClickBeeks()
        {
            InitItemButtonImage();
            beekSelectedBtn.gameObject.SetActive(true);
            beekBtn.gameObject.SetActive(false);
            selectedItem = _coin.beek;
            selectedItemMaxCount = beeksCount;
            InitItemBar();
            ItemSlideUpdate(1f);
        }

        private void OnClickFineDining()
        {
            InitItemButtonImage();
            fineDiningSelectedBtn.gameObject.SetActive(true);
            fineDiningBtn.gameObject.SetActive(false);
            selectedItem = _coin.fineDining;
            selectedItemMaxCount = fineDiningCount;
            InitItemBar();
            ItemSlideUpdate(1f);
        }

        private void OnClickMasterChef()
        {
            InitItemButtonImage();
            masterChefSelectedBtn.gameObject.SetActive(true);
            masterChefBtn.gameObject.SetActive(false);
            selectedItem = _coin.masterChef;
            selectedItemMaxCount = masterChefCount;
            InitItemBar();
            ItemSlideUpdate(1f);
        }

        private void InitItemButtonImage()
        {
            beekBtn.gameObject.SetActive(true);
            beekSelectedBtn.gameObject.SetActive(false);
            fineDiningBtn.gameObject.SetActive(true);
            fineDiningSelectedBtn.gameObject.SetActive(false);
            masterChefBtn.gameObject.SetActive(true);
            masterChefSelectedBtn.gameObject.SetActive(false);
        }

        private void InitItemBar()
        {
            if (selectedItemMaxCount <= 0)
            {
                if (addedExpText.gameObject.activeSelf) addedExpText.gameObject.SetActive(false);
                if (addedLevelText.gameObject.activeSelf) addedLevelText.gameObject.SetActive(false);
                return;
            }
            useItemBtn.gameObject.SetActive(true);
            itemBarSlider.gameObject.SetActive(true);

            itemBarSlider.wholeNumbers = true;
            itemBarSlider.minValue = 1;
            itemBarSlider.maxValue = selectedItemMaxCount;
            itemBarSlider.value = 1;

            selectedItemUseCount = 1;
            itemBar.fillAmount = (float)selectedItemUseCount / selectedItemMaxCount;
            useItemCountText.text = $"{selectedItemUseCount} / {selectedItemMaxCount}";

            InitItemCountText();
        }

        #endregion

        #region 아이템 사용

        public void ItemSlideUpdate(float value)
        {
            if (selectedItemMaxCount <= 0 || selectedCharacterData == null) return;

            selectedItemUseCount = Mathf.Clamp(Mathf.RoundToInt(value), 1, selectedItemMaxCount);

            itemBar.fillAmount = (float)selectedItemUseCount / selectedItemMaxCount;
            useItemCountText.text = $"{selectedItemUseCount} / {selectedItemMaxCount}";

            if (!itemExpTable.TryGetValue(selectedItem, out int perItemExp))
                perItemExp = 0;
            int gainedExp = perItemExp * selectedItemUseCount;

            if (!addedExpText.gameObject.activeSelf) addedExpText.gameObject.SetActive(true);

            addedExpText.text = $"+{gainedExp}";

            var chaKey = selectedCharacterData._chaBaseData;
            int curExp = GameManager.Instance.CharacterData.CharEnNameExp[chaKey.ChaEnName];
            int curLevel;

            if (!GameManager.Instance.CharacterData.CharEnNameLevel.TryGetValue(chaKey.ChaEnName, out curLevel))
            {
                curLevel = selectedCharacterData._chaLv;
            }

            previewExp = curExp;
            previewLevel = curLevel;
            remainingExp = gainedExp;
            int levelUpCount = 0;

            while (remainingExp > 0)
            {
                if (!GameManager.Instance.CharacterData.ChaLevelUpStatData.TryGetValue(previewLevel, out var levelData))
                {
                    Debug.LogWarning($"Level data 오류 {previewLevel}. 경험치 증가 while문 break");
                    break;
                }

                int maxExpForLevel = levelData.ChaLevelPoint;
                int needToNext = Mathf.Max(0, maxExpForLevel - previewExp);

                if (remainingExp >= needToNext && needToNext > 0)
                {
                    remainingExp -= needToNext;
                    previewLevel++;
                    levelUpCount++;
                    previewExp = 0;
                }
                else
                {
                    previewExp += remainingExp;
                    remainingExp = 0;
                }
            }

            float previewFill = 0f;
            if (GameManager.Instance.CharacterData.ChaLevelUpStatData.TryGetValue(previewLevel, out var finalLevelData))
            {
                previewFill = finalLevelData.ChaLevelPoint > 0 ? (float)previewExp / finalLevelData.ChaLevelPoint : 0f;
            }
            expBar.fillAmount = Mathf.Clamp01(previewFill);

            if (!addedLevelText.gameObject.activeSelf)
            {
                addedLevelText.gameObject.SetActive(true);
            }

            if (addedExpText.gameObject.activeSelf && previewLevel > 30)
                addedExpText.gameObject.SetActive(false);
            if (addedLevelText.gameObject.activeSelf && previewLevel > 30 &&
                GameManager.Instance.CharacterData.CharEnNameExp[selectedCharacterData._chaBaseData.ChaEnName] + gainedExp >
                GameManager.Instance.CharacterData
                    .ChaLevelUpStatData[
                        GameManager.Instance.CharacterData.CharEnNameLevel[selectedCharacterData._chaBaseData.ChaEnName]]
                    .ChaLevelPoint)
                addedLevelText.gameObject.SetActive(false);

            addedLevelText.text = levelUpCount > 0 ? $"+{levelUpCount}" : "";
        }

        public void UseItem()
        {
            if (selectedItem == null || selectedItemUseCount <= 0) return;

            if (previewLevel > 30) previewLevel = 30;
            int level = previewLevel;
            int exp = previewExp;

            useItemBtn.gameObject.SetActive(false);
            itemBarSlider.gameObject.SetActive(false);

            int gainedExp;
            if (selectedItem == _coin.beek) gainedExp = itemExpTable[_coin.beek] * selectedItemUseCount;
            else if (selectedItem == _coin.fineDining) gainedExp = itemExpTable[_coin.fineDining] * selectedItemUseCount;
            else if (selectedItem == _coin.masterChef) gainedExp = itemExpTable[_coin.masterChef] * selectedItemUseCount;
            else gainedExp = 0;

            if (gainedExp <= 0 ||
                GameManager.Instance.CharacterData.CharEnNameLevel[selectedCharacterData._chaBaseData.ChaEnName] == 30 &&
                GameManager.Instance.CharacterData.CharEnNameExp[selectedCharacterData._chaBaseData.ChaEnName] + gainedExp >
                GameManager.Instance.CharacterData
                    .ChaLevelUpStatData[
                        GameManager.Instance.CharacterData.CharEnNameLevel[selectedCharacterData._chaBaseData.ChaEnName]]
                    .ChaLevelPoint)
                return;

            _coin.SubtractRecipeItem(selectedItem, selectedItemUseCount);

            StartCoroutine(AddExpRoutine(gainedExp));

            addedExpText.gameObject.SetActive(false);
            addedLevelText.gameObject.SetActive(false);

            GameManager.Instance.CharacterData.SetCharLevel(selectedCharacterData._chaBaseData.ChaEnName, level);
            GameManager.Instance.CharacterData.SetCharExp(selectedCharacterData._chaBaseData.ChaEnName, exp);
            Debug.Log($"{GameManager.Instance.CharacterData.CharEnNameExp[selectedCharacterData._chaBaseData.ChaEnName]}");
            previewExp = 0;
        }

        private IEnumerator AddExpRoutine(int gainedExp)
        {
            beekBtn.interactable = false;
            fineDiningBtn.interactable = false;
            masterChefBtn.interactable = false;

            var chaKey = selectedCharacterData._chaBaseData;
            int curExp = GameManager.Instance.CharacterData.CharEnNameExp[chaKey.ChaEnName];
            int curLevel = GameManager.Instance.CharacterData.CharEnNameLevel[chaKey.ChaEnName];

            if (!GameManager.Instance.CharacterData.ChaLevelUpStatData.ContainsKey(curLevel))
            {
                Debug.LogWarning("levelData 오류 " + curLevel);
            }

            while (gainedExp > 0)
            {
                var levelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[curLevel];

                int maxExp = levelData.ChaLevelPoint;

                curExp += 250;
                gainedExp -= 250;

                if (curExp >= maxExp)
                {
                    curExp = 0;
                    curLevel++;

                    if (curLevel > 30) yield break;

                    GameManager.Instance.DailyQuest.CompleteQuestInLobbyScene(QuestType.CharacterLevelUp, 1);
                    GameManager.Instance.Firebase.SetQuestState(QuestType.CharacterLevelUp, true, 1);
                }

                var newLevelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[curLevel];

                expBar.fillAmount = (float)curExp / newLevelData.ChaLevelPoint;

                _classLevelText.text = $"{curLevel}";
                _currentEXP.text = $"{curExp} / {newLevelData.ChaLevelPoint}";

                yield return new WaitForSeconds(0.01f);
            }
            beekBtn.interactable = true;
            fineDiningBtn.interactable = true;
            masterChefBtn.interactable = true;
        }
        private void BackButtonClicked()
        {
            StopAllCoroutines();
            beekBtn.interactable = true;
            fineDiningBtn.interactable = true;
            masterChefBtn.interactable = true;
            InitCharacterInfo(selectedCharacterData);
        }

        #endregion
    }
}