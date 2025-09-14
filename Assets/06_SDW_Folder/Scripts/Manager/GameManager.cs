using JJY;
using UnityEngine;
using KSH;
using KGW;

namespace SDW
{
    public class GameManager : MonoBehaviour
    {
        //# 싱글턴
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        //# Firebase
        private FirebaseManager _firebase;
        public FirebaseManager Firebase => _firebase;

        //# UI
        private UIManager _ui;
        public UIManager UI => _ui;

        //# Scene
        private MySceneManager _scene;
        public MySceneManager Scene => _scene;

        //# Time - 새벽 5시 초기화
        private TimeManager _time;
        public TimeManager Time => _time;

        //# Daily Quest
        private DailyQuestManager _dailyQuest;
        public DailyQuestManager DailyQuest => _dailyQuest;

        //# Data Table - Character
        private CharacterDataManager _characterData;
        public CharacterDataManager CharacterData => _characterData;

        //# Data Table - Monster
        private MonsterDataManager _monsterData;
        public MonsterDataManager MonsterData => _monsterData;

        //# Data Table - Encounter
        private EncounterDataManager _encounter;
        public EncounterDataManager Encounter => _encounter;

        //# Data Table - Battle Monster
        private BattleMonsterManager _battleMonster;
        public BattleMonsterManager BattleMonster => _battleMonster;

        //# Data Table - Relic
        private RelicDataManager _relic;
        public RelicDataManager Relic => _relic;

        //# Gacha
        private CharacterGacha _gacha;
        public CharacterGacha Gacha => _gacha;

        //# Reward
        private RewardChangeManager _reward;
        public RewardChangeManager Reward => _reward;

        //# Coin
        private CoinManager _coin;
        public CoinManager Coin => _coin;

        //# InGameItem
        private InGameItemManager _inGameItem;
        public InGameItemManager InGameItem => _inGameItem;

        //# Character Battle Data Save
        private CharacterBattleDataSaveManager _characterBattleDataSave;
        public CharacterBattleDataSaveManager CharacterBattleDataSave => _characterBattleDataSave;

        //todo 추후 유료 관련 Manager로 이동해야 함
        [SerializeField] private bool _buyAdRemover;
        public bool BuyAdRemover => _buyAdRemover;

        //# Download 완료 여부 확인 Flag
        private bool _completeDownload;
        public bool CompleteDownload => _completeDownload;

        //# Image Sprite Connected;
        private bool _imageSpriteConnected;
        public bool ImageSpriteConnected => _imageSpriteConnected;

        //# Prefab and SO Connected;
        private bool _prefabAndSoConnected;
        public bool PrefabAndSoConnected => _prefabAndSoConnected;

        //# BattleMonsterManager로 이전?
        private bool _lastBoss;
        public bool LastBoss => _lastBoss;

        //todo 추후 재화 관련 Manager로 이동해야 함
        private int rainbowStarCandy = 999999;
        public int RainbowStarCandy => rainbowStarCandy;

        //todo Gacha에 통함?
        private int _gachaCount;
        public int GachaCount => _gachaCount;

        /// <summary>
        /// Singletone 설정 및 각 Component 연결
        /// </summary>
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(gameObject);

            _firebase = GetComponentInChildren<FirebaseManager>();
            _ui = GetComponentInChildren<UIManager>();
            _scene = GetComponentInChildren<MySceneManager>();
            _time = GetComponentInChildren<TimeManager>();
            _dailyQuest = GetComponentInChildren<DailyQuestManager>();

            //# DataTable
            _characterData = GetComponentInChildren<CharacterDataManager>();
            _monsterData = GetComponentInChildren<MonsterDataManager>();
            _encounter = GetComponentInChildren<EncounterDataManager>();
            _battleMonster = GetComponentInChildren<BattleMonsterManager>();
            _relic = GetComponentInChildren<RelicDataManager>();

            //# Gacha
            _gacha = GetComponentInChildren<CharacterGacha>();
            _reward = GetComponentInChildren<RewardChangeManager>();

            //# Coin & Item
            _coin = GetComponentInChildren<CoinManager>();
            _inGameItem = GetComponentInChildren<InGameItemManager>();

            //# Character Stat Save
            _characterBattleDataSave = GetComponentInChildren<CharacterBattleDataSaveManager>();
        }

        /// <summary>
        /// FrameRate 설정
        /// </summary>
        private void Start()
        {
#if PLATFORM_ANDROID
            Application.targetFrameRate = 60;
#else
            QualitySettings.vSyncCount = 1;
#endif
            FixPortrait();
        }

        /// <summary>
        /// 런타임에 화면 방향을 세로 모드를 강제로 설정하기 위한 메서드
        /// </summary>
        private void FixPortrait()
        {
            Screen.orientation = ScreenOrientation.Portrait;

            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }

        /// <summary>
        /// 스테이지 보스 모드 설정을 변경
        /// </summary>
        /// <param name="isBoss">현재 전투가 보스 모드인지 여부를 나타냄</param>
        public void SetStageBoss(bool isBoss) => _lastBoss = isBoss;

        /// <summary>
        /// 무지개 별 사탕 개수를 설정
        /// </summary>
        /// <param name="number">설정할 무지개 별 사탕의 개수</param>
        public void SetRainbowStarCandy(int number) => rainbowStarCandy = number;

        /// <summary>
        /// 게임 내 가챠 카운트를 지정된 값만큼 증가시킴
        /// </summary>
        /// <param name="number">추가할 가챠 카운트 값</param>
        public void AddGachaCount(int number) => _gachaCount += number;

        /// <summary>
        /// 게임 내 가챠 카운트를 초기화하여 0으로 설정
        /// </summary>
        public void ClearGachaCount() => _gachaCount = 0;

        /// <summary>
        /// 다운로드 완료 상태를 설정
        /// </summary>
        /// <param name="complete">다운로드 완료 여부를 나타내는 bool 값</param>
        public void SetCompleteDownload(bool complete) => _completeDownload = complete;

        /// <summary>
        /// 이미지 스프라이트 연결 상태 설정
        /// </summary>
        /// <param name="connected">이미지 스프라이트 연결 여부</param>
        public void SetImageSpriteConnected(bool connected) => _imageSpriteConnected = connected;

        /// <summary>
        /// 설정된 프리팹과 ScriptableObject (SO) 리소스 연결 상태를 업데이트
        /// </summary>
        /// <param name="connected">프리팹과 SO 리소스 연결 상태 여부</param>
        public void SetPrefabAndSoConnected(bool connected) => _prefabAndSoConnected = connected;
    }
}