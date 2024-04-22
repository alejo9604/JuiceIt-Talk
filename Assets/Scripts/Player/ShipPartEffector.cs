using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class ShipPartEffector : MonoBehaviour
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _rotation;
        [Space]
        [SerializeField] private float _moveSpeed = 8;
        [SerializeField] private float _rotSpeed = 8;

        private Vector3 _initLocalPosition;
        private float _initZLocalRotation;
        private bool _active = false;

        private Vector3 _targetLocalPosition;
        private Quaternion _targetLocalRotation;
        
        private void Start()
        {
            _initLocalPosition = transform.localPosition;
            _initZLocalRotation = transform.localEulerAngles.z;
        }

        private void Update()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetLocalPosition, _moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetLocalRotation, _rotSpeed * Time.deltaTime);
        }

        public void Apply(Vector2 normalizeVelocity)
        {
            if (!_active)
            {
                _targetLocalPosition = _initLocalPosition;
                _targetLocalRotation = Quaternion.Euler(new Vector3(0, 0, _initZLocalRotation));
                return;
            }
            
            _targetLocalPosition = _initLocalPosition + new Vector3(_offset.x * normalizeVelocity.x, _offset.y * normalizeVelocity.y);
            _targetLocalRotation = Quaternion.Euler(0, 0, _initZLocalRotation + _rotation* normalizeVelocity.x);
        }

        public void SetActive(bool active) => _active = active;
    }
}
