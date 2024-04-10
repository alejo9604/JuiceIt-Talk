using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class Camera : MonoBehaviour
    {
        [Header("Lerp")]
        [SerializeField] private float _lerpSpeed = 5;
        [SerializeField] private float _stopLerpSpeed = 5;
        [SerializeField] private float _stationaryPredictionAmount = 0;
        [SerializeField] private float _movementPredictionAmount = 5;
        
        [Header("Shake")] 
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _maxOffset;

        private Vector3 _initPosition;
        private Vector3 _targetOffset;
        
        private Vector2 _targetMovementDirection;
        private Vector2 _LastMovementDirection;
        
        private Vector3 TargetPos => GameManager.Instance.Player.transform.position;

        private void Start()
        {
            _targetOffset = _initPosition = transform.position;
        }

        private void FixedUpdate()
        {
            _targetMovementDirection = GameManager.Instance.Player.MovementDirection;
            float movementPredictionPercent = 0;
            if (!GameManager.Instance.Player.IsAccelerating)
            {
                movementPredictionPercent = Mathf.Lerp(movementPredictionPercent, 0, _stopLerpSpeed * Time.deltaTime);
            }
            else
            {
                movementPredictionPercent = GameManager.Instance.Player.SpeedNormalize * _movementPredictionAmount;
                _LastMovementDirection = GameManager.Instance.Player.AimDirection;
            }

            // TODO: Only move if accelerating?
            // TODO: Weight system?
            UpdateTargetOffsetByPrediction(_targetMovementDirection, movementPredictionPercent, _LastMovementDirection, _stationaryPredictionAmount);

            var pos = TargetPos + _targetOffset;
            //Ensure Z-Offset
            pos.z = _initPosition.z;
            
            transform.position = pos;
            transform.rotation = Quaternion.identity;
            
            ApplyShake(GameManager.Instance.ShakeValue);
        }

        private void UpdateTargetOffsetByPrediction( Vector2 predictionDirection, float predictionAmount, Vector2 constantOffset, float constantOffsetAmount)
        {
            var offset = constantOffset * constantOffsetAmount;
            if (GameManager.Instance.GetConfigValue<bool>(EConfigKey.CameraPrediction))
            {
                offset += predictionDirection * predictionAmount;
                _targetOffset = Vector3.Lerp(_targetOffset, offset, _lerpSpeed * Time.deltaTime);
            }
            else
            {
                _targetOffset = offset;
            }
        }
        
        private void ApplyShake(float shake)
        {
            if(shake <= 0)
                return;
            
            float angle = _maxAngle * shake * GetRandomFloatNegOneToOne();
            float offsetX = _maxOffset * shake * GetRandomFloatNegOneToOne();
            float offsetY = _maxOffset * shake * GetRandomFloatNegOneToOne();

            Vector3 rot = transform.eulerAngles;
            rot.z += angle;

            Vector3 pos = transform.position;
            pos.x += offsetX;
            pos.y += offsetY;

            transform.eulerAngles = rot;
            transform.position = pos;
        }
        

        private float GetRandomFloatNegOneToOne()
        {
            return Random.Range(-1f, 1f);
        }
    }
}
