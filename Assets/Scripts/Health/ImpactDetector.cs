using System;
using UnityEngine;

namespace AllieJoe.JuiceIt.Utils
{
    public class ImpactDetector: MonoBehaviour, IDamageable
    {
        [SerializeField] private Health _parentHealth;

        private void Start()
        {
            if(_parentHealth == null)
                _parentHealth = GetComponentInParent<Health>();
        }

        public void TakeDamage(int damage, Vector2 hitPoint)
        {
            if(_parentHealth)
                _parentHealth.TakeDamage(damage, hitPoint);
        }

        private void OnValidate()
        {
            _parentHealth = GetComponentInParent<Health>();
        }
    }
}