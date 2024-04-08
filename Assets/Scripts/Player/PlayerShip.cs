using UnityEngine;
using UnityEngine.EventSystems;

namespace AllieJoe.JuiceIt
{
    [RequireComponent(typeof(PlayerShipShoot))]
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
        
        private PlayerShipShoot _shootComponent;

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

        private float GetMaxMovementSpeed()
        {
            //No shooting custom movement
            if (!GameManager.Instance.GetConfigValue<bool>(EConfigKey.ShootingMovementRestriction))
                return _maxSpeed;

            return _isShooting ? _maxShootingSpeed : _maxSpeed;
        }

        private float GetTurnSpeed()
        {
            //No shooting custom movement
            if (!GameManager.Instance.GetConfigValue<bool>(EConfigKey.ShootingMovementRestriction))
                return _isAccelerating ? _movementTurnSpeed : _stillTurnSpeed;

            if (_isAccelerating)
                return _isShooting ? _movementTurnShootingSpeed : _movementTurnSpeed;
            return _isShooting ? _movementTurnShootingSpeed : _stillTurnSpeed;
        }

        private void Start()
        {
            _shootComponent = GetComponent<PlayerShipShoot>();
        }

        void Update()
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");
            _isShooting = !EventSystem.current.IsPointerOverGameObject() && 
                (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space));

            //TODO: Do we need to normalize? Not for now
            _input.x *= -1; //Invert X-axis. We have our ship rotated 180deg
            _isAccelerating = _input.y != 0;

            if (_isShooting)
                _shootComponent.Shoot();

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
    }
}