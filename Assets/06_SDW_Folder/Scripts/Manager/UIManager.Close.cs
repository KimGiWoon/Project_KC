using System;
using KSH;
using UnityEngine;

namespace SDW
{
    public partial class UIManager
    {
        #region Close Panel Methods

        /// <summary>
        /// 현재 활성화된 패널을 비활성화하고 관련 이벤트 핸들러에서 해당 메서드를 제거
        /// </summary>
        /// <param name="uiName">비활성화할 패널의 이름</param>
        public void ClosePanel(UIName uiName)
        {
            if (_prevClosedUI == uiName) return;

            Debug.Log($"Close UI name : {uiName}");
            _uiDic[uiName].Close();

            _prevClosedUI = uiName;

            var sceneName = (SceneName)Enum.Parse(typeof(SceneName), GameManager.Instance.Scene.GetActiveScene());

            switch (sceneName)
            {
                case SceneName.SDW_SignInScene: CloseSignInScene(uiName); break;
                case SceneName.KSH_Gacha:
                case SceneName.SDW_LobbyScene: CloseLobbyScene(uiName); break;
                case SceneName.SDW_RoguelikeScene: CloseRoguelikeScene(uiName); break;
            }


            if (_prevOpenedUI == _prevClosedUI)
                _prevOpenedUI = UIName.None;
        }

        /// <summary>
        /// SignInScene에 맞는 UI 패널을 초기화하고 연결
        /// </summary>
        /// <param name="uiName">닫으려는 UI 패널의 이름</param>
        private void CloseSignInScene(UIName uiName)
        {
            switch (uiName)
            {
                //# Signin Scene
                case UIName.SignInUI: DisconnectSignInUI(uiName); break;
                case UIName.SetNicknameUI: DisconnectNicknameUI(uiName); break;
                case UIName.DownloadUI: DisconnectDownloadUI(uiName); break;
            }
        }

        /// <summary>
        /// LobbyScene에 맞는 UI 패널을 초기화하고 연결
        /// </summary>
        /// <param name="uiName">닫으려는 UI 패널의 이름</param>
        private void CloseLobbyScene(UIName uiName)
        {
            switch (uiName)
            {
                //# Main Lobby Scene
                case UIName.MainLobbyUI: DisconnectMainLobbyUI(uiName); break;
                //@ User Info UI
                case UIName.UserInfoUI: DisconnectUserInfoUI(uiName); break;
                case UIName.DeleteAccountUI: DisconnectDeleteAccountUI(uiName); break;
                case UIName.EditUsernameUI: DisconnectEdiUsernameUI(uiName); break;
                case UIName.ChangeIconUI: DisconnectChangeIconUI(uiName); break;
                //@ Daily Quest UI
                case UIName.DailyQuestUI: DisconnectDailyQuestUI(uiName); break;
                //@ Gacha UI
                case UIName.GachaMainUI: DisconnectGachaMainUI(uiName); break;
                case UIName.GachaResultUI: DisconnectGachaResultUI(uiName); break;
            }
        }

        private void CloseRoguelikeScene(UIName uiName)
        {
            switch (uiName)
            {
                //# Roguelike Scene
                case UIName.StageGlobalUI: DisconnectStageGlobalUI(uiName); break;
                case UIName.PopupSettingUI: DisconnectPopupSettingUI(uiName); break;
                case UIName.RouteSelectUI: DisconnectRouteSelectUI(uiName); break;
                case UIName.PartyUI: DisconnectPartyUI(uiName); break;
                case UIName.PartyCharListUI: DisconnectPartyCharListUI(uiName); break;
                case UIName.CharInfoUI: DisconnectCharInfoUI(uiName); break;
                case UIName.ShoppingUI: DisconnectShoppingUI(uiName); break;
                case UIName.InventoryUI: DisconnectInventoryUI(uiName); break;
                case UIName.CookingUI: DisconnectCookingUI(uiName); break;
                case UIName.FoodDescriptionUI: DisconnectFoodDescriptionUI(uiName); break;

                //# Battle scene
                case UIName.BattleUI: DisconnectBattleUI(uiName); break;
                case UIName.MenuUI: DisconnectMenuUI(uiName); break;
                case UIName.ClearChapterUI: DisconnectClearChapterUI(uiName); break;
                case UIName.ClearStageUI: DisconnectClearStageUI(uiName); break;
                case UIName.NonRemoveADUI: DisconnectNonRemoveADUI(uiName); break;
                case UIName.RemoveADUI: DisconnectRemoveADUI(uiName); break;
                case UIName.DefeatChapterUI: DisconnectDefeatChapterUI(uiName); break;
            }
        }

        /// <summary>
        /// 지정된 UI 패널을 UIManager에서 제거
        /// </summary>
        /// <param name="ui">제거할 BaseUI 파생 클래스 인스턴스</param>
        public void RemovePanel(BaseUI ui) => _uiDic.Remove(ui.Name);

        #endregion

        #region SignIn Scene UI Disconnect Methods

        /// <summary>
        /// SignInUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 DefeatChapterUI 패널의 이름</param>
        private void DisconnectSignInUI(UIName uiName)
        {
            var signUI = _uiDic[uiName] as SignInUI;

            if (_firebase != null)
            {
                signUI.OnSignInButtonClicked -= _firebase.SignInWithGoogle;
            }
        }

        /// <summary>
        /// NicknameUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 NicknameUI 패널의 이름</param>
        private void DisconnectNicknameUI(UIName uiName)
        {
            var setNicknameUI = _uiDic[uiName] as SetNicknameUI;

            if (_firebase != null)
                setNicknameUI.OnNicknameChange -= _firebase.SetNickname;
        }

        /// <summary>
        /// DownloadUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 DownloadUI 패널의 이름</param>
        private void DisconnectDownloadUI(UIName uiName)
        {
            var downloadUI = _uiDic[uiName] as DownloadUI;
            OpenPanel(UIName.SignInUI);
            downloadUI.OnUIOpenRequested -= OpenPanel;
            downloadUI.OnUICloseRequested -= ClosePanel;
        }

        #endregion

        #region Lobby Scene UI Disconnect Methods

        /// <summary>
        /// MainLobbyUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 MainLobbyUI 패널의 이름</param>
        private void DisconnectMainLobbyUI(UIName uiName)
        {
            var mainLobbyUI = _uiDic[uiName] as MainLobbyUI;
            var changeIconUI = _uiDic[UIName.ChangeIconUI] as ChangeIconUI;
            var userInfoUI = _uiDic[UIName.UserInfoUI] as UserInfoUI;
            mainLobbyUI.OnUIOpenRequested -= OpenPanel;
            mainLobbyUI.OnUICloseRequested -= ClosePanel;
            mainLobbyUI.OnIconRequested -= (index) =>
            {
                var sprite = changeIconUI.GetIcon(index);
                mainLobbyUI.SetIcon(sprite);
                userInfoUI.SetIcon(sprite);
            };

            if (_firebase != null)
                _firebase.OnSendUserInfo -= mainLobbyUI.UpdateUserInfo;
        }

        /// <summary>
        /// UserInfoUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 UserInfoUI 패널의 이름</param>
        private void DisconnectUserInfoUI(UIName uiName)
        {
            var userInfoUI = _uiDic[uiName] as UserInfoUI;
            var mainLobbyUI = _uiDic[UIName.MainLobbyUI] as MainLobbyUI;

            userInfoUI.OnUICloseRequested -= ClosePanel;
            userInfoUI.OnUIOpenButtonClicked -= OpenPanel;
            userInfoUI.OnIconChanged -= mainLobbyUI.SetIcon;

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo -= userInfoUI.UpdateUserInfo;
                userInfoUI.OnSignOutButtonClicked -= _firebase.SignOut;
            }
        }

        /// <summary>
        /// DeleteAccountUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 DeleteAccountUI 패널의 이름</param>
        private void DisconnectDeleteAccountUI(UIName uiName)
        {
            var deleteAccountUI = _uiDic[uiName] as DeleteAccountUI;
            var userInfoUI = _uiDic[UIName.UserInfoUI] as UserInfoUI;

            deleteAccountUI.OnDeleteAcceptButtonClicked -= _firebase.DeleteAccount;
            deleteAccountUI.OnDeleteAcceptButtonClicked -= userInfoUI.DeactiveDeleteButton;
            deleteAccountUI.OnCloseButtonClicked -= ClosePanel;
        }

        /// <summary>
        /// EditUsernameUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 EditUsernameUI 패널의 이름</param>
        private void DisconnectEdiUsernameUI(UIName uiName)
        {
            var editUsernameUI = _uiDic[uiName] as EditUsernameUI;
            var userInfoUI = _uiDic[UIName.UserInfoUI] as UserInfoUI;
            editUsernameUI.OnConfirmButtonClicked -= _firebase.SetNickname;
            editUsernameUI.OnConfirmButtonClicked -= (value) => userInfoUI.PopUI(uiName);
            editUsernameUI.OnCloseRequested -= ClosePanel;

            if (_firebase != null)
            {
                _firebase.OnSendUserInfo -= editUsernameUI.UpdateUserInfo;
                _firebase.RequestUserInfo();
            }
        }

        /// <summary>
        /// ChangeIconUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 ChaneIconUI 패널의 이름</param>
        private void DisconnectChangeIconUI(UIName uiName)
        {
            var changeIconUI = _uiDic[uiName] as ChangeIconUI;
            var userInfoUI = _uiDic[UIName.UserInfoUI] as UserInfoUI;

            changeIconUI.OnIconSelected -= userInfoUI.SetIcon;
            changeIconUI.OnApplyIconClicked -= userInfoUI.SetIconConfirmed;
            changeIconUI.OnApplyIconClicked -= () => userInfoUI.PopUI(uiName);
            changeIconUI.OnUICloseRequested -= ClosePanel;

            if (_firebase != null)
            {
                changeIconUI.OnIconSelectedIndex -= _firebase.SetIconNumber;
            }
        }

        // /// <summary>
        // /// KGW_StageUI 연결해제 및 이벤트 핸들러 연결해제 설정
        // /// </summary>
        // /// <param name="uiName">연결해제할 KGW_StageUI 패널의 이름</param>
        // private void DisconnectKGW_StageUI(UIName uiName)
        // {
        //     var kgwStageUI = _uiDic[uiName] as KGW_StageSelectUI;
        //     kgwStageUI.OnUIOpenRequested -= OpenPanel;
        //     kgwStageUI.OnUICloseRequested -= ClosePanel;
        // }
        //
        // /// <summary>
        // /// KGW_CharacterSelectUI 연결해제 및 이벤트 핸들러 연결해제 설정
        // /// </summary>
        // /// <param name="uiName">연결해제할 KGW_CharacterSelectUI 패널의 이름</param>
        // private void DisconnectKGW_CharacterSelectUI(UIName uiName)
        // {
        //     var kgwCharacterSelectUI = _uiDic[uiName] as KGW_CharacterSelectUI;
        //     kgwCharacterSelectUI.OnUIOpenRequested -= OpenPanel;
        //     kgwCharacterSelectUI.OnUICloseRequested -= ClosePanel;
        // }

        /// <summary>
        /// DailyQuestUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 DailyQuestUI 패널의 이름</param>
        private void DisconnectDailyQuestUI(UIName uiName)
        {
            var dailyQuestUI = _uiDic[uiName] as DailyQuestUI;
            // GameManager.Instance.DailyQuest.InitQuest();
            dailyQuestUI.OnRewardButtonClicked -= GameManager.Instance.DailyQuest.Reward;
            dailyQuestUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// GachaMainUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 GachaMainUi 패널의 이름</param>
        private void DisconnectGachaMainUI(UIName uiName)
        {
            var gachaMainUI = _uiDic[uiName] as GachaMainUI;
            gachaMainUI.OnUIOpenRequested -= OpenPanel;
            gachaMainUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// GachaResultUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 GachaResultUI 패널의 이름</param>
        private void DisconnectGachaResultUI(UIName uiName)
        {
            var gachaResultUI = _uiDic[uiName] as GachaResultUI;
            gachaResultUI.OnUIOpenRequested -= OpenPanel;
            gachaResultUI.OnUICloseRequested -= ClosePanel;
        }

        #endregion

        #region Stage UI Disconnect Methods

        private void DisconnectStageGlobalUI(UIName uiName)
        {
            var stageGlobalUI = _uiDic[uiName] as StageGlobalUI;
            stageGlobalUI.OnUIOpenRequested -= OpenPanel;
            stageGlobalUI.OnUICloseRequested -= (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void DisconnectPopupSettingUI(UIName uiName)
        {
            var popupSettingUI = _uiDic[uiName] as PopupSettingUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;

            stageGlobalUI.ButtonContainerMoveBack();
            stageGlobalUI.PushPrevUI();
            popupSettingUI.OnUICloseRequested -= ClosePanel;
        }

        private void DisconnectRouteSelectUI(UIName uiName)
        {
            var routeSelectUI = _uiDic[uiName] as RouteSelectUI;
            routeSelectUI.OnUIOpenRequested -= OpenPanel;
            routeSelectUI.OnUICloseRequested -= ClosePanel;
        }

        private void DisconnectPartyUI(UIName uiName)
        {
            var partyUI = _uiDic[uiName] as PartyUI;
            partyUI.OnUIOpenRequested -= OpenPanel;
            partyUI.OnUICloseRequested -= ClosePanel;
        }

        private void DisconnectPartyCharListUI(UIName uiName)
        {
            var partyCharListUI = _uiDic[uiName] as PartyCharListUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;
            var partyUI = _uiDic[UIName.PartyUI] as PartyUI;
            var charInfoUI = _uiDic[UIName.CharInfoUI] as CharacterInfoUI;

            stageGlobalUI.ButtonContainerMoveBack();
            partyUI.UIMoveBack();
            partyCharListUI.OnUIOpenRequested -= (ui, characterData) =>
            {
                charInfoUI.SetCharacterInfo(characterData);
                OpenPanel(ui);
            };
            partyCharListUI.OnUICloseRequested -= ClosePanel;
        }

        private void DisconnectCharInfoUI(UIName uiName)
        {
            var charInfoUI = _uiDic[uiName] as CharacterInfoUI;
            charInfoUI.OnUICloseRequested -= ClosePanel;
        }

        private void DisconnectShoppingUI(UIName uiName)
        {
            var shoppingUI = _uiDic[uiName] as ShoppingUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;
            var partyUI = _uiDic[UIName.PartyUI] as PartyUI;

            if (!_uiOnly)
            {
                stageGlobalUI.ButtonToMoveBack();
                stageGlobalUI.PushPrevUI();
                partyUI.UIMoveBack();
            }
            shoppingUI.OnUICloseRequested -= (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void DisconnectInventoryUI(UIName uiName)
        {
            var inventoryUI = _uiDic[uiName] as InventoryUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;
            var partyUI = _uiDic[UIName.PartyUI] as PartyUI;

            if (!_uiOnly)
            {
                stageGlobalUI.ButtonToMoveBack();
                stageGlobalUI.PushPrevUI();
                partyUI.UIMoveBack();
            }
            inventoryUI.OnUICloseRequested -= (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void DisconnectCookingUI(UIName uiName)
        {
            var cookingUI = _uiDic[uiName] as CookingUI;
            var stageGlobalUI = _uiDic[UIName.StageGlobalUI] as StageGlobalUI;
            var partyUI = _uiDic[UIName.PartyUI] as PartyUI;
            var foodDescription = _uiDic[UIName.FoodDescriptionUI] as FoodDescriptionUI;

            if (!_uiOnly)
            {
                stageGlobalUI.ButtonToMoveBack();
                stageGlobalUI.PushPrevUI();
                partyUI.UIMoveBack();
            }
            cookingUI.OnUIOpenRequested -= (ui, description) =>
            {
                OpenPanel(ui);
                foodDescription.SetFoodDescription(description);
            };
            cookingUI.OnUICloseRequested -= (ui, uiOnly) =>
            {
                _uiOnly = uiOnly;
                ClosePanel(ui);
            };
        }

        private void DisconnectFoodDescriptionUI(UIName uiName)
        {
            var foodDescriptionUI = _uiDic[uiName] as FoodDescriptionUI;

            foodDescriptionUI.OnUICloseRequrested -= ClosePanel;
        }

        /// <summary>
        /// BattleUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 BattleUI 패널의 이름</param>
        private void DisconnectBattleUI(UIName uiName)
        {
            var battleUI = _uiDic[uiName] as BattleUI;
            battleUI.OnUIOpenRequested -= OpenPanel;
            battleUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// MenuUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 MenuUI 패널의 이름</param>
        private void DisconnectMenuUI(UIName uiName)
        {
            var menuUI = _uiDic[uiName] as MenuUI;
            menuUI.OnUIOpenRequested -= OpenPanel;
            menuUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// ClearChapterUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 ClearChapterUI 패널의 이름</param>
        private void DisconnectClearChapterUI(UIName uiName)
        {
            var clearChapterUI = _uiDic[uiName] as ClearChapterUI;
            clearChapterUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// ClearStageUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 ClearStageUI 패널의 이름</param>
        private void DisconnectClearStageUI(UIName uiName)
        {
            var clearStageUI = _uiDic[uiName] as ClearStageUI;
            clearStageUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// NonRemoveADUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 NonRemoveADUI 패널의 이름</param>
        private void DisconnectNonRemoveADUI(UIName uiName)
        {
            var nonRemoveADUI = _uiDic[uiName] as NonRemoveADUI;
            nonRemoveADUI.OnUIOpenRequested -= OpenPanel;
            nonRemoveADUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// RemoveADUI 연결해제 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 RemoveADUI 패널의 이름</param>
        private void DisconnectRemoveADUI(UIName uiName)
        {
            var removeADUI = _uiDic[uiName] as RemoveADUI;
            removeADUI.OnUIOpenRequested -= OpenPanel;
            removeADUI.OnUICloseRequested -= ClosePanel;
        }

        /// <summary>
        /// DefeatChapter UI 및 이벤트 핸들러 연결해제 설정
        /// </summary>
        /// <param name="uiName">연결해제할 DefeatChapter UI 이름</param>
        private void DisconnectDefeatChapterUI(UIName uiName)
        {
            var defeatChapterUI = _uiDic[uiName] as DefeatChapterUI;
            defeatChapterUI.OnUICloseRequested -= ClosePanel;
        }

        #endregion
    }
}