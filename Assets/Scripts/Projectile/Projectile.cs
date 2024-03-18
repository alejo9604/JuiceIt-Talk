using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed = 18f;
    private float _lifeTime = 3;
    
    private float _timeToDisable = -1;
    private bool _isPlayer = false;
    private LayerMask _collisionMask;
    private ContactFilter2D _contactFilter;
    private readonly RaycastHit2D[] _cachedContactHit = new RaycastHit2D[1];
    
    private const float SKIN_WIDTH = 0.1f;
    
    public void Init(Vector3 dir, float speed, bool isPlayer, float lifeTime = 3)
    {
        _speed = speed;
        _isPlayer = isPlayer;
        _lifeTime = lifeTime;
        _timeToDisable = Time.time + _lifeTime;
        
        transform.up = dir;
        
        if(_isPlayer)
            _collisionMask = 1 << LayerMask.NameToLayer("Enemy");
        else
            _collisionMask = 1 << LayerMask.NameToLayer("Player");

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
        int contacts = Physics2D.Raycast(transform.position, transform.forward, 
            _contactFilter, 
            _cachedContactHit,
            moveDistance + SKIN_WIDTH);
        if(contacts > 0)
            OnHitObject(_cachedContactHit[0].collider, _cachedContactHit[0].point, _cachedContactHit[0].normal);
    }
    
    private void OnHitObject(Collider2D collision, Vector2 hitPoint, Vector2 normalHitPoint)
    {
        TryToApplyDamageToTarget(collision.transform, hitPoint);
        Debug.Log($"Hit {collision.gameObject.name}");
        
        GameManager.Instance.DoImpactPause();
        
        DestroyProjectile();
    }

    private void TryToApplyDamageToTarget(Transform target, Vector2 hitPoint)
    {
        
    }
    
    private void DestroyProjectile()
    {
        Destroy(gameObject);
        //PoolManager.Destroy(gameObject);
    }
}
