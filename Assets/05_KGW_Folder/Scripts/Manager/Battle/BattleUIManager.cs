using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("Battle Manager Reference")]
    [SerializeField] BattleManager _battleManager;
    [SerializeField] GameObject _wall;

    [Header("Character UI Setting")]
    [SerializeField] public CharacterInfoSlotUI[] _infoSlot = new CharacterInfoSlotUI[3];

    [Header("Panel UI Reference")]
    [SerializeField] public GameObject _menuUI;
    [SerializeField] public GameObject _clearChapterUI;
    [SerializeField] public GameObject _clearStageUI;
    [SerializeField] public GameObject _noneRemoveADUI;
    [SerializeField] public GameObject _RemoveADUI;
    [SerializeField] public GameObject _defeatChapterUI;

    [Header("Option UI Setting")]
    [SerializeField] Button _optionButton;
    [SerializeField] Button _fastButtonX1;
    [SerializeField] Button _fastButtonX2;
    [SerializeField] TMP_Text _timerText;
    [SerializeField] TMP_Text _stageInfo;
    [SerializeField] TMP_Text _totalHpText;
    [SerializeField] Slider _totalMonsterHp;

    Coroutine _timerRoutine;
    public WaitForSeconds _playTime;
    float _time;
    float _count;
    public float _currentTotalHp;
    public float _TotalHp;

    private void Awake()
    {
        _fastButtonX1.onClick.AddListener(X1FastButtonClick);
        _fastButtonX2.onClick.AddListener(X2FastButtonClick);
    }

    private void OnEnable()
    {
        // 게임 결과 확인 이벤트 구독
        _battleManager.OnGameResult += GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구독
        _battleManager.OnTotalHpChange += MonsterTotalHpChange;
    }

    private void Start()
    {
        _time = _battleManager._timer;
        _count = 5f;
        _wall.gameObject.SetActive(false);

        if (CharacterSelectManager.Instance._isFastGame)
        {
            _fastButtonX1.gameObject.SetActive(true);
            _fastButtonX2.gameObject.SetActive(false);
        }
        else
        {
            _fastButtonX1 .gameObject.SetActive(false);
            _fastButtonX2.gameObject.SetActive(true);
        }

        ChangeGameTimer();

        // 타이머 코루틴 시작
        _timerRoutine = StartCoroutine(TimerCoroutine());
    }

    private void OnDestroy()
    {
        // 게임 결과 확인 이벤트 구독 해제
        _battleManager.OnGameResult -= GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구독 해제
        _battleManager.OnTotalHpChange -= MonsterTotalHpChange;
    }

    // 게임 결과 확인
    public void GamePlayResultCheck(bool result)
    {
        // 게임 클리어
        if (result)
        {
            if (_clearChapterUI)
            {
                Debug.Log("클리어 UI 오픈");

                // 챕터 클리어 UI 오픈
                _clearChapterUI.SetActive(true);
            }
            // TODO : 김기운 : 추후에 UI매니저에서 관리
            //GameManager.Instance.UI.OpenPanel(UIName.ClearChapterUI);
        }
        else    // 게임 실패
        {
            if (_defeatChapterUI)
            {
                Debug.Log("클리어 실패 UI 오픈");

                // 클리어 실패 UI 오픈
                _defeatChapterUI.SetActive(true);
            }
            // TODO : 김기운 : 추후에 UI매니저에서 관리
            //GameManager.Instance.UI.OpenPanel(UIName.DefeatChapterUI);
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

    }

    // 타이머 코루틴
    private IEnumerator TimerCoroutine()
    {
        // 타이머 UI 출력
        _timerText.text = _time.ToString();

        // 게임 진행중이고 시간이 남아있으면 반복
        while (!_battleManager._isGameOver && _time > 0)
        {
            yield return _playTime;

            _time--;
            _count--;

            // 타이머 UI 출력
            _timerText.text = _time.ToString();

            if (_count <= 0f)
            {
                _wall.gameObject.SetActive(true);
            }
        }

        // 시간 초과하면 게임 패배
        if (_time <= 0)
        {
            // 타이머 코루틴 정지
            StopTimeCoroutine();
        }
    }

    // 타이머 코루틴 정지
    private void StopTimeCoroutine()
    {
        if(_timerRoutine != null)
        {
            StopCoroutine(_timerRoutine);
            _timerRoutine = null;
        }
    }
}
