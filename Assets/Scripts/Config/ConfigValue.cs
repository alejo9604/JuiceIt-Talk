using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public enum EConfigKey
    {
        Trail,
        
        ShootingMovementRestriction,
        ShootingAccuracy,
        
        ImpactPause,
        //ImpactPauseDuration,
        
        ProjectileSpeed,
        ProjectileRateFire,
        ProjectilePrefab,
    }
    
    [Serializable]
    public abstract class ConfigValue
    {
        public EConfigKey Key;
        [TextArea]
        public string Comments;

        public abstract void Reset();
        public abstract void FullActive();
    }

    public interface IConfigOption
    {
        public void Next();
        public void Prev();
    }

    //=================================================================================================
    // Generics
    //=================================================================================================
    public abstract class ConfigValue<T> : ConfigValue
    {
        public abstract T GetValue();
    }
    
    public abstract class ConfigOptionsValue<T> : ConfigValue<T>, IConfigOption
    {
        public T[] Options;
        [NonSerialized] 
        private int _currentSelected = 0;

        public void Next() => _currentSelected = Mathf.Min(_currentSelected + 1, Options.Length - 1);
        public void Prev() => _currentSelected = Mathf.Max(0, _currentSelected - 1);
        
        public override T GetValue() => Options[_currentSelected];
        public override void Reset() => _currentSelected = 0;
        public override void FullActive() => _currentSelected = Options.Length - 1;
    }

    //=================================================================================================
    // Implementations
    //=================================================================================================
    [Serializable]
    public class ConfigEnabledValue : ConfigValue<bool>
    {
        public bool Enabled;
        public override bool GetValue() => Enabled;
        public override void Reset() => Enabled = false;
        public override void FullActive() => Enabled = true;
    }
    
    [Serializable]
    public class ConfigFloatValue : ConfigValue<float>
    {
        public bool Enabled;
        public float ActiveValue;
        public float DisableValue;
        public override float GetValue() => Enabled ? ActiveValue : DisableValue;
        public override void Reset() => Enabled = false;
        public override void FullActive() => Enabled = true;
    }

    [Serializable]
    public class ConfigFloatOptionsValue : ConfigOptionsValue<float> { }
    
    [Serializable]
    public class ConfigPrefabOptionsValue : ConfigOptionsValue<Projectile> { }
    
}