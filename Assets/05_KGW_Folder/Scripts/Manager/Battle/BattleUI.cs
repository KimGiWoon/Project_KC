using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SDW;

public class BattleUI : BaseUI
{
    [Header("Battle Manager Reference")]
    [SerializeField] private BattleManager _battleManager;

    [Header("Character UI Setting")]
    [SerializeField] public CharacterInfoSlotUI[] _infoSlot = new CharacterInfoSlotUI[3];

    [Header("Panel UI Reference")]
    [SerializeField] public GameObject _popupUI;
    [SerializeField] public GameObject _bottomUI;

    [Header("Option UI Setting")]
    [SerializeField] private Button _optionButton;
    [SerializeField] private Image _fastButtonBG;
    [SerializeField] private Button _fastButtonX2;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _stageInfo;
    [SerializeField] private TMP_Text _totalHpText;
    [SerializeField] private Slider _totalMonsterHp;

    public WaitForSeconds _playTime;
    public float _count;
    public float _currentTotalHp;
    public float _TotalHp;
    public bool _isOnMenu;
    private float _speed;

    public float _time;
    private bool _isFast;
    private Coroutine _timerRoutine;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;
    public event Action<bool> OnTimeOver;

    private void Awake()
    {
        _isOnMenu = false;
        _isFast = false;
        _panelContainer.SetActive(false);
        _bottomUI.SetActive(false);
        _fastButtonX2.onClick.AddListener(X2FastButtonClick);
        _optionButton.onClick.AddListener(MenuButtonClick);
    }

    protected override void Start()
    {
        base.Start();
        // 게임 결과 확인 이벤트 구독
        _battleManager.OnGameResult += GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구독
        _battleManager.OnTotalHpChange += MonsterTotalHpChange;
        RoguelikeManager.Instance.OnBattleStart += BattleStart;
        RoguelikeManager.Instance.OnBattleEnd += BattleEnd;
    }

    private void OnDisable()
    {
        // 게임 결과 확인 이벤트 구독
        _battleManager.OnGameResult -= GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구독
        _battleManager.OnTotalHpChange -= MonsterTotalHpChange;
        RoguelikeManager.Instance.OnBattleStart -= BattleStart;
        RoguelikeManager.Instance.OnBattleEnd -= BattleEnd;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 게임 결과 확인 이벤트 구독 해제
        _battleManager.OnGameResult -= GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구독 해제
        _battleManager.OnTotalHpChange -= MonsterTotalHpChange;

        _fastButtonX2.onClick.RemoveListener(X2FastButtonClick);
        _optionButton.onClick.RemoveListener(MenuButtonClick);
    }

    public override void Open()
    {
        base.Open();
        _bottomUI.SetActive(true);
    }

    public override void Close()
    {
        base.Close();
        _bottomUI.SetActive(false);
    }

    private void BattleStart()
    {
        _time = GameManager.Instance.BattleMonster.BattleStageTypeRuleTimeDataTable[RoguelikeManager.Instance.MonsterType];
        _battleManager._timer = _time;
        _count = 3f;
        _battleManager.Wall.gameObject.SetActive(false);

        if (CharacterSelectManager.Instance._isFastGame && GameManager.Instance._canFaster)
        {
            _fastButtonBG.gameObject.SetActive(true);
        }
        else
        {
            _fastButtonBG.gameObject.SetActive(false);
        }

        ChangeGameTimer();

        // 타이머 코루틴 시작
        _timerRoutine = StartCoroutine(TimerCoroutine());
    }

    private void BattleEnd()
    {
        StopTimeCoroutine();
        _battleManager.Wall.gameObject.SetActive(false);
        _popupUI.gameObject.SetActive(false);
        OnUIOpenRequested?.Invoke(UIName.StageGlobalUI);
        OnUICloseRequested?.Invoke(UIName.BattleUI);
    }

    // 게임 결과 확인
    public void GamePlayResultCheck(bool result)
    {
        _popupUI.SetActive(true);

        // 게임 클리어
        if (result)
        {
            if (_battleManager.IsLastBoss) OnUIOpenRequested?.Invoke(UIName.ClearChapterUI);
            else OnUIOpenRequested?.Invoke(UIName.ClearStageUI);
        }
        else // 게임 실패
        {
            if (_battleManager._canResurrection)
            {
                if (GameManager.Instance.BuyAdRemover) OnUIOpenRequested?.Invoke(UIName.RemoveADUI);
                else OnUIOpenRequested?.Invoke(UIName.NonRemoveADUI);
            }
            else
            {
                OnUIOpenRequested?.Invoke(UIName.DefeatChapterUI);
            }
        }
    }

    // 몬스터 총합 체력 변화
    public void MonsterTotalHpChange(float totalCurrentHp, float totalMaxHp)
    {
        _totalMonsterHp.minValue = 0f;
        _totalMonsterHp.maxValue = 1f;

        _totalHpText.text = totalCurrentHp.ToString("F0");

        _totalMonsterHp.value = totalCurrentHp / totalMaxHp;
    }

    // 타이머 배속 변경
    private void ChangeGameTimer()
    {
        // 게임 스피드 설정
        _speed = CharacterSelectManager.Instance._isFastGame ? 2f : 1f;

        // 타이머 1배속, 2배속 세팅
        _playTime = new WaitForSeconds(1f / _speed);
    }

    // X2 속도 버튼 클릭
    private void X2FastButtonClick()
    {
        if (_isFast)
        {
            CharacterSelectManager.Instance._isFastGame = false;

            _fastButtonBG.gameObject.SetActive(false);
            _isFast = false;

            ChangeGameTimer();
        }
        else
        {
            CharacterSelectManager.Instance._isFastGame = true;

            _fastButtonBG.gameObject.SetActive(true);
            _isFast = true;

            ChangeGameTimer();
        }
    }

    // 메뉴 버튼 클릭
    private void MenuButtonClick()
    {
        _popupUI.SetActive(true);
        _isOnMenu = true;
        OnUIOpenRequested?.Invoke(UIName.MenuUI);
    }

    // 타이머 코루틴
    private IEnumerator TimerCoroutine()
    {
        // 타이머 UI 출력
        _timerText.text = _time.ToString();

        // 시간이 남아있으면 반복
        while (_time > 0)
        {
            yield return _playTime;

            // 게임이 종료되거나 메뉴창이 오픈되면 타이머 정지
            if (_battleManager._isGameOver)
            {
                // 부활을 하고 다시 죽으면 코루틴 정지
                if (!_battleManager._canResurrection) break;
            }
            else if (_isOnMenu)
                yield return null;
            else
            {
                _time--;
                _count--;

                // 타이머 UI 출력
                _timerText.text = _time.ToString();

                if (_count <= 0f && !_battleManager.Wall.gameObject.activeSelf)
                {
                    _battleManager.Wall.gameObject.SetActive(true);
                }

                // 시간 초과하면 게임 패배
                if (_time <= 0)
                {
                    _popupUI.SetActive(true);

                    Debug.Log("클리어 실패!");
                    _battleManager._isClear = false;
                    _battleManager._isGameOver = true;
                    _battleManager._isTimeOver = true;

                    // 타임오버 시 
                    OnTimeOver?.Invoke(_battleManager._isTimeOver);

                    // 클리어 실패 UI 오픈
                    GamePlayResultCheck(_battleManager._isClear);
                }
                yield return null;
            }
        }
        _timerRoutine = null;
    }

    public void StartTimeCoroutine()
    {
        if (_timerRoutine != null)
        {
            StopCoroutine(_timerRoutine);
            _timerRoutine = null;
        }

        _playTime = new WaitForSeconds(1f / _speed);
        _timerRoutine = StartCoroutine(TimerCoroutine());
    }

    // 타이머 코루틴 정지
    private void StopTimeCoroutine()
    {
        if (_timerRoutine != null)
        {
            StopCoroutine(_timerRoutine);
            _timerRoutine = null;
        }
    }
}