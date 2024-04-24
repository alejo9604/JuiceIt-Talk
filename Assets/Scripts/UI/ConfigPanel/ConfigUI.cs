using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AllieJoe.JuiceIt
{
    public class ConfigUI : UIPanel
    {
        [SerializeField] private RectTransform _container;
        [Space]
        [SerializeField] private Button _allOnButton;
        [SerializeField] private Button _allOffButton;
        
        [Space(20)]
        [SerializeField] private ConfigOptionUI _optionPrefab;
        [SerializeField] private ConfigOptionEnabledUI _optionEnabledPrefab;
        [SerializeField] private ConfigOptionSelectUI _optionSelectPrefab;
        [SerializeField] private ConfigOptionToggleGroupUI _optionSToggleGroupPrefab;

        private List<ConfigOptionUI> _options = new();

        public override void Start()
        {
            base.Start();
            _allOnButton.onClick.AddListener(OnAllOnClicked);
            _allOffButton.onClick.AddListener(OnAllOffClicked);
        }

        public void Init(List<ConfigValue> configValues)
        {
            foreach (var config in configValues)
            {
                if(config is IConfigEnabledOption)
                    _options.Add(CreateEnabledOption(config));
                else if (config is IConfigSelectOption selectOption)
                {
                    if(selectOption.UseToggleGroup())
                        _options.Add(CreateToggleGroupOption(config));
                    else
                        _options.Add(CreateSelectOption(config));
                }
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

        public void RefreshAll()
        {
            foreach (ConfigOptionUI optionUI in _options)
                optionUI.Refresh();
        }

        public void SetCurrentIndex(int currentIndex)
        {
            for (int i = 0; i < _options.Count; i++)
            {
                _options[i].SetSelected(currentIndex == i);
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
        
        private ConfigOptionToggleGroupUI CreateToggleGroupOption(ConfigValue configValue)
        {
            var option = Instantiate(_optionSToggleGroupPrefab, _container);
            option.Init(configValue);
            return option;
        }

        private void OnAllOnClicked() => GameManager.Instance.AllConfigActive();
        private void OnAllOffClicked() => GameManager.Instance.ResetAllConfig();
    }
}