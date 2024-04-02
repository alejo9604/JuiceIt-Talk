using System;
using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [CreateAssetMenu(menuName = "Config/Juice", fileName = "JuiceConfig")]
    public class JuiceConfigSO : ScriptableObject
    {
        [SerializeReference, SubclassSelector] // SubclassSelector from https://github.com/mackysoft/Unity-SerializeReferenceExtensions 
        public List<ConfigValue> EnableSequence = new();

        [Header("Impact Pause")]
        [Range(0, 1)] public float ImpactPauseDurationFramesPercent = 0.1f;
        public int ImpactPauseDurationMaxFrames = 15;

        public T GetValue<T>(EConfigKey key)
        {
            foreach (ConfigValue configValue in EnableSequence)
            {
                if(configValue.Key == key && configValue is ConfigValue<T> value)
                    return value.GetValue();
            }

            return default;
        }
        
        [ContextMenu("Reset to Defaults")]
        public void ResetToDefault()
        {
            foreach (ConfigValue configValue in EnableSequence)
                configValue.Reset();
        }

        [ContextMenu("Enable All")]
        public void ActiveAll()
        {
            foreach (ConfigValue configValue in EnableSequence)
                configValue.FullActive();
        }

        public void TryNext(EConfigKey key)
        {
            foreach (ConfigValue configValue in EnableSequence)
            {
                if(configValue.Key == key && configValue is IConfigOption option)
                    option.Next();
            }
        }
        
        public void TryPrev(EConfigKey key)
        {
            foreach (ConfigValue configValue in EnableSequence)
            {
                if(configValue.Key == key && configValue is IConfigOption option)
                    option.Prev();
            }
        }
    }
}