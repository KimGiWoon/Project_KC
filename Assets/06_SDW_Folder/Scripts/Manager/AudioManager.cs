using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SDW
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClipSO _audioClip;
        [SerializeField] private AudioMixerGroup _bgmMixer;
        [SerializeField] private AudioMixerGroup _sfxMixer;
        private AudioSource _bgmAudioSource;

        //# SfxClipName - SfxEntry
        private Dictionary<AudioClipName, AudioEntry> _audioClipNameEntry = new Dictionary<AudioClipName, AudioEntry>();
        public IReadOnlyDictionary<AudioClipName, AudioEntry> AudioClipNameEntry => _audioClipNameEntry;

        //# SfxType - SfxEntry
        private Dictionary<AudioType, List<AudioEntry>> _audioTypeEntryList = new Dictionary<AudioType, List<AudioEntry>>();
        public IReadOnlyDictionary<AudioType, List<AudioEntry>> AudioTypeEntryList => _audioTypeEntryList;

        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;

            StartCoroutine(LoadCoroutine());
        }

        // private void Start() => PlayBGM(AudioClipName.TitleBackground);

        private IEnumerator LoadCoroutine()
        {
            while (true)
            {
                yield return null;
                if (!_gameManager.CompleteDownload || !_gameManager.ImageSpriteConnected ||
                    !_gameManager.PrefabAndSoConnected) continue;

                break;
            }

            InitializeAudioClip();
            InitBGMAudioSource();
        }

        private void InitializeAudioClip()
        {
            if (_audioClip.AudioEntries == null) return;

            foreach (var audioEntry in _audioClip.AudioEntries)
            {
                _audioClipNameEntry.Add(audioEntry.Name, audioEntry);

                if (!_audioTypeEntryList.ContainsKey(audioEntry.Type))
                    _audioTypeEntryList[audioEntry.Type] = new List<AudioEntry>();
                _audioTypeEntryList[audioEntry.Type].Add(audioEntry);
            }
        }

        private void InitBGMAudioSource()
        {
            if (_bgmAudioSource == null)
            {
                _bgmAudioSource = gameObject.AddComponent<AudioSource>();
            }

            //# BGM 설정
            _bgmAudioSource.outputAudioMixerGroup = _bgmMixer;
            _bgmAudioSource.loop = true;
            _bgmAudioSource.playOnAwake = false;

            //# 2D 사운드 - BGM 입체적으로 들릴 필요가 없음
            _bgmAudioSource.spatialBlend = 0f;
        }

        //# BGM 재생
        public void PlayBGM(AudioClipName clipName)
        {
            if (_audioClipNameEntry.TryGetValue(clipName, out var entry))
            {
                _bgmAudioSource.clip = entry.Clip;
                _bgmAudioSource.Play();
            }
        }

        //# SFX 재생 - Pool 사용
        public void PlaySFX(AudioClipName clipName, Vector3 position, bool isUI = false, float volume = 1f, float pitch = 1f)
        {
            if (_audioClipNameEntry.TryGetValue(clipName, out var entry))
            {
                var audioController = _gameManager.AudioPool.Pool.Get();

                // Pool에서 가져온 AudioSource 설정 강제 적용
                if (isUI)
                {
                    audioController.AudioSource.spatialBlend = 0f;
                }
                else
                {
                    audioController.AudioSource.spatialBlend = 1f;
                    audioController.AudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                    audioController.AudioSource.minDistance = 5f;
                    audioController.AudioSource.maxDistance = 30f; // 현재 설정과 맞춤
                }

                audioController.AudioSource.outputAudioMixerGroup = _sfxMixer;
                audioController.PlayAudio(entry.Clip, position, volume, pitch);
            }
        }

        //# UI(Button Click) 재생
        public void Play2DSFX(AudioClipName clipName, float volume = 1f)
        {
            PlaySFX(clipName, Vector3.zero, true, volume);
        }

        public void StopBGM()
        {
            _bgmAudioSource.Stop();
        }
    }
}