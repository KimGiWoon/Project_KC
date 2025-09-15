using System;
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

        public Action<ButtonType> OnSignInSetButtonType;
        public Action<UserInfo> OnSendUserInfo;
        public Action<Dictionary<string, object>> OnCoinDataLoaded;
        public Action<Dictionary<string, object>> OnCharacterDataLoaded;
        public Action<Dictionary<string, object>> OnDailyQuestedDataLoaded;
        public Action<Dictionary<string, object>> OnEtcDataLoaded;

        [SerializeField] private FirebaseDataSO _cliendData;
        private string _googleClientId;
        private GoogleSignInConfiguration _googleConfig;

        private UserData _userData;
        private UIManager _ui;
        private CharacterDataManager _character;
        private Dictionary<string, object> _loadedCharacters;

        #region Firebase Intialize Methods

        /// <summary>
        /// 시작 시 필요한 Firebase 관련 초기화 및 설정을 수행
        /// </summary>
        public void ConnectToFirebase()
        {
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

                    //todo signinUI가 open되기 전에 invoke 되기에 반응이 없음
                    if (PlayerPrefs.GetInt("SignedUp", 0) == 0)
                        OnSignInSetButtonType?.Invoke(ButtonType.SignUpButton);
                    else if (_auth.CurrentUser != null)
                        OnSignInSetButtonType?.Invoke(ButtonType.ContinueButton);
                    else
                        OnSignInSetButtonType?.Invoke(ButtonType.SignInButton);

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
                { "baekRecipeBook", 0 }, //# 1000 경험치 재화
                { "fineDiningRecipeBook", 0 }, //# 5000 경험치 재화
                { "masterChefRecipeBook", 0 }, //# 20000 경험치 재화
                { "point", 9999999 }, //# 영구 성장 포인트
                //todo 추후 0으로 설정
                { "starCandy", 9999999 }, //# 유료 -> 뽑기 재화
                { "shiningStarCandy", 9999999 } //# 유료 재화
            };

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

            foreach (QuestType quest in Enum.GetValues(typeof(QuestType)))
            {
                dailyQuests[quest.ToString()] = false;
            }

            var etcData = new Dictionary<string, object>
            {
                { "score", 0 },
                { "questUpdate", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") },
                { "buyAdRemover", false },
                { "gachaCount", 0 }
            };

            var userData = new Dictionary<string, object>
            {
                { "profile", profileData },
                { "coinData", coinData },
                { "characters", characters },
                { "dailyQuests", dailyQuests },
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

                CheckUserInDatabase(user);
            });
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

            FirebaseDatabase.DefaultInstance.GoOffline();
            FirebaseDatabase.DefaultInstance.GoOnline();

            _db.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"데이터베이스 읽기 실패 : {task.Exception.Message}");
                    return;
                }

                var result = task.Result;

                _ui.ClosePanel(UIName.SignInUI);

                if (result.Exists) LoadUserData(result);
                else SaveUserData(user);
            });
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

                _loadedCharacters = userData["characters"] as Dictionary<string, object>;

                if (userData.ContainsKey("coinData"))
                    OnCoinDataLoaded?.Invoke(userData["coinData"] as Dictionary<string, object>);
                if (userData.ContainsKey("characters"))
                    OnCharacterDataLoaded?.Invoke(userData["characters"] as Dictionary<string, object>);
                if (userData.ContainsKey("dailyQuest"))
                    OnDailyQuestedDataLoaded?.Invoke(userData["dailyQuest"] as Dictionary<string, object>);
                if (userData.ContainsKey("etcData"))
                    OnEtcDataLoaded?.Invoke(userData["etcData"] as Dictionary<string, object>);
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
                { "baekRecipeBook", 0 }, //# 1000 경험치 재화
                { "fineDiningRecipeBook", 0 }, //# 5000 경험치 재화
                { "masterChefRecipeBook", 0 }, //# 20000 경험치 재화
                //todo 추후 0으로 설정
                { "point", 9999999 }, //# 영구 성장 포인트
                { "starCandy", 9999999 }, //# 유료 -> 뽑기 재화
                { "shiningStarCandy", 9999999 } //# 유료 재화
            };

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

            foreach (QuestType quest in Enum.GetValues(typeof(QuestType)))
            {
                dailyQuests[quest.ToString()] = false;
            }

            var etcData = new Dictionary<string, object>
            {
                { "score", 0 },
                { "questUpdate", DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss") },
                { "buyAdRemover", false },
                { "gachaCount", 0 }
            };

            var userData = new Dictionary<string, object>
            {
                { "profile", profileData },
                { "coinData", coinData },
                { "characters", characters },
                { "dailyQuests", dailyQuests },
                { "etcData", etcData }
            };

            _userData = new UserData(
                profileData.ContainsKey("email") ? profileData["email"].ToString() : "",
                profileData.ContainsKey("joinDate") ? profileData["joinDate"].ToString() : "",
                profileData.ContainsKey("nickname") ? profileData["nickname"].ToString() : "",
                profileData.ContainsKey("icon") ? Convert.ToInt32(profileData["icon"]) : 0
            );

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
                _ui.OpenPanel(UIName.SetNicknameUI);
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
                    Debug.LogWarning($"닉네임 저장 실패: {task.Exception.Message}");
                }
            });
        }

        //todo Userdata도 업데이트를 해야 함 - 필수는 아님, CharacterDataManager는 게임 실행 시에만 로드되기 때문
        //@ 변경에 대한 반영을 할지는 고민
        public void SetStarCandy(int starCandy)
        {
            var updateData = new Dictionary<string, object>
            {
                { "coinData/starCandy", starCandy }
            };

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"닉네임 저장 실패: {task.Exception.Message}");
                }
            });
        }

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
                    Debug.LogWarning($"닉네임 저장 실패: {task.Exception.Message}");
                }
            });
        }

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
                    Debug.LogWarning($"닉네임 저장 실패: {task.Exception.Message}");
                }
            });
        }

        public void SetSelectedTeam(Dictionary<string, bool> selectedTeam)
        {
            var updateData = new Dictionary<string, object>();
            var selectedKeyList = new List<string>();

            foreach (var selectedCharacter in selectedTeam)
            {
                selectedKeyList.Add(selectedCharacter.Key);
                var loadedCharacter = _loadedCharacters[selectedCharacter.Key] as Dictionary<string, object>;
                if (Convert.ToBoolean(loadedCharacter["selected"])) continue;

                updateData.Add($"characters/{selectedCharacter.Key}/selected", selectedCharacter.Value);
            }

            foreach (var character in _loadedCharacters)
            {
                if (selectedKeyList.Contains(character.Key)) continue;

                updateData.Add($"characters/{character.Key}/selected", false);
            }

            _db.Child("users").Child(_auth.CurrentUser.UserId).UpdateChildrenAsync(updateData).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"닉네임 저장 실패: {task.Exception.Message}");
                }
            });
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

            _userData = null;

            _ui.ClosePanel(UIName.MainLobbyUI);
            _ui.ClosePanel(UIName.UserInfoUI);

            GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_SignInScene);
        }

        /// <summary>
        /// 사용자의 Firebase 계정과 관련된 데이터를 영구적으로 삭제하고, 모든 연결을 종료
        /// </summary>
        public void DeleteAccount()
        {
            if (_auth.CurrentUser == null)
            {
                Debug.LogWarning("로그인된 사용가자 없습니다.");
                return;
            }

            string userId = _auth.CurrentUser.UserId;

            _db.Child("users").Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"데이터베이스 삭제 실패: {task.Exception.Message}");
                    return;
                }

                _auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(deleteTask =>
                {
                    if (deleteTask.IsFaulted)
                    {
                        Debug.LogWarning($"Firebase 계정 삭제 실패: {deleteTask.Exception.Message}");
                        return;
                    }

                    _userData = null;

#if !UNITY_EDITOR
                    GoogleSignIn.DefaultInstance.SignOut();
#endif

                    PlayerPrefs.SetInt("SignedUp", 0);
                    PlayerPrefs.Save();

                    _ui.ClosePanel(UIName.MainLobbyUI);
                    _ui.ClosePanel(UIName.UserInfoUI);

                    GameManager.Instance.Scene.LoadSceneAsync(SceneName.SDW_SignInScene);
                });
            });
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

        #endregion
    }
}