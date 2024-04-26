using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class GameDelegates : MonoBehaviour
    {
        public event Action OnResetLevel;
        public void EmitOnResetLevel() => OnResetLevel?.Invoke();
        
        public event Action<EConfigKey> OnConfigUpdated;
        public void EmitOnConfigUpdated(EConfigKey key) => OnConfigUpdated?.Invoke(key);
        
        public event Action AllConfigUpdated;
        public void EmitAllConfigUpdated() => AllConfigUpdated?.Invoke();
        
        public event Action<Vector3> OnPlayerHit;
        public void EmitOnPlayerHit(Vector3 pos) => OnPlayerHit?.Invoke(pos);
        
        public event Action OnEnemyDeath;
        public void EmitOnEnemyDeath() => OnEnemyDeath?.Invoke();
        
    }
}