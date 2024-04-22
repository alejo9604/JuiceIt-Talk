using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class AudioManager : MonoBehaviour
    {
        public enum AudioChannel { Master, Sfx, Music };
        
        public static AudioManager Instance;
        
        [SerializeField] private AudioLibrary _library;
        
        private AudioSource _sfxSource;
        private AudioSource[] _musicSources;
        private int _activeMusicSourceIndex;
        
        private float _masterVolumePercent;
        private float _sfxVolumePercent;
        private float _musicVolumePercent;

        private const string MASTER_VOL_KEY = "master_vol";
        private const string MUSIC_VOL_KEY = "music_vol";
        private const string SFX_VOL_KEY = "sfx_vol";

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject($"Music_Source_{i + 1}");
                _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            GameObject sfxSource = new GameObject("SFX_Source");
            _sfxSource = sfxSource.AddComponent<AudioSource>();
            sfxSource.transform.parent = transform;
            
            _masterVolumePercent = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1);
            _sfxVolumePercent = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1);
            _musicVolumePercent = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1);
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


        public void PlayMusic(AudioClip clip, float fadeDuration = 1)
        {
            _activeMusicSourceIndex = 1 - _activeMusicSourceIndex;
            _musicSources[_activeMusicSourceIndex].clip = clip;
            _musicSources[_activeMusicSourceIndex].Play();

            StartCoroutine(AnimateMusicCrossFade(fadeDuration));
        }
        

        public void PlaySound(string soundName)
        {
            AudioTuning tuning = _library.GetClipTuning(soundName);
            _sfxSource.PlayOneShot(tuning.GetClip(), tuning.Volume * _sfxVolumePercent * _masterVolumePercent);
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
        }
    }
}
