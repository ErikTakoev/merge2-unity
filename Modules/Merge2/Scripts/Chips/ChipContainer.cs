using System.Collections.Generic;
using UnityEngine;

namespace Merge2
{
	public class ChipContainer : Chip
	{
		protected delegate void FillContainerDelegate(Chip chip, bool isFull);

		protected event FillContainerDelegate OnFillContainer;

		protected Dictionary<ChipContainerData.ContainerInfo, int> containers;

		[SerializeReference]
		ChipContainerEffect containerEffect;

		public override void Init(ChipData data)
		{
			base.Init(data);

			if (data.ChipContainerData == null)
			{
				Debug.LogError("ChipContainer: data.chipContainerData is empty");
				return;
			}
			containers = new Dictionary<ChipContainerData.ContainerInfo, int>();
			foreach (var item in data.ChipContainerData.containers)
			{
				containers.Add(item, 0);
			}

			if (containerEffect != null)
			{
				containerEffect.Activate(this);
			}
			SendTrigger(AnimatorTrigger.ContainerEmpty);
		}

		public virtual bool ChipSuitableForContainer(Chip chip)
		{
			ChipData data = chip.Data;

			foreach (var container in containers)
			{
				string id = null;
				if (container.Key.Type == ChipContainerData.ContainerType.ChipType)
				{
					id = data.Type;
				}
				else if (container.Key.Type == ChipContainerData.ContainerType.ChipId)
				{
					id = data.name;
				}

				if (id == container.Key.TypeOrId)
				{
					if (container.Value + 1 == container.Key.Count)
					{
						containers.Remove(container.Key);

						bool isFull = containers.Count == 0;
						OnFillContainer?.Invoke(chip, isFull);
						if (isFull && Data.ChipContainerData.NextChipData != null)
						{
							Cell cell = transform.parent.GetComponent<Cell>();
							cell.Chip = null;
							Destroy();
							ChipFactory.CreateChip(cell, Data.ChipContainerData.NextChipData);
						}
						return true;
					}
					else
					{
						containers[container.Key] = container.Value + 1;
						return true;
					}
				}
			}
			return false;
		}
	}
}
