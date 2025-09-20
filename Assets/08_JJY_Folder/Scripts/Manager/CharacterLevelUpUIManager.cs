using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;
using System;

namespace JJY
{
    // TODO : CoinManager => GameManager 연결 시 코드 재정리.
    public class CharacterLevelUpUIManager : MonoBehaviour
    {
        [Header("Button Prefab")]
        [SerializeField] private CharacterLevelUpUI _characterLevelUpUI;
        [SerializeField] private GameObject characterButtonPrefab;

        // [SerializeField] private TextMeshProUGUI 
        // CharacterInfoUI.cs에서 캐릭터 정보 가져오고 세팅하는 코드 확인.
        // CharacterDataManager.cs에서 아래 리스트에서 모든 캐릭터 가져오기.
        // public List<CharacterDataSO> CharacterLists => _characterLists;

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
        [SerializeField] private GameObject useItemPanel; // 아이템 버튼 클릭 시, SetActive true가 될 Panel.

        [Header("Dialog Text")]
        [SerializeField] private Image dialogPanelColor;
        [SerializeField] private TextMeshProUGUI dialogText; // dialog 표시

        [Header("Button")]
        [SerializeField] private Button backBtn; // TODO : 돌아가기 버튼
        [SerializeField] private Button useItemBtn; // 아이템 사용 버튼
        [SerializeField] private Button beekBtn; // beek 버튼
        [SerializeField] private Button fineDiningBtn; // fineDining 버튼
        [SerializeField] private Button masterChefBtn; // masterChef 버튼

        // Addressable 연결
        [Header("Image Assets")]
        [SerializeField] private Sprite BGimage;
        // 버튼 기본 이미지
        [SerializeField] private Sprite beekimage;
        [SerializeField] private Sprite fineimage;
        [SerializeField] private Sprite masterimage;
        // 버튼 선택 이미지
        [SerializeField] private Sprite beekSelectedimage;
        [SerializeField] private Sprite fineSelectedimage;
        [SerializeField] private Sprite masterSelectedimage;

        [Header("Level Info")]
        [SerializeField] private TextMeshProUGUI addedExpText; // 아이템을 사용해서 얻는 경험치 수치
        [SerializeField] private TextMeshProUGUI addedLevelText; // 아이템을 사용해서 얻는 레벨 수치
        [SerializeField] private Image expBar; // 현재 경험치 바

        [Header("Selected Character Info")]
        // 캐릭터들의 이미지
        // private Dictionary<캐릭터, 캐릭터 이미지>
        [SerializeField] private Image _characterImage;
        [SerializeField] private Dictionary<CharacterRole, Image> _characterTypeImageDic = new Dictionary<CharacterRole, Image>();
        [SerializeField] private Image _characterTypeImage;
        [SerializeField] private Image _passiveSkillImage;
        [SerializeField] private Image _activeSkillImage;
        [SerializeField] private TextMeshProUGUI _classLevelText;
        [SerializeField] private TextMeshProUGUI _currentEXP;
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private TextMeshProUGUI _classNameText;
        // TODO : 캐릭터 소개 문구
        [SerializeField] private TextMeshProUGUI _passiveSkillDescriptionText;
        [SerializeField] private TextMeshProUGUI _activeSkillDescriptionText;
        [SerializeField] private TextMeshProUGUI _mpText;
        [SerializeField] private TextMeshProUGUI _hpText;
        [SerializeField] private TextMeshProUGUI _attackSpeedText;
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _defenceText;
        [SerializeField] private TextMeshProUGUI _criticalChanceText;
        [SerializeField] private TextMeshProUGUI _criticalDamageText;

        // [Header("Test")]
        // [SerializeField] private List<CharacterDataSO> _testCharacterLists; // 테스트 캐릭터 리스트

        private Dictionary<string, int> itemExpTable = new Dictionary<string, int>();
        private Coroutine dialogCoroutine;
        private CoinManager _coin;
        private GameManager _gameManager;
        private bool _isLoaded;
        // 선택된 캐릭터의 데이터
        private CharacterDataSO selectedCharacterData;

        #region 초기화 작업
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _coin = GameManager.Instance.Coin;
            // _coin.OnItemsChanged += InitItemCountText;
            // InitEXPTable();
            // InitItemCountText();
            // // InitUserInfo();
            // InitButtonFunctions();
            // InitItemButtonImage();
        }

        private void Update()
        {
            if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                !_gameManager.Firebase.IsLoaded || _isLoaded) return;

            _coin.OnItemsChanged += InitItemCountText;
            InitEXPTable();
            InitItemCountText();
            // InitUserInfo();
            InitButtonFunctions();
            InitItemButtonImage();

            _isLoaded = true;
        }

        // TODO : CSV 연결
        /// <summary>
        /// 아이템의 경험치 수치 초기화.
        /// </summary>
        private void InitEXPTable()
        {
            itemExpTable[_coin.beek] = 1000;
            itemExpTable[_coin.fineDining] = 5000;
            itemExpTable[_coin.masterChef] = 20000;
        }

        /// <summary>
        /// 현재 보유중인 아이템의 수량을 표기하기 위한 초기화 작업
        /// </summary>
        private void InitItemCountText()
        {
            beeksCount = _coin.GetRecipeItemCount(_coin.beek);
            fineDiningCount = _coin.GetRecipeItemCount(_coin.fineDining);
            masterChefCount = _coin.GetRecipeItemCount(_coin.masterChef);

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
            // TODO : 현재 이 게임의 전체 캐릭터를 가져와야 함. 그 다음에 가지고 있지 않은 캐릭터 회색처리.

            // 돌파 카운트 가져오기
            // foreach (var ownedCharacter in AllOwnedCharacters)
            // {
            //     var beadsCount = BeadsInventory[ownedCharacter._chaBaseData.ChaEnName];
            // }

            yield return null;

            var characterList = new List<GameObject>();

            var list = GameManager.Instance.CharacterData.CharacterLists;
            for (int i = 0; i < list.Count; i++)
            {
                var character = list[i];
                if (character == null) continue;

                var go = Instantiate(characterButtonPrefab);
                var button = go.GetComponent<Button>();
                var image = go.GetComponent<Image>();
                var beadsCount = go.GetComponentInChildren<TextMeshProUGUI>();

                // TODO : 돌파 카운트는 UI 선정되고 적용
                if (beadsCount != null)
                {
                    beadsCount.text = character.Beads > 0 ? $"{character.Beads}" : "";
                }
                image.sprite = character._characterSprite;

                button.onClick.AddListener(() => InitCharacterInfo(character));

                bool hasCharacter;
                GameManager.Instance.CharacterData.OwnedCharacters.TryGetValue(character._chaBaseData.ChaEnName, out hasCharacter);
                if (!hasCharacter)
                {
                    button.interactable = false;
                }
                characterList.Add(go);
            }
            _characterLevelUpUI.SetContent(characterList);
        }

        private void InitCharacterInfo(CharacterDataSO data)
        {
            // TODO : 선택된 캐릭터의 정보를 가져와야 함.
            selectedCharacterData = data;
            // 키가 존재하지 않으면 추가 (초기 세팅)
            if (!GameManager.Instance.CharacterBattleDataSave._chaLevel.ContainsKey(data._chaBaseData.ChaEnName))
            {
                Debug.LogError("키가 존재하지 않음.");
            }

            var levelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName]];

            _classLevelText.text = "Lv. " + GameManager.Instance.CharacterBattleDataSave._chaLevel[data._chaBaseData.ChaEnName];
            // CharacterDataManager의 함수 참고하기.
            GameManager.Instance.Firebase.Characters.TryGetValue(data._chaBaseData.ChaEnName.ToString(), out object raw);
            var dict = raw as IDictionary<string, object>;
            dict.TryGetValue("exp", out object expObj);
            _currentEXP.text = expObj.ToString() + " / " + levelData.ChaLevelPoint.ToString();

            // _characterImage.sprite = 캐릭터 전신 이미지
            _characterNameText.text = data._chaBaseData.ChaName;
            // 캐릭터 타입 아이콘
            Image typeIcon;
            _characterTypeImageDic.TryGetValue(data._chaTypeData.ChaRole, out typeIcon);
            _characterTypeImage.sprite = typeIcon.sprite;

            // _startValueText.text = GameManager.Instance.CharacterBattleDataSave._chaUpgrade[data._chaBaseData.ChaEnName].ToString();
            _classNameText.text = data._chaBaseData.ChaRole.ToString();
            // 캐릭터 소개

            var characterBaseData = data._chaBaseData;

            var passiveSkill = data._chaPassiveSkill;
            _passiveSkillImage.sprite = data._passiveSkillSprite;
            // _passiveSkillNameText.text = passiveSkill._chaSkillName;
            _passiveSkillDescriptionText.text = GetDescription(passiveSkill, characterBaseData);

            var activeSkill = data._chaActiveSkill;
            if (activeSkill == null) return;

            if (activeSkill._chaSkillID == -1) return;

            _activeSkillImage.sprite = data._activeSkillSprite;
            // _activeSkillNameText.text = activeSkill._chaSkillName;
            _activeSkillDescriptionText.text = GetDescription(activeSkill, characterBaseData);

            _mpText.text = data._chaBaseData.ChaMP.ToString();
            _hpText.text = (data._chaBaseData.ChaHP * levelData.ChaHPIncrease).ToString();
            _attackSpeedText.text = data._chaBaseData.ChaAtkSpeed.ToString();
            _attackText.text = (data._chaBaseData.ChaAttack * levelData.ChaAttackIncrease).ToString();
            _defenceText.text = (data._chaBaseData.ChaArmor * levelData.ChaArmorIncrease).ToString();
            _criticalChanceText.text = data._chaTypeData.ChaCrit.ToString();
            _criticalDamageText.text = data._chaTypeData.ChaCritDmg.ToString();
        }

        private string GetDescription(CharacterSkillDataSO skillData, CharacterBaseDataFileData characterBaseData)
        {
            string description = skillData._chaSkillDescription.Replace(
                "{chaSkillChance}", skillData._chaSkillChance.ToString()
            );
            description = description.Replace(
                "{chaAttack*chaSkillValue}", (skillData._chaSkillValue * characterBaseData.ChaAttack).ToString()
            );
            description = description.Replace(
                "{chaEffectValue}", skillData._chaEffectValue.ToString()
            );
            description = description.Replace(
                "{chaSkillHit}", skillData._chaSkillHit.ToString()
            );

            return description;
        }

        private void InitButtonFunctions()
        {
            itemBarSlider.onValueChanged.AddListener(ItemSlideUpdate);
            useItemBtn.onClick.AddListener(UseItem);
            beekBtn.onClick.AddListener(OnClickBeeks);
            fineDiningBtn.onClick.AddListener(OnClickFineDining);
            masterChefBtn.onClick.AddListener(OnClickMasterChef);
        }
        private void OnClickBeeks()
        {
            InitItemButtonImage();
            beekBtn.image.sprite = beekimage;
            selectedItem = _coin.beek;
            selectedItemMaxCount = beeksCount;
            InitItemBar();
        }
        private void OnClickFineDining()
        {
            InitItemButtonImage();
            fineDiningBtn.image.sprite = fineimage;
            selectedItem = _coin.fineDining;
            selectedItemMaxCount = fineDiningCount;
            InitItemBar();
        }
        private void OnClickMasterChef()
        {
            InitItemButtonImage();
            masterChefBtn.image.sprite = masterimage;
            selectedItem = _coin.masterChef;
            selectedItemMaxCount = masterChefCount;
            InitItemBar();
        }
        private void InitItemButtonImage()
        {
            beekBtn.image.sprite = beekimage;
            fineDiningBtn.image.sprite = fineimage;
            masterChefBtn.image.sprite = masterimage;
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

            var chaKey = selectedCharacterData._chaBaseData.ChaEnName;
            int curExp = GetExpFromFirebase(chaKey);
            int curLevel;

            if (!GameManager.Instance.CharacterBattleDataSave._chaLevel.TryGetValue(chaKey, out curLevel))
            {
                curLevel = selectedCharacterData._chaLv;
            }

            int previewExp = curExp;
            int previewLevel = curLevel;
            int remainingExp = gainedExp;
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

            if (!addedLevelText.gameObject.activeSelf) addedLevelText.gameObject.SetActive(true);
            addedLevelText.text = levelUpCount > 0 ? $"+{levelUpCount}" : "";

        }

        /// <summary>
        /// 아이템 사용하기 버튼
        /// </summary>
        public void UseItem()
        {
            if (selectedItem == null || selectedItemUseCount <= 0) return;

            if (backBtn.interactable) backBtn.interactable = false;

            _coin.SubtractRecipeItem(selectedItem, selectedItemUseCount);

            int gainedExp;
            if (selectedItem == _coin.beek) gainedExp = itemExpTable[_coin.beek] * selectedItemUseCount;
            else if (selectedItem == _coin.fineDining) gainedExp = itemExpTable[_coin.fineDining] * selectedItemUseCount;
            else if (selectedItem == _coin.masterChef) gainedExp = itemExpTable[_coin.masterChef] * selectedItemUseCount;
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
            fineDiningBtn.interactable = false;
            masterChefBtn.interactable = false;

            var chaKey = selectedCharacterData._chaBaseData.ChaEnName;
            int curExp = GetExpFromFirebase(chaKey);
            int curLevel = GameManager.Instance.CharacterBattleDataSave._chaLevel[chaKey];

            if (!GameManager.Instance.CharacterData.ChaLevelUpStatData.ContainsKey(curLevel))
            {
                Debug.LogWarning("levelData 오류 " + curLevel);
            }

            while (gainedExp > 0)
            {
                var levelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[curLevel];
                int maxExp = levelData.ChaLevelPoint;

                // curExp += 10;
                // 얻는 도중 강제종료 시 어떻게 하지
                int need = maxExp - curExp;
                int toAdd = Mathf.Min(need, gainedExp);
                curExp += toAdd;
                gainedExp -= toAdd;

                if (curExp >= maxExp)
                {
                    curExp = 0;
                    curLevel++;
                    GameManager.Instance.CharacterBattleDataSave._chaLevel[chaKey] = curLevel;
                    // SaveExpToFirebase(chaKey, curExp);
                }

                var newLevelData = GameManager.Instance.CharacterData.ChaLevelUpStatData[curLevel];
                expBar.fillAmount = (float)curExp / newLevelData.ChaLevelPoint;
                _classLevelText.text = "Lv. " + curLevel;
                _currentEXP.text = $"{curExp} / {newLevelData.ChaLevelPoint}";

                yield return new WaitForSeconds(0.01f);
            }
            beekBtn.interactable = true;
            fineDiningBtn.interactable = true;
            masterChefBtn.interactable = true;
            if (!backBtn.interactable) backBtn.interactable = true;

            SaveExpToFirebase(chaKey, curExp);
        }

        private IEnumerator DialogPanelFadeOut()
        {
            dialogPanelColor.color = new Color(dialogPanelColor.color.r, dialogPanelColor.color.g, dialogPanelColor.color.b,
                200f / 255f);
            dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, 1f);
            yield return new WaitForSeconds(10f);

            float duration = 0.5f;
            float elapsed = 0f;

            var startPanelColor = dialogPanelColor.color;
            var startDialogColor = dialogText.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                var newPanelColor = startPanelColor;
                newPanelColor.a = Mathf.Lerp(startPanelColor.a, 0f, t);

                var newDialogColor = startDialogColor;
                newDialogColor.a = Mathf.Lerp(startDialogColor.a, 0f, t);

                dialogPanelColor.color = newPanelColor;
                dialogText.color = newDialogColor;
                yield return null;
            }

            dialogPanelColor.color = new Color(dialogPanelColor.color.r, dialogPanelColor.color.g, dialogPanelColor.color.b, 0f);
            dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, 0f);
        }
        #endregion
        #region Firebase
        private int GetExpFromFirebase(Enum chaEnName)
        {
            if (GameManager.Instance == null || GameManager.Instance.Firebase == null) return 0;
            string key = chaEnName.ToString();

            if (!GameManager.Instance.Firebase.Characters.TryGetValue(key, out object raw)) return 0;
            if (raw == null) return 0;

            var dict = raw as IDictionary<string, object>;

            if (dict != null && dict.TryGetValue("exp", out object expObj))
            {
                if (expObj is int i) return i;
                if (expObj is float f) return Mathf.RoundToInt(f);
                if (int.TryParse(expObj.ToString(), out int parsed)) return parsed;
            }

            return 0;
        }
        private void SaveExpToFirebase(Enum chaKey, int newExp)
        {
            // TODO : Firebase에 경험치 변화량 저장
            // GameManager.Instance.Firebase.UpdateCharacterExp(chaKey, newExp);
        }
        #endregion
    }
}