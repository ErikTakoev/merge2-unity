using UnityEngine;

public class ChipContainer : Chip
{
    public delegate void AddContainerDelegate(Chip chip);

    public event AddContainerDelegate OnAddContainerDelegate;

    public override void Init(ChipData data)
    {
        base.Init(data);


    }
}
