using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AllieJoe.JuiceIt
{
    public class PlayerShipShoot : MonoBehaviour
    {
        [Serializable]
        class WeaponPreset
        {
            public WeaponTuning.ESubType Type;
            public SpriteRenderer[] CannonSprites;
            public Transform[] CannonFire;
            public GameObject[] MuzzleFlash;

            private bool _areMuzzleFlashActive;
            
            public void Set(WeaponTuning tuning)
            {
                foreach (SpriteRenderer sprite in CannonSprites)
                {
                    sprite.sprite = tuning.CannonSprite;
                    sprite.gameObject.SetActive(true);
                }
                foreach (Transform cannon in CannonFire)
                    cannon.gameObject.SetActive(true);
                foreach (GameObject muzzleFlash in MuzzleFlash)
                    muzzleFlash.SetActive(false);
            }

            public void Hide()
            {
                foreach (SpriteRenderer sprite in CannonSprites)
                    sprite.gameObject.SetActive(false);
                foreach (Transform cannon in CannonFire)
                    cannon.gameObject.SetActive(false);
                foreach (GameObject muzzleFlash in MuzzleFlash)
                    muzzleFlash.SetActive(false);
            }

            public void HideMuzzleFlash()
            {
                if(!_areMuzzleFlashActive)
                    return;
                _areMuzzleFlashActive = false;
                foreach (GameObject muzzleFlash in MuzzleFlash)
                    muzzleFlash.SetActive(false);
            }

            public void ShowMuzzleFlash()
            {
                _areMuzzleFlashActive = true;
                foreach (GameObject muzzleFlash in MuzzleFlash)
                    muzzleFlash.SetActive(true);
            }
        }
        
        
        [SerializeField] private WeaponPreset[] _weaponPresets;

        private int _damage = 0;
        private int _bulletsToShootAmount = 1;
        private int _maxAngleToShoot = 90;
        private int _currentWeaponPreset = 0;
        private float _nextFireAt;
        private float _lastFireAt;

        private void Start()
        {
            RefreshSelectedWeapon();
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdated;
        }

        private void Update()
        {
            if (Time.time > _lastFireAt + GameManager.Instance.JuiceConfig.MuzzleFlashTime)
                GetCurrentPresetActive().HideMuzzleFlash();
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnConfigUpdated;
        }

        public void Shoot(float extraSpeed = 0)
        {
            FireProjectile(extraSpeed);
        }

        private WeaponPreset GetCurrentPresetActive() => _weaponPresets[_currentWeaponPreset];
        
        private void FireProjectile(float extraSpeed = 0)
        {
            if (Time.time < _nextFireAt)
                return;

            _nextFireAt = Time.time + GameManager.Instance.GetConfigValue<float>(EConfigKey.ProjectileRateFire);
            _lastFireAt = Time.time;

            WeaponPreset weaponPresetData = GetCurrentPresetActive();
            
            float accuracy = 0;
            float accuracyBaseValue = GameManager.Instance.GetConfigValue<float>(EConfigKey.ProjectileAccuracy);
            bool accuracyEnabled = GameManager.Instance.GetConfigValue<bool>(EConfigKey.ShootingAccuracy);
            if(accuracyEnabled && !GameManager.Instance.JuiceConfig.ShootAccuracyPerCannon)
                accuracy = Random.Range(-accuracyBaseValue, accuracyBaseValue);

            Projectile projectilePrefab = GameManager.Instance.GetConfigValue<Projectile>(EConfigKey.ProjectilePrefab);
            float projectileSpeed = GameManager.Instance.GetConfigValue<float>(EConfigKey.ProjectileSpeed);

            foreach (var cannon in weaponPresetData.CannonFire)
            {

                float initAngle = _bulletsToShootAmount <= 1 ? 0 : _maxAngleToShoot / 2f;
                float angle = _bulletsToShootAmount <= 1 ? 0 : _maxAngleToShoot / (_bulletsToShootAmount - 1f);
                
                for (int i = 0; i < _bulletsToShootAmount; i++)
                {
                    if (accuracyEnabled && !GameManager.Instance.JuiceConfig.ShootAccuracyPerCannon)
                        accuracy = Random.Range(-accuracyBaseValue, accuracyBaseValue);
                    
                    var initRotation = Vector3.forward * (initAngle - angle * i);
                    initRotation.z += accuracy;
                    var dir = Quaternion.Euler(initRotation) * cannon.transform.up;
                    
                    Projectile projectile = Instantiate(projectilePrefab, cannon.position, Quaternion.identity);
                    projectile.Init(dir, projectileSpeed, true, extraSpeed: extraSpeed);
                    projectile.SetDamage(_damage);
                }
            }

            if(GameManager.Instance.GetConfigValue<bool>(EConfigKey.MuzzleFlash))
                weaponPresetData.ShowMuzzleFlash();
            
            GameManager.Instance.AddTrauma(GameManager.Instance.JuiceConfig.TraumaAddPerShoot);
        }

        private void RefreshSelectedWeapon()
        {
            EquipWeapon(GameManager.Instance.GetWeaponTuningSelected());
        }

        private void EquipWeapon(WeaponTuning tuning)
        {
            if (tuning == null)
            {
                Debug.Log("[PlayerShipShoot] Ignoring Weapon Tuning null...");
                return;
            }
            
            _currentWeaponPreset = -1;
            for (int i = 0; i < _weaponPresets.Length; i++)
            {
                var preset = _weaponPresets[i];
                if (preset.Type != tuning.SubType || _currentWeaponPreset >= 0)
                {
                    preset.Hide();
                    continue;
                }
                
                preset.Set(tuning);
                _currentWeaponPreset = i;
                _damage = tuning.Damage;
                _bulletsToShootAmount = tuning.BulletsToShoot;
                _maxAngleToShoot = tuning.MaxAngleToShoot;
            }
        }
        
        //Events
        private void OnConfigUpdated(EConfigKey key)
        {
            if(key == EConfigKey.WeaponType)
                RefreshSelectedWeapon();
        }

      
    }
}