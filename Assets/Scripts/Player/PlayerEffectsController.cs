using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class PlayerEffectsController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _trails;
        [SerializeField] private GameObject _stopVFX;

        private PlayerShip _playerShip;
        private bool _wasEmitting;

        private bool _useTrail;
        private bool _useStopVFX;

        private void Start()
        {
            _playerShip = GetComponent<PlayerShip>();
            UpdateTrails(GameManager.Instance.GetConfigValue(EConfigKey.Trail));
            
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated += RefreshConfig;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated -= RefreshConfig;
        }

        private void OnConfigUpdated(EConfigKey key)
        {
            if (key is EConfigKey.Trail or EConfigKey.TrailStopVFX)
                RefreshConfig();
        }

        private void RefreshConfig()
        {
            _useTrail = GameManager.Instance.GetConfigValue(EConfigKey.Trail);
            _useStopVFX = GameManager.Instance.GetConfigValue(EConfigKey.TrailStopVFX);
        }

        private void Update()
        {
            UpdateTrails(_useTrail);
            TryPlayAccelerateSFX();
        }

        private void UpdateTrails(bool enable)
        {
            if (_playerShip.IsAccelerating && enable)
            {
                foreach (var trail in _trails)
                {
                    if (!trail.isEmitting)
                        trail.Play(true);
                }
            }
            else
            {
                foreach (var trail in _trails)
                {
                    if (trail.isEmitting)
                    {
                        trail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        if(_useStopVFX)
                            Instantiate(_stopVFX, trail.transform.position, trail.transform.rotation);
                    }
                }
            }
        }

        private void TryPlayAccelerateSFX()
        {
            if (_playerShip.IsAccelerating)
            {
                if(!_wasEmitting)
                    AudioManager.Instance.PlaySound(AudioLibrary.PLAYER_ACCELERATE);
                _wasEmitting = true;
            }
            else
            {
                _wasEmitting = false;
            }
        }
    }
}