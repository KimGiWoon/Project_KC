using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("Battle Manager Reference")]
    [SerializeField] BattleManager _battleManager;

    [Header("Panel UI Setting")]
    [SerializeField] GameObject _clearPanel;
    [SerializeField] GameObject _DefeatPanel;

    [Header("Character UI Setting")]
    [SerializeField] public CharacterInfoSlotUI[] _infoSlot = new CharacterInfoSlotUI[3];

    [Header("Option UI Setting")]
    [SerializeField] Button _optionButton;
    [SerializeField] Button _fastButton;
    [SerializeField] TMP_Text _timerText;
    [SerializeField] TMP_Text _stageInfo;
    [SerializeField] Slider _totalMonsterHp;

    MonsterController _monsterController;
    Coroutine _gameResultRoutine;
    Coroutine _timerRoutine;
    float _time;
    public float _currentTotalHp;
    public float _TotalHp;

    private void Awake()
    {
        // 체력 게이지 최소, 최대값 초기화
        if (_totalMonsterHp) { _totalMonsterHp.minValue = 0; _totalMonsterHp.maxValue = 1; }
    }

    private void Start()
    {
        _time = _battleManager._timer;

        _totalMonsterHp.value = _currentTotalHp / _TotalHp;

        // 타이머 코루틴 시작
        _timerRoutine = StartCoroutine(TimerCoroutine());

        // 게임 결과 확인 이벤트 구독
        _battleManager.OnGameResult += GamePlayResultCheck;
    }

    private void OnDestroy()
    {
        // 게임 결과 확인 이벤트 구독 해제
        _battleManager.OnGameResult -= GamePlayResultCheck;
        // 몬스터 통합 체력 변화 이벤트 구족 해제
        _monsterController.OnTotalHpChange -= MonsterTotalHpChange;
    }

    // 몬스터 컨트롤러 가져오기
    public void GetMonsterController(MonsterController monData)
    {
        // 기존에 구독되어 있으면 해제
        if (_monsterController != null)
        {
            _monsterController.OnTotalHpChange -= MonsterTotalHpChange;
        }

        _monsterController = monData;

        // 체력 변화 이벤트 구독
        _monsterController.OnTotalHpChange += MonsterTotalHpChange;
    }

    // 게임 결과 확인
    public void GamePlayResultCheck(bool result)
    {
        // 게임 클리어
        if (result)
        {
            if(_gameResultRoutine != null)
            {
                StopCoroutine(_gameResultRoutine);
                _gameResultRoutine = null;
            }
            else
            {
                // 클리어 코루틴 진행
                _gameResultRoutine = StartCoroutine(ClearPanelCoroutine());
            }
        }
        else    // 게임 실패
        {
            if (_gameResultRoutine != null)
            {
                StopCoroutine(_gameResultRoutine);
                _gameResultRoutine = null;
            }
            else
            {
                // 실패 코루틴 진행
                _gameResultRoutine = StartCoroutine(DefeatPanelCoroutine());
            }
        }
    }

    // 몬스터 총합 체력 변화
    public void MonsterTotalHpChange(float damage)
    {
        _currentTotalHp -= damage;

        _totalMonsterHp.value = _currentTotalHp / _TotalHp;
    }

    // 클리어 패널 코루틴
    private IEnumerator ClearPanelCoroutine()
    {
        _clearPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        // TODO : 김기운 : 추후에 마이씬 매니저 교체 예정
        SceneManager.LoadScene("KGW_TestLobbyScene");

        _clearPanel.SetActive(false);
    }

    // 실패 패널 코루틴
    private IEnumerator DefeatPanelCoroutine()
    {
        _DefeatPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        // TODO : 김기운 : 추후에 마이씬 매니저 교체 예정
        SceneManager.LoadScene("KGW_TestLobbyScene");

        _DefeatPanel.SetActive(false);
    }

    // 타이머 코루틴
    private IEnumerator TimerCoroutine()
    {
        WaitForSeconds time = new WaitForSeconds(1f);
        // 타이머 UI 출력
        _timerText.text = _time.ToString();

        // 게임 진행중이고 시간이 남아있으면 반복
        while (!_battleManager._isGameOver && _time > 0)
        {
            yield return time;
            _time--;

            // 타이머 UI 출력
            _timerText.text = _time.ToString();
        }

        // 시간 초과하면 게임 패배
        if (_time <= 0)
        {
            // 실패 코루틴 진행
            _gameResultRoutine = StartCoroutine(DefeatPanelCoroutine());
        }
    }
}
