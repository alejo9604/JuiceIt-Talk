using UnityEngine;

public class PlayerShip : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rb;
    
    [Header("Movement")]
    [SerializeField] private float _acceleration = 16f;
    [SerializeField] private float _desacceleration = 10f;
    [SerializeField] private float _maxSpeed = 13f;
    [SerializeField] private float _maxShootingSpeed = 13f;

    [Header("Rotation")]
    [SerializeField] private float _movementTurnSpeed = 200f;
    [SerializeField] private float _movementTurnShootingSpeed = 275f;
    [SerializeField] private float _stillTurnSpeed = 350f;

    [Header("Fire")]
    [SerializeField] private float _fireDelay = 0.5f;
    [SerializeField] private Transform[] _cannonFire;
    [SerializeField] private Projectile _projectilePrefab;
    

    //Input
    private Vector2 _input = Vector2.zero;
    private bool _isAccelerating;
    private bool _wasAccelerating;
    private bool _isShooting;
    
    // Movement
    private float _currentSpeed;
    private Vector2 _movementDir;
    private float _changeDirectionDotValue;
    private float _rotAngleChange;
    
    //Fire
    private float _nextFireAt;

    private float GetMaxMovementSpeed()
    {
        //No shooting custom movement
        if (!GameManager.Instance.JuiceConfig.ShootingMovementRestrictionEnabled)
            return _maxSpeed;
        
        return _isShooting ? _maxShootingSpeed : _maxSpeed;
    }

    private float GetTurnSpeed()
    {
        //No shooting custom movement
        if (!GameManager.Instance.JuiceConfig.ShootingMovementRestrictionEnabled)
            return _isAccelerating ? _movementTurnSpeed : _stillTurnSpeed;
        
        if (_isAccelerating)
            return _isShooting ? _movementTurnShootingSpeed : _movementTurnSpeed;
        return _isShooting ? _movementTurnShootingSpeed : _stillTurnSpeed;
    }
    
    void Update()
    {
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        _isShooting = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
        
        //TODO: Do we need to normalize? Not for now
        _input.x *= -1; //Invert X-axis. We have our ship rotated 180deg
        _isAccelerating = _input.y != 0;

        if (_isShooting)
            FireProjectile();
        
    }
    
    private void FixedUpdate()
    {
        HandleMovement(Time.fixedDeltaTime);
    }

    private void HandleMovement(float deltaTime)
    {
        //Rotation
        if (_input.x == 0)
            _rotAngleChange = 0;
        else
            _rotAngleChange = Mathf.Sign(_input.x) * deltaTime * GetTurnSpeed(); 
        
        //Movement
        if (_isAccelerating)
        {
            //Drift hack
            if (!_wasAccelerating)
                _currentSpeed *= Mathf.Clamp(_changeDirectionDotValue, 0.1f, 1);
            _wasAccelerating = true;
            
            _movementDir = transform.up;
            _currentSpeed += _acceleration * deltaTime;
        }
        else
        {
            _wasAccelerating = false;
            _changeDirectionDotValue = Vector2.Dot(_movementDir, transform.up);
            _currentSpeed -= _desacceleration * deltaTime;
        }

        //Clamp speed
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, GetMaxMovementSpeed());

        //Apply to RB
        _rb.rotation += _rotAngleChange; 
        _rb.velocity = _movementDir * _currentSpeed;
    }

    private void FireProjectile()
    {
        if(Time.time < _nextFireAt)
            return;

        _nextFireAt = Time.time + _fireDelay;
        
        //TODO: Expose it
        var accuracy = Quaternion.identity;
        if(GameManager.Instance.JuiceConfig.ShootingAccuracyEnabled)
            accuracy = Quaternion.Euler(0, 0, Random.Range(-4, 4));

        foreach (var cannon in _cannonFire)
        {
            var dir = accuracy * cannon.transform.up;
            Projectile projectile = Instantiate( _projectilePrefab, cannon.position, Quaternion.identity); ;
            projectile.Init(dir, _currentSpeed);
        }
    }
}
