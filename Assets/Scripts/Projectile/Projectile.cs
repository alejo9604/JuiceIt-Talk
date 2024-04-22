using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class Projectile : MonoBehaviour
    {
        private const float SKIN_WIDTH = 0.1f;
        
        [SerializeField] private GameObject _hitPrefab;
        
        private float _speed = 18f;
        private float _lifeTime = 3;
        private int _damage = 0;

        private float _timeToDisable = -1;
        private bool _isPlayer = false;
        private LayerMask _collisionMask;
        private ContactFilter2D _contactFilter;
        private readonly RaycastHit2D[] _cachedContactHit = new RaycastHit2D[1];


        public void Init(Vector3 dir, float speed, bool isPlayer, float lifeTime = 3, float extraSpeed = 0)
        {
            _speed = speed + extraSpeed;
            _isPlayer = isPlayer;
            _lifeTime = lifeTime;
            _timeToDisable = Time.time + _lifeTime;

            transform.up = dir;

            if (_isPlayer)
                _collisionMask = 1 << LayerMask.NameToLayer("Enemy");
            else
                _collisionMask = 1 << LayerMask.NameToLayer("Player");

            _contactFilter = new ContactFilter2D();
            _contactFilter.layerMask = _collisionMask;
            _contactFilter.useLayerMask = true;
            _contactFilter.useTriggers = true;
        }

        public void SetDamage(int damage) => _damage = damage;

        void Update()
        {
            if (_timeToDisable < 0)
            {
                return;
            }

            if (_timeToDisable < Time.time)
            {
                _timeToDisable = -1;
                DestroyProjectile();
            }

            float moveDistance = _speed * Time.deltaTime;
            CheckCollision(moveDistance);
            transform.Translate(Vector3.up * moveDistance);
        }

        void CheckCollision(float moveDistance)
        {
            int contacts = Physics2D.Raycast(transform.position, transform.forward,
                _contactFilter,
                _cachedContactHit,
                moveDistance + SKIN_WIDTH);
            if (contacts > 0)
                OnHitObject(_cachedContactHit[0].collider, _cachedContactHit[0].point, _cachedContactHit[0].normal);
        }

        private void OnHitObject(Collider2D collision, Vector2 hitPoint, Vector2 normalHitPoint)
        {
            TryToApplyDamageToTarget(collision.transform, hitPoint);
            
            GameManager.Instance.DoImpactPause(false);

            SpawnImpactVFX(hitPoint, normalHitPoint);
            
            AudioManager.Instance.PlaySound(AudioLibrary.PROJECTILE_IMPACT);

            DestroyProjectile();
        }

        private void TryToApplyDamageToTarget(Transform target, Vector2 hitPoint)
        {
            if (target.TryGetComponent(out IDamageable targetHealth))
                targetHealth.TakeDamage(_damage, hitPoint);
        }

        private void SpawnImpactVFX(Vector2 hitPoint, Vector2 normal)
        {
            if(_hitPrefab == null || !GameManager.Instance.GetConfigValue(EConfigKey.ProjectileHitVFX))
                return;
            GameObject vfx = Instantiate(_hitPrefab, hitPoint, Quaternion.Euler(0, 0, 180) * transform.rotation);
            Destroy(vfx, 3);
        }

        private void DestroyProjectile()
        {
            Destroy(gameObject);
            //TODO: PoolManager.Destroy(gameObject);
        }
    }
}