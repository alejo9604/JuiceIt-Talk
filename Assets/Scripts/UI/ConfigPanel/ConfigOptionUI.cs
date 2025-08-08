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

        public void SetSelected(bool selected) => _selected.SetActive(selected);

        public virtual void Refresh(){}

        protected void OnValueUpdated()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                GameManager.Instance.GameDelegates.EmitOnConfigToggleWithAllPrevious(ConfigKey);
            else
            {
                GameManager.Instance.GameDelegates.EmitOnConfigUpdated(ConfigKey);
                GameManager.Instance.GameDelegates.EmitOnTitleAnimRequested(ConfigKey);
            }
        }
    }
}
