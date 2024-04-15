using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AllieJoe.JuiceIt
{
    public class ConfigOptionToggleGroupUI : ConfigOptionUI
    {
        [SerializeField] private Toggle _toggleTemplate;
        [SerializeField] private RectTransform _toggleParent;
        
        private IConfigSelectOption _option;
        private List<Toggle> _toggles = new();
        
        public override void Init(ConfigValue configValue)
        {
            base.Init(configValue);
            
            if (configValue is not IConfigSelectOption)
                Debug.LogError(
                    $"[ConfigOptionToggleGroupUI] Config value {configValue.Key}: {configValue.GetType()} doesn't match expected type IConfigOption");

            _option = configValue as IConfigSelectOption;
            int total = _option.Max();
            for (int i = 0; i <= total; i++)
            {
                var index = i;
                var t = Instantiate(_toggleTemplate, _toggleParent);
                t.gameObject.SetActive(true);
                _toggles.Add(t);
                
                t.isOn = index == _option.CurrentSelected();
                t.onValueChanged.AddListener(isOn => OnValueChange(index, isOn));
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            
            if(_option == null)
                return;
            
            RefreshToggleGroup(_option.CurrentSelected());
        }

        private void OnValueChange(int index, bool isOn)
        {
            _option.SetSelected((int)index);
            OnValueUpdated();

            // If set to false, set the first one to On
            RefreshToggleGroup(isOn ? index : 0);
        }

        private void RefreshToggleGroup(int indexOn)
        {
            for (int i = 0; i < _toggles.Count; i++)
                _toggles[i].SetIsOnWithoutNotify(i == indexOn);
        }
    }
}
