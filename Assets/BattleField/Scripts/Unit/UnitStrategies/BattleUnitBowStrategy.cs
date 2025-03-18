using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitBowStrategy : BattleUnitAbstractStrategy
	{

		public override void Init(BattleUnit unit)
		{
			base.Init(unit);

			Actions.Add(new BattleUnitAction_FindTarget(this));
			Actions.Add(new BattleUnitAction_Defense(this));
			Actions.Add(new BattleUnitAction_BowAttack(this));
			Actions.Add(new BattleUnitAction_MoveKeepDistance(this));
			Actions.Add(new BattleUnitAction_Move(this));
		}
	}
}
