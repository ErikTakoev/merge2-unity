using UnityEngine;

namespace Merge2
{
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
            ChipContainer chipContainer = newChip as ChipContainer;
            if (!chipContainer)
            {
                return false;
            }

            if (chipContainer.ChipSuitableForContainer(prevChip))
            {
                prevCell.Chip.Destroy();
                prevCell.Chip = null;

                return true;
            }
            return false;
        }
    }
}