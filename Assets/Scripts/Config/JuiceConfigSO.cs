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
}
