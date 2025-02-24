using System.Collections.Generic;
using HeroEditor.Common.Enums;
using Merge2;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "BossFight/HeroData")]
public class HeroData : ScriptableObject
{
    [SerializeField]
    HeroWeaponsData weapons;
    [SerializeField]
    HeroArmorsData armors;
    [SerializeField]
    HeroShieldsData shields;

    public class AData
    {
        public ChipData ChipData;
        public string ItemId;
        public EquipmentPart EquipmentPart;
    }

    Dictionary<ChipData, AData> caches;

    void AddCacheData(AData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            AData wdata = data[i];
            caches.Add(wdata.ChipData, wdata);
        }
    }

    public AData Find(ChipData data)
    {
        if (caches == null)
        {
            caches = new Dictionary<ChipData, AData>();
            AddCacheData(weapons.weapons);
            AddCacheData(armors.armors);
            AddCacheData(shields.shields);
        }
        if (caches.ContainsKey(data))
        {
            return caches[data];
        }
        return null;
    }
}


