using System;
using System.Collections;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_Defense : BattleUnitAction
    {
        public BattleUnitAction_Defense(IBattleUnitStrategy strategy)
            : base (strategy)
        {
        }


        public override bool Action()
        {
            var unit = Unit;
            bool result = false;
            if (strategy.Attackers.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < strategy.Attackers.Count; i++)
            {
                var attacker = strategy.Attackers[i];
                if (Defense(attacker))
                {
                    result = true;
                }
            }

            strategy.Attackers.Clear();
            return result;
        }

        protected virtual bool Defense(BattleUnit attacker)
        {
            var attackerStats = attacker.Stats;
            var unit = Unit;
            var unitStats = unit.Stats;

            int damage = attackerStats.GetAttackPower();
            if (attackerStats.IsCritDamage())
            {
                damage = (int)(damage * 1.5);
            }
            damage -= unitStats.GetDefense();
            damage = Math.Max(damage, 1);
            unitStats.AttackUnit(damage);
            unit.Character.Spring();
            return false;
        }


        //public override void Update() {}
    }
}