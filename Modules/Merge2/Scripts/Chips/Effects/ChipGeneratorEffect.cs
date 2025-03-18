using UnityEngine;

namespace Merge2   // MonoBehaviour is the base class from which every Unity script derives.
{
	public class ChipGeneratorEffect : ChipEffect
	{
		[SerializeField]
		RectTransform maskRectTransform;

		public void OnCharging(float progress)
		{
			maskRectTransform.localPosition = new Vector3(0, progress * maskRectTransform.sizeDelta.y, 0);
		}
	}
}
