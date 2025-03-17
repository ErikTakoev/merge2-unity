using System.Collections.Generic;
using HeroEditor.Common.Enums;
using Merge2;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroDataBase", menuName = "BossFight/HeroDataBase")]
public class HeroDataBase : ScriptableObject
{
    [SerializeField]
    public Data[] Armors;
    [SerializeField]
    public Data[] Weapons;
    [SerializeField]
    public Data[] Bows;
    [SerializeField]
    public Data[] Helmets;
    
    [SerializeField]
    public Data[] Shields;

    Dictionary<ChipData, Data> caches;


    [System.Serializable]
    public class Data
    {
        public ChipData ChipData;
        public BattleField.EquipmentItem Item;
    }


    void AddCacheData(Data[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            Data wdata = data[i];
            caches.Add(wdata.ChipData, wdata);
        }
    }

    public Data Find(ChipData data)
    {
        if (caches == null)
        {
            caches = new Dictionary<ChipData, Data>();
            AddCacheData(Armors);
            AddCacheData(Weapons);
            AddCacheData(Bows);
            AddCacheData(Helmets);
            AddCacheData(Shields);
        }
        if (caches.ContainsKey(data))
        {
            return caches[data];
        }
        return null;
    }
}


