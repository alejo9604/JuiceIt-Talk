using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
        protected Transform Player => GameManager.Instance.Player.transform;

        private Health _health;
        private Animator _animator;
        private static readonly int Hit_AnimHash = Animator.StringToHash("Hit");
        private static readonly int HitSpeed_AnimHash = Animator.StringToHash("HitSpeed");

        private void Start()
        {
            _health = GetComponent<Health>();
            _animator = GetComponent<Animator>();
            
            _health.OnTakeDamage.AddListener(OnTakeDamage);
            _health.OnDeath.AddListener(OnDeath);
            
            // Make the hit animation the same duration as the InvisibilityTime
            _animator.SetFloat(HitSpeed_AnimHash, 1/ (_health.InvisibilityTime <= 0 ? 0.15f : _health.InvisibilityTime) );
        }

        protected virtual void OnTakeDamage()
        {
            Debug.Log("[Enemy] Take damage. TODO: animation");

            if (GameManager.Instance.GetConfigValue(EConfigKey.EnemyHitImpact))
            {
                _animator.ResetTrigger(Hit_AnimHash);
                _animator.SetTrigger(Hit_AnimHash);
            }
        }
        
        protected virtual void OnDeath()
        {
            Debug.Log("[Enemy] Death. TODO: animation");
            gameObject.SetActive(false);
        }
    }
}