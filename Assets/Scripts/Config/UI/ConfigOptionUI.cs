using TMPro;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class ConfigOptionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private GameObject _selected;

        public EConfigKey ConfigKey = EConfigKey._INVALID; 
        
        public virtual void Init(ConfigValue configValue)
        {
            _label.SetText(configValue.Label);
            _selected.SetActive(false);
            ConfigKey = configValue.Key;
        }

        public virtual void Refresh(){}
    }
}
