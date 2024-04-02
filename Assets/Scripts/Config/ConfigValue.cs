﻿using System;
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
        public string Label;
        [TextArea]
        public string Comments;

        public abstract void Reset();
        public abstract void FullActive();
    }

    public interface IConfigEnabledOption
    {
        public void Set(bool enabled);
        public bool IsEnable();
    }

    public interface IConfigSelectOption
    {
        public int CurrentSelected();
        public void Next();
        public void Prev();
        public void SetSelected(int index);
        public int Max();
    }

    //=================================================================================================
    // Generics
    //=================================================================================================
    public abstract class ConfigValue<T> : ConfigValue
    {
        public abstract T GetValue();
    }
    
    public abstract class ConfigSelectOptionsValue<T> : ConfigValue<T>, IConfigSelectOption
    {
        public T[] Options;
        [NonSerialized] 
        private int _currentSelected = 0;

        public int CurrentSelected() => _currentSelected;
        public void Next() => _currentSelected = Mathf.Min(_currentSelected + 1, Options.Length - 1);
        public void Prev() => _currentSelected = Mathf.Max(0, _currentSelected - 1);
        public void SetSelected(int index)
        {
            _currentSelected = index;
            _currentSelected = Mathf.Clamp(_currentSelected, 0, Options.Length - 1);
        }
        public int Max() => Options.Length - 1;

        public override T GetValue() => Options[_currentSelected];
        public override void Reset() => _currentSelected = 0;
        public override void FullActive() => _currentSelected = Options.Length - 1;
    }

    //=================================================================================================
    // Implementations
    //=================================================================================================
    [Serializable]
    public class ConfigEnabledValue : ConfigValue<bool>, IConfigEnabledOption
    {
        public bool Enabled;
        public override bool GetValue() => Enabled;
        public override void Reset() => Enabled = false;
        public override void FullActive() => Enabled = true;
        public void Set(bool enabled) => Enabled = enabled;
        public bool IsEnable() => Enabled;
    }
    
    [Serializable]
    public class ConfigFloatValue : ConfigValue<float>, IConfigEnabledOption
    {
        public bool Enabled;
        public float ActiveValue;
        public float DisableValue;
        public override float GetValue() => Enabled ? ActiveValue : DisableValue;
        public override void Reset() => Enabled = false;
        public override void FullActive() => Enabled = true;
        public void Set(bool enabled) => Enabled = enabled;
        public bool IsEnable() => Enabled;
    }

    [Serializable]
    public class ConfigSelectFloatOptionsValue : ConfigSelectOptionsValue<float> { }
    
    [Serializable]
    public class ConfigSelectPrefabOptionsValue : ConfigSelectOptionsValue<Projectile> { }
    
}