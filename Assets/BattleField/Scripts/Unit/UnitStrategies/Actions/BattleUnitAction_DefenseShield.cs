using System;
using System.Collections;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_DefenseShield : BattleUnitAction_Defense
    {
        public BattleUnitAction_DefenseShield(BattleUnitAbstractStrategy strategy)
            : base (strategy)
        {
        }

        protected override bool Defense(BattleUnit attacker)
        {
            var unit = Unit;
            var unitStats = unit.Stats;

            if (!unit.IsAttacking && !unit.IsStunning && !unit.IsDodgeRolling && unitStats.IsBlocking())
            {
                ShieldBlock(attacker);
                return true;
            }
            return base.Defense(attacker);
        }

        public void ShieldBlock(BattleUnit target)
        {
            var unit = Unit;
            if (unit.LogEnable)
            {
                Debug.Log($"{unit.name} DodgeRoll target:{target.name}");
            }
            unit.StartCoroutine(ShieldBlockCoroutine(target));
        }

        private IEnumerator ShieldBlockCoroutine(BattleUnit target)
        {
            var unit = Unit;
            unit.IsDodgeRolling = true;
            unit.Character.Animator.SetTrigger("ShieldDefense");
            yield return new WaitForSeconds(1.0f);
            unit.IsDodgeRolling = false;
        }

        //public override void Update() {}
    }
}