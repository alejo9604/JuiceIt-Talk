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
        [Range(0, 1)] public float ShootImpactPauseDurationInSec = 0.05f;
        [Range(0, 1)] public float PlayerImpactPauseDurationInSec = 0.15f;

        [Header("Shooting")]
        public bool ShootAccuracyPerCannon = false;
        public float ShootAccuracy = 4f;
        public float TraumaAddPerShoot = 0.2f;

        [Header("Background/Tiles")] 
        public float InitExtraDelay = 0.1f;

        public T GetValue<T>(EConfigKey key)
        {
            ConfigValue configValue = GetConfig(key);
            if(configValue is ConfigValue<T> value)
                return value.GetValue();

            return default;
        }
        
        public string GetLabel(EConfigKey key)
        {
            return GetConfig(key).GetLabel();
        }
        
        private ConfigValue GetConfig(EConfigKey key)
        {
            foreach (ConfigValue configValue in EnableSequence)
                if(configValue.Key == key)
                    return configValue;

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

        // public void TryNext(EConfigKey key)
        // {
        //     foreach (ConfigValue configValue in EnableSequence)
        //     {
        //         if(configValue.Key == key && configValue is IConfigSelectOption option)
        //             option.Next();
        //     }
        // }
        //
        // public void TryPrev(EConfigKey key)
        // {
        //     foreach (ConfigValue configValue in EnableSequence)
        //     {
        //         if(configValue.Key == key && configValue is IConfigSelectOption option)
        //             option.Prev();
        //     }
        // }

        private void OnValidate()
        {
            foreach (var config in EnableSequence)
                config?.OnValidate();
        }
    }
}