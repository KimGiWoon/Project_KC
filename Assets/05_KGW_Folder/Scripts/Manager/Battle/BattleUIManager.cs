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
    [SerializeField] Button _fastButtonX1;
    [SerializeField] Button _fastButtonX2;
    [SerializeField] TMP_Text _timerText;
    [SerializeField] TMP_Text _stageInfo;
    [SerializeField] TMP_Text _totalHpText;
    [SerializeField] Slider _totalMonsterHp;

    Coroutine _gameResultRoutine;
    Coroutine _timerRoutine;
    float _time;
    public float _currentTotalHp;
    public float _TotalHp;

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
    public void MonsterTotalHpChange(float totalCurrnetHp, float totalMaxHp)
    {
        _totalMonsterHp.minValue = 0f;
        _totalMonsterHp.maxValue = 1f;

        _totalHpText.text = totalCurrnetHp.ToString();

        _totalMonsterHp.value = totalCurrnetHp / totalMaxHp;
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
            // 타이머 코루틴 정지
            StopTimeCoroutine();
        }
    }

    // 타이머 코루틴 정지
    private void StopTimeCoroutine()
    {
        _timerRoutine = null;
        StopCoroutine(_timerRoutine);
    }
}
