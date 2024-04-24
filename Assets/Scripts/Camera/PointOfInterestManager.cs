using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class PointOfInterestManager : MonoBehaviour
    {
        public HashSet<PointOfInterest> inRangeObjectsHashSet = new();
        public List<PointOfInterest> inRangeObjects = new();
        
        [SerializeField] private float _maxRange;
        
        private void Awake()
        {
            PointOfInterest.OnPointOfInterestSpawnEvent += OnPointOfInterestSpawn;
            PointOfInterest.OnPointOfInterestDespawnEvent += OnPointOfInterestDespawn;
        }

        private void OnDestroy()
        {
            PointOfInterest.OnPointOfInterestSpawnEvent -= OnPointOfInterestSpawn;
            PointOfInterest.OnPointOfInterestDespawnEvent -= OnPointOfInterestDespawn;
        }

        // private void Update()
        // {
        //     CalculateCenter();
        // }

        public Vector3 GetCenter(Vector3? refPoint)
        {
            
            Vector3 initPoint = refPoint ?? Vector3.zero;
            float sqrMaxRange = _maxRange * _maxRange;
            
            Vector3 center = initPoint;
            float totalWeight = refPoint.HasValue ? 1 : 0;
            
            foreach (PointOfInterest point in inRangeObjects)
            {
                if((initPoint - point.transform.position).sqrMagnitude >sqrMaxRange)
                    continue;
                
                center += point.transform.position * point.Weight;
                totalWeight += point.Weight;
            }

            if(totalWeight > 0)
                center /= totalWeight;
            center.z = 0;

            return center;
        }
        
        private void OnPointOfInterestSpawn(PointOfInterest point)
        {
            if(inRangeObjectsHashSet.Contains(point))
                return;

            inRangeObjectsHashSet.Add(point);
            inRangeObjects = inRangeObjectsHashSet.ToList();
        }

        private void OnPointOfInterestDespawn(PointOfInterest point)
        {
            inRangeObjectsHashSet.Remove(point);
            inRangeObjects = inRangeObjectsHashSet.ToList();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, _maxRange);
        }
    }
}