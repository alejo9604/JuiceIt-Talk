using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;
        [SerializeField] private ImpactDetector _impactDetector;
        [SerializeField] private int _damage;
        
        [Header("Damage")]
        [SerializeField] private GameObject[] _scratch;
        [SerializeField] private GameObject _deathVFX;

        protected Transform Player => GameManager.Instance.Player.transform;

        private Health _health;
        private Animator _animator;
        private static readonly int Hit_AnimHash = Animator.StringToHash("Hit");
        private static readonly int HitSpeed_AnimHash = Animator.StringToHash("HitSpeed");
        private static readonly int Death_AnimHash = Animator.StringToHash("Death");

        private Action<Enemy> OnDeathCompleteEvent;

        protected bool IsDeath => _health.IsDeath;

        private void Start()
        {
            _health = GetComponent<Health>();
            _animator = GetComponent<Animator>();
            
            _health.OnTakeDamage.AddListener(OnTakeDamage);
            _health.OnDeath.AddListener(OnDeath);

            _impactDetector.SetDamage(_damage);
            
            // Make the hit animation the same duration as the InvisibilityTime
            _animator.SetFloat(HitSpeed_AnimHash, 1/ (_health.InvisibilityTime <= 0 ? 0.15f : _health.InvisibilityTime) );
        }

        public virtual void OnSpawn()
        {
            _animator.ResetTrigger(Hit_AnimHash);
            _animator.ResetTrigger(Death_AnimHash);
            _collider.enabled = true;
        }

        public void SetDeathCallback(Action<Enemy> onDeath) => OnDeathCompleteEvent = onDeath; 

        protected virtual void OnTakeDamage()
        {
            //Hit animation
            if (GameManager.Instance.GetConfigValue(EConfigKey.EnemyHitImpact))
            {
                _animator.ResetTrigger(Hit_AnimHash);
                _animator.SetTrigger(Hit_AnimHash);
            }

            //Enable Scratches
            if (GameManager.Instance.GetConfigValue(EConfigKey.EnemyScratch))
            {
                int scratchToEnable = Mathf.RoundToInt((_scratch.Length + 2) * _health.HealthNormalize);
                for (int i = 0; i < _scratch.Length; i++)
                    _scratch[i].SetActive(i + 1 >= scratchToEnable);
            }
        }
        
        protected virtual void OnDeath()
        {
            if (!GameManager.Instance.GetConfigValue(EConfigKey.EnemyDeath))
            {
                OnDeathComplete();
                return;
            }
            
            _animator.ResetTrigger(Hit_AnimHash);
            _animator.SetTrigger(Death_AnimHash);
            _collider.enabled = false;
            if (_deathVFX != null)
            {
                GameObject vfx = Instantiate(_deathVFX, transform.position, Quaternion.identity);
                Destroy(vfx, 2);
            }
            
            AudioManager.Instance.PlaySound(AudioLibrary.ENEMY_DEATH);
        }

        //Called via AnimationEvent
        protected void OnDeathComplete()
        {
            OnDeathCompleteEvent?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}