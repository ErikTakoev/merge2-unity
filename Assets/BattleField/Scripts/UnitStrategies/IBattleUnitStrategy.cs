using System.Collections.Generic;
using UnityEngine;

namespace BattleField
{
    public abstract class IBattleUnitStrategy
    {
        public IBattleUnitStrategy(BattleHero unit, List<EquipmentItem> items)
        {
            Unit = unit;
            Items = items;
            Attackers = new List<BattleHero>();
        }
        
        protected BattleHero Unit;
        protected List<EquipmentItem> Items;
        protected List<BattleHero> Attackers;

        protected abstract bool DodgeRoll();
        protected abstract bool Move();
        protected abstract bool SpecialAttack();
        protected abstract bool Attack();

        public virtual void AddAttacker(BattleHero attacker)
        {
            Attackers.Add(attacker);
        }

        public virtual void Update()
        {
            if (DodgeRoll())
            {
                return;
            }
            if (SpecialAttack())
            {
                return;
            }
            if (Attack())
            {
                return;
            }
            if (Move())
            {
                return;
            }
        }
    }
}