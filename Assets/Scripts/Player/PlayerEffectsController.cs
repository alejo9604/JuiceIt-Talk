using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class PlayerEffectsController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _trails;

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
            TryPlayAccelerateSFX(GameManager.Instance.GetConfigValue(EConfigKey.SFX));
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
                        trail.Stop();
                }

            }
        }

        private void TryPlayAccelerateSFX(bool enable)
        {
            if (_playerShip.IsAccelerating && enable)
            {
                if (!_wasEmitting)
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