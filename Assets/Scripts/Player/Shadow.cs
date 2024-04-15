using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Shadow : MonoBehaviour
    {
        [SerializeField] private Transform _shadow;
        [SerializeField] private bool _updateRotation;
        private Vector2 _baseOffset;

        private void Start()
        {
            _baseOffset = _shadow.localPosition;
            
            _shadow.gameObject.SetActive(GameManager.Instance.GetConfigValue(EConfigKey.Shadows));
            
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnOnConfigUpdated;
        }

        private void Update()
        {
            if(_updateRotation)
                _shadow.localPosition = Quaternion.Euler(0, 0, -transform.eulerAngles.z) * _baseOffset;
        }
        
        private void OnOnConfigUpdated(EConfigKey key)
        {
            if(key != EConfigKey.Shadows)
                return;
            
            _shadow.gameObject.SetActive(GameManager.Instance.GetConfigValue(key));
        }
    }
}