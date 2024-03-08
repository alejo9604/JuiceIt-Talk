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


    private Vector2 _input = Vector2.zero;
    private bool _isAccelerating;
    private bool _wasAccelerating;
    
    private float _currentSpeed;
    private Vector2 _movementDir;
    private float _changeDirectionDotValue;
    private float _rotAngleChange;
    
    void Update()
    {
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        
        //TODO: Do we need to normalize? Not for now
        _input.x *= -1; //Invert X-axis. We have our ship rotated 180deg
        _isAccelerating = _input.y != 0;
    }
    
    private void FixedUpdate()
    {

        //Rotation
        if (_input.x == 0)
            _rotAngleChange = 0;
        else
            _rotAngleChange = Mathf.Sign(_input.x) * Time.fixedDeltaTime * (_isAccelerating ? _movementTurnSpeed : _stillTurnSpeed); 
        
        //Movement
        if (_isAccelerating)
        {
            //Drift hack
            if (!_wasAccelerating)
                _currentSpeed *= Mathf.Clamp(_changeDirectionDotValue, 0.1f, 1);
            _wasAccelerating = true;
            
            _movementDir = transform.up;
            _currentSpeed += _acceleration * Time.fixedDeltaTime;
        }
        else
        {
            _wasAccelerating = false;
            _changeDirectionDotValue = Vector2.Dot(_movementDir, transform.up);
            _currentSpeed -= _desacceleration * Time.fixedDeltaTime;
        }

        //Clamp speed
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

        //Apply to RB
        _rb.rotation += _rotAngleChange; 
        _rb.velocity = _movementDir * _currentSpeed;
    }

    private float GetCurlFactor(float x)
    {
        if(x > 0)
            return x * x * x * x;
        return 0.1f;
    }
}
