﻿using System;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [Serializable]
    public abstract class ConfigValue
    {
        [HideInInspector] public string name;
        public EConfigKey Key;
        public string Label;
        [TextArea]
        public string Comments;

        public abstract void Reset();
        public abstract void FullActive();

        public virtual string GetLabel() => Label;
        public virtual void OnValidate()
        {
            name = Key.ToString();
        }
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
        
        public bool UseToggleGroup();
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
        public string[] OptionsLabel;
        [NonSerialized] 
        private int _currentSelected = 0;

        [SerializeField]
        private bool _useToggleGroup = false; 

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

        public override string GetLabel()
        {
            if (OptionsLabel == null || OptionsLabel.Length == 0)
                return base.GetLabel();
            return OptionsLabel[_currentSelected];
        }

        public virtual bool UseToggleGroup() => _useToggleGroup;

        public override void OnValidate()
        {
            base.OnValidate();
            if(Options == null)
                return;
            if (OptionsLabel == null)
                OptionsLabel = new string[Options.Length];
            else if(OptionsLabel.Length != Options.Length)
                Array.Resize(ref OptionsLabel, Options.Length);
        }
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
    
    [Serializable]
    public class ConfigSelectWeaponOptionsValue : ConfigSelectOptionsValue<WeaponTuning.EType> { }
    
    [Serializable]
    public class ConfigSelectTileSpawnTweenOptionsValue : ConfigSelectOptionsValue<TileSpawnTuning> { }
    
}