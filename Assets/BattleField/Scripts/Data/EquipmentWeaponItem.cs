using UnityEngine;

namespace BattleField
{
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "BattleField/WeaponItem")]
	public class EquipmentWeaponItem : EquipmentItem
	{
        public int MinAttackDamage;
        public int MaxAttackDamage;
	}
}
