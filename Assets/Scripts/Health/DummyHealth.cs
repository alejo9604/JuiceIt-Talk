using UnityEngine;
using UnityEngine.Events;

namespace AllieJoe.JuiceIt
{
    public class DummyHealth : Health
    {
        public UnityEvent OnReset;
        
        protected override void Death()
        {
            base.Death();

            transform.position = Random.insideUnitCircle * 6;
                
            gameObject.SetActive(true);
            OnReset?.Invoke();
        }
    }
}
