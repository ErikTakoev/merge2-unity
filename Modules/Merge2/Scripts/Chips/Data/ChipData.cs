using System;
using UnityEngine;

namespace Merge2
{
    [CreateAssetMenu(fileName = "ChipData", menuName = "Scriptable Objects/ChipData")]
    public class ChipData : ScriptableObject
    {
        public string Type = "Default";
        public GameObject PrefabLink;
        public ChipMergeData MergeData;
        public ChipContainerData chipContainerData;
    }

    [Serializable]
    public class ChipMergeData
    {
        public ChipData NextChip;
    }

    [Serializable]
    public class ChipContainerData
    {
        [Serializable]
        public enum ContainerType
        {
            ChipType,
            ChipId
        }
        [Serializable]
        public class ContainerInfo
        {
            public ContainerType Type = ContainerType.ChipType;
            public string TypeOrId;
            public int Count = 1;
        }

        public ContainerInfo[] containers;
    }

}