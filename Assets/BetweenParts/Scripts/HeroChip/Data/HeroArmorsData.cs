using HeroEditor.Common.Enums;
using Merge2;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroArmorsData", menuName = "BossFight/HeroArmorsData")]
public class HeroArmorsData : ScriptableObject
{
    public ArmorData[] armors;

    [Serializable]
    public class ArmorData : HeroData.AData
    {
        public int Defence;
    }
}
