using UnityEngine;
using Merge2;
using System;
using System.Collections.Generic;
using HeroEditor.Common.Enums;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "HeroWeapon", menuName = "BossFight/HeroWeapon")]
public class HeroWeaponsData : ScriptableObject
{
    public WeaponData[] weapons;

    [Serializable]
    public class WeaponData : HeroData.AData
    {
        public int MinAttackPower;
        public int MaxAttackPower;
    }
}
