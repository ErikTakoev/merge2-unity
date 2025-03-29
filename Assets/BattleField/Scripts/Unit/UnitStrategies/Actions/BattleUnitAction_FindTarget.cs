using System.Collections.Generic;
using HeroEditor.Common.Enums;
using VContainer;

namespace BattleField
{
	public class BattleUnitAction_FindTarget : BattleUnitAction
	{
		[Inject] BattleField field;

		public BattleUnitAction_FindTarget(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
		}

		bool SmartTarget()
		{
			bool result = false;
			List<(BattleUnit, int)> nearTargets = field.FindTargetsNear(Unit);
			if (nearTargets.Count == 0)
			{
				return result;
			}
			result = true;
			if (nearTargets.Count == 1)
			{
				strategy.Target = nearTargets[0].Item1;
			}
			else
			{
				var unitPosX = Unit.NextCell.CellPos.x;
				for (int i = 0; i < nearTargets.Count; i++)
				{
					var unit_distance = nearTargets[i];
					var weaponType = unit_distance.Item1.Character.WeaponType;
					switch (weaponType)
					{
						case WeaponType.MeleePaired:
							unit_distance.Item2 -= 1;
							break;
						case WeaponType.Bow:
							unit_distance.Item2 -= 2;
							if (unit_distance.Item1.Strategy.Target == Unit)
							{
								unit_distance.Item2 -= 1;
							}
							break;
					}

					{
						var posX = unit_distance.Item1.NextCell.CellPos.x;
						if (Unit.IsHero)
						{
							if (posX <= unitPosX)
							{
								unit_distance.Item2 -= 1;
							}
						}
						else
						{
							if (posX >= unitPosX)
							{
								unit_distance.Item2 -= 1;
							}
						}
					}


					nearTargets[i] = unit_distance;
				}

				(BattleUnit, int) target = new(null, int.MaxValue);
				for (int i = 0; i < nearTargets.Count; i++)
				{
					var unit_distance = nearTargets[i];
					if (unit_distance.Item1 == Target && unit_distance.Item2 <= target.Item2)
					{
						target = unit_distance;
					}
					else if (unit_distance.Item2 < target.Item2)
					{
						target = unit_distance;
					}
				}
				strategy.Target = target.Item1;
				if (Unit.LogEnable)
				{
					UnityEngine.Debug.Log($"SmartTarget: unit: {Unit.name}, target: {Target.name}");
				}
			}

			return result;
		}


		public override bool Action()
		{
			if (!SmartTarget())
			{
				strategy.Target = field.FindTarget(Unit, Target);
			}
			return false;
		}
		//public override void Update() {}
	}
}
