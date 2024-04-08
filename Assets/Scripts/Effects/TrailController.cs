using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class TrailController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _trails;

        private bool _trailEnabled;

        private void Start()
        {
            UpdateTrails(GameManager.Instance.GetConfigValue<bool>(EConfigKey.Trail));
        }

        private void Update()
        {
            if (_trailEnabled == GameManager.Instance.GetConfigValue<bool>(EConfigKey.Trail))
                return;

            UpdateTrails(GameManager.Instance.GetConfigValue<bool>(EConfigKey.Trail));
        }

        private void UpdateTrails(bool enable)
        {
            _trailEnabled = enable;
            foreach (var t in _trails)
                t.SetActive(enable);
        }
    }
}