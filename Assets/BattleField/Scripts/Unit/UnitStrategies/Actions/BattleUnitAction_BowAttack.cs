using System.Linq;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BattleField
{
	public class BattleUnitAction_BowAttack : BattleUnitAction
	{
		Transform armLTransform;
		Transform bowTransform;
		Animator unitAnimator;

		Transform arrowParentTransform;

		Transform arrow;

		float randomAngle;

		[Inject] BattleArrowController arrowController;

		public BattleUnitAction_BowAttack(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
			var sculptor = Unit.Character.GetComponent<CharacterBodySculptor>();
			armLTransform = sculptor.ArmL;
			bowTransform = sculptor.Bow[0];

			unitAnimator = Unit.Character.Animator;

			arrowParentTransform = Unit.Character.BowRenderers.Last().transform.parent;
		}


		public override bool Action()
		{
			if (Target == null)
			{
				return false;
			}

			bool result = false;

			var distance = Pathfinding.GetManhattanDistance(Target.NextCell, Unit.NextCell);
			if (distance <= 6)
			{
				strategy.Mover.MoveStop();
				//result = true;
				unitAnimator.SetBool("Ready", true);

				if (Unit.IsAttackReady)
				{
					randomAngle = Random.Range(-15, 15) - 10;
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

			if (arrow == null)
			{
				arrow = arrowController.GetArrow(arrowParentTransform);
			}

			unitAnimator.SetInteger("Charge", 1);

			await UniTask.WaitForSeconds(0.4f);

			if (Target == null)
			{
				Unit.IsAttacking = false;
				Unit.NeedTimeToReady = false;
				return;
			}

			arrowController.Shoot(arrow, Unit, Target);
			arrow = null;
			unitAnimator.SetInteger("Charge", 2);
			await UniTask.WaitForSeconds(0.2f);

			Unit.IsAttacking = false;
			Unit.NeedTimeToReady = true;
		}

		public override void LateUpdate()
		{
			if (Target == null)
			{
				return;
			}

			RotateArm(armLTransform, bowTransform, Target.transform.position, -90, 90, randomAngle);
		}

		public static void RotateArm(Transform arm, Transform weapon, Vector2 target, float angleMin, float angleMax, float randomAngle) // TODO: Very hard to understand logic.
		{
			target = arm.transform.InverseTransformPoint(target);

			var angleToTarget = Vector2.SignedAngle(Vector2.right, target);
			var angleToArm = Vector2.SignedAngle(weapon.right, arm.transform.right) * System.Math.Sign(weapon.lossyScale.x);
			var fix = weapon.InverseTransformPoint(arm.transform.position).y / target.magnitude;

			if (fix < -1) fix = -1;
			else if (fix > 1) fix = 1;

			var angleFix = Mathf.Asin(fix) * Mathf.Rad2Deg;
			var angle = angleToTarget + angleFix + arm.transform.localEulerAngles.z + randomAngle;

			angle = NormalizeAngle(angle);

			if (angle > angleMax)
			{
				angle = angleMax;
			}
			else if (angle < angleMin)
			{
				angle = angleMin;
			}

			if (float.IsNaN(angle))
			{
				Debug.LogWarning(angle);
			}

			arm.transform.localEulerAngles = new Vector3(0, 0, angle + angleToArm);
		}

		private static float NormalizeAngle(float angle)
		{
			while (angle > 180) angle -= 360;
			while (angle < -180) angle += 360;

			return angle;
		}
	}
}
