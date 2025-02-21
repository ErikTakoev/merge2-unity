using UnityEngine;
using static ChipContainer;

public class FillContainerLogic
{
    public bool ChipSuitableForContainer(Cell prevCell, Cell newCell)
    {
        if (!prevCell || !newCell)
        {
            Debug.LogError("ChipSuitableForContainer: prevCell or newCell is null");
            return false;
        }

        Chip prevChip = prevCell.Chip;
        Chip newChip = newCell.Chip;

        if (!prevChip || !newChip)
        {
            return false;
        }
        if (prevChip == newChip)
        {
            return false;
        }
        ChipContainer chipContainer = newCell.Chip as ChipContainer;

        if (chipContainer.ChipSuitableForContainer(prevCell.Chip))
        {
            prevCell.Chip.Destroy();
            prevCell.Chip = null;

            return true;
        }
        return false;
    }
}
