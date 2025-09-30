using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using KSH;
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

        public Action<UserInfo> OnSendUserInfo;

        [SerializeField] private FirebaseDataSO _cliendData;
        private string _googleClientId;
        private GoogleSignInConfiguration _googleConfig;

        private UIManager _ui;
        private CharacterDataManager _character;

        private UserData _userData;
        public UserData UserData => _userData;

        private Dictionary<string, object> _coinData;
        public IReadOnlyDictionary<string, object> CoinData => _coinData;

        private Dictionary<string, object> _characters;
        public IReadOnlyDictionary<string, object> Characters => _characters;

        private Dictionary<string, object> _dailyQuest;
        public IReadOnlyDictionary<string, object> DailyQuest => _dailyQuest;

        private Dictionary<string, object> _dailyQuestProgress;
        public IReadOnlyDictionary<string, object> DailyQuestProgress => _dailyQuestProgress;

        private Dictionary<string, object> _etcData;
        public IReadOnlyDictionary<string, object> EtcData => _etcData;

        private Dictionary<string, object> _growthData;
        public IReadOnlyDictionary<string, object> GrowthData => _growthData;

        private bool _isLoaded;
        public bool IsLoaded => _isLoaded;

        private ButtonType _buttonType;
        public ButtonType ButtonType => _buttonType;

        public Action OnUserInfoUpdated;

        private const bool _isQA = false;

        #region Firebase Intialize Methods

        /// <summary>
        /// 시작 시 필요한 Firebase 관련 초기화 및 설정을 수행
        /// </summary>
        public void ConnectToFirebase()
        {
            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
            _ui = GameManager.Instance.UI;
            _character = GameManager.Instance.CharacterData;
            InitializeFirebaseDependencies();
        }

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
                    _app = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _db = FirebaseDatabase.DefaultInstance.RootReference;
                    _db.KeepSynced(false);

                    FirebaseDatabase.DefaultInstance.GoOffline();
                    FirebaseDatabase.DefaultInstance.GoOnline();
                    FirebaseDatabase.DefaultInstance.GoOffline();
                    FirebaseDatabase.DefaultInstance.GoOnline();

                    UpdateButtonIcon();

                    InitializeGoogleSignIn();
                }
                else
                {
                    Debug.LogWarning($"파이어 베이스 설정이 충족되지 않아 실패했습니다 : {result}");
                    _app = null;
                    _auth = null;
                    _db = null;
                }
            });
        }
        private void UpdateButtonIcon()
        {
            if (PlayerPrefs.GetInt("SignedUp", 0) == 0)
                _buttonType = ButtonType.SignUpButton;
            else if (_auth.CurrentUser != null)
                _buttonType = ButtonType.ContinueButton;
            else
                _buttonType = ButtonType.SignInButton;
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
#if UNITY_EDITOR
            string email = "team11@test.com";
            string password = "kga1111";

            if (PlayerPrefs.GetInt("SignedUp", 0) == 0)
                SignUp(email, password);
            else if (_auth.CurrentUser != null)
                SignIn(email, password);
            else
                SignIn(email, password);
#else
            GoogleSignIn.Configuration = _googleConfig;
            GoogleSignIn.DefaultInstance.EnableDebugLogging(true);
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"로그인에 실패하였습니다 : {task.Exception.Message}");
                    return;
                }

                var result = task.Result;

                if (string.IsNullOrEmpty(result.IdToken))
                {
                    Debug.LogWarning("Google ID 토큰을 가져오지 못했습니다.");
                    return;
                }

                FirebaseAuthentication(result.IdToken);
            });
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// 사용자 인증을 위한 이메일과 비밀번호로 Firebase에 로그인하는 메서드
        /// </summary>
        /// <param name="email">로그인하려는 사용자의 이메일 주소</param>
        /// <param name="password">로그인하려는 사용자의 비밀번호</param
        private void SignIn(string email, string password)
        {
            _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"로그인에 실패하였습니다 : {task.Exception.Message}");
                    return;
                }

                PlayerPrefs.SetInt("SignedUp", 1);
                PlayerPrefs.Save();

                CheckUserData(email, task.Result.User);
            });
        }

        /// <summary>
        /// 이메일과 비밀번호로 Firebase에 회원가입을 하는 메서드
        /// </summary>
        /// <param name="email">로그인하려는 사용자의 이메일 주소</param>
        /// <param name="password">로그인하려는 사용자의 비밀번호</param>
        private void SignUp(string email, string password)
        {
            _auth.CreateUserWithEmailAndPasswordAsync(email, password)
                .ContinueWithOnMainThread(task =>
                {
                    //# 가입이 실패한 경우
                    if (task.IsFaulted)
                    {
                        Debug.LogWarning($"가입 실패 : {task.Exception.Message}");

                        //# 이미 있는 계정일 경우, 로그인 진행
                        if (task.Exception.Message.Contains("The email address is already in use by another account"))
                        {
                            PlayerPrefs.SetInt("SignedUp", 1);
                            PlayerPrefs.Save();
                            SignIn(email, password);
                        }
                        return;
                    }

                    PlayerPrefs.SetInt("SignedUp", 1);
                    PlayerPrefs.Save();

                    CheckUserData(email, task.Result.User);
                });
        }

        /// <summary>
        /// 사용자 데이터 확인 및 업데이트 로직 수행
        /// </summary>
        /// <param name="email">사용자의 이메일 주소</param>
        /// <param name="user">Auth의 유저 정보</param>
        private void CheckUserData(string email, FirebaseUser user)
        {
            string uid = _auth.CurrentUser.UserId;
            var usernameRef = _db.Child("users").Child(uid);

            usernameRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log($"유저 정보 확인 실패 : {task.Exception?.Message}");
                    return;
                }

                var result = task.Result;

                if (!result.Exists) RegisterEmail(email, uid, user);
                else CheckUserInDatabase(user);
            });
        }

        /// <summary>
        /// Firebase에 등록된 사용자 데이터를 서버에 저장
        /// </summary>
        /// <param name="email">사용자의 이메일 주소</param>
        /// <param name="uid">사용자의 고유 ID</param>
        /// <param name="user">Auth의 유저 정보</param>
        private void RegisterEmail(string email, string uid, FirebaseUser user)
        {
            var profileData = new Dictionary<string, object>
            {
                { "email", email },
                { "joinDate", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") },
                { "nickname", "" },
                { "icon", 0 }
            };

            var coinData = new Dictionary<string, object>
            {
                { "beeksRecipeBook", 0 }, //# 1000 경험치 재화
                { "fineDiningRecipeBook", 0 }, //# 5000 경험치 재화
                { "masterChefRecipeBook", 0 }, //# 20000 경험치 재화
                { "point", 0 }, //# 영구 성장 포인트
                //todo 추후 0으로 설정
                { "starCandy", 0 }, //# 유료 -> 뽑기 재화
                { "shiningStarCandy", 0 }, //# 유료 재화
                { "totalYeopjeon", 0 } //# 총 획득 엽전 재화, 정산 시 사용
            };

            if (_isQA)
            {
                coinData["beeksRecipeBook"] = 999;
                coinData["fineDiningRecipeBook"] = 999;
                coinData["masterChefRecipeBook"] = 999;
                coinData["point"] = 99999;
                coinData["starCandy"] = 99999;
                coinData["shiningStarCandy"] = 99999;
                coinData["totalYeopjeon"] = 99999;
            }

            var characters = new Dictionary<string, object>();

            int selectedTeamCount = 0;
            foreach (var character in _character.CharacterLists)
            {
                var characterData = new Dictionary<string, object>
                {
                    { "owned", false },
                    { "count", 0 },
                    { "level", 1 },
                    { "exp", 0 },
                    { "selected", false }
                };

                if (character._chaBaseData.ChaGrade == CharacterGrade.Normal)
                {
                    characterData["owned"] = true;
                    characterData["count"] = 0;

                    if (selectedTeamCount < 3)
                    {
                        characterData["selected"] = true;
                        selectedTeamCount++;
                    }
                }

                characters[character._chaBaseData.ChaID.ToString()] = characterData;
            }

            Debug.Log($"Number of characters : {characters.Count}");

            var dailyQuests = new Dictionary<string, object>();
            var dailyQuestsProgress = new Dictionary<string, object>();

            foreach (QuestType quest in Enum.GetValues(typeof(QuestType)))
            {
                dailyQuests[quest.ToString()] = false;
                dailyQuestsProgress[quest.ToString()] = 0;
            }

            dailyQuests["GetReward"] = false;

            var etcData = new Dictionary<string, object>
            {
                { "score", 0 },
                { "totalScore", 0 },
                { "questUpdate", 0 },
                { "buyAdRemover", false },
                { "gachaCount", 0 },
                { "chapter", 1 },
                { "stage", 1 },
                { "stamina", 120 },
                { "lastStaminaUpdate", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") },
                { "battleCount", 0 },
                { "relicCount", 0 },
                { "cookCount", 0 },
                { "stageCount", 0 },
                { "canGacha", false },
                { "canFaster", false }
            };

            foreach (string grade in Enum.GetNames(typeof(RelicGrade)))
            {
                etcData.Add($"relic{grade}", 0);
            }

            var userData = new Dictionary<string, object>
            {
                { "profile", profileData },
                { "coinData", coinData },
                { "characters", characters },
                { "dailyQuests", dailyQuests },
                { "dailyQuestsProgress", dailyQuestsProgress },
                { "etcData", etcData }
            };

            _db.Child("users").Child(uid).SetValueAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"이메일 등록 실패 : {task.Exception.Message}");

                    _auth.SignOut();
                    return;
                }

                if (task.IsCanceled)
                {
                    Debug.LogWarning("이메일 등록 취소됨");
                    return;
                }

                StartCoroutine(DelayedCall(user));
            });
        }

        private IEnumerator DelayedCall(FirebaseUser user)
        {
            yield return new WaitForSeconds(1f);
            CheckUserInDatabase(user);
        }
#endif

        /// <summary>
        /// Firebase 사용자 인증을 위한 Google ID 토큰을 Firebase 인증 시스템에 전달
        /// </summary>
        /// <param name="googleIdToken">Google 로그인을 통해 얻은 ID 토큰 문자열</param>
        private void FirebaseAuthentication(string googleIdToken)
        {
            var credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

            _auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogWarning("로그인 취소");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogWarning($"로그인에 실패하였습니다 : {task.Exception.Message}");
                    return;
                }

                var result = task.Result;

                PlayerPrefs.SetInt("SignedUp", 1);
                PlayerPrefs.Save();

                CheckUserInDatabase(result.User);
            });
        }

        /// <summary>
        /// 사용자 정보가 데이터베이스에 존재하는지 확인하고, 존재하면 데이터를 로드하거나 존재하지 않으면 새로운 데이터를 저장하는 메서드
        /// </summary>
        /// <param name="user">Firebase 사용자 정보</param>
        private void CheckUserInDatabase(FirebaseUser user)
        {
            string userId = user.UserId;

            _db.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"데이터베이스 읽기 실패 : {task.Exception.Message}");
                    return;
                }

                var result = task.Result;
                StartCoroutine(WaitForConnect(user, result));

                _ui.OpenPanel(UIName.ImagePrefabLoadingUI);
                _ui.ClosePanel(UIName.SignInUI);
            });
        }

        private IEnumerator WaitForConnect(FirebaseUser user, DataSnapshot result)
        {
            while (true)
            {
                yield return null;
                if (GameManager.Instance.CharacterData.IsInitialized) break;
            }

            if (result.Exists) LoadUserData(result);
            else SaveUserData(user);
        }

        /// <summary>
        /// 사용자의 데이터베이스에서 로드된 데이터를 처리하여 UserData 객체를 초기화
        /// </summary>
        /// <param name="result">사용자 데이터를 포함하는 Firebase DataSnapshot 객체</param>
        private void LoadUserData(DataSnapshot result)
        {
            var userData = result.Value as Dictionary<string, object>;

            if (userData != null)
            {
                if (userData.ContainsKey("profile"))
                {
                    var profileData = userData["profile"] as Dictionary<string, object>;

                    if (profileData != null)
                    {
                        _userData = new UserData(
                            profileData.ContainsKey("email") ? profileData["email"].ToString() : "",
                            profileData.ContainsKey("joinDate") ? profileData["joinDate"].ToString() : "",
                            profileData.ContainsKey("nickname") ? profileData["nickname"].ToString() : "",
                            profileData.ContainsKey("icon") ? Convert.ToInt32(profileData["icon"]) : 0
                        );

                        CheckNicknameRequired();
                    }
                    else Debug.LogWarning("사용자 데이터를 Dictionary로 변환할 수 없습니다");
                }


                _coinData = userData["coinData"] as Dictionary<string, object>;
                _characters = userData["characters"] as Dictionary<string, object>;
                _dailyQuest = userData["dailyQuests"] as Dictionary<string, object>;
                _dailyQuestProgress = userData["dailyQuestsProgress"] as Dictionary<string, object>;
                _etcData = userData["etcData"] as Dictionary<string, object>;
                if (userData.ContainsKey("growthData"))
                    _growthData = userData["growthData"] as Dictionary<string, object>;
                else
                    _growthData = new Dictionary<string, object>();

                OnUserInfoUpdated?.Invoke();
                _isLoaded = true;
            }
        }

        /// <summary>
        /// 최초 회원가입 시 Firebase 사용자 데이터를 서버에 저장
        /// </summary>
        /// <param name="user">Firebase에서 인증된 사용자 정보</param>
        private void SaveUserData(FirebaseUser user)
        {
            var profileData = new Dictionary<string, object>
            {
                { "email", user.Email },
                { "joinDate", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") },
                { "nickname", "" },
                { "icon", 0 }
            };

            var coinData = new Dictionary<string, object>
            {
                { "beeksRecipeBook", 0 }, //# 1000 경험치 재화
                { "fineDiningRecipeBook", 0 }, //# 5000 경험치 재화
                { "masterChefRecipeBook", 0 }, //# 20000 경험치 재화
                //todo 추후 0으로 설정
                { "point", 0 }, //# 영구 성장 포인트
                { "starCandy", 0 }, //# 유료 -> 뽑기 재화
                { "shiningStarCandy", 0 }, //# 유료 재화
                { "totalYeopjeon", 0 } //# 총 획득 엽전 재화, 정산 시 사용
            };
            _coinData = coinData;

            if (_isQA)
            {
                coinData["beeksRecipeBook"] = 999;
                coinData["fineDiningRecipeBook"] = 999;
                coinData["masterChefRecipeBook"] = 999;
                coinData["point"] = 99999;
                coinData["starCandy"] = 99999;
                coinData["shiningStarCandy"] = 99999;
                coinData["totalYeopjeon"] = 99999;
            }

            var characters = new Dictionary<string, object>();

            int selectedTeamCount = 0;
            foreach (var character in _character.CharacterLists)
            {
                var characterData = new Dictionary<string, object>
                {
                    { "owned", false },
                    { "count", 0 },
                    { "level", 1 },
                    { "exp", 0 },
                    { "selected", false }
                };

                if (character._chaBaseData.ChaGrade == CharacterGrade.Normal)
                {
                    characterData["owned"] = true;
                    characterData["count"] = 0;

                    if (selectedTeamCount < 3)
                    {
                        characterData["selected"] = true;
                        selectedTeamCount++;
                    }
                }

                characters[character._chaBaseData.ChaID.ToString()] = characterData;
                _characters = characters;
            }

            Debug.Log($"Number of characters : {characters.Count}");

            var dailyQuests = new Dictionary<string, object>();
            var dailyQuestsProgress = new Dictionary<string, object>();

            foreach (QuestType quest in Enum.GetValues(typeof(QuestType)))
            {
                dailyQuests[quest.ToString()] = false;
                dailyQuestsProgress[quest.ToString()] = 0;
            }

            dailyQuests["GetReward"] = false;

            _dailyQuest = dailyQuests;
            _dailyQuestProgress = dailyQuestsProgress;

            var etcData = new Dictionary<string, object>
            {
                { "score", 0 },
                { "totalScore", 0 },
                { "questUpdate", 0 },
                { "buyAdRemover", false },
                { "gachaCount", 0 },
                { "chapter", 1 },
                { "stage", 1 },
                { "stamina", 120 },
                { "lastStaminaUpdate", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") },
                { "battleCount", 0 },
                { "relicCount", 0 },
                { "cookCount", 0 },
                { "stageCount", 0 },
                { "canGacha", false },
                { "canFaster", false }
            };
            _etcData = etcData;

            var userData = new Dictionary<string, object>
            {
                { "profile", profileData },
                { "coinData", coinData },
                { "characters", characters },
                { "dailyQuests", dailyQuests },
                { "dailyQuestsProgress", dailyQuestsProgress },
                { "etcData", etcData }
            };

            foreach (string grade in Enum.GetNames(typeof(RelicGrade)))
            {
                etcData.Add($"relic{grade}", 0);
            }

            _userData = new UserData(
                profileData.ContainsKey("email") ? profileData["email"].ToString() : "",
                profileData.ContainsKey("joinDate") ? profileData["joinDate"].ToString() : "",
                profileData.ContainsKey("nickname") ? profileData["nickname"].ToString() : "",
                profileData.ContainsKey("icon") ? Convert.ToInt32(profileData["icon"]) : 0
            );

            _growthData = new Dictionary<string, object>();

            OnUserInfoUpdated?.Invoke();
            _isLoaded = true;

            _db.Child("users").Child(user.UserId).SetValueAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning("사용자 데이터 저장에 실패했습니다.");
                    return;
                }

                CheckNicknameRequired();
            });
        }

        /// <summary>
        /// 사용자의 닉네임이 비어있는지 확인하고 필요한 경우 닉네임 설정 패널을 표시하도록 요청하는 메서드
        /// </summary>
        private void CheckNicknameRequired()
        {
            if (string.IsNullOrEmpty(_userData.Nickname))
            {
                _ui.ClosePanel(UIName.ImagePrefabLoadingUI);
                _ui.OpenPanel(UIName.SetNicknameUI);
            }
            else
                OnSignInComplete();
        }

        /// <summary>
        /// 호출이 완료된 후 필요한 후속 작업을 수행하는 메서드
        /// </summary>
        private void OnSignInComplete()
        {
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_LobbyScene);
        }

        #endregion

        #region Sign Out & Delete Methos

        /// <summary>
        /// 사용자의 Firebase 세션을 종료하고 관련 자원을 해제
        /// </summary>
        public void SignOut()
        {
            if (_auth.CurrentUser == null) return;

            _auth.SignOut();

#if !UNITY_EDITOR
            GoogleSignIn.DefaultInstance.SignOut();
#endif
            _coinData = null;
            _characters = null;
            _dailyQuest = null;
            _dailyQuestProgress = null;
            _etcData = null;
            _growthData = null;
            _userData = null;

            _ui.ClosePanel(UIName.MainLobbyUI);
            _ui.ClosePanel(UIName.UserInfoUI);


            UpdateButtonIcon();
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_SignInScene);
        }

        public void DeleteAccount()
        {
            if (_auth.CurrentUser == null)
            {
                Debug.LogWarning("로그인된 사용자가 없습니다.");
                return;
            }

#if !UNITY_EDITOR
            ReauthenticateUser(_auth.CurrentUser);
#else
            Debug.LogWarning("Unity Editor에서는 계정 삭제 테스트 불가 (재인증 필요)");
#endif
        }

        private void ReauthenticateUser(FirebaseUser user)
        {
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(googleTask =>
            {
                if (googleTask.IsCanceled || googleTask.IsFaulted)
                {
                    Debug.LogWarning("구글 로그인 재인증 실패");
                    return;
                }

                var googleUser = googleTask.Result;
                var credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

                user.ReauthenticateAsync(credential).ContinueWithOnMainThread(reAuthTask =>
                {
                    if (reAuthTask.IsFaulted)
                    {
                        Debug.LogWarning($"재인증 실패: {reAuthTask.Exception?.Message}");
                        return;
                    }

                    Debug.Log("재인증 성공");
                    DeleteUserData(user);
                });
            });
        }

        private void DeleteUserData(FirebaseUser user)
        {
            string userId = user.UserId;

            _db.Child("users").Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"데이터베이스 삭제 실패: {task.Exception?.Message}");
                    return;
                }

                Debug.Log("데이터베이스 삭제 성공");
                DeleteFirebaseAccount(user);
            });
        }

        private void DeleteFirebaseAccount(FirebaseUser user)
        {
            user.DeleteAsync().ContinueWithOnMainThread(deleteTask =>
            {
                if (deleteTask.IsFaulted)
                {
                    Debug.LogWarning($"Firebase 계정 삭제 실패: {deleteTask.Exception?.Message}");
                    return;
                }

                Debug.Log("Firebase 계정 삭제 성공");
                CleanupLocalData();
            });
        }

        private void CleanupLocalData()
        {
            _coinData = null;
            _characters = null;
            _dailyQuest = null;
            _dailyQuestProgress = null;
            _etcData = null;
            _growthData = null;
            _userData = null;

#if !UNITY_EDITOR
            GoogleSignIn.DefaultInstance.SignOut();
            GoogleSignIn.DefaultInstance.Disconnect();
#endif

            PlayerPrefs.SetInt("SignedUp", 0);
            PlayerPrefs.Save();

            _ui.ClosePanel(UIName.MainLobbyUI);
            _ui.ClosePanel(UIName.UserInfoUI);

            UpdateButtonIcon();
            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_SignInScene);
            ConnectToFirebase();

            Debug.Log("회원 탈퇴 완료");
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// 유저에 대한 정보를 UI로 전달
        /// </summary>
        public void RequestUserInfo()
        {
            OnSendUserInfo?.Invoke(new UserInfo(
                _userData.Nickname,
                _userData.Email,
                _auth.CurrentUser.UserId,
                _userData.IconNumber
            ));
        }

        public UserInfo GetUserInfo() => new UserInfo(
            _userData.Nickname,
            _userData.Email,
            _auth.CurrentUser.UserId,
            _userData.IconNumber
        );

        #endregion

        #region Update Data

        /// <summary>
        /// 사용자의 닉네임을 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="nickname">업데이트할 사용자의 닉네임 문자열</param>
        public void SetNickname(string nickname)
        {
            _userData.Nickname = nickname;

            var updateData = new Dictionary<string, object>
            {
                { "profile/nickname", nickname }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"닉네임 저장 실패: {task.Exception.Message}");
                    return;
                }


                var activeScene = (SceneName)Enum.Parse(typeof(SceneName), GameManager.Instance.Scene.GetActiveScene());

                switch (activeScene)
                {
                    case SceneName.SDW_SignInScene:
                        _ui.ClosePanel(UIName.SetNicknameUI);
                        OnSignInComplete();
                        break;
                    case SceneName.SDW_LobbyScene:
                        RequestUserInfo();
                        break;
                }
            });
        }

        /// <summary>
        /// 사용자의 아이콘 번호를 설정하고 Firebase 데이터베이스에 저장
        /// </summary>
        /// <param name="iconNumber">설정할 아이콘 번호</param>
        public void SetIconNumber(int iconNumber)
        {
            _userData.IconNumber = iconNumber;

            var updateData = new Dictionary<string, object>
            {
                { "profile/icon", iconNumber }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Icon 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 지정된 별사탕(별 캐시) 값을 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="starCandy">업데이트할 별사탕의 값</param>
        public void SetStarCandy(int starCandy)
        {
            var updateData = new Dictionary<string, object>
            {
                { "coinData/starCandy", starCandy }
            };

            _coinData["starCandy"] = starCandy;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"StarCandy 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 사용자의 빛나는 사탕 개수를 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="shiningStarCandy">업데이트할 빛나는 사탕 개수</param>
        public void SetShiningStarCandy(int shiningStarCandy)
        {
            var updateData = new Dictionary<string, object>
            {
                { "coinData/shiningStarCandy", shiningStarCandy }
            };

            _coinData["shiningStarCandy"] = shiningStarCandy;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"ShiningStarCandy 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetTotalYeopjeon(int yeopjeon)
        {
            var updateData = new Dictionary<string, object>
            {
                { "coinData/totalYeopjeon", yeopjeon }
            };

            _coinData["totalYeopjeon"] = yeopjeon;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"ShiningStarCandy 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetPoint(int point)
        {
            var updateData = new Dictionary<string, object>
            {
                { "coinData/point", point }
            };

            _coinData["point"] = point;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"ShiningStarCandy 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetRecipeItem(string key, int value)
        {
            var updateData = new Dictionary<string, object>
            {
                { $"coinData/{key}", value }
            };

            _coinData[key] = value;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"ShiningStarCandy 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 특정 캐릭터의 경험치를 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="key">업데이트할 캐릭터의 고유 키</param>
        /// <param name="value">경험치</param>
        public void SetExp(string key, int value)
        {
            var updateData = new Dictionary<string, object>
            {
                { $"characters/{key}/exp", value }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Exp 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 특정 캐릭터의 Level을 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="key">업데이트할 캐릭터의 고유 키</param>
        /// <param name="value">Level</param>
        public void SetLevel(string key, int value)
        {
            var updateData = new Dictionary<string, object>
            {
                { $"characters/{key}/level", value }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Level 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 특정 캐릭터의 비드(장식 아이템) 수량을 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="key">업데이트할 캐릭터의 고유 키</param>
        /// <param name="value">비드 수량</param>
        public void SetBead(string key, int value)
        {
            var updateData = new Dictionary<string, object>
            {
                { $"characters/{key}/count", value }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Bead 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 캐릭터의 소유 상태를 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="key">업데이트할 캐릭터의 고유 키</param>
        /// <param name="value">캐릭터 소유 여부</param>
        public void SetOwnedCharacter(string key, bool value)
        {
            var updateData = new Dictionary<string, object>
            {
                { $"characters/{key}/owned", value }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"OwneCharacter 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 설정된 팀을 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="selectedTeam">팀 구성원별 선택 상태를 담은 딕셔너리</param>
        public void SetSelectedTeam(Dictionary<string, bool> selectedTeam)
        {
            var updateData = new Dictionary<string, object>();
            var selectedKeyList = new List<string>();

            foreach (var selectedCharacter in selectedTeam)
            {
                selectedKeyList.Add(selectedCharacter.Key);
                var loadedCharacter = _characters[selectedCharacter.Key] as Dictionary<string, object>;
                if (Convert.ToBoolean(loadedCharacter["selected"])) continue;

                updateData.Add($"characters/{selectedCharacter.Key}/selected", selectedCharacter.Value);
            }

            foreach (var character in _characters)
            {
                if (selectedKeyList.Contains(character.Key)) continue;

                updateData.Add($"characters/{character.Key}/selected", false);
            }

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"SelectedTeam 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 게임 내 점수를 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="score">개별 플레이어의 점수</param>
        public void SetScores(int score)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/score", score }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Score/score 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 게임 내 총 점수(경쟁)를 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="totalScore">전체 총 점수</param>
        public void SetTotalScores(int totalScore)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/totalScore", totalScore }
            };

            _etcData["totalScore"] = totalScore;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Score/TotalScore 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 사용자의 퀘스트 업데이트 정보를 Firebase 데이터베이스에 설정
        /// </summary>
        public void SetQuestUpdate(string nextReset)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/questUpdate", nextReset }
            };

            _etcData["questUpdate"] = nextReset;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Score/TotalScore 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 사용자 인터페이스를 통해 광고 제거 기능의 상태를 토글
        /// </summary>
        /// <param name="value">광고 제거 기능의 활성화 상태</param>
        public void SetBuyAdRemover(bool value)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/buyAdRemover", false }
            };

            _etcData["buyAdRemover"] = false;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"BuyAdRemover 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 지정된 가챠 횟수를 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="gachaCount">업데이트할 가챠 횟수</param>
        public void SetGachaCount(int gachaCount)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/gachaCount", gachaCount }
            };

            _etcData["gachaCount"] = gachaCount;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"GachaCount 저장 실패: {task.Exception.Message}");
                }
            });
        }

        /// <summary>
        /// 지정된 Chapter을 Firebase 데이터베이스에 업데이트
        /// </summary>
        /// <param name="chapter">업데이트할 Chapter 번호</param>
        public void SetChapter(int chapter)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/chapter", chapter }
            };

            _etcData["chapter"] = chapter;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Chapter 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetCanFaster(bool canFaster)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/canFaster", canFaster }
            };

            _etcData["canFaster"] = canFaster;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"CanFaster 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetCanGacha(bool canGacha)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/canGacha", canGacha }
            };

            _etcData["canGacha"] = canGacha;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"CanGacha 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetStage(int stage)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/stage", stage }
            };

            _etcData["stage"] = stage;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"Stage 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetStamina(int stamina)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/stamina", stamina },
                { "etcData/lastStaminaUpdate", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") }
            };

            _etcData["stamina"] = stamina;
            _etcData["lastStaminaUpdate"] = DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss");

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"stamina 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetBattleCount(int battleCount)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/battleCount", battleCount }
            };

            _etcData["battleCount"] = battleCount;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"battleCount 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetRelicCount(int relicCount, Dictionary<RelicGrade, int> relicGradeCount)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/relicCount", relicCount }
            };

            _etcData["relicCount"] = relicCount;

            foreach (var grade in relicGradeCount)
            {
                updateData.Add($"etcData/relic{grade.Key}", grade.Value);
            }

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"relicCount 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetCookCount(int cookCount)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/cookCount", cookCount }
            };

            _etcData["cookCount"] = cookCount;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"cookCount 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetStageCount(int stageCount)
        {
            var updateData = new Dictionary<string, object>
            {
                { "etcData/stageCount", stageCount }
            };

            _etcData["stageCount"] = stageCount;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"stageCount 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetGrowthUnlockData(IReadOnlyList<int> unlockNodes)
        {
            var updateData = new Dictionary<string, object>
            {
                { "growthData/unlockNodes", unlockNodes }
            };

            _growthData["unlockNodes"] = unlockNodes;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"stageCount 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetGrowthCompleteData(IReadOnlyList<int> completeNodes)
        {
            var updateData = new Dictionary<string, object>
            {
                { "growthData/completeNodes", completeNodes }
            };

            _growthData["completeNodes"] = completeNodes;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"stageCount 저장 실패: {task.Exception.Message}");
                }
            });
        }
        public void InitQuest()
        {
            _dailyQuest = new Dictionary<string, object>();
            _dailyQuestProgress = new Dictionary<string, object>();

            foreach (QuestType quest in Enum.GetValues(typeof(QuestType)))
            {
                _dailyQuest[quest.ToString()] = false;
                _dailyQuestProgress[quest.ToString()] = 0;
            }

            _dailyQuest["GetReward"] = false;

            var updateData = new Dictionary<string, object>
            {
                { "dailyQuests", _dailyQuest },
                { "dailyQuestsProgress", _dailyQuestProgress }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"quest 초기화 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetQuestState(QuestType questType, bool isCompleted, int progress)
        {
            var updateData = new Dictionary<string, object>
            {
                { $"dailyQuests/{questType.ToString()}", isCompleted },
                { $"dailyQuestsProgress/{questType.ToString()}", progress }
            };

            _dailyQuest[questType.ToString()] = isCompleted;
            _dailyQuestProgress[questType.ToString()] = progress;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"quest 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetQuestReward(bool getReward)
        {
            var updateData = new Dictionary<string, object>
            {
                { "dailyQuests/GetReward", getReward }
            };

            _dailyQuest["GetReward"] = getReward;

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"quest reward 저장 실패: {task.Exception.Message}");
                }
            });
        }

        #endregion
    }
}