using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _lifeTime = 3;

    private float _timeToDisable = -1;
    private LayerMask _collisionMask;
    private ContactFilter2D _contactFilter;
    private readonly RaycastHit2D[] _cachedContactHit = new RaycastHit2D[1];
    
    private const float _skinWidth = 0.1f;
    
    public void Init(Vector3 dir, float extraSpeed, float lifeTime = 3)
    {
        _speed += extraSpeed;
        _lifeTime = lifeTime;
        _timeToDisable = Time.time + _lifeTime;
        
        transform.up = dir;
        
        _collisionMask = 1 << LayerMask.NameToLayer("Player");
        _collisionMask |= 1 << LayerMask.NameToLayer("Enemy");

        _contactFilter = new ContactFilter2D();
        _contactFilter.layerMask = _collisionMask;
        _contactFilter.useLayerMask = true;
        _contactFilter.useTriggers = true;
    }

    void Update()
    {
        if(_timeToDisable < 0) {
            return;
        }

        if(_timeToDisable < Time.time) {
            _timeToDisable = -1;
            DestroyProjectile();
        }
        
        float moveDistance = _speed * Time.deltaTime;
        CheckCollision(moveDistance);
        transform.Translate(Vector3.up * moveDistance);
    }
    
    void CheckCollision(float moveDistance)
    {
        int contacts = Physics2D.Raycast(transform.position, transform.forward, _contactFilter, _cachedContactHit,
            moveDistance + _skinWidth);
        if(contacts > 0)
            OnHitObject(_cachedContactHit[0].collider, _cachedContactHit[0].point, _cachedContactHit[0].normal);
    }
    
    private void OnHitObject(Collider2D collision, Vector2 hitPoint, Vector2 normalHitPoint)
    {
        //TryToApplyDamageToTarget(collision.transform, hitPoint);
        Debug.Log($"Hit {collision.gameObject.name}");
        DestroyProjectile();
    }
    
    private void DestroyProjectile()
    {
        Destroy(gameObject);
        //PoolManager.Destroy(gameObject);
    }
}
