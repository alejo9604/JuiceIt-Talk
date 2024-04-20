﻿using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class RecoverHealthOverTime : Health
    {
        [Space]
        [SerializeField, Range(0, 1)] private float _recoverPerSecond = 0.1f;
        [SerializeField] private GameObject _damageVFX;

        private bool _canRecover;
        private float _currentHealthFloat;

        public void SetCanRecover(bool value) => _canRecover = value;
        
        private void Update()
        {
            _damageVFX.SetActive(_currentHealth < _totalHealth);
            if (!_canRecover || IsDeath || _currentHealth >= _totalHealth || IsInvincible)
                return;
            
            _currentHealthFloat += _recoverPerSecond * Time.deltaTime;
            if (_currentHealthFloat - 1 >= _currentHealth)
                _currentHealth = Mathf.RoundToInt(_currentHealthFloat);
        }

        protected override void SetHealth(int health)
        {
            base.SetHealth(health);
            _currentHealthFloat = _currentHealth;
        }
    }
}