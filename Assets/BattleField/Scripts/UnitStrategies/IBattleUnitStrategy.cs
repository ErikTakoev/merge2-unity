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
        
        protected bool isPathfinding;
        protected BattleHero target;
        protected BattleCell movingToCell;

        protected abstract bool FindTarget();
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
            if (FindTarget())
            {
                return;
            }
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