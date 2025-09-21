using System.Collections;
using UnityEngine;

namespace SDW
{
    public class AudioSourceController : MonoBehaviour
    {
        private AudioSource _audioSource;

        public AudioSource AudioSource
        {
            get
            {
                if (_audioSource == null)
                    _audioSource = GetComponent<AudioSource>();
                return _audioSource;
            }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayAudio(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            transform.position = position;
            AudioSource.clip = clip;
            AudioSource.volume = volume;
            AudioSource.pitch = pitch;
            AudioSource.Play();

            //# 오디오 재생이 끝나면 자동으로 풀에 반환
            StartCoroutine(ReturnToPoolAfterPlay());
        }

        private IEnumerator ReturnToPoolAfterPlay()
        {
            yield return new WaitWhile(() => AudioSource.isPlaying);
            GameManager.Instance.AudioPool.Pool.Release(this);
        }

        //# 풀에서 나올 때 초기화
        private void OnEnable()
        {
            if (_audioSource != null)
            {
                _audioSource.Stop();
                _audioSource.clip = null;
            }
        }
    }
}