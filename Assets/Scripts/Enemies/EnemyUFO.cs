using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class EnemyUFO : Enemy
    {
        [Space]
        [SerializeField] private float _maxSpeed = 5;
        [SerializeField] private float _maxSeekForce = 0.25f;
        
        [Header("Body")]
        [SerializeField] private Transform _body;
        [SerializeField] private float _bodyMaxDistance = 0.5f;
        [SerializeField] private float _bodySpeed = 5;
        [SerializeField] private float _bodyRefreshTime = 1f;
        [SerializeField] private float _bodyRotSpeedDeg = 5;
        
        private Vector3 _velocity;
        private Vector3 _acceleration;

        private Vector3 _targetBodyDirection;
        private float _nextBodyPositionRefresh;
        
        private void ApplyForce(Vector3 force)
        {
            //force /= mass;
            _acceleration += force;
        }
        
        private void Update()
        {
            Seek(base.Player);
            Move();
            MoveBody();
        }

        private void Move()
        {
            _velocity += _acceleration * Time.deltaTime;
            if (_velocity.sqrMagnitude > _maxSpeed * _maxSpeed)
                _velocity = _velocity.normalized * _maxSpeed;
            
            transform.position += _velocity * Time.deltaTime;

            Debug.DrawLine(transform.position, transform.position + _velocity, Color.red);
            
            _acceleration *= 0;
        }

        private void MoveBody()
        {
            if (Time.time > _nextBodyPositionRefresh || _body.localPosition.sqrMagnitude > _bodyMaxDistance * _bodyMaxDistance)
            {
                _targetBodyDirection = (Random.insideUnitCircle * _bodyMaxDistance).normalized;
                _nextBodyPositionRefresh = Time.time + _bodyRefreshTime;
            }

            _body.localPosition += _targetBodyDirection * _bodySpeed * Time.deltaTime;
            _body.Rotate(Vector3.forward, _bodyRotSpeedDeg * Time.deltaTime);
            
        }

        private void Seek(Transform target)
        {
            Vector3 desired = (target.position - transform.position).normalized * _maxSpeed;
            Vector3 steer = desired - _velocity;
            if(steer.sqrMagnitude > _maxSeekForce * _maxSeekForce)
                steer = steer.normalized * _maxSeekForce;
            
            Debug.DrawLine(transform.position, transform.position + desired, Color.green);
            Debug.DrawLine(transform.position + _velocity, transform.position + _velocity + steer, Color.blue);

            ApplyForce(steer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _bodyMaxDistance);
        }
    }
}