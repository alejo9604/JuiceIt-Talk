﻿using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class ConfigUI : UIPanel
    {
        [SerializeField] private RectTransform _container;
        [Space(20)]
        [SerializeField] private ConfigOptionUI _optionPrefab;
        [SerializeField] private ConfigOptionEnabledUI _optionEnabledPrefab;
        [SerializeField] private ConfigOptionSelectUI _optionSelectPrefab;

        private List<ConfigOptionUI> _options = new();
        
        public void Init(List<ConfigValue> configValues)
        {
            foreach (var config in configValues)
            {
                if(config is IConfigEnabledOption)
                    _options.Add(CreateEnabledOption(config));
                else if(config is IConfigSelectOption)
                    _options.Add(CreateSelectOption(config));
            }
        }
        
        public void Refresh(ConfigValue config)
        {
            foreach (ConfigOptionUI optionUI in _options)
            {
                if(optionUI.ConfigKey == config.Key)
                    optionUI.Refresh();
            }
        }
        
        private ConfigOptionEnabledUI CreateEnabledOption(ConfigValue configValue)
        {
            var option = Instantiate(_optionEnabledPrefab, _container);
            option.Init(configValue);
            return option;
        }
        
        private ConfigOptionSelectUI CreateSelectOption(ConfigValue configValue)
        {
            var option = Instantiate(_optionSelectPrefab, _container);
            option.Init(configValue);
            return option;
        }
    }
}