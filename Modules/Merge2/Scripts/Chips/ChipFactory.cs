using UnityEngine;

namespace Merge2
{
	public static class ChipFactory
	{
		public static Chip CreateChip(Cell cell, ChipData chipData, Vector3? parentWorldPosition = null)
		{
			if (chipData == null)
			{
				Debug.LogError("CreateChip: ChipData is empty");
				return null;
			}
			if (chipData.PrefabLink == null)
			{
				Debug.LogError("CreateChip: PrefabLink is empty");
				return null;
			}
			if (cell.Chip != null)
			{
				Debug.LogError("CreateChip: in Cell.Chip is not empty");
				return null;
			}

			GameObject chipGO = GameObject.Instantiate(chipData.PrefabLink);
			Chip chip = chipGO.GetComponent<Chip>();
			chip.Init(chipData);
			if (parentWorldPosition.HasValue)
			{
				chip.transform.localPosition = parentWorldPosition.Value;
				chip.SetDragging(true);
			}

			cell.Chip = chip;
			return chip;
		}
	}
}
