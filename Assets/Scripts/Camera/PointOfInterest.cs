using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class PointOfInterest : MonoBehaviour
    {
        public delegate void PointOfInterestSpawnEvent(PointOfInterest point);
        public static PointOfInterestSpawnEvent OnPointOfInterestSpawnEvent;
        
        public delegate void PointOfInterestDespawnEvent(PointOfInterest point);
        public static PointOfInterestDespawnEvent OnPointOfInterestDespawnEvent;


        [Range(0, 1)] public float Weight;
        
        public void OnEnable()
        {
            OnPointOfInterestSpawnEvent?.Invoke(this);
        }

        private void OnDisable()
        {
            OnPointOfInterestDespawnEvent?.Invoke(this);
        }
    }
}