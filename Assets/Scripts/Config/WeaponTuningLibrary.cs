using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [Serializable]
    public class WeaponTuning
    {
        public enum EType { Basic, MachineGun }
        public enum ESubType {SingleCannon, TwoCannon}

        [HideInInspector] public string name;
        public EType Type;
        public ESubType SubType;
        public Sprite CannonSprite;
        public int Damage;

        public int BulletsToShoot = 1;
        public int MaxAngleToShoot = 90;
    }
    
    [CreateAssetMenu(menuName = "Config/WeaponLibrary", fileName = "WeaponLibrary")]
    public class WeaponTuningLibrary : ScriptableObject
    {
        public List<WeaponTuning> Weapons = new();

        public WeaponTuning GetTuning(WeaponTuning.EType type)
        {
            foreach (WeaponTuning weapon in Weapons)
                if (weapon.Type == type)
                    return weapon;
            
            Debug.LogError($"Can't find Weapon Tuning for {type}!");
            return null;
        }
        
        private void OnValidate()
        {
            foreach (WeaponTuning weapon in Weapons)
                weapon.name = weapon.Type.ToString();
        }
    }
}
