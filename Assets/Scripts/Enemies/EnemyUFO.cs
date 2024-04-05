using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class EnemyUFO : Enemy
    {
        [SerializeField] private float _maxSpeed = 5;
        [SerializeField] private float _maxSeekForce = 0.25f;

        private Vector3 _velocity;
        private Vector3 _acceleration;
        
        private void ApplyForce(Vector3 force)
        {
            //force /= mass;
            _acceleration += force;
        }
        
        private void Update()
        {
            Seek(base.Player);
            Move();
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
    }
}