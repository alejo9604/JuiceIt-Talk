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
        public struct TileBiomeGroup
        {
            public Sprite Sprite;
            [Range(0, 100)] public int Weight;
        }
        
        [Serializable]
        public struct TileGroupWeight
        {
            public string Name;
            [Range(0, 100)] public int Height;
            public TileBiomeGroup[] BiomeGroups;
        }
        
        [Space]
        public float TileSize = 0.7f; 
        public float Scale = 1;

        public float Size => TileSize * Scale;

        [Space] 
        public Tile TilePrefab;
        public TileGroupWeight[] TilesGroupsWeights = Array.Empty<TileGroupWeight>();

       
        public Sprite GetTileByHeightValue(float height, float biomeValue, bool useBiome = true)
        {
            foreach (var t in TilesGroupsWeights)
            {
                if (height <= t.Height * 0.01f)
                {
                    if(useBiome)
                        return GetRandomBiomeSprite(t.BiomeGroups, biomeValue);
                    return GetDefaultBiomeSprite(t.BiomeGroups);
                }
            }

            return null;
        }
        
        private Sprite GetRandomBiomeSprite(TileBiomeGroup[] biomeGroups, float biomeRanValue)
        {
            if (biomeGroups == null || biomeGroups.Length == 0)
                return null;
            
            float weightSum = 0f;
            foreach (var t in biomeGroups)
                weightSum += t.Weight;

            if (weightSum <= 0f)
                return null;
        
            float roll = Mathf.Clamp01(biomeRanValue) * weightSum;
            
            foreach (var t in biomeGroups)
            {
                roll -= t.Weight;
                if (roll <= 0f)
                    return t.Sprite;
            }
        
            return null;
        }

        private Sprite GetDefaultBiomeSprite(TileBiomeGroup[] biomeGroups)
        {
            if (biomeGroups == null || biomeGroups.Length == 0)
                return null;

            return biomeGroups[0].Sprite;
        }
        
    }
}
