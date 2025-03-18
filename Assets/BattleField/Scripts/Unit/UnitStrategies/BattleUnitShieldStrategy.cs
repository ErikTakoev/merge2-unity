using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitShieldStrategy : BattleUnitAbstractStrategy
	{

		public override void Init(BattleUnit unit)
		{
			base.Init(unit);

			Actions.Add(new BattleUnitAction_FindTarget(this));
			Actions.Add(new BattleUnitAction_DefenseShield(this));
			Actions.Add(new BattleUnitAction_Retreat(this));
			Actions.Add(new BattleUnitAction_AttackShield(this));
			Actions.Add(new BattleUnitAction_MeleeAttack(this));
			Actions.Add(new BattleUnitAction_Move(this));
		}
	}
}
