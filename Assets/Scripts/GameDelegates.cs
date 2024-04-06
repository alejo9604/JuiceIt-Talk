using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class GameDelegates : MonoBehaviour
    {
        public event Action<EConfigKey> OnConfigUpdated;
        public void EmitOnConfigUpdated(EConfigKey key) => OnConfigUpdated?.Invoke(key);
    }
}