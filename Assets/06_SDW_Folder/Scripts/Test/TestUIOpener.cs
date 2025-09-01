using UnityEngine;

namespace SDW
{
    public class TestUIOpener : MonoBehaviour
    {
        [Header("Scene 시작 시 Open할 UI")]
        [SerializeField] private UIName _uiName;
        private bool _isOpened;

        private void Update()
        {
            if (GameManager.Instance == null) return;

            if (_isOpened) return;

            GameManager.Instance.UI.OpenPanel(_uiName);
            _isOpened = true;
        }
    }
}