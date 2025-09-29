using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OfflinePopupManager : MonoBehaviour
{
    // TODO GameManager 연결
    [Header("UI")] // 이 오브젝트들을 전역으로 써야하는데 방법을 잘 모르겠음.
    // 이미지, 폰트 다운로드 중 연결이 끊겼을 때 어떻게 보일지.
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText; // 팝업 내 메시지 Text (Text 컴포넌트)
    [SerializeField] private Button retryButton; // 재시도 버튼
    [SerializeField] private Button quitButton; // 종료 버튼

    [Header("Settings")]
    [SerializeField] private bool autoRetryWhenPopupShown = true; // 자동 재연결
    [SerializeField] private float pollIntervalSeconds = 2f; // 네트워크 체크 주기
    [SerializeField] private float autoRetryDelaySeconds = 3f; // 자동 재연결 주기
    [SerializeField] private float disconnectGraceSeconds = 0.5f; // 오프라인 돌입 시간

    private bool popupVisible = false;
    private Coroutine pollingCoroutine;
    private Coroutine autoRetryCoroutine;

    // 기본 메시지
    private const string DEFAULT_MESSAGE = "인터넷 연결이 끊어졌습니다.\n연결을 확인 후 다시 시도해 주세요.";
    private const string CHECKING_MESSAGE = "연결 확인 중...";
    private const string QUITTING_MESSAGE = "앱을 종료합니다.";

    private void OnEnable()
    {
        // 시작시 폴링 시작
        StartPolling();

        retryButton.onClick.AddListener(OnRetryClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDisable()
    {
        StopPolling();
        retryButton.onClick.RemoveListener(OnRetryClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    public void StartPolling(float initialDelay = 0f)
    {
        if (pollingCoroutine != null) StopCoroutine(pollingCoroutine);
        pollingCoroutine = StartCoroutine(PollRoutine(initialDelay));
    }

    public void StopPolling()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
        if (autoRetryCoroutine != null)
        {
            StopCoroutine(autoRetryCoroutine);
            autoRetryCoroutine = null;
        }
    }

    private IEnumerator PollRoutine(float initialDelay)
    {
        if (initialDelay > 0) yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            bool reachable = Application.internetReachability != NetworkReachability.NotReachable;
            if (!reachable)
            {
                // 오프라인 돌입 후 대기
                yield return new WaitForSeconds(disconnectGraceSeconds);

                // 다시 확인
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    ShowPopup(DEFAULT_MESSAGE);
                    break;
                }
            }

            yield return new WaitForSeconds(pollIntervalSeconds);
        }
    }

    private void ShowPopup(string message)
    {
        popupVisible = true;
        popupPanel.SetActive(true);

        popupText.text = message;

        // 자동 재시도 옵션
        if (autoRetryWhenPopupShown)
        {
            if (autoRetryCoroutine != null) StopCoroutine(autoRetryCoroutine);
            autoRetryCoroutine = StartCoroutine(AutoRetryLoop());
        }
    }

    private void HidePopup()
    {
        popupVisible = false;
        popupPanel.SetActive(false);

        if (autoRetryCoroutine != null)
        {
            StopCoroutine(autoRetryCoroutine);
            autoRetryCoroutine = null;
        }

        StartPolling();
    }

    private IEnumerator AutoRetryLoop()
    {
        while (popupVisible)
        {
            yield return new WaitForSeconds(autoRetryDelaySeconds);

            popupText.text = CHECKING_MESSAGE;

            if (CheckReachability())
            {
                HidePopup();
                yield break;
            }
            else
            {
                popupText.text = DEFAULT_MESSAGE;
            }
        }
    }

    private void OnRetryClicked()
    {
        popupText.text = CHECKING_MESSAGE;

        if (CheckReachability())
        {
            HidePopup();
        }
        else
        {
            popupText.text = DEFAULT_MESSAGE;
        }
    }

    private void OnQuitClicked()
    {
        popupText.text = QUITTING_MESSAGE;
        SaveBeforeQuit();

        StartCoroutine(QuitAfterDelay(0.5f));
    }

    private IEnumerator QuitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        QuitApp();
    }

    private bool CheckReachability()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }


    private void SaveBeforeQuit()
    {
        // 예: PlayerPrefs.Save(); 또는 GameManager.Instance.SaveLocalProgress(); 등
        // PlayerPrefs.Save();
        // Debug.Log("OfflinePopupManager: SaveBeforeQuit called (implement real save).");
        // 따로 여기서 저장해야할 정보가 있는가??
    }

    private void QuitApp()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();

#if UNITY_ANDROID
        // Android에서 확실히 모든 Activity를 종료
        try
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finishAffinity");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("finishAffinity failed: " + ex);
        }
#endif
#endif
    }
}
