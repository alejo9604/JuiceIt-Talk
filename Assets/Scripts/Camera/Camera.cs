using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class Camera : MonoBehaviour
    {
        [Header("Shake")] 
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _maxOffset;

        private Vector3 _targetPosition;

        private void Start()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            transform.position = _targetPosition;
            transform.rotation = Quaternion.identity;
            ApplyShake(GameManager.Instance.ShakeValue);
        }

        private void ApplyShake(float shake)
        {
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
