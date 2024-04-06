using UnityEngine;
using UnityEngine.Events;

namespace AllieJoe.JuiceIt
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _totalHealth = 5;
        [SerializeField] private int _currentHealth = 5;

        [Space]
        [SerializeField] private float _invisibilityTime = 0;
        
        [Space]
        public UnityEvent OnTakeDamage;
        public UnityEvent OnDeath;
        
        private bool _isDeath;
        private float _nextHitAt;

        public void Start()
        {
            Reset();
        }

        public void Reset()
        {
            _currentHealth = _totalHealth;
            _isDeath = false;
            _nextHitAt = 0;
        }

        public void TakeDamage(int damage, Vector2 hitPoint)
        {
            if(_isDeath || Time.time < _nextHitAt)
                return;
            
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            if (_invisibilityTime > 0)
                _nextHitAt = Time.time + _invisibilityTime;
            
            OnTakeDamage?.Invoke();

            if(_currentHealth <= 0)
                Death();
        }

        private void Death()
        {
            if(_isDeath)
                return;
            _isDeath = true;
            OnDeath?.Invoke();
        }
    }
}