using UnityEngine;

public class PlayerShip : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rb;
    
    [Header("Movement")]
    [SerializeField] private float _acceleration = 3f;
    [SerializeField] private float _desacceleration = 1f;
    [SerializeField] private float _maxSpeed = 1.6f;

    [Header("Rotation")]
    [SerializeField] private float _movementTurnSpeed = 100f;
    [SerializeField] private float _stillTurnSpeed = 250f;
    
    [Header("Fire")]
    [SerializeField] private float _fireDelay = 0.5f;
    [SerializeField] private Transform[] _cannonFire;
    [SerializeField] private Projectile _projectilePrefab;


    //Input
    private Vector2 _input = Vector2.zero;
    private bool _isAccelerating;
    private bool _wasAccelerating;
    
    // Movement
    private float _currentSpeed;
    private Vector2 _movementDir;
    private float _changeDirectionDotValue;
    private float _rotAngleChange;
    
    //Fire
    private float _nextFireAt;
    
    void Update()
    {
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        //TODO: Do we need to normalize? Not for now
        _input.x *= -1; //Invert X-axis. We have our ship rotated 180deg
        _isAccelerating = _input.y != 0;

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
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
            _rotAngleChange = Mathf.Sign(_input.x) * deltaTime * (_isAccelerating ? _movementTurnSpeed : _stillTurnSpeed); 
        
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
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

        //Apply to RB
        _rb.rotation += _rotAngleChange; 
        _rb.velocity = _movementDir * _currentSpeed;
    }

    private void FireProjectile()
    {
        if(Time.time < _nextFireAt)
            return;

        _nextFireAt = Time.time + _fireDelay;

        foreach (var cannon in _cannonFire)
        {
            var dir = cannon.transform.up;
            //dir = Quaternion.Euler(0, 0, Random.Range( -4, 4 ) ) * dir;
            
            Projectile projectile = Instantiate( _projectilePrefab, cannon.position, Quaternion.identity); ;
            projectile.transform.up = dir;
        }
    }
}
