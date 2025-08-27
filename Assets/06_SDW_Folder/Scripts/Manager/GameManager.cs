using UnityEngine;

namespace SDW
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private FirebaseManager _firebase;
        public FirebaseManager Firebase => _firebase;

        private UIManager _ui;
        public UIManager UI => _ui;

        private MySceneManager _scene;
        public MySceneManager Scene => _scene;

        private TimeManager _time;
        public TimeManager Time => _time;

        private DailyQuestManager _dailyQuest;
        public DailyQuestManager DailyQuest => _dailyQuest;

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
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }
    }
}