using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class TrailController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _trails;

        private PlayerShip _playerShip;

        private void Start()
        {
            _playerShip = GetComponent<PlayerShip>();
            UpdateTrails(GameManager.Instance.GetConfigValue(EConfigKey.Trail));
        }

        private void Update()
        {
            UpdateTrails(GameManager.Instance.GetConfigValue(EConfigKey.Trail));
        }

        private void UpdateTrails(bool enable)
        {
            if (_playerShip.IsAccelerating && enable)
            {
                foreach (var trail in _trails)
                {
                    if(!trail.isEmitting)
                        trail.Play(true);
                }
            }
            else
            {
                foreach (var trail in _trails)
                {
                    if(trail.isEmitting)
                        trail.Stop();
                }
            }
        }
    }
}