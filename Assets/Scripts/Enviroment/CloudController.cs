using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class CloudController : MonoBehaviour
    {
        
        [SerializeField] private Cloud _cloudPrefab;
        
        [Space]
        [SerializeField] private Transform _container;
        
        [Space] 
        [SerializeField] private int _maxClouds = 5;
        [SerializeField] private AnimationCurve _cloudHeight;
        [SerializeField] private Vector2 _cloudScale = new Vector2(1, 2);
        [SerializeField] private Vector2 _cloudSpeed = new Vector2(10, 5);

        [Space]
        [SerializeField] private Camera _cam;
        [SerializeField] private Vector2 _safeZoneOffset;

        private bool _spawnActive = false;
        private IObjectPool<Cloud> _cloudPool;
        private readonly List<Cloud> _clouds = new();

        private void Start()
        {
            if (_cam == null)
                _cam = Camera.main;
            
            _cloudPool = new ObjectPool<Cloud>(
                () => Instantiate(_cloudPrefab, _container),
                tile => tile.gameObject.SetActive(true),
                tile => tile.gameObject.SetActive(false),
                tile => Destroy(tile.gameObject),
                defaultCapacity: _maxClouds);

            _spawnActive = GameManager.Instance.GetConfigValue(EConfigKey.Clouds);
            
            GenerateInitGroup();
            GameManager.Instance.GameDelegates.OnResetLevel += OnResetLevel;
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated += RefreshConfig;
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnResetLevel -= OnResetLevel;
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated -= RefreshConfig;
        }

        private void Update()
        {
            Vector2 center = _cam.transform.position;
            float maxSqrDistance = Mathf.Max(_safeZoneOffset.x, _safeZoneOffset.y) * 0.5f; //Half to count from the center
            maxSqrDistance *= maxSqrDistance;
            for (int i = 0; i < _clouds.Count; i++)
            {
                if ((center - (Vector2)_clouds[i].transform.position).sqrMagnitude > maxSqrDistance)
                {
                    _cloudPool.Release(_clouds[i]);
                    _clouds.RemoveAt(i);
                    i--;
                    continue;
                }
                
                _clouds[i].Move();
            }

            //Don nothing if spawn it's not enabled
            if(!_spawnActive)
                return;
            
            //Spawn new clouds
            int currentCount = _clouds.Count;
            for (int i = currentCount; i < _maxClouds; i++)
            {
                SpawnCloud(true);
            }
        }

        private void OnResetLevel()
        {
            ClearTiles();
            GenerateInitGroup();
        }
        
        private void OnConfigUpdated(EConfigKey key)
        {
            if (key == EConfigKey.Clouds)
            {
                RefreshConfig();
                OnResetLevel();
            }
        }

        private void RefreshConfig()
        {
            _spawnActive = GameManager.Instance.GetConfigValue(EConfigKey.Clouds);
        }
        
        private void GenerateInitGroup()
        {
            if(!_spawnActive) 
                return;
            
            for (int i = 0; i < _maxClouds; i++)
            {
                SpawnCloud(false);
            }
        }
        
        private void ClearTiles()
        {
            foreach (var c in _clouds)
            {
                _cloudPool.Release(c);
            }
            _clouds.Clear();
        }

        private void SpawnCloud(bool outsideCamera = true)
        {
            Cloud cloud = _cloudPool.Get();
            Vector3 position = GetRandomPos(outsideCamera);
            position.z = 0;
            cloud.transform.position = position;

            float height = _cloudHeight.Evaluate(Random.value);
            
            cloud.transform.localScale = Vector3.one * Mathf.Lerp(_cloudScale.x, _cloudScale.y, height);
            cloud.Height = height;
            cloud.Speed = Mathf.Lerp(_cloudSpeed.x, _cloudSpeed.y, height);
            cloud.MoveDirection = Random.insideUnitCircle.normalized;
            
            _clouds.Add(cloud);
        }

        private Vector3 GetRandomPos(bool outsideCamera = true)
        {
            Vector2 offset;
            if (outsideCamera)
                offset = GetRandomOffsetInRandomSafeZone();
            else
            {
                Vector2 safeZone = GetSafeZone() / 2;
                offset = new Vector2(Random.Range(-safeZone.x, safeZone.x), Random.Range(-safeZone.y, safeZone.y));
            }

            return _cam.transform.position + (Vector3) offset;
        }
        
        private Vector2 GetSafeZone()
        {
            float worldHeight = _cam.orthographicSize * 2;
            float worldWidth = worldHeight * _cam.aspect;

            return new Vector2(worldWidth, worldHeight) + _safeZoneOffset;
        }

        private Vector2 GetRandomOffsetInRandomSafeZone()
        {
            float worldHalfHeight = _cam.orthographicSize;
            float worldHalfWidth = worldHalfHeight * _cam.aspect;
            
            int zone = Random.Range(1, 5); //4 zones
            int padding = 5;
            switch (zone)
            {
                case 1:
                    return GetRandomPointInZone(
                        -(worldHalfWidth + _safeZoneOffset.x), worldHalfWidth + _safeZoneOffset.x, 
                        worldHalfHeight + padding, worldHalfHeight + _safeZoneOffset.y);
                case 2:
                    return GetRandomPointInZone(
                        -(worldHalfWidth + _safeZoneOffset.x), worldHalfWidth + _safeZoneOffset.x, 
                        -worldHalfHeight - padding, -(worldHalfHeight + _safeZoneOffset.y));
                case 3:
                    return GetRandomPointInZone(
                        worldHalfWidth + padding, worldHalfWidth + _safeZoneOffset.x, 
                        -(worldHalfHeight + _safeZoneOffset.y), worldHalfHeight + _safeZoneOffset.y);
                default:
                    return GetRandomPointInZone(
                        -worldHalfWidth - padding, -(worldHalfWidth + _safeZoneOffset.x), 
                        -(worldHalfHeight + _safeZoneOffset.y), worldHalfHeight + _safeZoneOffset.y);
            }
        }
        
        private Vector2 GetRandomPointInZone(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

        private void OnDrawGizmosSelected()
        {
            if (_cam == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_cam.transform.position, GetSafeZone());

        }
    }
}
