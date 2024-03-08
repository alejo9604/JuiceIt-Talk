using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _lifeTime = 3;
    
    private float _timeToDisable = -1;

    private void Start()
    {
        _timeToDisable = Time.time + _lifeTime;
    }

    void Update()
    {
        if(_timeToDisable < 0) {
            return;
        }

        if(_timeToDisable < Time.time) {
            _timeToDisable = -1;
            DestroyProjectile();
        }
        
        float moveDistance = _speed * Time.deltaTime;
        transform.Translate(Vector3.up * moveDistance);
    }
    
    private void DestroyProjectile()
    {
        Destroy(gameObject);
        //PoolManager.Destroy(gameObject);
    }
}
