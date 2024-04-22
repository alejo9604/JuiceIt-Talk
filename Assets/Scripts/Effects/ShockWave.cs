using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class ShockWave : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _shockWave;
        [SerializeField] private float _shockWaveTime = 0.5f;
        [SerializeField, Range(-.1f, 1f)] private float _shockStartValue = 0.01f;
        
        [Space]
        [SerializeField] private Camera _cam;
        [SerializeField] private Vector2 _padding;
        
        private Coroutine _coroutine;
        private Material _material;
        
        private Vector2 _camSize;
        
        private static readonly int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
        private static readonly int _ringSpawnPosition = Shader.PropertyToID("_RingSpawnPosition");

        private void Start()
        {
            _material = _shockWave.material;
            _shockWave.gameObject.SetActive(false);
            
            if(_cam == null)
                _cam = Camera.main;

            _camSize = new Vector2(_cam.orthographicSize * 2 * _cam.aspect, _cam.orthographicSize * 2);
            SetScale();

            GameManager.Instance.GameDelegates.OnPlayerHit += Play;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnPlayerHit -= Play;
        }

        private void Play(Vector3 targetPos)
        {
           if(!GameManager.Instance.GetConfigValue(EConfigKey.ShockWave))
               return;
           if(_coroutine != null)
                StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(PlayShockWave(targetPos, startPos: _shockStartValue));
        }

        private void SetScale()
        {
            Vector3 scale = Vector3.one;
            scale.x = _camSize.x + _padding.x;
            scale.y = _camSize.y + _padding.y;
            _shockWave.transform.localScale = scale;
        }
        
        private Vector2 GetNormalizePosition(Vector3 targetPos)
        {
            Vector3 dif = targetPos - transform.position;
            Vector2 normalizePosition = Vector2.zero;
            normalizePosition.x = Mathf.Clamp01(dif.x / (_cam.orthographicSize * 2 *  _cam.aspect) + 0.5f);
            normalizePosition.y = Mathf.Clamp01(dif.y / (_cam.orthographicSize * 2) + 0.5f);
            return normalizePosition;
        }

        IEnumerator PlayShockWave(Vector3 targetPos, float startPos = -0.1f, float endPos = 1f)
        {
            _shockWave.gameObject.SetActive(true);
            Vector2 normalizePos = GetNormalizePosition(targetPos);
            //TODO: Use property block
            _material.SetFloat(_waveDistanceFromCenter, startPos);
            _material.SetVector(_ringSpawnPosition, new Vector4(normalizePos.x, normalizePos.y));

            float lerpedAmount = 0;
            float elapsedTime = 0;
            while (elapsedTime < _shockWaveTime)
            {
                elapsedTime += Time.deltaTime;
                lerpedAmount = Mathf.Lerp(startPos, endPos, elapsedTime / _shockWaveTime);
                _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);

                yield return null;
            }
            _shockWave.gameObject.SetActive(false);
        }
        
    }
}
