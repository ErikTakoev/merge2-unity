using HeroEditor.Common.Enums;
using Merge2;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroHelmetsData", menuName = "BossFight/HeroHelmetsData")]
public class HeroHelmetsData : ScriptableObject
{
    public HelmetData[] helmets;

    [Serializable]
    public class HelmetData : HeroData.AData
    {
        public int minusChanceCrit;
    }
}
