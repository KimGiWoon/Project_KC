using System;
using KSH;
using UnityEngine;

namespace SDW
{
    public partial class UIManager
    {
        #region Open Panel Methods

        /// <summary>
        /// 지정된 UI 패널을 활성화하고 관련 이벤트 핸들러를 연결
        /// </summary>
        /// <param name="uiName">활성화할 패널의 이름</param>
        public void OpenPanel(UIName uiName)
        {
            if (_prevOpenedUI == uiName) return;

            _prevOpenedUI = uiName;

            if (uiName == UIName.GlobalSettingUI)
            {
                ConnectGlobalSettingUI(uiName);
            }
            else
            {
                var sceneName = (SceneName)Enum.Parse(typeof(SceneName), GameManager.Instance.Scene.GetActiveScene());

                switch (sceneName)
                {
                    case SceneName.SDW_SignInScene: OpenSignInScene(uiName); break;
                    case SceneName.KSH_Gacha:
                    case SceneName.SDW_LobbyScene: OpenLobbyScene(uiName); break;
                    case SceneName.SDW_RoguelikeScene: OpenRoguelikeScene(uiName); break;
                }
            }


            if (_prevOpenedUI == _prevClosedUI)
                _prevClosedUI = UIName.None;

            Debug.Log($"Open UI name : {uiName}");
            _uiDic[uiName].Open();
        }

        /// <summary>
        /// SignInScene에 맞는 UI 패널을 초기화하고 연결
        /// </summary>
        /// <param name="uiName">열려는 UI 패널의 이름</param>
        private void OpenSignInScene(UIName uiName)
        {
            switch (uiName)
            {
                //# Signin Scene
                case UIName.SignInUI: ConnectSignInUI(uiName); break;
                case UIName.SetNicknameUI: ConnectNicknameUI(uiName); break;
                case UIName.DownloadUI: ConnectDownloadUI(uiName); break;
            }
        }

        /// <summary>
        /// LobbyScene에 맞는 UI 패널을 초기화하고 연결
        /// </summary>
        /// <param name="uiName">열려는 UI 패널의 이름</param>
        private void OpenLobbyScene(UIName uiName)
        {
            switch (uiName)
            {
                //# Main Lobby Scene
                case UIName.MainLobbyUI: ConnectMainLobbyUI(uiName); break;
                //@ User Info UI
                case UIName.UserInfoUI: ConnectUserInfoUI(uiName); break;
                case UIName.DeleteAccountUI: ConnectDeleteAccountUI(uiName); break;
                case UIName.EditUsernameUI: ConnectEditUsernameUI(uiName); break;
                case UIName.ChangeIconUI: ConnectChangeIconUI(uiName); break;
                //@ Daily Quest UI
                case UIName.DailyQuestUI: ConnectDailyQuestUI(uiName); break;
                //@ Gacha UI
                case UIName.GachaMainUI: ConnectGachaMainUI(uiName); break;
                case UIName.GachaResultUI: ConnectGachaResultUI(uiName); break;
                //@ PermanentGrowth UI
                case UIName.PermanentGrowthUI: ConnectPermanentGrowthUI(uiName); break;
                case UIName.NodeDescriptionUI: ConnectNodeDescriptionUI(uiName); break;
                case UIName.NodeInitializeUI: ConnectNodeInitializeUI(uiName); break;
                //@ LevelUP UI
                case UIName.CharLevelUpMainUI: ConnectCharLevelUpMainUI(uiName); break;
                case UIName.CharInfoStatsUI: ConnectCharInfoStatsUI(uiName); break;
                case UIName.CharInfoBottomUI: ConnectCharInfoBottomUI(uiName); break;
                case UIName.LevelUpUI: ConnectLevelUpUI(uiName); break;
            }
        }

        private void OpenRoguelikeScene(UIName uiName)
        {
            switch (uiName)
            {
                //# Roguelike Scene
                case UIName.StageGlobalUI: ConnectStageGlobalUI(uiName); break;
                case UIName.PopupSettingUI: ConnectPopupSettingUI(uiName); break;
                case UIName.RouteSelectUI: ConnectRouteSelectUI(uiName); break;
                case UIName.PartyUI: ConnectPartyUI(uiName); break;
                case UIName.PartyCharListUI: ConnectPartyCharListUI(uiName); break;
                case UIName.CharInfoUI: ConnectCharInfoUI(uiName); break;
                case UIName.ShoppingUI: ConnectShoppingUI(uiName); break;
                case UIName.InventoryUI: ConnectInventoryUI(uiName); break;
                case UIName.CookingUI: ConnectCookingUI(uiName); break;
                case UIName.FoodDescriptionUI: ConnectFoodDescriptionUI(uiName); break;

                //# Battle Scene
                case UIName.BattleUI: ConnectBattleUI(uiName); break;
                case UIName.MenuUI: ConnectMenuUI(uiName); break;
                case UIName.ClearChapterUI: ConnectClearChapterUI(uiName); break;
                case UIName.ClearStageUI: ConnectClearStageUI(uiName); break;
                case UIName.NonRemoveADUI: ConnectNonRemoveADUI(uiName); break;
                case UIName.RemoveADUI: ConnectRemoveADUI(uiName); break;
                case UIName.DefeatChapterUI: ConnectDefeatChapterUI(uiName); break;
                case UIName.RoguelikeClosingUI: ConnectRoguelikeClosingUI(uiName); break;
            }
        }

        /// <summary>
        /// 지정된 UI 패널을 관리자에 추가
        /// </summary>
        /// <param name="ui">추가할 BaseUI 파생 클래스 인스턴스</param>
        public void AddPanel(BaseUI ui) => _uiDic[ui.Name] = ui;

        #endregion

        #region SignIn Scene UI Connect Methods

        /// <summary>
        /// 지정된 SignInUI 패널을 Firebase와 연결
        /// </summary>
        /// <param name="uiName">연결할 SignInUI 패널의 이름</param>
        private void ConnectSignInUI(UIName uiName)
        {
            var signUI = _uiDic[uiName] as SignInUI;

            if (_firebase != null)
            {
                signUI.OnSignInButtonClicked += _firebase.SignInWithGoogle;
            }
        }

        /// <summary>
        /// 지정된 NickName 패널을 Firebase와 연결
        /// </summary>
        /// <param name="uiName">연결할 NicknameUI 패널의 이름</param>
        private void ConnectNicknameUI(UIName uiName)
        {
            var setNicknameUI = _uiDic[uiName] as SetNicknameUI;

            if (_firebase != null)
            {
                setNicknameUI.OnNicknameChange += _firebase.SetNickname;
            }
        }

        /// <summary>
        /// 지정된 DownLoadUI 패널을 Firebase와 연결
        /// </summary>
        /// <param name="uiName">연결할 DownLoadUI 패널의 이름</param>
        private void ConnectDownloadUI(UIName uiName)
        {
            var downloadUI = _uiDic[uiName] as DownloadUI;
            downloadUI.OnUIOpenRequested += OpenPanel;
            downloadUI.OnUICloseRequested += ClosePanel;
            downloadUI.OnCheckUpdate();
        }

        private void ConnectGlobalSettingUI(UIName uiName)
        {
            var globalSettingUI = _uiDic[uiName] as GlobalSettingUI;
            globalSettingUI.OnUIOpenRequested += OpenPanel;
            globalSettingUI.OnUICloseRequested += ClosePanel;

            var sceneName = (SceneName)Enum.Parse(typeof(SceneName), GameManager.Instance.Scene.GetActiveScene());

            if (sceneName == SceneName.SDW_RoguelikeScene)
            {
                var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;
                stageGlobalUI.ButtonContainerMoveAway();
            }

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo += globalSettingUI.UpdateUserInfo;
                globalSettingUI.OnSignOutButtonClicked += _firebase.SignOut;
            }
        }

        #endregion

        #region Lobby Scene UI Connect Methods

        /// <summary>
        /// MainLobbyUI 연결 및 초기화 수행
        /// </summary>
        /// <param name="uiName">연결할 MainLobbyUI 패널의 이름</param>
        private void ConnectMainLobbyUI(UIName uiName)
        {
            var mainLobbyUI = _uiDic[uiName] as MainLobbyUI;
            var changeIconUI = _uiDic[UIName.ChangeIconUI] as ChangeIconUI;
            var userInfoUI = _uiDic[UIName.UserInfoUI] as UserInfoUI;
            mainLobbyUI.OnUIOpenRequested += OpenPanel;
            mainLobbyUI.OnUICloseRequested += ClosePanel;
            mainLobbyUI.OnIconRequested += (index) =>
            {
                var sprite = changeIconUI.GetIcon(index);
                mainLobbyUI.SetIcon(sprite);
                userInfoUI.SetIcon(sprite);
            };

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo += mainLobbyUI.UpdateUserInfo;
            }
        }

        /// <summary>
        /// UserInfoUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 UserInfoUI 패널의 이름</param>
        private void ConnectUserInfoUI(UIName uiName)
        {
            var userInfoUI = _uiDic[uiName] as UserInfoUI;
            var mainLobbyUI = _uiDic[UIName.MainLobbyUI] as MainLobbyUI;

            userInfoUI.OnUIOpenButtonRequested += OpenPanel;
            userInfoUI.OnUICloseRequested += (uiName) =>
            {
                mainLobbyUI.ResetMainText();
                ClosePanel(uiName);
            };
            userInfoUI.OnIconChanged += mainLobbyUI.SetIcon;

            if (_firebase != null)
                _firebase.OnSendUserInfo += userInfoUI.UpdateUserInfo;
        }

        /// <summary>
        /// DeleteAccountUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 DeleteAccountUI 패널의 이름</param>
        private void ConnectDeleteAccountUI(UIName uiName)
        {
            var deleteAccountUI = _uiDic[uiName] as DeleteAccountUI;
            var globalSettingUI = _uiDic[UIName.GlobalSettingUI] as GlobalSettingUI;

            deleteAccountUI.OnDeleteAcceptButtonClicked += _firebase.DeleteAccount;
            deleteAccountUI.OnDeleteAcceptButtonClicked += globalSettingUI.DeactiveDeleteButton;
            deleteAccountUI.OnCloseButtonClicked += ClosePanel;
        }

        /// <summary>
        /// EditUsernameUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 EditUsernameUI 패널의 이름</param>
        private void ConnectEditUsernameUI(UIName uiName)
        {
            var editUsernameUI = _uiDic[uiName] as EditUsernameUI;
            var userInfoUI = _uiDic[UIName.UserInfoUI] as UserInfoUI;
            editUsernameUI.OnConfirmButtonClicked += _firebase.SetNickname;
            editUsernameUI.OnCloseRequested += (uiName) =>
            {
                userInfoUI.PopUI(uiName);
                ClosePanel(uiName);
            };

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo += editUsernameUI.UpdateUserInfo;
            }
        }

        /// <summary>
        /// ChangeIconUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 ChangeIconUI 패널의 이름</param>
        private void ConnectChangeIconUI(UIName uiName)
        {
            var changeIconUI = _uiDic[uiName] as ChangeIconUI;
            var userInfoUI = _uiDic[UIName.UserInfoUI] as UserInfoUI;

            changeIconUI.OnIconSelected += userInfoUI.SetIcon;
            changeIconUI.OnApplyIconClicked += userInfoUI.SetIconConfirmed;
            changeIconUI.OnApplyIconClicked += () => userInfoUI.PopUI(uiName);
            changeIconUI.OnUICloseRequested += ClosePanel;


            if (_firebase != null)
            {
                changeIconUI.OnIconSelectedIndex += _firebase.SetIconNumber;
            }
        }

        /// <summary>
        /// DailyQuestUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 DailyQuestUI 패널의 이름</param>
        private void ConnectDailyQuestUI(UIName uiName)
        {
            var dailyQuestUI = _uiDic[uiName] as DailyQuestUI;
            var mainLobbyUI = _uiDic[UIName.MainLobbyUI] as MainLobbyUI;

            dailyQuestUI.OnRewardButtonClicked += GameManager.Instance.DailyQuest.Reward;
            dailyQuestUI.OnUICloseRequested += (uiName) =>
            {
                mainLobbyUI.ResetMainText();
                ClosePanel(uiName);
            };
            GameManager.Instance.DailyQuest.AddQuestUI(dailyQuestUI);
        }

        /// <summary>
        /// GachaMainUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 GachaMainUI 패널의 이름</param>
        private void ConnectGachaMainUI(UIName uiName)
        {
            var gachaMainUI = _uiDic[uiName] as GachaMainUI;
            var mainLobbyUI = _uiDic[UIName.MainLobbyUI] as MainLobbyUI;

            gachaMainUI.OnUIOpenRequested += OpenPanel;
            gachaMainUI.OnUICloseRequested += (uiName) =>
            {
                mainLobbyUI.ResetMainText();
                ClosePanel(uiName);
            };
        }

        /// <summary>
        /// GachaResultUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 GachaResultUI 패널의 이름</param>
        private void ConnectGachaResultUI(UIName uiName)
        {
            var gachaResultUI = _uiDic[uiName] as GachaResultUI;
            gachaResultUI.OnUIOpenRequested += OpenPanel;
            gachaResultUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectPermanentGrowthUI(UIName uiName)
        {
            var permanentUI = _uiDic[uiName] as PermanentGrowthUI;
            var nodeDescriptionUI = _uiDic[UIName.NodeDescriptionUI] as NodeDescriptionUI;

            permanentUI.OnUIOpenRequested += (uiName, growthNode) =>
            {
                if (growthNode != null) nodeDescriptionUI.SetDescription(growthNode);
                OpenPanel(uiName);
            };
            permanentUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectNodeDescriptionUI(UIName uiName)
        {
            var nodeDescriptionUI = _uiDic[uiName] as NodeDescriptionUI;

            nodeDescriptionUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectNodeInitializeUI(UIName uiName)
        {
            var nodeInitializeUI = _uiDic[uiName] as NodeInitializeUI;

            nodeInitializeUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectCharLevelUpMainUI(UIName uiName)
        {
            var charLevelUpMainUI = _uiDic[uiName] as CharLevelUpMainUI;
            var charInfoStatsUI = _uiDic[UIName.CharInfoStatsUI] as CharInfoStatsUI;
            var charInfoBottomUI = _uiDic[UIName.CharInfoBottomUI] as CharInfoBottomUI;
            var mainLobbyUI = _uiDic[UIName.MainLobbyUI] as MainLobbyUI;

            charLevelUpMainUI.OnUIOpenRequested += OpenPanel;
            charLevelUpMainUI.OnUICloseRequested += (uiName) =>
            {
                charInfoStatsUI.SetShrink();
                mainLobbyUI.SetButtonsInteractable(false);
                mainLobbyUI.MainLobbyMoveBack();
                ClosePanel(uiName);
            };

            charLevelUpMainUI.OnSubUIOpenRequested += (firstUI, secondUI) =>
            {
                charInfoStatsUI.fromMain = true;
                charInfoBottomUI.fromMain = true;
                OpenPanel(firstUI);
                OpenPanel(secondUI);
            };

            charLevelUpMainUI.OnSubUICloseRequested += (firstUI, secondUI) =>
            {
                charInfoStatsUI.fromMain = true;
                charInfoBottomUI.fromMain = true;
                ClosePanel(firstUI);
                ClosePanel(secondUI);
            };
        }

        private void ConnectCharInfoStatsUI(UIName uiName)
        {
            var charInfoStatsUI = _uiDic[uiName] as CharInfoStatsUI;
            var charLevelUpMainUI = _uiDic[UIName.CharLevelUpMainUI] as CharLevelUpMainUI;
            var charInfoBottomUI = _uiDic[UIName.CharInfoBottomUI] as CharInfoBottomUI;
            var mainLobbyUI = _uiDic[UIName.MainLobbyUI] as MainLobbyUI;

            charInfoStatsUI.OnUIOpenRequested += (uiName) =>
            {
                mainLobbyUI.ButtonsMoveAway();
                charLevelUpMainUI.CharacterMoveAway();
                charInfoBottomUI.BottomMoveAway();
                OpenPanel(uiName);
            };
            charInfoStatsUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectCharInfoBottomUI(UIName uiName)
        {
            // var charInfoBottomUI = _uiDic[uiName] as CharInfoBottomUI;

            //todo Open Close는 필요없을 것 같은데..?
        }

        private void ConnectLevelUpUI(UIName uiName)
        {
            var levelUpUI = _uiDic[uiName] as LevelUpUI;
            var charLevelUpMainUI = _uiDic[UIName.CharLevelUpMainUI] as CharLevelUpMainUI;
            var charInfoBottomUI = _uiDic[UIName.CharInfoBottomUI] as CharInfoBottomUI;
            var mainLobbyUI = _uiDic[UIName.MainLobbyUI] as MainLobbyUI;

            levelUpUI.OnUIOpenRequested += (uiName) =>
            {
                charLevelUpMainUI.CharacterMoveBack();
                charInfoBottomUI.BottomMoveBack();
                OpenPanel(uiName);
            };

            levelUpUI.OnUICloseRequested += (uiName) =>
            {
                mainLobbyUI.ButtonsMoveBack();
                ClosePanel(uiName);
            };
        }

        #endregion

        #region Stage UI Connect Methods

        private void ConnectStageGlobalUI(UIName uiName)
        {
            var stageGlobalUI = _uiDic[uiName] as StageGlobalUI;
            stageGlobalUI.OnUIOpenRequested += OpenPanel;
            stageGlobalUI.OnUICloseRequested += (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void ConnectPopupSettingUI(UIName uiName)
        {
            var popupSettingUI = _uiDic[uiName] as PopupSettingUI;
            popupSettingUI.OnUIOpenRequested += OpenPanel;
            popupSettingUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectRouteSelectUI(UIName uiName)
        {
            var routeSelectUI = _uiDic[uiName] as RouteSelectUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;

            stageGlobalUI.SetPrevUI(uiName);
            routeSelectUI.OnUIOpenRequested += OpenPanel;
            routeSelectUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectPartyUI(UIName uiName)
        {
            var partyUI = _uiDic[uiName] as PartyUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;

            stageGlobalUI.SetPrevUI(uiName);
            partyUI.OnUIOpenRequested += OpenPanel;
            partyUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectPartyCharListUI(UIName uiName)
        {
            var partyCharListUI = _uiDic[uiName] as PartyCharListUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;
            var partyUI = _uiDic[UIName.PartyUI] as PartyUI;
            var charInfoUI = _uiDic[UIName.CharInfoUI] as CharacterInfoUI;

            stageGlobalUI.ButtonContainerMoveAway();
            partyUI.UISecondPositionMoveAway();
            partyCharListUI.OnUIOpenRequested += (ui, characterData) =>
            {
                charInfoUI.SetCharacterInfo(characterData);
                OpenPanel(ui);
            };
            partyCharListUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectCharInfoUI(UIName uiName)
        {
            var charInfoUI = _uiDic[uiName] as CharacterInfoUI;
            charInfoUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectShoppingUI(UIName uiName)
        {
            var shoppingUI = _uiDic[uiName] as ShoppingUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;

            stageGlobalUI.ButtonToBottomMoveAway();
            shoppingUI.OnUICloseRequested += (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void ConnectInventoryUI(UIName uiName)
        {
            var inventoryUI = _uiDic[uiName] as InventoryUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;

            stageGlobalUI.ButtonToBottomMoveAway();
            inventoryUI.OnUICloseRequested += (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void ConnectCookingUI(UIName uiName)
        {
            var cookingUI = _uiDic[uiName] as CookingUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;
            var foodDescription = _uiDic[UIName.FoodDescriptionUI] as FoodDescriptionUI;

            stageGlobalUI.ButtonToBottomMoveAway();
            cookingUI.OnUIOpenRequested += (ui, description) =>
            {
                OpenPanel(ui);
                foodDescription.SetFoodDescription(description);
            };
            cookingUI.OnUICloseRequested += (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void ConnectFoodDescriptionUI(UIName uiName)
        {
            var foodDescriptionUI = _uiDic[uiName] as FoodDescriptionUI;

            foodDescriptionUI.OnUICloseRequrested += ClosePanel;
        }

        /// <summary>
        /// BattleUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 BattleUI 패널의 이름</param>
        private void ConnectBattleUI(UIName uiName)
        {
            var battleUI = _uiDic[uiName] as BattleUI;
            battleUI.OnUIOpenRequested += OpenPanel;
            battleUI.OnUICloseRequested += ClosePanel;
        }

        /// <summary>
        /// MenuUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 MenuUI 패널의 이름</param>
        private void ConnectMenuUI(UIName uiName)
        {
            var menuUI = _uiDic[uiName] as MenuUI;
            menuUI.OnUIOpenRequested += OpenPanel;
            menuUI.OnUICloseRequested += ClosePanel;
        }

        /// <summary>
        /// ClearChapterUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 ClearChapterUI 패널의 이름</param>
        private void ConnectClearChapterUI(UIName uiName)
        {
            var clearChapterUI = _uiDic[uiName] as ClearChapterUI;
            clearChapterUI.OnUIOpenRequested += OpenPanel;
            clearChapterUI.OnUICloseRequested += ClosePanel;
        }

        /// <summary>
        /// ClearStageUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 ClearStageUI 패널의 이름</param>
        private void ConnectClearStageUI(UIName uiName)
        {
            var clearStageUI = _uiDic[uiName] as ClearStageUI;
            clearStageUI.OnUICloseRequested += ClosePanel;
        }

        /// <summary>
        /// NonRemoveADUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 NonRemoveADUI 패널의 이름</param>
        private void ConnectNonRemoveADUI(UIName uiName)
        {
            var nonRemoveADUI = _uiDic[uiName] as NonRemoveADUI;
            nonRemoveADUI.OnUIOpenRequested += OpenPanel;
            nonRemoveADUI.OnUICloseRequested += ClosePanel;
        }

        /// <summary>
        /// RemoveADUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 RemoveADUI 패널의 이름</param>
        private void ConnectRemoveADUI(UIName uiName)
        {
            var removeADUI = _uiDic[uiName] as RemoveADUI;
            removeADUI.OnUIOpenRequested += OpenPanel;
            removeADUI.OnUICloseRequested += ClosePanel;
        }

        /// <summary>
        /// DefeatChapterUI 연결 및 이벤트 핸들러 설정
        /// </summary>
        /// <param name="uiName">연결할 DefeatChapterUI 패널의 이름</param>
        private void ConnectDefeatChapterUI(UIName uiName)
        {
            var defeatChapterUI = _uiDic[uiName] as DefeatChapterUI;
            defeatChapterUI.OnUIOpenRequested += OpenPanel;
            defeatChapterUI.OnUICloseRequested += ClosePanel;
        }

        private void ConnectRoguelikeClosingUI(UIName uiName)
        {
            var roguelikeClosingUI = _uiDic[uiName] as RoguelikeClosingUI;
            roguelikeClosingUI.OnUICloseRequested += ClosePanel;
        }

        #endregion
    }
}