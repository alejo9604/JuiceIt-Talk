using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public JuiceConfigSO JuiceConfig;
    
    private void Awake()
    {
        //Quick "singleton"
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            NextWeapon();
    }


    private int _currentProjectile = 0;
    private void NextWeapon()
    {
        _currentProjectile = (_currentProjectile + 1) % JuiceConfig.ProjectilePrefabs.Length;
    }

    public Projectile GetCurrentProjectile() => JuiceConfig.ProjectilePrefabs[_currentProjectile];


    //Impact pause - Hit Stop
    private bool _isOnImpactPause;
    public void DoImpactPause()
    {
        if(!JuiceConfig.ImpactPauseEnabled)
            return;
        
        if(_isOnImpactPause)
            return;

        float dummyFps =  (1 / Time.deltaTime);
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
