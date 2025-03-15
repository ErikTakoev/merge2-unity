using System.Collections;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_BowAttack : BattleUnitAction
    {
        Transform armLTransform;
        Transform bowTransform;
        Animator unitAnimator;

        public BattleUnitAction_BowAttack(IBattleUnitStrategy strategy)
            : base (strategy)
        {
            var sculptor = Unit.Character.GetComponent<CharacterBodySculptor>();
            armLTransform = sculptor.ArmL;
            bowTransform = sculptor.Bow[0];
            
            unitAnimator = Unit.Character.Animator;
        }


        public override bool Action()
        {
            bool result = false;

            var distance = Pathfinding.GetManhattanDistance(Target.NextCell, Unit.NextCell);
            if (distance <= 5)
            {
                strategy.Mover.MoveStop();
                //result = true;
                unitAnimator.SetBool("Ready", true);
            }
            if (Unit.IsAttackReady)
            {
                Attack(Target);
            }

            return result;
        }

        public void Attack(BattleHero target)
        {
            var Unit = this.Unit;
            if (Unit.LogEnable)
            {
                Debug.Log($"{Unit.name} Attack target:{target.name}");
            }
            Unit.Turn(target.transform.position);
            Unit.IsAttacking = true;
            Unit.StartCoroutine(AttackCoroutine(target));
        }

        private IEnumerator AttackCoroutine(BattleHero target)
        {
            var Unit = this.Unit;
            target.AddAttacker(Unit);
            unitAnimator.SetInteger("Charge", 1);

            yield return new WaitForSeconds(0.4f);
            unitAnimator.SetInteger("Charge", 2);
            yield return new WaitForSeconds(0.2f);
            
            Unit.IsAttacking = false;
            Unit.NeedTimeToReady = true;
        }
        public override void LateUpdate() 
        {
            if (Target == null)
            {
                return;
            }
            
            RotateArm(armLTransform, bowTransform, Target.transform.position, -40, 40);
        }

        public static void RotateArm(Transform arm, Transform weapon, Vector2 target, float angleMin, float angleMax) // TODO: Very hard to understand logic.
        {
            target = arm.transform.InverseTransformPoint(target);
            
            var angleToTarget = Vector2.SignedAngle(Vector2.right, target);
            var angleToArm = Vector2.SignedAngle(weapon.right, arm.transform.right) * System.Math.Sign(weapon.lossyScale.x);
            var fix = weapon.InverseTransformPoint(arm.transform.position).y / target.magnitude;

            if (fix < -1) fix = -1;
            else if (fix > 1) fix = 1;

            var angleFix = Mathf.Asin(fix) * Mathf.Rad2Deg;
            var angle = angleToTarget + angleFix + arm.transform.localEulerAngles.z;

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