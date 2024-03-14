using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Juice", fileName = "JuiceConfig")]
public class JuiceConfigSO : ScriptableObject
{
    [Header("Ship")] 
    public bool TrailEnabled;
    public bool ShootingMovementRestrictionEnabled;
    public bool ShootingAccuracyEnabled;

    [Header("Impact Pause")] 
    public bool ImpactPauseEnabled;
    [Range(0,1)]
    public float ImpactPauseDurationFramesPercent = 0.1f;
    public int ImpactPauseDurationMaxFrames = 15;


    [Header("Projectiles")]
    public Projectile[] ProjectilePrefabs;


    [ContextMenu("Reset to Defaults")]
    public void ResetToDefault()
    {
        //Ship
        TrailEnabled = false;
        ShootingMovementRestrictionEnabled = false;
        ShootingAccuracyEnabled = false;
        
        //Impact pause
        ImpactPauseEnabled = false;
    }
}
