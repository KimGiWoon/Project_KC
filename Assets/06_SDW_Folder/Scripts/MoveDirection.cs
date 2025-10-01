using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public class MoveDirection : MonoBehaviour
    {
        [SerializeField] private GameObject _straightDiction;
        [SerializeField] private GameObject _leftDiction;
        [SerializeField] private GameObject _rightDiction;

        public void SetButtonFromDirection(ButtonDirection direction)
        {
            switch (direction)
            {
                case ButtonDirection.Stright:
                    _straightDiction.SetActive(true);
                    _leftDiction.SetActive(false);
                    _rightDiction.SetActive(false);
                    break;
                case ButtonDirection.Left:
                    _straightDiction.SetActive(false);
                    _leftDiction.SetActive(true);
                    _rightDiction.SetActive(false);
                    break;
                case ButtonDirection.Right:
                    _straightDiction.SetActive(false);
                    _leftDiction.SetActive(false);
                    _rightDiction.SetActive(true);
                    break;
            }
        }
    }
}