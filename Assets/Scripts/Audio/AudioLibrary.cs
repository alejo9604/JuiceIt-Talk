using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    [Serializable]
    public struct AudioTuning
    {
        public string Key;
        public AudioClip[] Clips;
    }
    
    [CreateAssetMenu(menuName = "Config/AudioLibrary", fileName = "AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        public AudioTuning[] Audios;

        public AudioClip GetClip(string key)
        {
            foreach (var tuning in Audios)
                if (tuning.Key == key)
                    return tuning.Clips[Random.Range(0, tuning.Clips.Length)];

            return null;
        }
        
        //KEYS
        public const string PLAYER_SHOOT = "PlayerShoot";
        public const string PLAYER_ACCELERATE = "PlayerAccelerate";
        public const string PLAYER_HIT = "Player";
        public const string PROJECTILE_IMPACT = "ProjectileImpact";
        public const string ENEMY_DEATH = "EnemyDeath";

        public static readonly string[] TUNING_KEYS = 
            { PLAYER_SHOOT, PLAYER_ACCELERATE, PLAYER_HIT, PROJECTILE_IMPACT, ENEMY_DEATH };
    }
}
