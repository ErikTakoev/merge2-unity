using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroShieldsData", menuName = "BossFight/HeroShields")]
public class HeroShieldsData : ScriptableObject
{
    public ShieldData[] shields;

    [Serializable]
    public class ShieldData : HeroData.AData
    {
        public int minusChanceCrit;
    }
}
