using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitAction_MeleeAttack : BattleUnitAction
	{
		public BattleUnitAction_MeleeAttack(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
		}


		public override bool Action()
		{
			bool result = false;

			if (strategy.IsMeleeBattle() != null)
			{
				result = true;
				if (Unit.IsAttackReady)
				{
					strategy.Mover.MoveStop();
					Attack(Target).Forget();
				}
			}

			return result;
		}

		async UniTask Attack(BattleUnit target)
		{
			var Unit = this.Unit;
			if (Unit.LogEnable)
			{
				Debug.Log($"{Unit.name} Attack target:{target.name}");
			}
			Unit.Turn(target.transform.position);
			Unit.IsAttacking = true;

			await UniTask.WaitUntil(() => !Unit.IsDodgeRolling);

			target.AddAttacker(Unit);
			if (UnityEngine.Random.value > 0.5)
			{
				Unit.Character.Slash();
			}
			else
			{
				Unit.Character.Jab();
			}
			await UniTask.WaitForSeconds(0.4f);
			Unit.IsAttacking = false;
			Unit.NeedTimeToReady = true;
		}
		//public override void Update() {}
	}
}
