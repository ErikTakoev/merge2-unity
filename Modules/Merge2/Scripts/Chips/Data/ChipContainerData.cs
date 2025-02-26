using System;

namespace Merge2
{
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