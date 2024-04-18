using UnityEngine;
using UnityEngine.Events;

namespace AllieJoe.JuiceIt
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] protected int _totalHealth = 5;
        [SerializeField] protected int _currentHealth = 5;

        [Space]
        [SerializeField] private float _invisibilityTime = 0;
        
        [Space(20)]
        public UnityEvent OnTakeDamage;
        public UnityEvent OnDeath;
        
        private bool _isDeath;
        private float _nextHitAt;

        public bool IsDeath => _isDeath;
        public float InvisibilityTime => _invisibilityTime;
        public bool IsInvincible => _nextHitAt >= Time.time;

        public void Start()
        {
            Reset();
        }
        
        [ContextMenu("Reset Health")]
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
            
            SetHealth(_currentHealth - damage);
            if (_invisibilityTime > 0)
                _nextHitAt = Time.time + _invisibilityTime;
            
            OnTakeDamage?.Invoke();

            if(_currentHealth <= 0)
                Death();
        }

        protected virtual void SetHealth(int health)
        {
            _currentHealth = Mathf.Max(0, health);
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
