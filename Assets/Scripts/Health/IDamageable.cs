using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public interface IDamageable
    {
        void TakeDamage(int damage, Vector2 hitPoint);
    }
}