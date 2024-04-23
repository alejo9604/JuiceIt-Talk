using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class AudioManager : MonoBehaviour
    {
        public enum AudioChannel { Master, Sfx, Music };
        
        public static AudioManager Instance;
        
        [SerializeField] private AudioLibrary _library;
        
        private AudioSource[] _musicSources;
        private int _activeMusicSourceIndex;
        
        private float _masterVolumePercent;
        private float _sfxVolumePercent;
        private float _musicVolumePercent;

        private const string MASTER_VOL_KEY = "master_vol";
        private const string MUSIC_VOL_KEY = "music_vol";
        private const string SFX_VOL_KEY = "sfx_vol";

        private IObjectPool<AudioSource> _sfxPool;
        private List<AudioSource> _activeSFX = new();

        private Coroutine _coroutine;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            
            _musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject($"Music_Source_{i + 1}");
                _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            
            _sfxPool = new ObjectPool<AudioSource>(
                () =>
                {
                    GameObject sfxSource = new GameObject("SFX_Source");
                    sfxSource.transform.parent = transform;
                    AudioSource source = sfxSource.AddComponent<AudioSource>();
                    source.playOnAwake = false;
                    return source;
                },
                source => source.gameObject.SetActive(true),
                source => source.gameObject.SetActive(false),
                source => Destroy(source.gameObject),
                defaultCapacity: 10);
            
            _masterVolumePercent = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1);
            _sfxVolumePercent = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1);
            _musicVolumePercent = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1);
        }

        private void Start()
        {
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnOnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated += RefreshMusic;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnOnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated -= RefreshMusic;
        }


        private void Update()
        {
            for (int i = 0; i < _activeSFX.Count; i++)
            {
                if (!_activeSFX[i].isPlaying)
                {
                    _sfxPool.Release(_activeSFX[i]);
                    _activeSFX.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SetVolume(float volumePercent, AudioChannel channel)
        {
            switch (channel)
            {
                case AudioChannel.Master:
                    _masterVolumePercent = volumePercent;
                    break;
                case AudioChannel.Sfx:
                    _sfxVolumePercent = volumePercent;
                    break;
                case AudioChannel.Music:
                    _musicVolumePercent = volumePercent;
                    break;
            }

            _musicSources[0].volume = _musicVolumePercent * _masterVolumePercent;
            _musicSources[1].volume = _musicVolumePercent * _masterVolumePercent;

            PlayerPrefs.SetFloat(MASTER_VOL_KEY, _masterVolumePercent);
            PlayerPrefs.SetFloat(SFX_VOL_KEY, _sfxVolumePercent);
            PlayerPrefs.SetFloat(MUSIC_VOL_KEY, _musicVolumePercent);
            PlayerPrefs.Save();
        }


        private void PlayMusic(AudioClip clip, float fadeDuration = 1)
        {
            _activeMusicSourceIndex = 1 - _activeMusicSourceIndex;
            _musicSources[_activeMusicSourceIndex].clip = clip;
            _musicSources[_activeMusicSourceIndex].Play();

            if(_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(AnimateMusicCrossFade(fadeDuration));
        }

        public void StopMusic()
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
            
            _musicSources[0].Stop();
            _musicSources[1].Stop();
        }
        
        

        public void PlaySound(string soundName)
        {
            if(!GameManager.Instance.GetConfigValue(EConfigKey.SFX))
                return;
            
            AudioSource sfxSource = _sfxPool.Get();
            AudioTuning tuning = _library.GetClipTuning(soundName);
            sfxSource.clip = tuning.GetClip();
            sfxSource.volume = tuning.Volume * _sfxVolumePercent * _masterVolumePercent;
            if(tuning.PitchVariation > 0)
                sfxSource.pitch = tuning.Pitch + Random.Range(-tuning.PitchVariation, tuning.PitchVariation);
            else
                sfxSource.pitch = tuning.Pitch;
            sfxSource.Play();
            _activeSFX.Add(sfxSource);
            
        }

        IEnumerator AnimateMusicCrossFade(float duration)
        {
            float percent = 0;
            float speed = 1 / duration;

            while (percent < 1)
            {
                percent += Time.deltaTime * speed;
                _musicSources[_activeMusicSourceIndex].volume = Mathf.Lerp(0, _musicVolumePercent * _masterVolumePercent, percent);
                _musicSources[1 - _activeMusicSourceIndex].volume = Mathf.Lerp(_musicVolumePercent * _masterVolumePercent, 0, percent);

                yield return null;
            }
            
            _musicSources[1 - _activeMusicSourceIndex].Stop();
        }
        
        private void OnOnConfigUpdated(EConfigKey key)
        {
            RefreshMusic();
        }

        private void RefreshMusic()
        {
            if (GameManager.Instance.GetConfigValue(EConfigKey.Music))
                PlayMusic(_library.Music, 0.5f);
            else
                StopMusic();
        }
    }
}
