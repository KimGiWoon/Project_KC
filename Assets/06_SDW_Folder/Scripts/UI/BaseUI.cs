using UnityEngine;

namespace SDW
{
    public class BaseUI : MonoBehaviour
    {
        public UIName Name;
        public GameObject _panelContainer;

        /// <summary>
        /// UI 요소가 활성화될 때 필요한 초기 설정 및 이벤트 연결을 수행
        /// </summary>
        private void Start() => GameManager.Instance.UI.AddPanel(this);

        /// <summary>
        /// UI 요소 해제 시 필요한 리소스 해제 및 참조 제거 수행
        /// </summary>
        private void OnDestroy() => GameManager.Instance.UI.RemovePanel(this);

        /// <summary>
        /// 해당 UI를 활성화
        /// </summary>
        public virtual void Open() => _panelContainer.SetActive(true);

        /// <summary>
        /// 해당 UI를 비활성화
        /// </summary>
        public virtual void Close() => _panelContainer.SetActive(false);
    }
}