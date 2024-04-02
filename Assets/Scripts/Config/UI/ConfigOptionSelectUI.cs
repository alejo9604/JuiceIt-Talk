using UnityEngine;
using UnityEngine.UI;

namespace AllieJoe.JuiceIt
{
    public class ConfigOptionSelectUI : ConfigOptionUI
    {
        [SerializeField] private Slider _slider;
        
        private IConfigSelectOption _option;
        public override void Init(ConfigValue configValue)
        {
            base.Init(configValue);

            if (configValue is IConfigSelectOption selectValue)
            {
                _option = selectValue;
                _slider.value = selectValue.CurrentSelected();
                _slider.minValue = 0;
                _slider.maxValue = selectValue.Max();
            }
            else
                Debug.LogError(
                    $"[ConfigOptionEnabledUI] Config value {configValue.Key}: {configValue.GetType()} doesn't match expected type IConfigOption");

            _slider.onValueChanged.AddListener(OnValueChange);
        }

        private void OnValueChange(float index)
        {
            _option.SetSelected((int)index);
        }
    }
}
