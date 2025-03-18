using UnityEngine;

namespace Merge2
{
	[CreateAssetMenu(fileName = "ChipData", menuName = "Merge2/ChipData")]
	public class ChipData : ScriptableObject
	{
		public string Type = "Default";
		public GameObject PrefabLink;
		public ChipMergeData MergeData;
		public ChipContainerData ChipContainerData;
		public ChipGeneratorData ChipGeneratorData;
	}
}
