using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using UnityEngine;

namespace SDW
{
    public class FirebaseManager : MonoBehaviour
    {
        private FirebaseApp _app;
        public FirebaseApp App => _app;

        private FirebaseAuth _auth;
        public FirebaseAuth Auth => _auth;

        private DatabaseReference _db;
        public DatabaseReference DB => _db;

        private string _checkedEmail;
        private bool _canDelete;
        private Coroutine _coroutine;
        private WaitForSeconds _verificationTime = new WaitForSeconds(2f);

        public Action<ButtonType> OnSignInSetButtonType;
        public Action<UIName> OnPlayerSigned;

        [SerializeField] private FirebaseDataSO _cliendData;
        private string _googleClientId;
        private GoogleSignInConfiguration _googleConfig;

        #region Firebase Intialize Methods

        /// <summary>
        /// 시작 시 필요한 Firebase 관련 초기화 및 설정을 수행
        /// </summary>
        public void ConnectToFirebase() => InitializeFirebaseDependencies();

        /// <summary>
        /// 네이티브 라이브러리 의존성 확인 및 자동 수정
        /// </summary>
        private void InitializeFirebaseDependencies()
        {
            _googleClientId = _cliendData.GoogleClientId;

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var result = task.Result;

                if (result == DependencyStatus.Available)
                {
                    Debug.Log("파이어 베이스 설정이 정상적으로 완료되었습니다.");

                    _app = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _db = FirebaseDatabase.DefaultInstance.RootReference;

                    Debug.Log($"Current User : {_auth.CurrentUser}");

                    if (_auth.CurrentUser != null) OnSignInSetButtonType?.Invoke(ButtonType.ContinueButton);
                    else OnSignInSetButtonType?.Invoke(ButtonType.SignUpButton);

                    InitializeGoogleSignIn();
                }
                else
                {
                    Debug.LogError($"파이어 베이스 설정이 충족되지 않아 실패했습니다 : {result}");
                    _app = null;
                    _auth = null;
                    _db = null;
                }
            });
        }

        #endregion

        #region Google SignIn methods

        /// <summary>
        /// 설정된 Google Sign-In 구성을 초기화하여 웹 클라이언트 ID를 포함하고 이메일 및 ID 토큰 요청을 활성화
        /// </summary>
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

        /// <summary>
        /// Google 계정을 통해 사용자 로그인을 시도하는 메서드
        /// </summary>
        public void SignInWithGoogle()
        {
            GoogleSignIn.Configuration = _googleConfig;
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"로그인에 실패하였습니다 : {task.Exception.Message}");
                    return;
                }

                var result = task.Result;

                if (string.IsNullOrEmpty(result.IdToken))
                {
                    Debug.LogError("Google ID 토큰을 가져오지 못했습니다.");
                    return;
                }

                FirebaseAuthentication(result.IdToken);
            });
        }

        private void FirebaseAuthentication(string googleIdToken)
        {
            var credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

            _auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("로그인 취소");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"로그인에 실패하였습니다 : {task.Exception.Message}");
                    return;
                }

                // OnPopupMessage?.Invoke($"로그인 성공: {user.DisplayName}", PopupState.Success);
                var result = task.Result;
                //todo 기존 유저 데이터가 있는지 확인하여 없으면 가입, 있으면 기존 데이터 읽어오기
                SaveUserData(result.User);
            });
        }

        /// <summary>
        /// Firebase 사용자 데이터를 서버에 저장
        /// </summary>
        /// <param name="user">Firebase에서 인증된 사용자 정보</param>
        private void SaveUserData(FirebaseUser user)
        {
            var userData = new Dictionary<string, object>
            {
                { "uid", user.DisplayName },
                { "email", user.Email },
                { "lastLogin", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") }
            };

            _db.Child("users").Child(user.UserId).SetValueAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("사용자 데이터 저장에 실패했습니다.");
                    return;
                }

                Debug.Log("사용자 데이터가 성공적으로 저장되었습니다.");
                OnPlayerSigned?.Invoke(UIName.SignInUI);
            });
        }

        #endregion
    }
}