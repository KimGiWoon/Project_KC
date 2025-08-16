using UnityEngine;

namespace SDW
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private FirebaseManager _firebase;
        public FirebaseManager Firebase => _firebase;

        private UIManager _ui;
        public UIManager UI => _ui;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(gameObject);

            _firebase = GetComponent<FirebaseManager>();
            _ui = GetComponent<UIManager>();
        }

        private void Start()
        {
        }
    }
}