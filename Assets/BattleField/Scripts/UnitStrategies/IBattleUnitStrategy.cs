using System.Collections.Generic;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace BattleField
{
    public abstract class IBattleUnitStrategy
    {
        public IBattleUnitStrategy(BattleHero unit, List<EquipmentItem> items)
        {
            Unit = unit;
            Items = new Dictionary<EquipmentPart, EquipmentItem>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    Items.Add(item.EquipmentPart, item);
                }
            }
            
            Attackers = new List<BattleHero>();
        }
        
        protected BattleHero Unit;
        public Dictionary<EquipmentPart, EquipmentItem> Items { get; private set; }
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
            if (Unit.IsStunning)
            {
                Debug.Log($"Unit: {Unit.name} is stunned");
                return;
            }
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