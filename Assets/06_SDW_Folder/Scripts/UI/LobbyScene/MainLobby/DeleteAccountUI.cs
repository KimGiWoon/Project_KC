using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SDW
{
    public class DeleteAccountUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private TweenAlpha_Image _backgroundPanel;
        [SerializeField] private VideoPlayer _videoPlayer;

        public Action<UIName, UIName> OnCloseButtonClicked;
        public Action OnDeleteAcceptButtonClicked;

        private Coroutine _coroutine;

        private void Awake()
        {
            _panelContainer.SetActive(false);

            _cancelButton.onClick.AddListener(DeleteCancelButtonClicked);
            _acceptButton.onClick.AddListener(DeleteAcceptButtonClicked);
        }

        /// <summary>
        /// UI 요소가 비활성화될 때 이벤트 리스너 제거를 수행
        /// </summary>
        private void OnDisable()
        {
            _cancelButton.onClick.RemoveListener(DeleteCancelButtonClicked);
            _acceptButton.onClick.RemoveListener(DeleteAcceptButtonClicked);

            if (_coroutine != null) StopCoroutine(_coroutine);
        }

        public override void Open()
        {
            _backgroundPanel.gameObject.SetActive(true);
            base.Open();
        }

        public override void Close()
        {
            _backgroundPanel.FadeOut();
            StartCoroutine(DelayedClose());
        }

        private IEnumerator DelayedClose()
        {
            yield return new WaitForSeconds(_backgroundPanel.TweenTime);
            base.Close();
        }

        /// <summary>
        /// DeleteCancelButtonClicked 핸들러 메서드 호출로 사용자가 취소 버튼을 클릭했을 때 취소 동작
        /// </summary>
        private void DeleteCancelButtonClicked() => OnCloseButtonClicked?.Invoke(UIName.DeleteAccountUI, UIName.None);

        /// <summary>
        /// DeleteAcceptButtonClicked 핸들러 메서드 호출로 사용자가 적용 버튼을 클랙했을 때 적용 동작
        /// </summary>
        private void DeleteAcceptButtonClicked()
        {
            _acceptButton.interactable = false;
            _coroutine = StartCoroutine(ActiveDeleteButton());

            _videoPlayer.Stop();
            GameManager.Instance.Video.UnloadAllVideos();
            OnDeleteAcceptButtonClicked?.Invoke();
            OnCloseButtonClicked?.Invoke(UIName.DeleteAccountUI, UIName.LobbySettingUI);
        }

        private IEnumerator ActiveDeleteButton()
        {
            yield return new WaitForSeconds(3f);
            _acceptButton.interactable = true;
            _coroutine = null;
        }
    }
}