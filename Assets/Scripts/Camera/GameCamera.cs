using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class GameCamera : MonoBehaviour
    {
        [Header("Lerp")]
        [SerializeField] private float _lerpSpeed = 5;
        [SerializeField] private float _stopLerpSpeed = 5;
        [SerializeField] private float _stationaryPredictionAmount = 0;
        [SerializeField] private float _movementPredictionAmount = 5;
        
        [Header("Shake")] 
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _maxOffset;
        [SerializeField] private float _perlinNoiseMultiplier = 1;
        private bool _usePerlinNoise;

        private Vector3 _initPosition;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        
        private Vector3 _targetOffset;
        private Vector2 _targetMovementDirection;
        private Vector2 _LastMovementDirection;
        private float _movementPredictionPercent = 0;
        
        private Vector3 TargetPos => GameManager.Instance.Player.transform.position;

        private const int ANGLE_PERLIN_SEED = 100;
        private const int OFFSET_X_PERLIN_SEED = 200;
        private const int OFFSET_Y_PERLIN_SEED = 300;

        private void Start()
        {
            _targetOffset = _initPosition = transform.position;
        }

        private void LateUpdate()
        {
            transform.position = _targetPosition;
            transform.rotation = _targetRotation;
        }

        private void FixedUpdate()
        {
            _targetMovementDirection = GameManager.Instance.Player.MovementDirection;
            
            if (!GameManager.Instance.Player.IsAccelerating)
            {
                _movementPredictionPercent = Mathf.Lerp(_movementPredictionPercent, 0, _stopLerpSpeed * Time.deltaTime);
            }
            else
            {
                _movementPredictionPercent = GameManager.Instance.Player.SpeedNormalize * _movementPredictionAmount;
                _LastMovementDirection = GameManager.Instance.Player.AimDirection;
            }

            // TODO: Only move if accelerating?
            // TODO: Weight system?
            UpdateTargetOffsetByPrediction(_targetMovementDirection, _movementPredictionPercent, _LastMovementDirection, _stationaryPredictionAmount);

            var pos = TargetPos + _targetOffset;
            //Ensure Z-Offset
            pos.z = _initPosition.z;
            
            _targetPosition = pos;
            _targetRotation = Quaternion.identity;
            
            ApplyShake(GameManager.Instance.ShakeValue);
        }

        private void UpdateTargetOffsetByPrediction( Vector2 predictionDirection, float predictionAmount, Vector2 constantOffset, float constantOffsetAmount)
        {
            if (GameManager.Instance.GetConfigValue(EConfigKey.CameraPrediction))
            {
                var offset = constantOffset * constantOffsetAmount + predictionDirection * predictionAmount;
                _targetOffset = Vector3.Lerp(_targetOffset, offset, _lerpSpeed * Time.deltaTime);
            }
            else
            {
                _targetOffset = Vector3.zero;
            }
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
