using System.Collections;
using System.Linq;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [Header("Config")]
        public JuiceConfigSO JuiceConfig;
        public WeaponTuningLibrary WeaponTuningLibrary;
        
        [Header("UI")]
        public ConfigUI ConfigUI;
        public TraumaUI TraumaUI;
        public GameObject CameraLines;

        [Header("Game")] 
        public GameDelegates GameDelegates;
        public PlayerShip Player;
        public EnemySpawner EnemySpawner;

        [Header("Trauma System")] 
        [Min(1)] public float _traumaDecreaseSpeed = 0.5f;

        private float _timeScale = 1;

        private void Awake()
        {
            //Quick "singleton"
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
            JuiceConfig.ResetToDefault();

            _timeScale = Time.timeScale;
        }

        private void Start()
        {
            ConfigUI.Init(JuiceConfig.EnableSequence);
            
            GameDelegates.OnConfigToogleWithAllPrevious += OnConfigToggleWithAllPrevious;
        }
        
        private void Update()
        {
            // UI
            if(Input.GetKeyDown(KeyCode.Escape))
                ConfigUI.ToggleHide();
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                    TraumaUI.ToggleHide();
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    EnemySpawner.ToggleSpawn();
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    Player.ResetHealth();
                if (Input.GetKeyDown(KeyCode.Alpha4))
                    Player.ToggleCanMove();
                if (Input.GetKeyDown(KeyCode.Alpha5))
                    CameraLines.SetActive(!CameraLines.activeSelf);
            }

            // Sequence
            if (Input.GetKeyDown(KeyCode.T))
                ToNextStep();
            if (Input.GetKeyDown(KeyCode.E))
                ToPrevStep();
            
            if (Input.GetKeyDown(KeyCode.R))
                ReloadLevel();

            //Trauma-Shake
            if(Input.GetKeyDown(KeyCode.N))
                AddTrauma();
            if(Input.GetKeyDown(KeyCode.M))
                AddTrauma(1f);

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Time.timeScale < 1)
                    Time.timeScale = 1;
                else
                    Time.timeScale = 0.1f;
            }
            
            ReduceTrauma(Time.deltaTime);
        }

        public T GetConfigValue<T>(EConfigKey key) => JuiceConfig.GetValue<T>(key);

        public bool GetConfigValue(EConfigKey key) => JuiceConfig.GetValue<bool>(key);

        public string GetConfigLabel(EConfigKey key) => JuiceConfig.GetLabel(key);

        public void AllConfigActive()
        {
            JuiceConfig.ActiveAll();
            _currentStep = JuiceConfig.EnableSequence.Count - 1;
            ConfigUI.RefreshAll();
            GameDelegates.EmitAllConfigUpdated();
        }
        
        public void ResetAllConfig()
        {
            JuiceConfig.ResetToDefault();
            _currentStep = 0;
            ConfigUI.RefreshAll();
            GameDelegates.EmitAllConfigUpdated();
        }
        
        private void ReloadLevel()
        {
            GameDelegates.EmitOnResetLevel();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

#region Sequence
        private int _currentStep = 0;
        private void ToNextStep()
        { 
            if(_currentStep >= JuiceConfig.EnableSequence.Count)
                return;

            if (_currentStep < 0)
                _currentStep = 0;
            
            ConfigValue step = JuiceConfig.EnableSequence[_currentStep];
            (bool success, bool allConfigStepsSet) = EnableConfigValue(step);
            
            ConfigUI.Refresh(step);
            
            if(allConfigStepsSet)
                _currentStep++;

            if (success)
            {
                GameDelegates.EmitOnConfigUpdated(step.Key);
                GameDelegates.EmitOnTitleAnimRequested(step.Key);
            }

            ConfigUI.SetCurrentIndex(_currentStep);
        }

        private void ToPrevStep()
        {
            if(_currentStep < 0)
                return;

            if (_currentStep >= JuiceConfig.EnableSequence.Count)
            {
                _currentStep--;
                ToPrevStep();
                return;
            }
            
            ConfigValue step = JuiceConfig.EnableSequence[_currentStep];
            (bool success, bool allConfigStepsSet) = DisableConfigValue(step);
            if (!success)
            {
                _currentStep--;
                ToPrevStep();
                return;
            }

            int indexToEmit = _currentStep;
            if(allConfigStepsSet)
                indexToEmit = _currentStep - 1;
            
            //Refresh UI
            ConfigUI.Refresh(step);
            ConfigUI.SetCurrentIndex(_currentStep);
            if (indexToEmit < 0) return;
            
            if(indexToEmit != _currentStep)
                GameDelegates.EmitOnConfigUpdated(JuiceConfig.EnableSequence[_currentStep].Key);
            GameDelegates.EmitOnConfigUpdated(JuiceConfig.EnableSequence[indexToEmit].Key);
            GameDelegates.EmitOnTitleAnimRequested(JuiceConfig.EnableSequence[indexToEmit].Key);
        }

        private (bool success, bool allConfigStepsSet) EnableConfigValue(ConfigValue configValue, bool allSteps = false)
        {
            bool success = false;
            bool allConfigStepsSet = false;
            switch (configValue)
            {
                case IConfigEnabledOption enabledOption:
                {
                    if (!enabledOption.IsEnable())
                        enabledOption.Set(true);
                    allConfigStepsSet = true;
                    success = true;
                    break;
                }
                case IConfigSelectOption selectOption:
                {
                    if (allSteps)
                        selectOption.SetSelected(selectOption.Max() - 1); //Hack so it enters the next check
                    
                    //Enable the option and wait for next input to check the next one
                    if (selectOption.CurrentSelected() < selectOption.Max())
                    {
                        selectOption.Next();
                        success = true;
                    }

                    allConfigStepsSet = selectOption.CurrentSelected() >= selectOption.Max();
                    break;
                }
            }
            
            return (success, allConfigStepsSet);
        }
        
        private (bool success, bool allConfigStepsSet) DisableConfigValue(ConfigValue configValue, bool allSteps = false)
        {
            bool success = false;
            bool allConfigStepsSet = true;
            switch (configValue)
            {
                case IConfigEnabledOption enabledOption:
                {
                    if (enabledOption.IsEnable())
                    {
                        enabledOption.Set(false);
                        success = true;
                    }

                    break;
                }
                case IConfigSelectOption selectOption:
                {
                    if (allSteps)
                        selectOption.SetSelected(1); //Hack so it enters the next check
                    
                    if (selectOption.CurrentSelected() > 0)
                    {
                        selectOption.Prev();
                        success = true;
                        allConfigStepsSet = selectOption.CurrentSelected() <= 0;
                    }

                    break;
                }
            }
            return (success, allConfigStepsSet);
        }
        
        private void OnConfigToggleWithAllPrevious(EConfigKey configKey)
        {
            int index = JuiceConfig.EnableSequence.FindIndex(x => x.Key == configKey);
            while (_currentStep != index)
            {
                var step = JuiceConfig.EnableSequence[_currentStep];
                if (_currentStep > index)
                {
                    DisableConfigValue(step, allSteps: true);
                    _currentStep--;
                }
                else
                {
                    EnableConfigValue(step, allSteps: true);
                    _currentStep++;
                }

                ConfigUI.Refresh(step);
                GameDelegates.EmitOnConfigUpdated(step.Key);
            }

            // Make sure to enable the config selected
            var currentStep = JuiceConfig.EnableSequence[_currentStep];
            EnableConfigValue(currentStep, allSteps: true);
            ConfigUI.Refresh(currentStep);
            GameDelegates.EmitOnConfigUpdated(currentStep.Key);
            GameDelegates.EmitOnTitleAnimRequested(currentStep.Key);
            
            _currentStep++;
            ConfigUI.SetCurrentIndex(_currentStep);
        }

        public WeaponTuning GetWeaponTuningSelected()
        {
            WeaponTuning.EType type = JuiceConfig.GetValue<WeaponTuning.EType>(EConfigKey.WeaponType);
            return WeaponTuningLibrary.GetTuning(type);
        }
#endregion      

#region Trauma
        private float _trauma;
        public float TraumaValue => _trauma;
        public float ShakeValue => _trauma * _trauma * _trauma;

        public void AddTrauma(float amount = 0.3f)
        {
            if(JuiceConfig.GetValue<bool>(EConfigKey.CameraShake))
                _trauma = Mathf.Clamp01(_trauma + amount);
        }

        private void ReduceTrauma(float deltaTime)
        {
            _trauma = Mathf.Lerp(_trauma, 0, _traumaDecreaseSpeed * deltaTime);
        }
#endregion
        
#region ImpactPause
        
        //Impact pause - Hit Stop
        private bool _isOnImpactPause;
        private float _impactPauseIntensity;
        private Coroutine _impactPauseCoroutine;

        public void DoImpactPause(bool isPlayer)
        {
            //Impact pause for player damage
            if (isPlayer && !JuiceConfig.GetValue<bool>(EConfigKey.PlayerImpactPause))
                return;
            
            //Impact pause for projectile impact
            if (!isPlayer && !JuiceConfig.GetValue<bool>(EConfigKey.ShootImpactPause))
                return;

            float timeToPause = isPlayer ? JuiceConfig.PlayerImpactPauseDurationInSec : JuiceConfig.ShootImpactPauseDurationInSec;
            
            //If player override the impact. If it's not player damage just ignore it
            if (_isOnImpactPause && timeToPause > _impactPauseIntensity)
                return;
            
            if(_impactPauseCoroutine != null)
                StopCoroutine(_impactPauseCoroutine);
            _impactPauseCoroutine = StartCoroutine(DoImpactPause(timeToPause));
        }

        private IEnumerator DoImpactPause(float timeToPause)
        {
            _impactPauseIntensity = timeToPause;
            _isOnImpactPause = true;
            
            Time.timeScale = 0f;
            
            yield return new WaitForSecondsRealtime(timeToPause);
            // for (int i = 0; i < frames; i++)
            //     yield return null;

            Time.timeScale = _timeScale;

            _isOnImpactPause = false;
            _impactPauseIntensity = 0;
        }
#endregion

    }
}