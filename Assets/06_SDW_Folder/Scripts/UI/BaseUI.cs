using UnityEngine;

namespace SDW
{
    public class BaseUI : MonoBehaviour
    {
        public UIName Name;
        /// <summary>
        /// 해당 UI를 활성화
        /// </summary>
        public virtual void Open() => gameObject.SetActive(true);

        /// <summary>
        /// 해당 UI를 비활성화
        /// </summary>
        public virtual void Close() => gameObject.SetActive(false);
    }
}