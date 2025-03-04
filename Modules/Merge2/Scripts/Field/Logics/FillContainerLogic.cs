using UnityEngine;

namespace Merge2
{
    public class FillContainerLogic
    {
        public bool Fill(Cell prevCell, Cell overCell)
        {
            if (!prevCell)
            {
                Debug.LogError("ChipSuitableForContainer: prevCell is null");
                return false;
            }
            if (!overCell) // Чип винесли за межі поля
            {
                return false;
            }

            Chip prevChip = prevCell.Chip;
            Chip newChip = overCell.Chip;

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