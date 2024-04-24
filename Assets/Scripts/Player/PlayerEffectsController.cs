using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class PlayerEffectsController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _trails;
        [SerializeField] private GameObject _stopVFX;

        private PlayerShip _playerShip;
        private bool _wasEmitting;

        private void Start()
        {
            _playerShip = GetComponent<PlayerShip>();
            UpdateTrails(GameManager.Instance.GetConfigValue(EConfigKey.Trail));
        }

        private void Update()
        {
            UpdateTrails(GameManager.Instance.GetConfigValue(EConfigKey.Trail));
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