using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitStats : MonoBehaviour
	{
		[SerializeField]
		int Health = 50;
		int FullHealth;

		[SerializeField]
		int Defense = 0;

		[SerializeField]
		int ChanceBlockDamage = 0;

		[SerializeField]
		int ChanceMinusCrit = 0;

		[SerializeField]
		int MinAttackDamage = 3;
		[SerializeField]
		int MaxAttackDamage = 6;

		public event Action OnUnitDeadEvent;
		public event Action<int, int> OnChangeHealth;

		public void Init(List<EquipmentItem> items)
		{
			if (items == null)
			{
				return;
			}

			foreach (var item in items)
			{
				if (item is EquipmentArmorItem armor)
				{
					Defense += armor.Defense;
				}
				else if (item is EquipmentHelmetItem helmet)
				{
					ChanceMinusCrit += helmet.ChanceMinusCrit;
				}
				else if (item is EquipmentShieldItem shield)
				{
					ChanceBlockDamage += shield.ChanceBlockDamage;
				}
				else if (item is EquipmentWeaponItem weapon)
				{
					MinAttackDamage += weapon.MinAttackDamage;
					MaxAttackDamage += weapon.MaxAttackDamage;
				}
			}
		}

		void Start()
		{
			FullHealth = Health;
		}

		public bool IsBlocking()
		{
			if (ChanceBlockDamage == 0)
				return false;
			return UnityEngine.Random.value * 100f < ChanceBlockDamage;
		}

		public int GetAttackPower()
		{
			return UnityEngine.Random.Range(MinAttackDamage, MaxAttackDamage + 1);
		}

		public int GetDefense()
		{
			return Defense;
		}

		public int GetHealth()
		{
			return Health;
		}

		public bool IsCritDamage()
		{
			return UnityEngine.Random.value * 100f < (50 - ChanceMinusCrit);
		}

		public void AttackUnit(int minusHealth)
		{
			if (Health == 0)
			{
				return;
			}
			Health -= minusHealth;
			if (Health <= 0)
			{
				Health = 0;
				OnChangeHealth?.Invoke(Health, FullHealth);
				OnChangeHealth = null;
				OnUnitDeadEvent?.Invoke();
				OnUnitDeadEvent = null;
				return;
			}
			OnChangeHealth?.Invoke(Health, FullHealth);
		}
	}
}
