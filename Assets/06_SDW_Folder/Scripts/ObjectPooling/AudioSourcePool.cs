using System.Collections;
using UnityEngine;

namespace SDW
{
    public class AudioSourcePool : MonoBehaviour
    {
        //# AudioSource는 MonoBehaviour를 상속받지 않게, 빈 게임 Object에 AudioSourceController를 추가하여 Prefab화
        [SerializeField] private GameObject _audioSourcePrefab;

        public PoolManager<AudioSourceController> Pool;
        private GameManager _gameManager;
        private static bool _isInitialized;

        private void Start()
        {
            if (_isInitialized) return;
            _gameManager = GameManager.Instance;
            StartCoroutine(LoadCoroutine());
        }

        private IEnumerator LoadCoroutine()
        {
            while (true)
            {
                yield return null;
                if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected ||
                    !_gameManager.PrefabAndSoConnected) continue;

                break;
            }
            var prefab = _audioSourcePrefab.GetComponent<AudioSourceController>();
            Pool = new PoolManager<AudioSourceController>(prefab, 10, 20, transform);
            _isInitialized = true;
        }
    }
}