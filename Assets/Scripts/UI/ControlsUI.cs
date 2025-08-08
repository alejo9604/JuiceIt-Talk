using UnityEngine;
using UnityEngine.UI;

namespace AllieJoe.JuiceIt
{
    public class ControlsUI : MonoBehaviour
    {
        
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _nextButton;
        
        void Start()
        {
            _prevButton.onClick.AddListener(OnPrevPressed);
            _resetButton.onClick.AddListener(OnResetPressed);
            _nextButton.onClick.AddListener(OnNextPressed);
        }
        
        
        private void OnPrevPressed() => GameManager.Instance.GameDelegates.EmitOnPrevStep();
        private void OnResetPressed() => GameManager.Instance.GameDelegates.EmitOnResetLevel();
        private void OnNextPressed() => GameManager.Instance.GameDelegates.EmitOnNextStep();
    }
}
