namespace AllieJoe.JuiceIt
{
    public enum EConfigKey
    {
        Intro = 0,
        Trail = 1,
        TrailStopVFX = 30,
        
        ShootingMovementRestriction = 2,
        ShootingAccuracy = 3,
        
        ShootImpactPause = 4,

        ProjectileSpeed = 5,
        ProjectileRateFire = 6,
        ProjectilePrefab = 7,
        ProjectileAccuracy = 8,
        ProjectileHitVFX = 9,
        
        MuzzleFlash = 10,
        WeaponType = 11,
        
        SpawnEnemies = 28,
        EnemyHitImpact = 12,
        EnemyDeath = 19,
        EnemyScratch = 20,
        EnemyPermanent = 28,
        
        CameraLerp = 13,
        CameraPointOfInterest = 31,
        CameraShake = 14,
        CameraPerlinNoise = 15,
        
        BackgroundSpawnTween = 16,
        Shadows = 17,
        Clouds = 18,
        
        
        PlayerImpactPause = 21,
        PlayerDamageVFX = 22,
        PlayerImpactVFX = 23,
        ShockWave = 26,
        
        Music = 24,
        SFX = 25,
        
        PlayerShipMovement = 27,
        WorldVariation = 29,

        //Next = 32
        _INVALID = 50
    }
}