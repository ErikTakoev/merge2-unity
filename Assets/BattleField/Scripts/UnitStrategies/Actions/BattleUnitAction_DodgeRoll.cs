using System.Collections;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_DodgeRoll : BattleUnitAction
    {
        public BattleUnitAction_DodgeRoll(IBattleUnitStrategy strategy)
            : base (strategy)
        {
        }


        public override bool Action()
        {
            if (strategy.Attackers.Count == 0)
            {
                return false;
            }
            if (!Unit.IsAttacking && !Unit.IsStunning && !Unit.IsDodgeRolling && UnityEngine.Random.value > 0.5f)
            {
                DodgeRoll(strategy.Attackers[0]);
                return true;
            }
            strategy.Attackers.Clear();
            return false;
        }

        public void DodgeRoll(BattleHero target)
        {
            var unit = Unit;
            if (unit.LogEnable)
            {
                Debug.Log($"{unit.name} DodgeRoll target:{target.name}");
            }
            unit.StartCoroutine(DodgeRollCoroutine(target));
        }

        private IEnumerator DodgeRollCoroutine(BattleHero target)
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