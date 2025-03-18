using UnityEngine;

namespace Merge2
{
	public class MergeableChipLogic
	{
		public bool IsMergeableChips(Cell prevCell, Cell overCell)
		{
			if (!prevCell)
			{
				Debug.LogError("IsMergeableChips: prevCell is null");
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
			if (prevChip.Data == newChip.Data)
			{
				ChipMergeData mergeData = prevChip.Data.MergeData;
				if (mergeData != null)
				{
					ChipData nextChip = overCell.Chip.Data.MergeData.NextChip;
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
			Chip chip = ChipFactory.CreateChip(newCell, nextChip);
			if (chip != null)
			{
				chip.SendTrigger(Chip.AnimatorTrigger.Merge);
				return true;
			}

			return false;
		}
	}
}
