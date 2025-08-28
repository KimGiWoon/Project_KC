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
    [SerializeField] public GameObject _clearStageUI;
    [SerializeField] public GameObject _noneRemoveADUI;
    [SerializeField] public GameObject _RemoveADUI;

    [Header("Option UI Setting")]
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _fastButtonX1;
    [SerializeField] private Button _fastButtonX2;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _stageInfo;
    [SerializeField] private TMP_Text _totalHpText;
    [SerializeField] private Slider _totalMonsterHp;

    private Coroutine _timerRoutine;
    public WaitForSeconds _playTime;
    private float _time;
    public float _count;
    public float _currentTotalHp;
    public float _TotalHp;
    public bool _isOnMenu;

    public Action<UIName> OnUIOpenRequested;

    private void Awake()
    {
        _isOnMenu = false;
        _panelContainer.SetActive(false);
        _fastButtonX1.onClick.AddListener(X1FastButtonClick);
        _fastButtonX2.onClick.AddListener(X2FastButtonClick);
        _optionButton.onClick.AddListener(MenuButtonClick);
    }

    private void OnEnable()
    {
        // 게임 결과 확인 이벤트 구독
        _battleManager.OnGameResult += GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구독
        _battleManager.OnTotalHpChange += MonsterTotalHpChange;
    }

    protected override void Start()
    {
        base.Start();
        _time = _battleManager._timer;
        _count = 3f;
        _battleManager.Wall.gameObject.SetActive(false);

        if (CharacterSelectManager.Instance._isFastGame)
        {
            _fastButtonX1.gameObject.SetActive(true);
            _fastButtonX2.gameObject.SetActive(false);
        }
        else
        {
            _fastButtonX1.gameObject.SetActive(false);
            _fastButtonX2.gameObject.SetActive(true);
        }

        ChangeGameTimer();

        // 타이머 코루틴 시작
        _timerRoutine = StartCoroutine(TimerCoroutine());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 게임 결과 확인 이벤트 구독 해제
        _battleManager.OnGameResult -= GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구독 해제
        _battleManager.OnTotalHpChange -= MonsterTotalHpChange;

        _fastButtonX1.onClick.RemoveListener(X1FastButtonClick);
        _fastButtonX2.onClick.RemoveListener(X2FastButtonClick);
        _optionButton.onClick.RemoveListener(MenuButtonClick);
    }

    //# Panel Container가 열리지 않게 override
    public override void Open()
    {
    }

    // 게임 결과 확인
    public void GamePlayResultCheck(bool result)
    {
        _panelContainer.SetActive(true);
        // 게임 클리어
        if (result)
        {
            if (_battleManager.IsLastBoss) OnUIOpenRequested?.Invoke(UIName.ClearChapterUI);
            else OnUIOpenRequested?.Invoke(UIName.ClearStageUI);
        }
        else // 게임 실패
        {
            if (GameManager.Instance.BuyAdRemover) OnUIOpenRequested?.Invoke(UIName.RemoveADUI);
            else OnUIOpenRequested?.Invoke(UIName.NonRemoveADUI);
        }
    }

    // 몬스터 총합 체력 변화
    public void MonsterTotalHpChange(float totalCurrnetHp, float totalMaxHp)
    {
        _totalMonsterHp.minValue = 0f;
        _totalMonsterHp.maxValue = 1f;

        _totalHpText.text = totalCurrnetHp.ToString();

        _totalMonsterHp.value = totalCurrnetHp / totalMaxHp;
    }

    // 타이머 배속 변경
    private void ChangeGameTimer()
    {
        // 게임 스피드 설정
        float speed = CharacterSelectManager.Instance._isFastGame ? 2f : 1f;

        // 타이머 1배속, 2배속 세팅
        _playTime = new WaitForSeconds(1f / speed);
    }

    // X1 속도 버튼 클릭
    private void X1FastButtonClick()
    {
        Debug.Log("1배속 진행");
        // 2배속 미진행
        CharacterSelectManager.Instance._isFastGame = false;

        _fastButtonX1.gameObject.SetActive(false);
        _fastButtonX2.gameObject.SetActive(true);

        ChangeGameTimer();
    }

    // X2 속도 버튼 클릭
    private void X2FastButtonClick()
    {
        Debug.Log("2배속 진행");
        // 2배속 진행
        CharacterSelectManager.Instance._isFastGame = true;

        _fastButtonX1.gameObject.SetActive(true);
        _fastButtonX2.gameObject.SetActive(false);

        ChangeGameTimer();
    }

    // 메뉴 버튼 클릭
    private void MenuButtonClick()
    {
        _panelContainer.SetActive(true);
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
                if (!_battleManager._canResurrection) StopTimeCoroutine();
            }
            else if (_isOnMenu)
                yield return null;
            else
            {
                _time--;
                _count--;

                // 타이머 UI 출력
                _timerText.text = _time.ToString();

                if (_count <= 0f)
                {
                    _battleManager.Wall.gameObject.SetActive(true);
                }

                // 시간 초과하면 게임 패배
                if (_time <= 0)
                {
                    // 타이머 코루틴 정지
                    StopTimeCoroutine();

                    _panelContainer.SetActive(true);
                    // 클리어 실패 UI 오픈
                    if (GameManager.Instance.BuyAdRemover) OnUIOpenRequested?.Invoke(UIName.RemoveADUI);
                    else OnUIOpenRequested?.Invoke(UIName.NonRemoveADUI);
                }
            }
        }
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

    //todo 재시작 시 panelContainer를 꺼줘야 함
    //_panelContainer.SetActive(false);
}