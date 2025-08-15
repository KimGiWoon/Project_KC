using System;
using System.Collections;
using System.Threading.Tasks; // System.Threading.Tasks 추가
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    private FirebaseApp _app;
    private FirebaseAuth _auth;
    private DatabaseReference _db;

    private string _checkedEmail;
    private bool _canDelete;
    private Coroutine _coroutine;
    private WaitForSeconds _verificationTime = new WaitForSeconds(2f);

    public Action OnFirebaseServerConnected;
    public Action OnOpenSignInUI;
    public Action<bool> OnEmailVerification;
    public Action<string> OnCheckNicknameIsSet;
    public Action<string> OnSetNickname;
    public Action OnBackToSignInMenu;
    public Action<string> OnClearEditUI;
    public Action<string, string> OnChangeNicknameOrPassword;
    public Action<string, string, string> OnHandleEmailVerification;
    public Action<string, PopupState> OnPopupMessage;
    public Action OnPopupDeleteUI;

    [SerializeField] private FirebaseDataSO _cliendData;
    private string _googleClientId;
    private GoogleSignInConfiguration _googleConfig;

    [SerializeField] private Button _button;

    #region Unity Event Function

    private void Awake()
    {
        _button.onClick.AddListener(SignInWithGoogle);
    }

    private void Start()
    {
        Debug.Log("디버깅을 시작합니다");

        _googleClientId = _cliendData.GoogleClientId;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("파이어 베이스 설정이 정상적으로 완료되었습니다.");
                _app = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;
                _db = FirebaseDatabase.DefaultInstance.RootReference;
                OnFirebaseServerConnected?.Invoke();

                InitializeGoogleSignIn();
            }
            else
            {
                Debug.Log($"파이어 베이스 설정이 충족되지 않아 실패했습니다 : {dependencyStatus}");
                _app = null;
                _auth = null;
                _db = null;
            }
        });
    }

    #endregion

    #region Google SignIn methods

    private void InitializeGoogleSignIn()
    {
        _googleConfig = new GoogleSignInConfiguration
        {
            WebClientId = _googleClientId,
            RequestIdToken = true,
            RequestEmail = true,
            UseGameSignIn = false
        };
    }

    // Google Sign-In을 시작하는 메서드
    public async void SignInWithGoogle()
    {
        try
        {
            GoogleSignIn.Configuration = _googleConfig;
            var googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            string googleIdToken = googleUser.IdToken;

            if (string.IsNullOrEmpty(googleIdToken))
            {
                Debug.LogError("Google ID 토큰을 가져오지 못했습니다.");
                OnPopupMessage?.Invoke("Google 로그인 실패: ID 토큰이 없습니다.", PopupState.Error);
                return;
            }

            await FirebaseAuthentication(googleIdToken);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Google 로그인 중 오류 발생: {ex.Message}");
            OnPopupMessage?.Invoke($"Google 로그인 실패: {ex.Message}", PopupState.Error);
        }
    }

    private async Task FirebaseAuthentication(string googleIdToken)
    {
        try
        {
            var credential = GoogleAuthProvider.GetCredential(googleIdToken, null); // accessToken은 필요 없음
            var result = await _auth.SignInAndRetrieveDataWithCredentialAsync(credential);

            var user = result.User;
            Debug.Log($"로그인 성공: {user.DisplayName} ({user.UserId})");
            OnPopupMessage?.Invoke($"로그인 성공: {user.DisplayName}", PopupState.Success);

            // 사용자 데이터 저장 (선택 사항)
            await SaveUserData(user);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Firebase 인증 중 오류 발생: {ex.Message}");
            OnPopupMessage?.Invoke($"Firebase 인증 실패: {ex.Message}", PopupState.Error);
        }
    }

    private async Task SaveUserData(FirebaseUser user)
    {
        try
        {
            var userData = new
            {
                displayName = user.DisplayName,
                email = user.Email,
                lastLogin = DateTime.UtcNow.ToString("o")
            };

            string json = JsonUtility.ToJson(userData);
            await _db.Child("users").Child(user.UserId).SetRawJsonValueAsync(json);
            Debug.Log("사용자 데이터가 성공적으로 저장되었습니다.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"사용자 데이터 저장 중 오류 발생: {ex.Message}");
        }
    }

    #endregion
}