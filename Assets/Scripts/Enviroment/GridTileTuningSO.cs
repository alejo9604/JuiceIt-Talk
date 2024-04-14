using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    [CreateAssetMenu(menuName = "Config/GridTile", fileName = "GridTile")]
    public class GridTileTuningSO : ScriptableObject
    {
        [Serializable]
        public struct TileGroupWeight
        {
            public Sprite Sprite;
            [Range(0, 100)] public int Weight;
        }
        
        [Space]
        public Vector2 GridSize;
        public float TileSize = 0.7f; 
        public float Scale = 1;

        public float Size => TileSize * Scale;

        [Space] 
        public Tile TilePrefab;
        public TileGroupWeight[] TilesGroupsWeights = Array.Empty<TileGroupWeight>();

        public Sprite GetRandomTileSet()
        {
            List<float> weights   = new List<float>();
            float       weightSum = 0f;

            foreach (var t in TilesGroupsWeights)
            {
                weights.Add(t.Weight);
                weightSum += t.Weight;
            }

            if (weightSum <= 0f)
                return null;

            float roll = Random.value * weightSum;

            int i = 0;
            foreach (var t in TilesGroupsWeights)
            {
                roll -= weights[i++];
                if (roll <= 0f)
                    return t.Sprite;
            }

            return null;
        }
    }
}
