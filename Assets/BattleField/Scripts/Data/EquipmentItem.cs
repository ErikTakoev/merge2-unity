using UnityEngine;
using HeroEditor.Common.Enums;

namespace BattleField
{
	public class EquipmentItem : ScriptableObject
	{
		public string ItemId;
		public EquipmentPart EquipmentPart;
	}

	[CreateAssetMenu(fileName = "WeaponItem", menuName = "BattleField/WeaponItem")]
	public class EquipmentWeaponItem : EquipmentItem
	{
        public int MinAttackDamage;
        public int MaxAttackDamage;
	}

	[CreateAssetMenu(fileName = "ShieldItem", menuName = "BattleField/ShieldItem")]
	public class EquipmentShieldItem : EquipmentItem
	{
		public int ChanceBlockDamage;
	}

	[CreateAssetMenu(fileName = "HelmetItem", menuName = "BattleField/HelmetItem")]
	public class EquipmentHelmetItem : EquipmentItem
	{
		public int MinusChanceCrit;
	}
}
