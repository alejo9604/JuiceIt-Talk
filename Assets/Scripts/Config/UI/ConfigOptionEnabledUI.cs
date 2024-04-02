using UnityEngine;
using UnityEngine.UI;

namespace AllieJoe.JuiceIt
{
    public class ConfigOptionEnabledUI : ConfigOptionUI
    {
        [SerializeField] private Toggle _toggle;

        private IConfigEnabledOption _option;
        public override void Init(ConfigValue configValue)
        {
            base.Init(configValue);

            if (configValue is IConfigEnabledOption enabledValue)
            {
                _option = enabledValue;
                _toggle.isOn = enabledValue.IsEnable();
            }
            else
                Debug.LogError($"[ConfigOptionEnabledUI] Config value {configValue.Key}: {configValue.GetType()} doesn't match expected type IConfigEnabledOption");
            
            _toggle.onValueChanged.AddListener(OnValueChange);
        }
        
        public override void Refresh()
        {
            base.Refresh();
            
            if(_option == null)
                return;
            
            _toggle.SetIsOnWithoutNotify(_option.IsEnable());
        }

        private void OnValueChange(bool enabled)
        {
            if(_option == null)
                return;
            _option.Set(enabled);
        }
    }
}
