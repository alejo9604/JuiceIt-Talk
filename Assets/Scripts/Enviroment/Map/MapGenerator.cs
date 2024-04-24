using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AllieJoe.JuiceIt
{
    [Serializable]
    public struct MapGeneratorTuning
    {
        public int mapSize;
        public float noiseScale;

        [Min(0)]
        public int octaves;
        [Range(0,1)]
        public float persistance;
        [Min(1)]
        public float lacunarity;

        public int seed;
        public int biomeSeed;
        public Vector2 offset;
        
    }
    
    public class MapGenerator : MonoBehaviour
    {
        public enum DrawMode {NoiseMap, ColourMap};
        public DrawMode drawMode;

        public MapGeneratorTuning Tuning;

        public bool autoUpdate;

        public TerrainType[] regions;
        
        
        public void GenerateMap() {
            float[,] noiseMap = Noise.GenerateNoiseMap (Tuning.mapSize, Tuning.mapSize, Tuning.seed, Tuning.noiseScale, Tuning.octaves, Tuning.persistance, Tuning.lacunarity, Tuning.offset);

            Color[] colourMap = new Color[Tuning.mapSize * Tuning.mapSize];
            for (int y = 0; y < Tuning.mapSize; y++) {
                for (int x = 0; x < Tuning.mapSize; x++) {
                    float currentHeight = noiseMap [x, y];
                    for (int i = 0; i < regions.Length; i++) {
                        if (currentHeight <= regions [i].height) {
                            colourMap [y * Tuning.mapSize + x] = regions [i].colour;
                            break;
                        }
                    }
                }
            }

            MapDisplay display = FindObjectOfType<MapDisplay> ();
            if (drawMode == DrawMode.NoiseMap) {
                display.DrawTexture (TextureGenerator.TextureFromHeightMap (noiseMap));
            } else if (drawMode == DrawMode.ColourMap) {
                display.DrawTexture (TextureGenerator.TextureFromColourMap (colourMap, Tuning.mapSize, Tuning.mapSize));
            }
        }
    }
    
    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color colour;
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof (MapGenerator))]
    public class MapGeneratorEditor : Editor {

        public override void OnInspectorGUI() {
            MapGenerator mapGen = (MapGenerator)target;

            if (DrawDefaultInspector ()) {
                if (mapGen.autoUpdate) {
                    mapGen.GenerateMap ();
                }
            }

            if (GUILayout.Button ("Generate")) {
                mapGen.GenerateMap ();
            }
        }
    }

#endif
}
