using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class DeleteAccountUI : BaseUI
    {
        [Header("UI Components")]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _acceptButton;

        public Action<UIName> OnCloseButtonClicked;
        public Action OnDeleteAcceptButtonClicked;

        private Coroutine _coroutine;

        /// <summary>
        /// UI 요소가 활성화 준비를 마치고 초기화 작업을 수행하는 메서드
        /// </summary>
        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 이벤트 연결 수행
        /// </summary>
        private void OnEnable()
        {
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

        /// <summary>
        /// DeleteCancelButtonClicked 핸들러 메서드 호출로 사용자가 취소 버튼을 클릭했을 때 취소 동작
        /// </summary>
        private void DeleteCancelButtonClicked() => OnCloseButtonClicked?.Invoke(UIName.DeleteAccountUI);

        /// <summary>
        /// DeleteAcceptButtonClicked 핸들러 메서드 호출로 사용자가 적용 버튼을 클랙했을 때 적용 동작
        /// </summary>
        private void DeleteAcceptButtonClicked()
        {
            _acceptButton.interactable = false;
            OnDeleteAcceptButtonClicked?.Invoke();
            OnCloseButtonClicked?.Invoke(UIName.DeleteAccountUI);
        }

        private IEnumerator ActiveDeleteButton()
        {
            yield return new WaitForSeconds(3f);
            _acceptButton.interactable = false;
            _coroutine = null;
        }
    }
}