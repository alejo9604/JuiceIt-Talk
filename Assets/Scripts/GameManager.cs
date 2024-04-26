using System.Collections;
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
        }

        private void Update()
        {
            // UI
            if(Input.GetKeyDown(KeyCode.Escape))
                ConfigUI.ToggleHide();
            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
                TraumaUI.ToggleHide();
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
                EnemySpawner.ToggleSpawn();
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
                Player.ResetHealth();

            // Sequence
            if (Input.GetKeyDown(KeyCode.P))
                ToNextStep();
            if (Input.GetKeyDown(KeyCode.O))
                ToPrevStep();

            //Trauma-Shake
            if(Input.GetKeyDown(KeyCode.N))
                AddTrauma();
            if(Input.GetKeyDown(KeyCode.M))
                AddTrauma(1f);

            if (Input.GetKeyDown(KeyCode.R))
                ReloadLevel();

            if (Input.GetKeyDown(KeyCode.T))
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
            bool nextStep = true;
            EConfigKey updatedKey = EConfigKey._INVALID;
            
            if (step is IConfigEnabledOption enabledOption)
            {
                //Enable the option and wait for next input to check the next one
                if (!enabledOption.IsEnable())
                {
                    enabledOption.Set(true);
                    updatedKey = step.Key;
                }
            }
            else if (step is IConfigSelectOption selectOption)
            {
                //Enable the option and wait for next input to check the next one
                if (selectOption.CurrentSelected() < selectOption.Max())
                {
                    selectOption.Next();
                    updatedKey = step.Key;
                }

                nextStep = selectOption.CurrentSelected() >= selectOption.Max();
            }
            
            ConfigUI.Refresh(step);
            
            if(nextStep)
                _currentStep++;
            
            if(updatedKey != EConfigKey._INVALID)
                GameDelegates.EmitOnConfigUpdated(updatedKey);
            
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
            
            //TODO: WIP, it's not reseting the proper one
            ConfigValue step = JuiceConfig.EnableSequence[_currentStep];
            
            if (step is IConfigEnabledOption enabledOption)
            {
                //Already disabled. Reset next one
                if (!enabledOption.IsEnable())
                {
                    _currentStep--;
                    ToPrevStep();
                    return;
                }
                
                enabledOption.Set(false);
            }
            else if (step is IConfigSelectOption selectOption)
            {
                //Already in the min selection. Reset next one
                if (selectOption.CurrentSelected() <= 0)
                {
                    _currentStep--;
                    ToPrevStep();
                    return;
                }
                
                selectOption.Prev();
            }
            
            //Refresh UI
            ConfigUI.Refresh(step);
            ConfigUI.SetCurrentIndex(_currentStep);
            if(_currentStep > 0)
                GameDelegates.EmitOnConfigUpdated(JuiceConfig.EnableSequence[_currentStep - 1].Key);
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

            Debug.Log("Hit impact");
            
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