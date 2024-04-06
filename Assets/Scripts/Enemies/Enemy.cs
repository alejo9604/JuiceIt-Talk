using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour
    {
        protected Transform Player => GameManager.Instance.Player.transform;

        private Health _health;

        private void Start()
        {
            _health = GetComponent<Health>();
            _health.OnTakeDamage.AddListener(OnTakeDamage);
            _health.OnDeath.AddListener(OnDeath);
        }

        protected virtual void OnTakeDamage()
        {
            Debug.Log("[Enemy] Take damage. TODO: animation");
        }
        
        protected virtual void OnDeath()
        {
            Debug.Log("[Enemy] Death. TODO: animation");
            gameObject.SetActive(false);
        }
    }
}