using UnityEngine;
using KSH;

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

        //# Gacha
        private CharacterGacha _gacha;
        public CharacterGacha Gacha => _gacha;

        private RewardChangeManager _reward;
        public RewardChangeManager Reward => _reward;

        //todo 추후 유료 관련 Manager로 이동해야 함
        [SerializeField] private bool _buyAdRemover;
        public bool BuyAdRemover => _buyAdRemover;

        //# Download 완료 여부 확인 Flag
        private bool _completeDownload;
        public bool CompleteDownload => _completeDownload;

        //# BattleMonsterManager로 이전?
        private bool _lastBoss;
        public bool LastBoss => _lastBoss;

        //todo 추후 재화 관련 Manager로 이동해야 함
        private int rainbowStarCandy = 999999;
        public int RainbowStarCandy => rainbowStarCandy;

        //todo Gacha에 통함?
        private int _gachaCount;
        public int GachaCount => _gachaCount;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(gameObject);

            _firebase = GetComponent<FirebaseManager>();
            _ui = GetComponent<UIManager>();
            _scene = GetComponent<MySceneManager>();
            _time = GetComponent<TimeManager>();
            _dailyQuest = GetComponent<DailyQuestManager>();

            //# DataTable
            _characterData = GetComponent<CharacterDataManager>();
            _monsterData = GetComponent<MonsterDataManager>();
            _encounter = GetComponent<EncounterDataManager>();
            _battleMonster = GetComponent<BattleMonsterManager>();

            //# Gacha
            _gacha = GetComponent<CharacterGacha>();
            _reward = GetComponent<RewardChangeManager>();
        }

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

        public void SetStageBoss(bool isBoss) => _lastBoss = isBoss;

        public void SetRainbowStarCandy(int number) => rainbowStarCandy = number;

        public void AddGachaCount(int number) => _gachaCount += number;

        public void SetCompleteDownload(bool complete) => _completeDownload = complete;
    }
}