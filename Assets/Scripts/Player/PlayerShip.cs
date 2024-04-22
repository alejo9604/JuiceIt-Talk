using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AllieJoe.JuiceIt
{
    [RequireComponent(typeof(PlayerShipShoot))]
    [RequireComponent(typeof(RecoverHealthOverTime))]
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

        [Header("Health")]
        [SerializeField] private GameObject _damageVFX; 
        
        private PlayerShipShoot _shootComponent;
        private RecoverHealthOverTime _health;
        
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

        public bool IsAccelerating => _isAccelerating;
        public Vector2 CurrentVelocity => _rb.velocity;
        public Vector2 MovementDirection => _movementDir;
        public float SpeedNormalize => _currentSpeed / _maxSpeed;
        public Vector2 AimDirection => transform.up;

        private float GetMaxMovementSpeed()
        {
            //No shooting custom movement
            if (!GameManager.Instance.GetConfigValue(EConfigKey.ShootingMovementRestriction))
                return _maxSpeed;

            return _isShooting ? _maxShootingSpeed : _maxSpeed;
        }

        private float GetTurnSpeed()
        {
            //No shooting custom movement
            if (!GameManager.Instance.GetConfigValue(EConfigKey.ShootingMovementRestriction))
                return _isAccelerating ? _movementTurnSpeed : _stillTurnSpeed;

            if (_isAccelerating)
                return _isShooting ? _movementTurnShootingSpeed : _movementTurnSpeed;
            return _isShooting ? _movementTurnShootingSpeed : _stillTurnSpeed;
        }

        private void Start()
        {
            _shootComponent = GetComponent<PlayerShipShoot>();
            _health = GetComponent<RecoverHealthOverTime>();
            
            _health.OnTakeDamage.AddListener(OnTakeDamage);
            
            _health.SetShowVFX(GameManager.Instance.GetConfigValue(EConfigKey.PlayerDamageVFX));
            
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated += RefreshConfig;
        }

        void Update()
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");
            _isShooting = !EventSystem.current.IsPointerOverGameObject() && 
                (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space));

            //TODO: Do we need to normalize? Not for now
            _input.x *= -1; //Invert X-axis. We have our ship rotated 180 Deg
            _isAccelerating = _input.y != 0;

            if (_isShooting)
                _shootComponent.Shoot(_currentSpeed);

            _health.SetCanRecover(!_isShooting);
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

                _movementDir = AimDirection;
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
        
        private void OnTakeDamage()
        {
            GameManager.Instance.DoImpactPause(true);
            AudioManager.Instance.PlaySound(AudioLibrary.PLAYER_HIT);
            if(GameManager.Instance.GetConfigValue(EConfigKey.PlayerImpactVFX))
                _damageVFX.SetActive(true);
            
            GameManager.Instance.GameDelegates.EmitOnPlayerHit(transform.position);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.TryGetComponent(out IImpact impact))
                _health.TakeDamage(impact.GetDamage(), other.ClosestPoint(transform.position));
        }
        
        private void OnConfigUpdated(EConfigKey key)
        {
            if (key == EConfigKey.PlayerDamageVFX)
                _health.SetShowVFX(GameManager.Instance.GetConfigValue(key));
        }

        private void RefreshConfig()
        {
            _health.SetShowVFX(GameManager.Instance.GetConfigValue(EConfigKey.PlayerDamageVFX));
        }
    }
}