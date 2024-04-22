using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Shadow : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _shadow;
        [SerializeField] private bool _updateRotation;
        [SerializeField, HideInInspector] private Vector2 _baseOffset;

        private void Start()
        {
            UpdateShadow();
            _shadow.gameObject.SetActive(GameManager.Instance.GetConfigValue(EConfigKey.Shadows));
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnOnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated += RefreshConfig;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnOnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated -= RefreshConfig;
        }

        private void Update()
        {
            if (_updateRotation)
                UpdateShadow();
        }

        private void UpdateShadow()
        {
            float zRot = _body == null ? transform.eulerAngles.z : _body.eulerAngles.z;
            _shadow.localPosition = Quaternion.Euler(0, 0, -zRot) * _baseOffset;
        }
        
        private void OnOnConfigUpdated(EConfigKey key)
        {
            if(key != EConfigKey.Shadows)
                return;
            RefreshConfig();
        }

        private void RefreshConfig()
        {
            _shadow.gameObject.SetActive(GameManager.Instance.GetConfigValue(EConfigKey.Shadows));
        }
        

        private void OnValidate()
        {
            _baseOffset = _shadow.localPosition;
        }
    }
}