using UnityEngine;

namespace Merge2   // MonoBehaviour is the base class from which every Unity script derives.
{
	public class ChipEffect : MonoBehaviour
	{
		public virtual void Activate(Chip chip)
		{
			gameObject.SetActive(true);
			if (chip.LogEnable)
			{
				Debug.Log($"ChipEffect: Activate for {chip.name}");
			}
		}
		public virtual void Deactivate(Chip chip)
		{
			gameObject.SetActive(false);
			if (chip.LogEnable)
			{
				Debug.Log("ChipEffect: Deactivate");
			}
		}
	}
}
