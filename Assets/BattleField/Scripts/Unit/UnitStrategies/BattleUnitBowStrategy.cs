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

			AddAction(new BattleUnitAction_FindTarget(this));
			AddAction(new BattleUnitAction_Defense(this));
			AddAction(new BattleUnitAction_BowAttack(this));
			AddAction(new BattleUnitAction_MoveKeepDistance(this));
			AddAction(new BattleUnitAction_Move(this));
		}
	}
}
