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
            if (Input.GetKeyDown(KeyCode.T))
                JuiceConfig.TryNext(EConfigKey.ProjectilePrefab);;

            if (Input.GetKeyDown(KeyCode.Y))
                JuiceConfig.TryNext(EConfigKey.ProjectileSpeed);

            if (Input.GetKeyDown(KeyCode.U))
                JuiceConfig.TryNext(EConfigKey.ProjectileRateFire);;
        }
        
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


    }
}