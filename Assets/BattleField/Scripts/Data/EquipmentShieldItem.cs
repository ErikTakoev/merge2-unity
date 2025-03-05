using UnityEngine;

namespace BattleField
{
    [CreateAssetMenu(fileName = "ShieldItem", menuName = "BattleField/ShieldItem")]
	public class EquipmentShieldItem : EquipmentItem
	{
		public int ChanceBlockDamage;
	}
}
