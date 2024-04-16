using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy _enemyPrefab;
        
        [Space]
        [SerializeField] private Camera _cam;

        [Space]
        [SerializeField] private float _initDelay = 3f;
        [SerializeField] private float _nextEnemyDelay = 1.2f;
        [SerializeField] private int _maxEnemies = 5;
        [SerializeField] private float _maxEnemiesSpawnTime = 5;
        [SerializeField] private Vector2 _minSpawnZone;
        [SerializeField] private Vector2 _maxSpawnZone;

        private List<Enemy> _enemies = new();
        private float _nextEnemySpawn = 0;
        private float _progressiveMaxEnemiesSpawnTime;
        private int _progressiveMaxEnemies;

        private void Start()
        {
            if (_cam == null)
                _cam = Camera.main;
            _nextEnemySpawn = Time.time + _initDelay;
            _progressiveMaxEnemiesSpawnTime = Time.time + _maxEnemiesSpawnTime;
        }

        private void Update()
        {
            float spawn_delta = Mathf.Clamp(1 - (_progressiveMaxEnemiesSpawnTime - Time.time) / _maxEnemiesSpawnTime, 0, 1);
            int calculated_max = Mathf.CeilToInt((_maxEnemies - 3) * spawn_delta);
            
            _progressiveMaxEnemies = 3 + calculated_max;
            
            SpawnEnemiesOverTime();
        }

        private void SpawnEnemiesOverTime()
        {
            if ( _enemies.Count >= _progressiveMaxEnemies || _nextEnemySpawn > Time.time )
                return;
            
            float worldHalfHeight = _cam.orthographicSize;
            float worldHalfWidth = worldHalfHeight * _cam.aspect;

            int zone = Random.Range(1, 5); //4 zones
            Vector3 offset = Vector3.zero;
            switch (zone)
            {
                case 1:
                    offset = GetRandomPointInZone(-(worldHalfWidth + _maxSpawnZone.x), worldHalfWidth + _maxSpawnZone.x, worldHalfHeight + _minSpawnZone.y, worldHalfHeight + _maxSpawnZone.y);
                    break;
                case 2:
                    offset = GetRandomPointInZone(-(worldHalfWidth + _maxSpawnZone.x), worldHalfWidth + _maxSpawnZone.x, -(worldHalfHeight + _minSpawnZone.y), -(worldHalfHeight + _maxSpawnZone.y));
                    break;
                case 3:
                    offset = GetRandomPointInZone(worldHalfWidth + _minSpawnZone.x, worldHalfWidth +_maxSpawnZone.x, -(worldHalfHeight + _maxSpawnZone.y), worldHalfHeight + _maxSpawnZone.y);
                    break;
                default:
                    offset = GetRandomPointInZone(-(worldHalfWidth + _minSpawnZone.x), -(worldHalfWidth +_maxSpawnZone.x), -(worldHalfHeight + _maxSpawnZone.y), worldHalfHeight + _maxSpawnZone.y);
                    break;
            }


            Enemy enemy = Instantiate(_enemyPrefab, _cam.transform.position + offset, Quaternion.identity);
            _enemies.Add(enemy);
            
            _nextEnemySpawn = Time.time + _nextEnemyDelay;
        }

        private Vector2 GetRandomPointInZone(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

        
        private void OnDrawGizmosSelected()
        {
        
            float worldHeight = _cam.orthographicSize * 2;
            float worldWidth = worldHeight * _cam.aspect;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_cam.transform.position, new Vector3(worldWidth + _minSpawnZone.x * 2, worldHeight + _minSpawnZone.y * 2, 0.1f));
            Gizmos.DrawWireCube(_cam.transform.position, new Vector3(worldWidth + _maxSpawnZone.x * 2, worldHeight + _maxSpawnZone.y * 2, 0.1f));
        }
    }
}
