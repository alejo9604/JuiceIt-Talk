using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class ImpactDetector : MonoBehaviour, IImpact
    {
        [SerializeField] private int _damage;

        public int GetDamage() => _damage;
        public int SetDamage(int damage) => _damage = damage;
    }
}