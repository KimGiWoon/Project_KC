using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SDW
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClipSO _audioClip;
        [SerializeField] private AudioMixerGroup _masterMixer;
        [SerializeField] private AudioMixerGroup _bgmMixer;
        [SerializeField] private AudioMixerGroup _sfxMixer;
        private AudioSource _bgmAudioSource;
        private List<AudioSource> _sfxAudioSourceList = new List<AudioSource>();

        private bool _isMasterVolumeMuted;
        private bool _isBGMVolumeMuted;
        private bool _isSFXVolumeMuted;

        //# SfxClipName - SfxEntry
        private Dictionary<AudioClipName, AudioEntry> _audioClipNameEntry = new Dictionary<AudioClipName, AudioEntry>();
        public IReadOnlyDictionary<AudioClipName, AudioEntry> AudioClipNameEntry => _audioClipNameEntry;

        //# SfxType - SfxEntry
        private Dictionary<AudioType, List<AudioEntry>> _audioTypeEntryList = new Dictionary<AudioType, List<AudioEntry>>();
        public IReadOnlyDictionary<AudioType, List<AudioEntry>> AudioTypeEntryList => _audioTypeEntryList;

        private GameManager _gameManager;

        private List<int> _volumeList = new List<int>();
        public IReadOnlyList<int> VolumeList => _volumeList;

        private List<bool> _volumeMuteList = new List<bool>();
        public IReadOnlyList<bool> VolumeMuteList => _volumeMuteList;

        private bool _isLoaded;
        public bool IsLoaded => _isLoaded;

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
            SetDefaultVolume();
            _isLoaded = true;
        }

        private void SetDefaultVolume()
        {
            if (!PlayerPrefs.HasKey("MasterVolume"))
            {
                PlayerPrefs.SetInt("MasterVolume", 100);
                PlayerPrefs.Save();
            }

            if (!PlayerPrefs.HasKey("MasterVolumeMute"))
            {
                PlayerPrefs.SetInt("MasterVolumeMute", 0);
                PlayerPrefs.Save();
            }

            if (!PlayerPrefs.HasKey("BGMVolume"))
            {
                PlayerPrefs.SetInt("BGMVolume", 100);
                PlayerPrefs.Save();
            }

            if (!PlayerPrefs.HasKey("BGMVolumeMute"))
            {
                PlayerPrefs.SetInt("BGMVolumeMute", 0);
                PlayerPrefs.Save();
            }

            if (!PlayerPrefs.HasKey("SFXVolume"))
            {
                PlayerPrefs.SetInt("SFXVolume", 100);
                PlayerPrefs.Save();
            }

            if (!PlayerPrefs.HasKey("SFXVolumeMute"))
            {
                PlayerPrefs.SetInt("SFXVolumeMute", 0);
                PlayerPrefs.Save();
            }

            int masterVolume = PlayerPrefs.GetInt("MasterVolume");
            _volumeList.Add(masterVolume);
            SetVolume(VolumeType.MasterVolume, masterVolume);

            int bgmVolume = PlayerPrefs.GetInt("BGMVolume");
            _volumeList.Add(bgmVolume);
            SetVolume(VolumeType.BGMVolume, bgmVolume);

            int sfxVolume = PlayerPrefs.GetInt("SFXVolume");
            _volumeList.Add(sfxVolume);
            SetVolume(VolumeType.SFXVolume, sfxVolume);

            _isMasterVolumeMuted = PlayerPrefs.GetInt("MasterVolumeMute") == 1;
            _volumeMuteList.Add(_isMasterVolumeMuted);
            SetMute(VolumeType.MasterVolume, _isMasterVolumeMuted);

            _isBGMVolumeMuted = PlayerPrefs.GetInt("BGMVolumeMute") == 1;
            _volumeMuteList.Add(_isBGMVolumeMuted);
            SetMute(VolumeType.BGMVolume, _isBGMVolumeMuted);

            _isSFXVolumeMuted = PlayerPrefs.GetInt("SFXVolumeMute") == 1;
            _volumeMuteList.Add(_isSFXVolumeMuted);
            SetMute(VolumeType.SFXVolume, _isSFXVolumeMuted);
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
                _bgmAudioSource.mute = _isBGMVolumeMuted || _isMasterVolumeMuted;
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

                _sfxAudioSourceList.Add(audioController.AudioSource);
                audioController.AudioSource.mute = _isSFXVolumeMuted || _isMasterVolumeMuted;
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

        public void SetVolume(VolumeType volumeType, float volume)
        {
            //# 0-100 범위를 -80 to 0 dB로 변환
            float dbValue = volume > 0 ? Mathf.Log10(volume / 100f) * 20f : -80f;

            switch (volumeType)
            {
                case VolumeType.MasterVolume:
                    _masterMixer.audioMixer.SetFloat("MasterVolume", dbValue);
                    _volumeList[0] = (int)volume;
                    break;
                case VolumeType.BGMVolume:
                    _bgmMixer.audioMixer.SetFloat("BGMVolume", dbValue);
                    _volumeList[1] = (int)volume;
                    break;
                case VolumeType.SFXVolume:
                    _sfxMixer.audioMixer.SetFloat("SFXVolume", dbValue);
                    _volumeList[2] = (int)volume;
                    break;
            }
        }

        public void SetMute(VolumeType volumeType, bool isMute)
        {
            if (volumeType == VolumeType.MasterVolume)
            {
                _isMasterVolumeMuted = isMute;
                BGMMute(isMute);
                SFXMute(isMute);
                _volumeMuteList[0] = isMute;
            }
            else if (volumeType == VolumeType.BGMVolume)
            {
                _isBGMVolumeMuted = isMute;
                BGMMute(isMute);
                _volumeMuteList[1] = isMute;
            }
            else if (volumeType == VolumeType.SFXVolume)
            {
                _isSFXVolumeMuted = isMute;
                SFXMute(isMute);
                _volumeMuteList[2] = isMute;
            }
        }

        private void BGMMute(bool isMute)
        {
            _bgmAudioSource.mute = isMute;
        }

        private void SFXMute(bool isMute)
        {
            foreach (var audio in _sfxAudioSourceList)
            {
                audio.mute = isMute;
            }
        }
    }
}