using System.Collections;
using UnityEngine;
using System;
using KSH;
using SDW;

// public class TimeManager : SingletonManager<TimeManager>
public class TimeManager : MonoBehaviour
{
    private DateTime nextDailyResetTime; //일일 리셋 시간
    private const string NextDailyResetKey = "NextDailyReset";

    public Action OnDailyReset;
    private GameManager _gameManager;
    private bool _isLoaded;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.Firebase.OnUserInfoUpdated += ClearIsLoaded;
    }

    private void Update()
    {
        if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected || !_gameManager.PrefabAndSoConnected ||
            !_gameManager.Firebase.IsLoaded || _isLoaded) return;

        LoadNextTimeReset(); // 실제 게임용
        StartCoroutine(DailyReset());
        _isLoaded = true;
    }

    private void ScheduleNextDayReset() //다음 리셋 시간 계산
    {
        var nowUtc = DateTime.UtcNow;
        var nowKst = nowUtc.AddHours(9); //UTC+9

        var todayFive = new DateTime(nowKst.Year, nowKst.Month, nowKst.Day, 5, 0, 0);
        nextDailyResetTime = nowKst < todayFive ? todayFive : todayFive.AddDays(1);

        //todo db에 저장해야 함
        //PlayerPrefs에 저장하여 게임을 껐다켜도 유지하게 함
        // PlayerPrefs.SetString("NextDailyReset", nextDailyResetTime.ToBinary().ToString());
        // PlayerPrefs.Save();
        _gameManager.Firebase.SetQuestUpdate(nextDailyResetTime.ToBinary().ToString());
    }

    public void LoadNextTimeReset() //리셋 시간 불러오기
    {
        var etcData = _gameManager.Firebase.EtcData;

        if (!etcData.ContainsKey("questUpdate"))
        {
            ScheduleNextDayReset();
            OnDailyReset?.Invoke();
            return;
        }

        long nextReset = Convert.ToInt64(etcData["questUpdate"]);
        // PlayerPrefs에 키가 저장 되어있고 그 값이 long으로 변환이 가능하다면
        nextDailyResetTime = DateTime.FromBinary(nextReset); //변환된 long값을 DateTime값으로 복원
    }

    private IEnumerator DailyReset()
    {
        while (true)
        {
            var nowKst = DateTime.UtcNow.AddHours(9); //UTC+9

            if (nowKst >= nextDailyResetTime && GameManager.Instance.DailyQuest.IsInitialized)
            {
                Debug.Log("리셋 발생! 현재 시각: " + nowKst);
                OnDailyReset?.Invoke();
                ScheduleNextDayReset();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void ClearIsLoaded() => _isLoaded = false;
}