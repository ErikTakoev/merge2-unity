using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitAction_DefenseShield : BattleUnitAction_Defense
	{
		public BattleUnitAction_DefenseShield(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
		}

		protected override bool Defense(BattleUnit attacker)
		{
			var unit = Unit;
			var unitStats = unit.Stats;

			if (!unit.IsAttacking && !unit.IsStunning && !unit.IsDodgeRolling && unitStats.IsBlocking())
			{
				ShieldBlock(attacker).Forget();
				return true;
			}
			return base.Defense(attacker);
		}

		async UniTask ShieldBlock(BattleUnit target)
		{
			var unit = Unit;
			if (unit.LogEnable)
			{
				Debug.Log($"{unit.name} DodgeRoll target:{target.name}");
			}
			unit.IsDodgeRolling = true;
			unit.Character.Animator.SetTrigger("ShieldDefense");
			await UniTask.WaitForSeconds(1f);
			unit.IsDodgeRolling = false;
		}

		//public override void Update() {}
	}
}
