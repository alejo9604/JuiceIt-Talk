using System;
using System.Collections;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public JuiceConfigSO JuiceConfig;
        public ConfigUI ConfigUI;

        private int _currentStep = 0;
        
        private void Awake()
        {
            //Quick "singleton"
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void Start()
        {
            JuiceConfig.ResetToDefault();
            ConfigUI.Init(JuiceConfig.EnableSequence);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                ConfigUI.ToggleHide();

            if (Input.GetKeyDown(KeyCode.P))
                ToNextStep();
            
            if (Input.GetKeyDown(KeyCode.O))
                ToPrevStep();
        }

        private void ToNextStep()
        { 
            if(_currentStep >= JuiceConfig.EnableSequence.Count)
                return;

            if (_currentStep < 0)
                _currentStep = 0;
            
            ConfigValue step = JuiceConfig.EnableSequence[_currentStep];
            bool nextStep = true;
            
            if (step is IConfigEnabledOption enabledOption)
            {
                //Enable the option and wait for next input to check the next one
                if (!enabledOption.IsEnable())
                    enabledOption.Set(true);
            }
            else if (step is IConfigSelectOption selectOption)
            {
                //Enable the option and wait for next input to check the next one
                if (selectOption.CurrentSelected() < selectOption.Max())
                    selectOption.Next();
                
                nextStep = selectOption.CurrentSelected() >= selectOption.Max();
            }
            
            ConfigUI.Refresh(step);
            
            if(nextStep)
                _currentStep++;
        }

        private void ToPrevStep()
        {
            if(_currentStep < 0)
                return;
            
            //TODO: WIP, it's not reseting the proper one
            ConfigValue step = JuiceConfig.EnableSequence[_currentStep];
            bool goToPrevStep = true;
            if (step is IConfigEnabledOption enabledOption)
            {
                //Enable the option and wait for next input to check the next one
                if (enabledOption.IsEnable())
                    enabledOption.Set(false);
            }
            else if (step is IConfigSelectOption selectOption)
            {
                //Enable the option and wait for next input to check the next one
                if (selectOption.CurrentSelected() > 0)
                    selectOption.Prev();
                
                goToPrevStep = selectOption.CurrentSelected() <= 0;
            }
            
            ConfigUI.Refresh(step);
            
            if(goToPrevStep)
                _currentStep--;
        }


#region ImpactPause
        
        //Impact pause - Hit Stop
        private bool _isOnImpactPause;

        public void DoImpactPause()
        {
            if (!JuiceConfig.GetValue<bool>(EConfigKey.ImpactPause))
                return;

            if (_isOnImpactPause)
                return;

            float dummyFps = (1 / Time.deltaTime);
            int framesToPause = (int)(dummyFps * JuiceConfig.ImpactPauseDurationFramesPercent);
            framesToPause = Mathf.Min(framesToPause, JuiceConfig.ImpactPauseDurationMaxFrames);

            StartCoroutine(DoImpactPause(framesToPause));
        }

        private IEnumerator DoImpactPause(int frames)
        {
            _isOnImpactPause = true;

            float originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            for (int i = 0; i < frames; i++)
                yield return null;

            Time.timeScale = originalTimeScale;

            _isOnImpactPause = false;
        }
#endregion

    }
}