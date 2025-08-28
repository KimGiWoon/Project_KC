using System.Collections;
using UnityEngine;
using System;
using KSH;

// public class TimeManager : SingletonManager<TimeManager>
public class TimeManager : MonoBehaviour
{
    private DateTime nextDailyResetTime; //일일 리셋 시간
    private const string NextDailyResetKey = "NextDailyReset";

    public event Action OnDailyReset;

    private void Awake()
    {
        LoadNextTimeReset(); // 실제 게임용
        StartCoroutine(DailyReset());
    }

    private void ScheduleNextDayReset() //다음 리셋 시간 계산
    {
        var koreaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
        var nowUtc = DateTime.UtcNow;
        var nowKst = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, koreaTimeZone); //한국 시간으로 변환

        var todayFive = new DateTime(nowKst.Year, nowKst.Month, nowKst.Day, 5, 0, 0);
        nextDailyResetTime = nowKst < todayFive ? todayFive : todayFive.AddDays(1);

        //PlayerPrefs에 저장하여 게임을 껐다켜도 유지하게 함
        PlayerPrefs.SetString("NextDailyReset", nextDailyResetTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void LoadNextTimeReset() //리셋 시간 불러오기
    {
        // PlayerPrefs에 키가 저장 되어있고 그 값이 long으로 변환이 가능하다면
        if (PlayerPrefs.HasKey(NextDailyResetKey) &&
            long.TryParse(PlayerPrefs.GetString(NextDailyResetKey), out long nextReset))
        {
            nextDailyResetTime = DateTime.FromBinary(nextReset); //변환된 long값을 DateTime값으로 복원
        }
        else
        {
            ScheduleNextDayReset();
            return;
        }
    }

    private IEnumerator DailyReset()
    {
        var koreaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");

        while (true)
        {
            var nowKst = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, koreaTimeZone);

            if (nowKst >= nextDailyResetTime)
            {
                Debug.Log("리셋 발생! 현재 시각: " + nowKst);
                OnDailyReset?.Invoke();
                ScheduleNextDayReset();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}