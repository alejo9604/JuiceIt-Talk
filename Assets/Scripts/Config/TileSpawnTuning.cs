using UnityEngine;
using DG.Tweening;

namespace AllieJoe.JuiceIt
{
    [CreateAssetMenu(menuName = "Config/TileSpawnTuning", fileName = "TileSpawnTuning")]
    public class TileSpawnTuning : ScriptableObject
    {
        [Header("Scale Animation")]
        public bool UseScale;
        public float ScaleFrom = 0;
        public float ScaleDelayPerDistance = 0;
        public float ScaleDuration = 0.1f;
        public Ease ScaleEase = Ease.Linear;

        [Header("Move Animation")] 
        public bool UseMove;
        public float MoveFrom = -.5f;
        public float MoveDelayPerDistance = 0;
        public float MoveDuration = 0.1f;
        public Ease MoveEase = Ease.Linear;
    }
}