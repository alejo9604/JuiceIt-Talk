using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class GameCamera : MonoBehaviour
    {
        [Header("Lerp")]
        [SerializeField] private float _lerpSpeed = 5;
        [SerializeField] private float _lerpAcceleration = 5;
        [SerializeField] private float _lerpDesacceleration = 5;
        [SerializeField] private float _movementPredictionAmount = 5;
        
        [Space]
        [SerializeField] private PointOfInterestManager _pointOfInterestManager;
        
        [Header("Shake")] 
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _maxOffset;
        [SerializeField] private float _perlinNoiseMultiplier = 1;
        private bool _usePerlinNoise;

        private Vector3 _initPosition;
        private Vector3 _lastCamPosition;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        
        private Vector3 _currentOffset;
        private Vector3 _targetOffset;
        private float _targetLerpSpeed;

        private Vector3 PlayerPos => GameManager.Instance.Player.transform.position;

        private const int ANGLE_PERLIN_SEED = 100;
        private const int OFFSET_X_PERLIN_SEED = 200;
        private const int OFFSET_Y_PERLIN_SEED = 300;

        private void Start()
        {
            _currentOffset = _initPosition = _lastCamPosition = transform.position;
        }

        private void LateUpdate()
        {
            transform.position = _targetPosition;
            transform.rotation = _targetRotation;
        }

        private Vector3 smoothVel;

        private void FixedUpdate()
        {
            if (GameManager.Instance.Player.IsAccelerating)
            {
                UpdateTargetOffset();
                _targetLerpSpeed += _lerpAcceleration * Time.deltaTime;
            }
            else
            {
                _targetLerpSpeed -= _lerpDesacceleration * Time.deltaTime;
            }

            _targetLerpSpeed = Mathf.Clamp(_targetLerpSpeed, 0, _lerpSpeed);
            //_currentOffset = Vector3.Lerp(_currentOffset, _targetOffset, _targetLerpSpeed * Time.deltaTime);
            //var pos = PlayerPos + _currentOffset;
            //pos.z = _initPosition.z; //Ensure Z-Offset
            
            _currentOffset = _targetOffset;
            _lastCamPosition = Vector3.Lerp(_lastCamPosition, PlayerPos + _currentOffset, _targetLerpSpeed * Time.deltaTime);
            _lastCamPosition.z = _initPosition.z; //Ensure Z-Offset
            
            _targetPosition = _lastCamPosition;
            _targetRotation = Quaternion.identity;

            ApplyShake(GameManager.Instance.ShakeValue);
        }
        
        private Vector3 GetNextPlayerPosition()
        {
            Vector3 movement = GameManager.Instance.Player.MovementDirection * GameManager.Instance.Player.SpeedNormalize * _movementPredictionAmount;
            return PlayerPos + movement;
        }
        
        private void UpdateTargetOffset()
        {
            Vector3 center = _pointOfInterestManager.GetCenter(GetNextPlayerPosition());

            _targetOffset = center - PlayerPos;
        }
        
        private void ApplyShake(float shake)
        {
            if(shake <= 0)
                return;

            _usePerlinNoise = GameManager.Instance.GetConfigValue(EConfigKey.CameraPerlinNoise);
            
            float angle = _maxAngle * shake * (_usePerlinNoise ? GetPerlin(ANGLE_PERLIN_SEED) : GetRandomFloatNegOneToOne());
            float offsetX = _maxOffset * shake * (_usePerlinNoise ? GetPerlin(OFFSET_X_PERLIN_SEED) : GetRandomFloatNegOneToOne());;
            float offsetY = _maxOffset * shake * (_usePerlinNoise ? GetPerlin(OFFSET_Y_PERLIN_SEED) : GetRandomFloatNegOneToOne());;

            
            _targetPosition.x += offsetX;
            _targetPosition.y += offsetY;
            
            Vector3 rot = _targetRotation.eulerAngles;
            rot.z += angle;
            _targetRotation = Quaternion.Euler(rot);
        }

        private float GetRandomFloatNegOneToOne() => Random.Range(-1f, 1f);
        private float GetPerlin(float seed) => (Mathf.PerlinNoise(seed, Time.time * _perlinNoiseMultiplier) - 0.5f) * 2f;
    }
}
