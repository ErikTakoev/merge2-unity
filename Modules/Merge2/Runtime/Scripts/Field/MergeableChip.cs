using UnityEngine;

public class MergeableChip
{
    public bool IsMergeableChips(Cell prevCell, Cell newCell)
    {
        if (!prevCell || !newCell)
        {
            Debug.LogError("IsMergeableChips: prevCell or newCell is null");
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
        if (prevChip.Data == newChip.Data)
        {
            ChipMergeData mergeData = prevChip.Data.MergeData;
            if (mergeData != null)
            {
                ChipData nextChip = newCell.Chip.Data.MergeData.NextChip;
                return nextChip != null;
            }
        }
        return false;
    }

    public bool MergeChip(Cell prevCell, Cell newCell)
    {
        if (IsMergeableChips(prevCell, newCell))
        {
            return MergeChipImpl(prevCell, newCell);
        }
        return false;
    }

    private bool MergeChipImpl(Cell prevCell, Cell newCell)
    {
        ChipData nextChip = newCell.Chip.Data.MergeData.NextChip;
        prevCell.Chip.Destroy();
        prevCell.Chip = null;
        newCell.Chip.Destroy();
        newCell.Chip = null;

        return ChipFactory.CreateChip(newCell, nextChip);
    }
}
