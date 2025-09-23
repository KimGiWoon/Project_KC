using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class CharacterDataManager : MonoBehaviour
    {
        //# ID(int) - CharacterBaseDataFileData
        private Dictionary<int, CharacterBaseDataFileData> _chaIdData = new Dictionary<int, CharacterBaseDataFileData>();
        public IReadOnlyDictionary<int, CharacterBaseDataFileData> ChaIdData => _chaIdData;

        //# CharacterEnName(enum) - CharacterBaseDataFileData
        private Dictionary<CharacterEnName, CharacterBaseDataFileData> _chaEnNameData =
            new Dictionary<CharacterEnName, CharacterBaseDataFileData>();
        public IReadOnlyDictionary<CharacterEnName, CharacterBaseDataFileData> ChaEnNameData => _chaEnNameData;

        //# CharacterRole(enum) - CharacterTypeFileData
        private Dictionary<CharacterRole, CharacterTypeFileData> _chaTypeData =
            new Dictionary<CharacterRole, CharacterTypeFileData>();
        public IReadOnlyDictionary<CharacterRole, CharacterTypeFileData> ChaTypeData => _chaTypeData;

        //# CharacterUpgrade(int) - CharacterUpgradeData
        private Dictionary<int, CharacterUpgradeFileData> _chaBeadsData = new Dictionary<int, CharacterUpgradeFileData>();
        public IReadOnlyDictionary<int, CharacterUpgradeFileData> ChaBeadsData => _chaBeadsData;

        //# CharacterLevel(int) - CharacterLevelUpStatData
        private Dictionary<int, CharacterLevelUpStatFileData> _chaLevelUpStatData =
            new Dictionary<int, CharacterLevelUpStatFileData>();
        public IReadOnlyDictionary<int, CharacterLevelUpStatFileData> ChaLevelUpStatData => _chaLevelUpStatData;

        //# SkillID(int) - CharacterSkillFileData
        private Dictionary<int, CharacterSkillFileData> _chaIdSkillData = new Dictionary<int, CharacterSkillFileData>();
        public IReadOnlyDictionary<int, CharacterSkillFileData> ChaIdSkillData => _chaIdSkillData;

        //# SkillEnName(enum) - CharacterSkillFileData
        private Dictionary<CharacterSkillEnName, CharacterSkillFileData> _chaEnNameSkillData =
            new Dictionary<CharacterSkillEnName, CharacterSkillFileData>();
        public IReadOnlyDictionary<CharacterSkillEnName, CharacterSkillFileData> ChaEnNameSkillData => _chaEnNameSkillData;

        //# Character List
        [SerializeField] private List<CharacterDataSO> _characterLists; //캐릭터 리스트
        public List<CharacterDataSO> CharacterLists => _characterLists;

        private Dictionary<int, CharacterDataSO> _characterIdData = new Dictionary<int, CharacterDataSO>();
        public IReadOnlyDictionary<int, CharacterDataSO> CharacterIdData => _characterIdData;

        private Dictionary<CharacterEnName, CharacterDataSO> _characterEnNameData =
            new Dictionary<CharacterEnName, CharacterDataSO>();
        public IReadOnlyDictionary<CharacterEnName, CharacterDataSO> CharacterEnNameData => _characterEnNameData;

        private Dictionary<CharacterEnName, bool> _ownedCharacters = new Dictionary<CharacterEnName, bool>();
        public IReadOnlyDictionary<CharacterEnName, bool> OwnedCharacters => _ownedCharacters;

        private HashSet<CharacterDataSO> _allOwnedCharacters = new HashSet<CharacterDataSO>();
        public HashSet<CharacterDataSO> AllOwnedCharacters => _allOwnedCharacters;

        private Dictionary<CharacterEnName, int> _beadsInventory = new Dictionary<CharacterEnName, int>();
        public IReadOnlyDictionary<CharacterEnName, int> BeadsInventory => _beadsInventory;

        private Dictionary<CharacterEnName, int> _charEnNameExp = new Dictionary<CharacterEnName, int>();
        public IReadOnlyDictionary<CharacterEnName, int> CharEnNameExp => _charEnNameExp;

        private Dictionary<CharacterEnName, int> _charEnNameLevel = new Dictionary<CharacterEnName, int>();
        public IReadOnlyDictionary<CharacterEnName, int> CharEnNameLevel => _charEnNameLevel;

        private List<CharacterDataSO> _selectedTeam = new List<CharacterDataSO>();
        public IReadOnlyList<CharacterDataSO> SelectedTeam => _selectedTeam;

        private GameManager _gameManager;
        private FirebaseManager _firebase;
        private bool _isDownloaded;
        public bool IsDownloaded => _isDownloaded;

        public Action OnFirstCharacterChanged;

        //CJH 코드 추가
        public CharacterDataSO MapPlayerCharacter { get; private set; }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _firebase = GameManager.Instance.Firebase;
        }

        /// <summary>
        /// 각 Data Table 데이터 연결
        /// </summary>
        private void Update()
        {
            if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
                _isDownloaded || !_gameManager.Firebase.IsLoaded) return;

            LoadCharacterBase();
            LoadCharacterType();
            LoadCharacterUpgrade();
            LoadCharacterLevelUpStat();
            LoadCharacterSkill();
            LoadCharacterSO();
            LoadOwnedCharacter(_firebase.Characters);
            _isDownloaded = true;
        }

        private void LoadOwnedCharacter(IReadOnlyDictionary<string, object> charData)
        {
            bool isFirstCharacter = true;
            foreach (string key in charData.Keys)
            {
                var character = charData[key] as Dictionary<string, object>;

                //# 보유 시 추가
                if (Convert.ToBoolean(character["owned"]))
                {
                    GameManager.Instance.Reward.AddFirstCharacter(
                        _characterIdData[int.Parse(key)],
                        Convert.ToInt32(character["count"])
                    );
                }

                if (Convert.ToBoolean(character["selected"]))
                {
                    _selectedTeam.Add(_characterIdData[int.Parse(key)]);

                    if (isFirstCharacter)
                    {
                        SetMapPlayerCharacter(_characterIdData[int.Parse(key)]);
                        isFirstCharacter = false;
                    }
                }

                _charEnNameExp[_characterIdData[int.Parse(key)]._chaBaseData.ChaEnName] = Convert.ToInt32(character["exp"]);
                _charEnNameLevel[_characterIdData[int.Parse(key)]._chaBaseData.ChaEnName] = Convert.ToInt32(character["level"]);
            }
            // 첫 번째 멤버를 맵 플레이어 캐릭터로 설정합니다.
        }

        private void SetCharLevel(CharacterEnName name, int level)
        {
            if (_charEnNameLevel[name] == level) return;

            _charEnNameLevel[name] = level;

            _firebase.SetLevel(_characterEnNameData[name]._chaBaseData.ChaID.ToString(), level);
        }

        private void SeCharExp(CharacterEnName name, int exp)
        {
            if (_charEnNameExp[name] == exp) return;

            _charEnNameExp[name] = exp;
            _firebase.SetExp(_characterEnNameData[name]._chaBaseData.ChaID.ToString(), exp);
        }

        /// <summary>
        /// CharacterBase를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterBase()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterBaseData");
            var chaBaseList = HandleCSV.ReadDataFromLines<CharacterBaseDataFileData>(fields);

            foreach (var chaBase in chaBaseList)
            {
                _chaIdData[chaBase.ChaID] = chaBase;
                _chaEnNameData[chaBase.ChaEnName] = chaBase;
            }
        }

        /// <summary>
        /// CharacterTypeFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterType()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterType");
            var chaRoleList = HandleCSV.ReadDataFromLines<CharacterTypeFileData>(fields);

            foreach (var chaRole in chaRoleList)
            {
                _chaTypeData[chaRole.ChaRole] = chaRole;
            }
        }

        /// <summary>
        /// CharacterUpgradeFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterUpgrade()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterUpgrade");
            var chaUpgradeList = HandleCSV.ReadDataFromLines<CharacterUpgradeFileData>(fields);

            foreach (var chaUpgrade in chaUpgradeList)
            {
                _chaBeadsData[chaUpgrade.ChaUpgrade] = chaUpgrade;
            }
        }

        /// <summary>
        /// CharacterLevelUpStatFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterLevelUpStat()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterLevelUpStat");
            var chaLevelUpStatList = HandleCSV.ReadDataFromLines<CharacterLevelUpStatFileData>(fields);

            foreach (var chaLevelUpStat in chaLevelUpStatList)
            {
                _chaLevelUpStatData[chaLevelUpStat.ChaLevel] = chaLevelUpStat;
            }
        }

        /// <summary>
        /// CharacterSkillFileData를 CSV 파일로부터 로드하고, 내부 딕셔너리에 저장
        /// </summary>
        private void LoadCharacterSkill()
        {
            string[] fields = HandleCSV.LoadFromCsv("Character/CharacterSkill");
            var chaSkillList = HandleCSV.ReadDataFromLines<CharacterSkillFileData>(fields);

            foreach (var chaSkill in chaSkillList)
            {
                _chaIdSkillData[chaSkill.ChaSkillID] = chaSkill;
                _chaEnNameSkillData[chaSkill.ChaSkillEnName] = chaSkill;
            }
        }

        private void LoadCharacterSO()
        {
            foreach (var character in _characterLists)
            {
                _characterIdData[character._chaBaseData.ChaID] = character;
                _characterEnNameData[character._chaBaseData.ChaEnName] = character;
            }
        }

        public void SetBead(CharacterEnName key, int value)
        {
            _beadsInventory[key] = value;
            _firebase.SetBead(_characterEnNameData[key]._chaBaseData.ChaID.ToString(), value);
        }

        public void SetOwnedCharacter(CharacterEnName key, bool value)
        {
            if (_ownedCharacters.ContainsKey(key)) return;

            _ownedCharacters[key] = value;
            _firebase.SetOwnedCharacter(_characterEnNameData[key]._chaBaseData.ChaID.ToString(), value);
        }

        public void SetSelectedTeam(List<CharacterDataSO> selectedTeam, bool updateToFirebase)
        {
            // 기존 팀 정보를 먼저 비웁니다.
            _selectedTeam.Clear();

            // 전달받은 새 팀 정보로 리스트를 채웁니다.
            foreach (var selectedTeamMember in selectedTeam)
            {
                _selectedTeam.Add(selectedTeamMember);
            }

            // 새 팀의 첫 번째 멤버로 맵 캐릭터를 설정합니다. (이벤트 호출 포함)
            if (selectedTeam != null && selectedTeam.Count > 0)
            {
                SetMapPlayerCharacter(selectedTeam[0]);
            }
            else
            {
                // 만약 팀이 비어있다면 null로 설정하거나 기본 캐릭터로 설정할 수 있습니다.
                SetMapPlayerCharacter(null);
            }

            var selectedTeamDic = new Dictionary<string, bool>();

            foreach (var selectedTeamMember in selectedTeam)
            {
                selectedTeamDic[selectedTeamMember._chaBaseData.ChaID.ToString()] = true;
            }

            if (!updateToFirebase) return;
            _firebase.SetSelectedTeam(selectedTeamDic);
        }

        public void AddSelectedTeamMember(CharacterDataSO selectedTeamMember)
        {
            _selectedTeam.Add(selectedTeamMember);
        }

        public void RemoveSelectedTeamMember(CharacterDataSO unselectedTeamMember)
        {
            _selectedTeam.Remove(unselectedTeamMember);
        }

        public void ClearSelectedTeam()
        {
            _selectedTeam.Clear();
        }
        //CJH 코드 추가

        /// <summary>
        /// 맵에 표시될 플레이어 캐릭터 정보를 설정합니다.
        /// 이 함수는 주로 팀 편성이 확정될 때 호출됩니다.
        /// </summary>
        /// <param name="characterData">선택된 팀의 첫 번째 캐릭터 데이터</param>
        private void SetMapPlayerCharacter(CharacterDataSO characterData)
        {
            MapPlayerCharacter = characterData;
            OnFirstCharacterChanged?.Invoke();
        }

        //todo 추후 캐릭터 레벨, 경험치 연동되어야 함
    }
}