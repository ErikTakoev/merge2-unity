using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitShieldStrategy : BattleUnitAbstractStrategy
	{
		[SerializeField]
		bool isMainHero;

		public override void Init(BattleUnit unit)
		{
			base.Init(unit);

			if (isMainHero)
			{
				AddAction(new BattleUnitAction_HeroMessages(this));
			}

			AddAction(new BattleUnitAction_FindTarget(this));
			AddAction(new BattleUnitAction_DefenseShield(this));
			AddAction(new BattleUnitAction_Retreat(this));
			AddAction(new BattleUnitAction_AttackShield(this));
			AddAction(new BattleUnitAction_MeleeAttack(this));
			AddAction(new BattleUnitAction_Move(this));
		}
	}
}
