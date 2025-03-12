using System.Collections.Generic;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace BattleField
{
    public abstract class IBattleUnitStrategy
    {
        public IBattleUnitStrategy(BattleHero unit)
        {
            Unit = unit;
            
            Attackers = new List<BattleHero>();
            Actions = new List<BattleUnitAction>();
        }
        
        public BattleHero Unit;
        public BattleHero Target;
        public Dictionary<EquipmentPart, EquipmentItem> Items { get { return Unit.Items; } }
        public List<BattleHero> Attackers;

        protected List<BattleUnitAction> Actions;
        

        public virtual void AddAttacker(BattleHero attacker)
        {
            Attackers.Add(attacker);
        }

        public Vector2Int? IsMeleeBattle()
        {
            if (Target == null)
            {
                return null;
            }
            var targetPos = Target.NextCell.CellPos;
            var unitPos = Unit.NextCell.CellPos;
            var diff = targetPos - unitPos;
            if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1)
            {
                if (diff.x == 0 && Mathf.Abs(diff.y) == 1)
                {
                    return null;
                }
                return diff;
            }
            return null;
        }

        public virtual void Update()
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Update();
            }
            if (Unit.IsStunning && Unit.LogEnable)
            {
                Debug.Log($"Unit: {Unit.name} is stunned");
                return;
            }
            

            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].Action())
                    return;
            }
        }
    }
}