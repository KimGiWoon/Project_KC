using System.Collections.Generic;
using UnityEngine;

namespace SDW
{
    public class UIManager : MonoBehaviour
    {
        private Dictionary<UIName, BaseUI> _uiDic = new Dictionary<UIName, BaseUI>();

        //todo Addressable에서 사용 - 다운로드 관련 UI
        [Header("Loading Panel")]
        private GameObject _loadingPanel;

        private FirebaseManager _firebase;

        private void Awake()
        {
            _firebase = GetComponent<FirebaseManager>();
        }

        private void Start()
        {
            _firebase.ConnectToFirebase();
        }

        #region Panel Methods

        /// <summary>
        /// 지정된 UI 패널을 활성화하고 관련 이벤트 핸들러를 연결
        /// </summary>
        /// <param name="uiName">활성화할 패널의 이름</param>
        public void OpenPanel(UIName uiName)
        {
            _uiDic[uiName].Open();

            switch (uiName)
            {
                case UIName.SignInUI:
                    var signUI = _uiDic[uiName] as SignInUI;
                    signUI.OnSignInButtonClicked += _firebase.SignInWithGoogle;
                    _firebase.OnSignInSetButtonType += signUI.SetButtonImage;

                    //# Test Code
                    _firebase.OnPlayerSigned += ClosePanel;
                    break;
                case UIName.NicknameUI:
                    break;
            }
        }

        /// <summary>
        /// 현재 활성화된 패널을 비활성화하고 관련 이벤트 핸들러에서 해당 메서드를 제거
        /// </summary>
        /// <param name="uiName">비활성화할 패널의 이름</param>
        public void ClosePanel(UIName uiName)
        {
            _uiDic[uiName].Close();

            switch (uiName)
            {
                case UIName.SignInUI:
                    var signUI = _uiDic[uiName] as SignInUI;
                    signUI.OnSignInButtonClicked -= _firebase.SignInWithGoogle;
                    _firebase.OnSignInSetButtonType -= signUI.SetButtonImage;

                    //# Test Code
                    _firebase.OnPlayerSigned -= ClosePanel;
                    break;
                case UIName.NicknameUI:
                    break;
            }
        }

        /// <summary>
        /// 지정된 UI 패널을 관리자에 추가
        /// </summary>
        /// <param name="ui">추가할 BaseUI 파생 클래스 인스턴스</param>
        public void AddPanel(BaseUI ui)
        {
            _uiDic[ui.Name] = ui;

            //# Test Code
            if (ui.Name == UIName.SignInUI)
            {
                OpenPanel(ui.Name);
            }
        }

        /// <summary>
        /// 지정된 UI 패널을 UIManager에서 제거
        /// </summary>
        /// <param name="ui">제거할 BaseUI 파생 클래스 인스턴스</param>
        public void RemovePanel(BaseUI ui) => _uiDic.Remove(ui.Name);

        #endregion
    }
}