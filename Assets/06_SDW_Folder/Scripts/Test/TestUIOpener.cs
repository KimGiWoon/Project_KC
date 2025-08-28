using UnityEngine;

namespace SDW
{
    public class TestUIOpener : MonoBehaviour
    {
        [Header("Scene 시작 시 Open할 UI")]
        [SerializeField] private UIName _uiName;

        private void Start()
        {
            GameManager.Instance.UI.OpenPanel(_uiName);
        }
    }
}