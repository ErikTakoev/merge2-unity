using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitAction_AttackShield : BattleUnitAction
	{
		float attackShieldCooldown = 3;

		public BattleUnitAction_AttackShield(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
		}


		public override bool Action()
		{
			attackShieldCooldown -= Time.deltaTime;

			if (attackShieldCooldown < 0 && Unit.IsAttackReady && UnityEngine.Random.value > 0.7f)
			{
				var diff = strategy.IsMeleeBattle();
				if (diff != null)
				{
					diff = diff.Value * 2;
					var result = Unit.NextCell.CellPos + diff.Value;
					var cell = BattleField.Instance.GetCell(result.x, result.y);
					if (cell != null && cell.IsAvailableCell())
					{
						attackShieldCooldown = 3;
						AttackShield(Target, cell);
						return true;
					}
				}
			}
			return false;
		}

		void AttackShield(BattleUnit target, BattleCell targetNewCell)
		{
			var unit = this.Unit;

			unit.IsAttacking = true;
			var tmpCell = target.NextCell;
			target.SetCell(targetNewCell, false);
			unit.SetCell(tmpCell, false);

			var newPos = tmpCell.WorldPosition;
			unit.Character.SetState(CharacterState.Run);
			unit.Character.Animator.SetTrigger("AttackShield");
			unit.transform.DOMove(newPos, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
			{
				unit.IsAttacking = false;
				unit.NeedTimeToReady = false;
				unit.Character.SetState(CharacterState.Idle);
			});

			target.IsStunning = true;
			target.Strategy.Mover.MoveStop();
			target.Character.SetState(CharacterState.Idle);
			target.Character.Animator.SetTrigger("Stun");
			var targetNewPos = targetNewCell.WorldPosition;
			target.transform.DOMove(targetNewPos, 1f).SetEase(Ease.OutExpo)
				.OnComplete(() =>
				{
					target.IsStunning = false;
					target.NeedTimeToReady = true;
				});
		}
		//public override void Update() {}
	}
}
